/* AI_Health.cs
Utilisé pour gérer la vie des NPCs
*/
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

//public enum MalusType { None, Slowed, InFire, Poisoned, Electrified }; // Used to know what projectile type hurt AI from ApplyMalus coroutine.

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(AI_Stats))] // To get healthpoints & speed
public class AI_Health : MonoBehaviour, IDamageable
{
    [Header("General")]
    // To know when AI just took damage, then we want to upgrade his chasing distance (via AI_Combat_Control)
    public bool damaged = false;
    [SerializeField] float damagedTimer = 5f;
    float currentDamagedTimer;
    [SerializeField] bool immuneToPush = false;

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
    public void SetAI_Collider(bool value)
    {
        AI_Collider.enabled = value;
    }

    // Sounds
    [SerializeField] AudioClip[] hurtSounds; 

    // others variables
    Animator animator;
    AI_Stats ai_stats;
    MalusApplier ai_MalusApplier;

    float lastDamagedTextXPosition = 0f;

    float timeLastHit = 0f;

    private void OnDisable()
    {
        StopAllCoroutines();

        if (GetComponent<AI_Enemy_Movement>())
        {
            if (GetComponent<AI_Enemy_Movement>().enabled == false)
                GetComponent<AI_Enemy_Movement>().enabled = true;
        }
    }

    private void OnDestroy()
    {
        StopAllCoroutines();

        if (GetComponent<AI_Enemy_Movement>())
        {
            if (GetComponent<AI_Enemy_Movement>().enabled == false)
                GetComponent<AI_Enemy_Movement>().enabled = true;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        ai_stats = GetComponent<AI_Stats>();
        ai_MalusApplier = GetComponent<MalusApplier>();
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
        if (ai_MalusApplier)
            ai_MalusApplier.RemoveCurrentEffect();

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

        // Drop GameObjects
        if (GetComponent<GameObjectDroper>())
        {
            GetComponent<GameObjectDroper>().DropItems();
        }

        // Drop quest items
        QuestItemDroper[] questItemDropers = GetComponentsInChildren<QuestItemDroper>();

        for (int i = 0; i < questItemDropers.Length; i++)
        {
            questItemDropers[i].DropQuestItems();
        }

        // Check if this AI was a quest's objective
        Quest_Objective_Target[] questObjectiveTargets = GetComponentsInChildren<Quest_Objective_Target>();

        for (int i = 0; i < questObjectiveTargets.Length; i++)
        {
                questObjectiveTargets[i].IncrementQuestObjective();
        }

        // Check if this AI was a success objective
        Success_Objective[] successObjectives = GetComponentsInChildren<Success_Objective>();

        for (int i = 0; i < successObjectives.Length; i++)
        {
            successObjectives[i].IncrementSuccessObjective();
        }
        
        Destroy(gameObject);
    }

    // To know in other scripts if current npc is dead
    public bool IsDead()
    {
        return isDead;
    }

    // A mirror simplified version of TakeDamage() method to apply damage over time on enemy.
    // I created this one because of an issue be discovered :
    // I add a security to avoid enemy to take all arrow damage at the same time of the multiple arrow ability.
    // But with this security, when a damage over time is applied, player could be not able to deal damage with a new attack on the enemy while he take damage over time.
    public void TakeDamageOverTime(int amount)
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
    }

    public void TakeDamage(int amount, bool playSound)
    {
        if (isDead)
            return;

        // Security to avoid multiple arrow to deal n*damage on the same enemy
        if (Time.time < timeLastHit + 0.01f)
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

        timeLastHit = Time.time;

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

    // Methods to apply a push effect when enemy get hitted by the player.
    // We must have acces to AI_Enemy_Movement to disable it (and the fact it block velocity to 0 when enemy's attacking).
    // Then use a coroutine to re active that after the delay of push is done.
    // We can get Player_Combat directly as parameter because we call this method from Player_Combat script.
    public void GetHit(float _delay, float _power, AI_Enemy_Movement _movement, Player_Combat _combat)
    {
        // If we havn't acces to AI_Enemy_Movement, no need to continue
        if (GetComponent<AI_Enemy_Movement>() && !immuneToPush)
        {
            StartCoroutine(GetHitCoroutine(_delay, _power, _movement, _combat));
        }
    }

    IEnumerator GetHitCoroutine(float _delay, float _power, AI_Enemy_Movement _movement, Player_Combat _combat)
    {
        _movement.hitted = true;

        Rigidbody2D curRb = GetComponent<Rigidbody2D>();

        if (curRb)
        {
            curRb = GetComponent<Rigidbody2D>();
            curRb.velocity = Vector2.zero;

            // We must know from where player hit the enemy to propulse it in the right place.
            // To do it : Get acces to the player fire point. With its z rotation we can know.
            // Right = 270, Left = 90, Up = 0, Down = 180
            // Try to acces to Player_Combat for its method float GetFirePointRotationZ().
            if (_combat)
            {
                float zRot = _combat.GetFirePointRotationZ();
                
                if (zRot >= -1 && zRot <= 1)
                {
                    // Push upward
                    curRb.velocity = Vector2.up * _power;
                }
                else if (zRot >= 179 && zRot <= 181)
                {
                    // Push backward
                    curRb.velocity = -Vector2.up * _power;
                }
                else if (zRot >= 89 && zRot <= 91)
                {
                    // Push leftward
                    curRb.velocity = -Vector2.right * _power;
                }
                else if (zRot >= 269 && zRot <= 271)
                {
                    // Push rightward
                    curRb.velocity = Vector2.right * _power;
                }
                else
                {
                    Debug.Log("Dont know the rotation to push the enemy.");
                }
            }
        }

        yield return new WaitForSeconds(_delay);

        if (curRb)
        {
            curRb.velocity = Vector2.zero;
        }

        // Adding a mini delay to wait just a bit when enemy reach and of pushed pos to come back to the player.
        yield return new WaitForSeconds(.1f);

        _movement.hitted = false;
    }
}
