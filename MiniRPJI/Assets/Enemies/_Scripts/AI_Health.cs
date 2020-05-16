/* AI_Health.cs
Utilisé pour gérer la vie des NPCs
*/
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public enum MalusType { None, Slowed, InFire, Poisoned, Electrified }; // Used to know what projectile type hurt AI from ApplyMalus coroutine.

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(AI_Stats))] // To get healthpoints & speed
public class AI_Health : MonoBehaviour
{
    [Header("General")]
    // To know when AI just took damage, then we want to upgrade his chasing distance (via AI_Combat_Control)
    public bool damaged = false;
    [SerializeField] float damagedTimer = 5f;
    float currentDamagedTimer;

    // Dead variables
    [SerializeField] bool deathAnimation = false;
    bool isDead = false;
    float deathTiming = 5f; // For fix a bug, sometimes death animation don't play so gameobject isnt destroy

    // others settings
    [SerializeField] GameObject damageText; // To display damage inflict by player
    [SerializeField] Transform damageTextParent; // Used to instantiate damageText as child of this
    [Tooltip("Let it null if you don't want fading when damage taken.")]
    [SerializeField] SpriteRenderer rend;

    // The one for avoid collision with others AI
    [SerializeField] Collider2D AI_Collider; // To disable when dead.

    // Sounds
    [SerializeField] AudioClip[] hurtSounds;

    //******* Malus variables **********\\

    // Slowed variables

    [Header("Slow Malus")]
    [SerializeField] float slowedTimer = 2f;
    [SerializeField] Color slowColor = Color.blue;

    // In fire variables
    [Header("Fire Malus")]
    [SerializeField] float fireTimer = 2f;
    [SerializeField] GameObject fireEffect; // A particle effect on AI's body. TODO Think about put it in Player_Projectile directly as malusEffect?

    // Poisoned variables
    [Header("Poison Malus")]
    [SerializeField] float poisonedTimer = 2f;
    [SerializeField] Color poisonColor = Color.magenta;

    // Electric variables
    [Header("Electric Malus")]
    [SerializeField] float electrifiedTimer = 2f;
    [SerializeField] float percentageChanceToParalyze; // a percentage of chance to be paralyze
    [SerializeField] GameObject electricEffect;

    // others variables
    Animator animator;
    AI_Stats ai_stats;

    Coroutine TakeDamagePerSecondCoroutine; // To know what damageOverTime coroutine is currently using
    Coroutine malusCoroutine; // To know when AI is already on a malus.
    MalusType currentMalus; // To know what is the current Malus on AI to disable it before got a new one.

    GameObject currentEffect; // The current effect on the AI. No cumulable.
    float originalAISpeed; // To know what's the AI speed at start. (When its got 0 malus on it)
    float lastDamagedTextXPosition = 0f;

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        ai_stats = GetComponent<AI_Stats>();

