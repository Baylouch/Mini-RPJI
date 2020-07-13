/* Player_Health.cs
 * 
 * Utilisé pour gérer la vie du joueur
 *
 * 
 */

using System.Collections;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Player_Combat))] // To know when we can regenerate health or not (with IsInCombat variable)
public class Player_Health : MonoBehaviour, IDamageable
{
    [SerializeField] private int totalHealthPoints = 100; // Total player healthpoints
    public int GetTotalHealthPoints()
    {
        return totalHealthPoints;
    }
    public void SetTotalHealthPoints(int newTotal)
    {
        totalHealthPoints = newTotal;
    }

    private int baseHealthPoints = 0; // We need this base for know how much healthpoints (without vitality multiplier) player have (for refreshing stats)
    public int GetBaseHealthPoints()
    {
        return baseHealthPoints;
    }
    public void SetBaseHealthPoints(int amount)
    {
        baseHealthPoints = amount;
    }

    private int currentHealthPoints; // Player current healthpoints
    public int GetCurrentHealthPoints()
    {
        return currentHealthPoints;
    }
    public void SetCurrentHealthPoints(float newHealthPoints)
    {
        int tempHealth = Mathf.RoundToInt(newHealthPoints);

        if (newHealthPoints < 0)
        {
            currentHealthPoints = 0;
        }
        else
        {
            currentHealthPoints = tempHealth;
        }
    }

    [SerializeField] float healthRegenerationTimer = 2f;
    float currentRegenerationTimer;
    float regenerationMultiplier = 0.01f; // We regenerate 1% of our total health

    [SerializeField] GameObject damageText;
    [SerializeField] Transform DamageTextParent;

    [Tooltip("Let it null if you don't want fading when damage taken.")]
    [SerializeField] SpriteRenderer rend;

    [SerializeField] GameObject hurtEffectUI; // GameObject to instantiate when player take damage (dont die) and parent to UI_Player
    GameObject currentHurtEffect; // TO destroy if player dies

    Player_Combat player_combat;
    MalusApplier player_MalusApplier;
    float lastDamagedTextXPosition = 0;
    bool isDead = false;
    public bool IsDead()
    {
        return isDead;
    }

    float timeLastHit = 0f; // To make a little timer before player can take more damage just after been hit

    private void OnDisable()
    {
        // Security when player die when hes on "alpha" because taking damage
        Color c = rend.material.color;
        c.a = 1;
        rend.material.color = c;
    }

    // Start is called before the first frame update
    void Start()
    {
        SetCurrentHealthPoints(totalHealthPoints); // Set player healthpoints

        player_combat = GetComponent<Player_Combat>();
        player_MalusApplier = GetComponent<MalusApplier>();

        currentRegenerationTimer = healthRegenerationTimer;
    }

    // Update is called once per frame
    void Update()
    {
        // Just a security if player decrement vitality points for exemple.
        if (currentHealthPoints > totalHealthPoints)
        {
            SetCurrentHealthPoints(totalHealthPoints);
            // Then refresh UI via UI_Player instance
            if (UI_Player.instance.playerStatsUI) // if its not null
            {
                UI_Player.instance.playerStatsUI.RefreshStatsDisplay();
            }
            return;
        }

        // If player is in combat dont regenerate health
        if (player_combat.isInCombat)
        {
            return;
        }
        else // Else regenerate
        {
            RegenerateHealth();
        }
    }

    void RegenerateHealth()
    {
        // If player got max healthpoints, just return.
        if (currentHealthPoints >= totalHealthPoints)
        {
            return;
        }

        if (currentRegenerationTimer > 0f)
        {
            currentRegenerationTimer -= Time.deltaTime;
        }
        else
        {
            float tempHealth = currentHealthPoints + totalHealthPoints * regenerationMultiplier; // Get the temp healthpoints
            if (tempHealth > totalHealthPoints) // If player got more than total health points
            {
                SetCurrentHealthPoints(totalHealthPoints); // set healthpoints to total
            }
            else
            {
                SetCurrentHealthPoints((int)tempHealth); // else set by temp healthpoints
            }

            // Then refresh UI via UI_Player instance
            if (UI_Player.instance.playerStatsUI) // if its not null
            {
                UI_Player.instance.playerStatsUI.RefreshStatsDisplay();
            }

            currentRegenerationTimer = healthRegenerationTimer;
        }
    }

