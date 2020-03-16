/* Health.cs
Utilisé pour gérer la vie du joueur
*/
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Player_Combat_Control))] // To know when we can regenerate health or not (with IsInCombat variable)
public class Player_Health : MonoBehaviour
{
    [SerializeField] private int totalHealthPoints = 100; // Total player healthpoints
    public int GetTotalHealthPoints()
    {
        return totalHealthPoints;
    }

    private int baseHealthPoints = 0; // We need this base for know how much healthpoints (without vitality multiplier) player have (for refreshing stats)
    public int GetBaseHealthPoints()
    {
        return baseHealthPoints;
    }

    private int currentHealthPoints; // Player current healthpoints
    public int GetCurrentHealthPoints()
    {
        return currentHealthPoints;
    }

    [SerializeField] float healthRegenerationTimer = 2f;
    float currentRegenerationTimer;
    float regenerationMultiplier = 0.01f; // We regenerate 0.01% of our total health

    [SerializeField] GameObject damageText;
    [SerializeField] Transform DamageTextParent;

    [Tooltip("Let it null if you don't want fading when damage taken.")]
    [SerializeField] SpriteRenderer rend;

    Player_Combat_Control player_combat;

    // Start is called before the first frame update
    void Start()
    {
        baseHealthPoints = totalHealthPoints;
        SetCurrentHealthPoints(totalHealthPoints); // Set player healthpoints

        player_combat = GetComponent<Player_Combat_Control>();

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
            if (UI_Player.ui_instance.playerStatsUI) // if its not null
            {
                UI_Player.ui_instance.playerStatsUI.RefreshStatsDisplay();
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
            if (UI_Player.ui_instance.playerStatsUI) // if its not null
            {
                UI_Player.ui_instance.playerStatsUI.RefreshStatsDisplay();
            }

            currentRegenerationTimer = healthRegenerationTimer;
        }
    }

    #region Fade Coroutines

    // A FadeIn and FadeOut coroutine methods to apply on each sprite who doesnt have "TakeDamage" animation
    IEnumerator FadeIn() // Reapear
    {
        StopCoroutine("FadeOut");// Make end of FadeOut Coroutine

        for (float f = 0.05f; f <= 1; f+=0.05f)
        {
            Color c = rend.material.color;
            c.a = f;
            rend.material.color = c;

            yield return new WaitForSeconds(0.01f);
        }
       
        StopCoroutine("FadeIn"); // Make end of this coroutine
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

    public void SetTotalHealthPoints(int newTotal)
    {
        totalHealthPoints = newTotal;
    }

    public void GetDamage(int amount)
    {
        // Reduction of damage amount by % depending of our armor.
        // Full calculation is : CurrentArmor * 0.05 = resultat (% of attack reduction)
        //                       resultat /= 100 to obtain like 0.05 to reduce attack by 5% for exemple
        //                       DamageTaken = TotalDmg - TotalDmg * resultat

        float percentageOfAttackReduction = Player_Stats.stats_instance.getArmor() * Player_Stats.stats_instance.getArmorMultiplier();
        percentageOfAttackReduction /= 100;
        // Security for max attack reduction (60% of the attack max)
        if (percentageOfAttackReduction > 0.60f)
        {
            percentageOfAttackReduction = 0.60f;
        }
        float tempDamageAmount = amount - amount * percentageOfAttackReduction;

        // Display on the screen damage received
        if (damageText)
        {
            GameObject _damagedText = Instantiate(damageText, DamageTextParent);
            _damagedText.GetComponent<Text>().text = amount.ToString();
        }

        float afterDamageHealthPoints = currentHealthPoints - tempDamageAmount;

        SetCurrentHealthPoints(afterDamageHealthPoints); // Then set healthpoint

        // If player take damage when he's already in the stats panel
        UI_Player.ui_instance.playerStatsUI.RefreshStatsDisplay();

        if (currentHealthPoints <= 0)
            Die();
        else if (rend)
        {
            StartCoroutine("FadeOut");
        }
    }

    // TODO Don't destroy gameobject entirely?
    public void Die()
    {
        // Pour que la caméra sur le joueur ne se detruise pas
        if (gameObject.GetComponentInChildren<Camera>())
        {
            gameObject.GetComponentInChildren<Camera>().gameObject.transform.parent = null;
        }

        Destroy(gameObject);
    }
}