        originalAISpeed = ai_stats.GetSpeed();
    }

    // Update is called once per frame
    void Update()
    {
        // Security for dead npcs
        if (isDead)
        {
            deathTiming -= Time.deltaTime;
            if (deathTiming <= 0f)
            {
                Die();
            }
            return;
        }

        // To know when enemy get damaged
        if (damaged)
        {
            if (currentDamagedTimer > 0f)
            {
                currentDamagedTimer -= Time.deltaTime;
            }
            else
            {
                damaged = false;
                currentDamagedTimer = damagedTimer;
            }
        }
    }    

    #region fade coroutines
    // A FadeIn and FadeOut coroutine methods to apply on each sprite who doesnt have "TakeDamage" animation
    IEnumerator FadeIn() // Reapear
    {
        for (float f = 0.05f; f <= 1; f+=0.05f)
        {
            Color c = rend.material.color;
            c.a = f;
            rend.material.color = c;

            yield return new WaitForSeconds(0.01f);
        }      
    }

    IEnumerator FadeOut() // Dissapear
    {
        for (float f = 1f; f >= -0.05f; f -= 0.05f)
        {
            Color c = rend.material.color;
            c.a = f;
            rend.material.color = c;
            
            yield return new WaitForSeconds(0.01f);
        }

        StartCoroutine("FadeIn"); // And now reappear
    }

    #endregion

    void PlayDeathAnimation()
    {
        // Reset color if AI had malus on it
        if (rend)
        {
            if (rend.color != Color.white)
            {
                rend.color = Color.white;
            }
        }

        // Disable others AI components
        if (GetComponent<AI_Enemy_Movement>())
            GetComponent<AI_Enemy_Movement>().enabled = false;
        if (GetComponent<AI_Moveset>())
            GetComponent<AI_Moveset>().enabled = false;
        if (GetComponent<AI_Enemy_Combat>())
            GetComponent<AI_Enemy_Combat>().enabled = false;
        // And collider (fix issue with xp twice a time)
        if (GetComponent<Collider2D>())
            GetComponent<Collider2D>().enabled = false;
        if (AI_Collider)
            AI_Collider.enabled = false;
        if (currentEffect)
            Destroy(currentEffect);

        // Set all animator parameters to false
        animator.SetBool("isMoving", false);
        animator.SetBool("isAttacking", false);
        animator.SetBool("normalAttack", false);

        // If AI was moving, stop it
        if (GetComponent<Rigidbody2D>())
            if (GetComponent<Rigidbody2D>().velocity != Vector2.zero)
                GetComponent<Rigidbody2D>().velocity = Vector2.zero;

        // Play animation
        animator.SetTrigger("isDead");
    }

    // Don't forget to put in every end of death's animations
    public void Die()
    {
        StopAllCoroutines();

        // Drop objects, golds, check if quest objective...

        // Drop items
        if (GetComponent<ItemDroper>())
        {
            GetComponent<ItemDroper>().DropItems(ai_stats.GetLevel());
        }

        // Drop quest items
        if (GetComponent<QuestItemDroper>())
        {
            GetComponent<QuestItemDroper>().DropQuestItems();
        }

        // Check if this AI was a quest's objective
        if (GetComponent<Quest_Objective_Target>())
        {
            GetComponent<Quest_Objective_Target>().IncrementQuestObjective();
        }
        
        Destroy(gameObject);
    }

    // To know in other scripts if current npc is dead
    public bool IsDead()
    {
        return isDead;
    }

    public void TakeDamage(int amount, bool playSound)
    {
        if (isDead)
            return;

        // Display damage on the UI
        DisplayDamagedTextUI(amount);

        int tempHealthPoints = ai_stats.GetCurrentHealthPoints() - amount;
        if (tempHealthPoints < 0)
        {
            ai_stats.SetCurrentHealthPoints(0);
        }
        else
        {
            ai_stats.SetCurrentHealthPoints(tempHealthPoints);
        }

        if (ai_stats.GetCurrentHealthPoints() <= 0)
        {
            isDead = true;

            if (GetComponentInChildren<UI_Enemy>())
                GetComponentInChildren<UI_Enemy>().gameObject.SetActive(false);

            if (deathAnimation) // Play death animation if there is one
            {
                PlayDeathAnimation();
            }
            else
            {
                Die();
            }
            return;
        }

        damaged = true;
        currentDamagedTimer = damagedTimer;

        if (rend)
        {
            StartCoroutine("FadeOut");
        }

        if (hurtSounds.Length > 0)
        {
            if (playSound && Sound_Manager.instance)
                Sound_Manager.instance.PlaySound(hurtSounds[Random.Range(0, hurtSounds.Length)], transform);
        }
    }

    void DisplayDamagedTextUI(int amount)
    {
        if (damageText)
        {
            GameObject _damagedText = Instantiate(damageText, damageTextParent); // Create damagedText GO
            RectTransform _damagedTextRect = _damagedText.GetComponent<RectTransform>(); // Get acces to the RectTransform component

            float randomPosX = 0;

            // Give random position to _damagedText on X between -0.5f - 0.5f.
            // Check what was the last X position of the damaged text
            if (lastDamagedTextXPosition > 0) // If it was on the left
            {
                randomPosX = Random.Range(_damagedTextRect.localPosition.x - 0.2f, _damagedTextRect.localPosition.x - 0.6f);
            }
            else if (lastDamagedTextXPosition < 0) // It was on the right
            {
                randomPosX = Random.Range(_damagedTextRect.localPosition.x + 0.2f, _damagedTextRect.localPosition.x + 0.6f);
            }
            else // If its equal to 0, first time we use it.
            {
                randomPosX = Random.Range(_damagedTextRect.localPosition.x - 0.6f, _damagedTextRect.localPosition.x + 0.6f);
            }

            lastDamagedTextXPosition = randomPosX;

            _damagedTextRect.localPosition = new Vector2(randomPosX, _damagedTextRect.localPosition.y);

            _damagedText.GetComponent<Text>().text = amount.ToString();
        }
    }

    // ************************************************************************************ \\
    // ********************************** MALUS METHODS *********************************** \\
    // ************************************************************************************ \\

    // TODO think about use only currentMalus and not malusCoroutine to check malus on the enemy ?

    // Method used into Player_Projectile to deal overPower attack when enemy is already affect by the same malus type.
    public MalusType GetCurrentMalusType()
    {
        return currentMalus;
    }

    // Method call from Player_Projectile to set a malus on the AI.
    // bool takeDamage parameter exist because of overpower projectile. To inflict malus and damageovertime to ennemies but not projectile damage directly.
    public void SetMalus(MalusType type, int playerDamage, bool takeDamage = true)
    {
        // First before add a new Malus, verifity there is none already by checking malusCoroutine.
        RemoveMalus();

        malusCoroutine = StartCoroutine(ApplyMalus(type, playerDamage, takeDamage));
    }

    // Method to remove current malus on enemy.
    public void RemoveMalus()
    {
        if (malusCoroutine != null)
        {
            StopCoroutine(malusCoroutine);

            switch (currentMalus)
            {
                case MalusType.Slowed:
                    UnApplySlow();
                    break;
                case MalusType.InFire:
                    UnApplyFire();
                    break;
                case MalusType.Poisoned:
                    UnApplyPoison();
                    break;
                case MalusType.Electrified:
                    UnApplyElectric();
                    break;
            }
        }
    }

    // Method to apply malus inside SetMalus()
    IEnumerator ApplyMalus(MalusType type, int playerDamage, bool takeDamage)
    {
        switch(type)
        {
            // SLOWED PART
            case MalusType.Slowed:
                // Apply slow malus
                ApplySlow(playerDamage, takeDamage);

                // Wait slowed timer
                yield return new WaitForSeconds(slowedTimer);

                // UnApply slow malus
                UnApplySlow();
                
                break;
            // FIRE PART
            case MalusType.InFire:
                // Apply fire malus
                ApplyFire(playerDamage, takeDamage);

                // Wait fire timer
                yield return new WaitForSeconds(fireTimer);

                // UnApply fire malus
                UnApplyFire();

                break;
            // POISONED PART
            case MalusType.Poisoned:
                // Apply poison malus
                ApplyPoison(playerDamage, takeDamage);

                // Wait poison timer
                yield return new WaitForSeconds(poisonedTimer);

                // UnApply poison malus
                UnApplyPoison();

                break;
            // ELECTRIFIED PART
            case MalusType.Electrified:
                // Apply electrified malus
                ApplyElectric(playerDamage, takeDamage);

                // Wait for electrified timer
                yield return new WaitForSeconds(electrifiedTimer);

                // UnApply electrified malus
                UnApplyElectric();

                break;
        }
    }

    // Method to inflict damage over time (used for fire and poison)
    IEnumerator TakeDamagePerSecond(int damagePerSecond)
    {
        // Just wait a short amount of time and do the first burst, then burst every 1 sec
        yield return new WaitForSeconds(.5f);

        TakeDamage(damagePerSecond, false);

        // Then go into infinite loop to do damage until we unapply malus.
        while (true)
        {
            yield return new WaitForSeconds(1);

            TakeDamage(damagePerSecond, false);
        }
    }

    // ********************************** SLOW MALUS *********************************** \\
    void ApplySlow(int playerDamage, bool takeDamage)
    {
        if (takeDamage)
            TakeDamage(playerDamage, true);

        // Security for enemy to dont get malused if dead.
        if (isDead)
            return;

        GetComponentInChildren<SpriteRenderer>().color = slowColor;
        ai_stats.SetSpeed(originalAISpeed / 2);

        currentMalus = MalusType.Slowed;
    }

    void UnApplySlow()
    {
        GetComponentInChildren<SpriteRenderer>().color = Color.white;
        ai_stats.SetSpeed(originalAISpeed);

        currentMalus = MalusType.None;
        malusCoroutine = null;
    }

    // ********************************** FIRE MALUS *********************************** \\
    void ApplyFire(int playerDamage, bool takeDamage)
    {
        if (takeDamage)
            TakeDamage(playerDamage, true);

        // Security for enemy to dont get malused if dead.
        if (isDead)
            return;

        if (fireEffect)
        {
            if (currentEffect != null)
            {
                Destroy(currentEffect);
            }

            float damagePerSecond = playerDamage / fireTimer;

            TakeDamagePerSecondCoroutine = StartCoroutine(TakeDamagePerSecond(Mathf.RoundToInt(damagePerSecond)));

            currentEffect = Instantiate(fireEffect, transform.position, Quaternion.identity);
            currentEffect.transform.parent = transform;
        }

        currentMalus = MalusType.InFire;
    }

    void UnApplyFire()
    {
        if (currentEffect != null)
        {
            Destroy(currentEffect);
        }

        if (TakeDamagePerSecondCoroutine != null)
        {
            StopCoroutine(TakeDamagePerSecondCoroutine);
        }

        currentMalus = MalusType.None;
        malusCoroutine = null;

    }

    // ********************************** POISON MALUS *********************************** \\
    void ApplyPoison(int playerDamage, bool takeDamage)
    {
        if (takeDamage)
            TakeDamage(playerDamage, true);

        // Security for enemy to dont get malused if dead.
        if (isDead)
            return;

        GetComponentInChildren<SpriteRenderer>().color = poisonColor;

        float damagePerSecond = playerDamage / poisonedTimer;
        if (damagePerSecond < 1)
        {
            damagePerSecond = 1;
        }
        TakeDamagePerSecondCoroutine = StartCoroutine(TakeDamagePerSecond(Mathf.RoundToInt(damagePerSecond)));

        currentMalus = MalusType.Poisoned;
    }

    void UnApplyPoison()
    {
        GetComponentInChildren<SpriteRenderer>().color = Color.white;

        if (TakeDamagePerSecondCoroutine != null)
        {
            StopCoroutine(TakeDamagePerSecondCoroutine);
        }

        currentMalus = MalusType.None;
        malusCoroutine = null;

    }

    // ********************************** ELECTRIC MALUS *********************************** \\
    void ApplyElectric(int playerDamage, bool takeDamage)
    {
        if (takeDamage)
            TakeDamage(playerDamage, true);

        // Security for enemy to dont get malused if dead.
        if (isDead)
            return;

        bool willApply = percentageChanceToParalyze > Random.Range(0, 101);
        if (willApply == true)
        {
            ai_stats.SetSpeed(0f);
            AI_Collider.enabled = false; // disable ai_collider to avoid AI to be push by others

            if (currentEffect != null)
            {
                Destroy(currentEffect);
            }

            currentEffect = Instantiate(electricEffect, transform.position, Quaternion.identity);
            currentEffect.transform.parent = transform;

            currentMalus = MalusType.Electrified;

        }
    }

    void UnApplyElectric()
    {
        ai_stats.SetSpeed(originalAISpeed);
        AI_Collider.enabled = true;

        if (currentEffect != null)
        {
            Destroy(currentEffect);
        }

        currentMalus = MalusType.None;
        malusCoroutine = null;

    }
}