    #region Fade Coroutines

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

    void DisplayDamagedTextUI(int amount)
    {
        if (damageText)
        {
            GameObject _damagedText = Instantiate(damageText, DamageTextParent); // Create damagedText GO
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

    public void TakeDamage(int amount, bool playSound)
    {
        // First of all, check if player entered godMode. If yes, just return before all calculation.
        if (Cheats.instance)
        {
            if (Cheats.instance.godModeActived)
            {
                return;
            }
        }

        if (Time.time < timeLastHit + .3f)
        {
            return;
        }

        // Reduction of damage amount by % depending of our armor.
        // Full calculation is : CurrentArmor * 0.05 = resultat (% of attack reduction)
        //                       resultat /= 100 to obtain like 0.05 to reduce attack by 5% for exemple
        //                       DamageTaken = TotalDmg - TotalDmg * resultat

        float percentageOfAttackReduction = Player_Stats.instance.GetArmor() * Player_Stats.instance.GetArmorMultiplier();
        percentageOfAttackReduction /= 100;
        // Security for max attack reduction (60% of the attack max)
        if (percentageOfAttackReduction > 0.60f)
        {
            percentageOfAttackReduction = 0.60f;
        }
        float tempDamageAmount = amount - amount * percentageOfAttackReduction;

        // Display on the screen damage received
        DisplayDamagedTextUI(amount);

        float afterDamageHealthPoints = currentHealthPoints - tempDamageAmount;

        SetCurrentHealthPoints(afterDamageHealthPoints); // Then set healthpoint

        timeLastHit = Time.time; // Set the timer

        // If player take damage when he's already in the stats panel
        UI_Player.instance.playerStatsUI.RefreshStatsDisplay();

        if (currentHealthPoints <= 0)
        {
            Die();
        }
        else if (rend)
        {
            StartCoroutine("FadeOut");

            // Set HurtEffect UI
            if (UI_Player.instance)
            {
                if (hurtEffectUI)
                {
                    if (currentHurtEffect == null)
                    {
                        currentHurtEffect = Instantiate(hurtEffectUI, UI_Player.instance.transform); // Its destroy itself in HurtEffect.cs

                        // Get current healthpoints as percentage then test it to decrease transparency
                        float healthAsPercentage = (currentHealthPoints * 100) / totalHealthPoints;

                        if (healthAsPercentage > 75)
                        {
                            currentHurtEffect.GetComponent<HurtEffect>().alphaLimit = .2f;
                        }
                        else if (healthAsPercentage <= 75 && healthAsPercentage > 50)
                        {
                            currentHurtEffect.GetComponent<HurtEffect>().alphaLimit = .5f;
                        }
                        else if (healthAsPercentage <= 50 && healthAsPercentage > 20)
                        {
                            currentHurtEffect.GetComponent<HurtEffect>().alphaLimit = .75f;
                        }
                        else
                        {
                            currentHurtEffect.GetComponent<HurtEffect>().alphaLimit = 1f;
                        }

                        currentHurtEffect.GetComponent<HurtEffect>().StartFadeEffect();
                    }
                }
            }
        }
    }
    
    public void Die()
    {
        isDead = true;

        if (player_MalusApplier)
            player_MalusApplier.RemoveCurrentEffect();

        FloatingText[] floatingTexts = FindObjectsOfType<FloatingText>();
        for (int i = 0; i < floatingTexts.Length; i++)
        {
            Destroy(floatingTexts[i].gameObject);
        }

        // it'll get all ai combat control in the scene to reset it because there is an issue when ai kill the player.
        AI_Enemy_Combat[] aI_Combat_Controls = FindObjectsOfType<AI_Enemy_Combat>(); 
        for (int i = 0; i < aI_Combat_Controls.Length; i++)
        {
            aI_Combat_Controls[i].TurnOffAnimatorParamAttack();
        }

        player_combat.isInCombat = false;
        player_combat.endingCombat = false;
        player_combat.ResetEndingCombatTimer();

        if (UI_Player.instance)
        {
            if (UI_Player.instance.gameOverUI)
            {
                UI_Player.instance.gameOverUI.gameObject.SetActive(true);
            }
        }

        Camera.main.transform.parent = null;
        gameObject.SetActive(false);
    }
}
