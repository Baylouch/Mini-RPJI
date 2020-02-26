/* Health.cs
Utilisé pour gérer la vie du joueur
*/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Player_Stats))]
[RequireComponent(typeof(Player_Combat_Control))] // To know when we can regenerate health or not (with IsInCombat variable)
public class Player_Health : MonoBehaviour
{
    [SerializeField] float healthRegenerationTimer = 2f;
    float currentRegenerationTimer;
    float regenerationDiviser = 70f; // I found 70 with testing some numbers. Calcul will be : CurrentHealthPoints += (MaxHealthPoints / regenerationDiviser)

    [Tooltip("Let it null if you don't want fading when damage taken.")]
    [SerializeField] SpriteRenderer rend;

    Player_Stats playerStats;
    Player_Combat_Control player_combat;

    // Start is called before the first frame update
    void Start()
    {
        playerStats = GetComponent<Player_Stats>();
        player_combat = GetComponent<Player_Combat_Control>();

        currentRegenerationTimer = healthRegenerationTimer;
    }

    // Update is called once per frame
    void Update()
    {
        // Just a security if player decrement vitality points for exemple.
        if (playerStats.getCurrentHealthPoints() > playerStats.getTotalHealthPoints())
        {
            playerStats.SetCurrentHealthPoints(playerStats.getTotalHealthPoints());
            // Then refresh UI via playerStats class
            if (playerStats.playerStatsUI) // if its not null
            {
                playerStats.playerStatsUI.RefreshStatsDisplay();
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
        if (playerStats.getCurrentHealthPoints() >= playerStats.getTotalHealthPoints())
        {
            return;
        }

        if (currentRegenerationTimer > 0f)
        {
            currentRegenerationTimer -= Time.deltaTime;
        }
        else
        {
            float tempHealth = playerStats.getCurrentHealthPoints() + (playerStats.getTotalHealthPoints() / regenerationDiviser); // Get the temp healthpoints by maths
            if (tempHealth > playerStats.getTotalHealthPoints()) // If player got more than total health points
            {
                playerStats.SetCurrentHealthPoints(playerStats.getTotalHealthPoints()); // set healthpoints to total
            }
            else
            {
                playerStats.SetCurrentHealthPoints((int)tempHealth); // else set by temp healthpoints
            }

            // Then refresh UI via playerStats class
            if (playerStats.playerStatsUI) // if its not null
            {
                playerStats.playerStatsUI.RefreshStatsDisplay();
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

    public void GetDamage(int amount)
    {
        // For now we proceed an easy armor way. We just / by 9.9 total armor points and use it to get less damage (to change later. or not.)
        int tempDamageAmount = Mathf.RoundToInt(amount - (playerStats.GetStatsByType(StatsType.ARMOR) / 9.9f));

        if (tempDamageAmount < 0)
        {
            tempDamageAmount = 0;
        }

        int tempcurrentHealthPoints = playerStats.getCurrentHealthPoints() - tempDamageAmount;

        if (tempcurrentHealthPoints <= 0) // To be sure we never get negative healthpoints
        {
            tempcurrentHealthPoints = 0;
        }

        playerStats.SetCurrentHealthPoints(tempcurrentHealthPoints); // Then set healthpoint

        if (GetComponentInChildren<UI_Player_Stats>()) // If player take damage when he's already in the stats panel
            GetComponentInChildren<UI_Player_Stats>().RefreshStatsDisplay();

        if (playerStats.getCurrentHealthPoints() <= 0)
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
