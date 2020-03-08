/* Health.cs
Utilisé pour gérer la vie du joueur
*/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Player_Combat_Control))] // To know when we can regenerate health or not (with IsInCombat variable)
public class Player_Health : MonoBehaviour
{
    [SerializeField] float healthRegenerationTimer = 2f;
    float currentRegenerationTimer;
    float regenerationDiviser = 70f; // I found 70 with testing some numbers. Calcul will be : CurrentHealthPoints += (MaxHealthPoints / regenerationDiviser)

    [Tooltip("Let it null if you don't want fading when damage taken.")]
    [SerializeField] SpriteRenderer rend;

    Player_Combat_Control player_combat;

    // Start is called before the first frame update
    void Start()
    {
        player_combat = GetComponent<Player_Combat_Control>();

        currentRegenerationTimer = healthRegenerationTimer;
    }

    // Update is called once per frame
    void Update()
    {
        // Just a security if player decrement vitality points for exemple.
        if (Player_Stats.stats_instance.getCurrentHealthPoints() > Player_Stats.stats_instance.getTotalHealthPoints())
        {
            Player_Stats.stats_instance.SetCurrentHealthPoints(Player_Stats.stats_instance.getTotalHealthPoints());
            // Then refresh UI via playerStats class
            if (Player_Stats.stats_instance.playerStatsUI) // if its not null
            {
                Player_Stats.stats_instance.playerStatsUI.RefreshStatsDisplay();
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
        if (Player_Stats.stats_instance.getCurrentHealthPoints() >= Player_Stats.stats_instance.getTotalHealthPoints())
        {
            return;
        }

        if (currentRegenerationTimer > 0f)
        {
            currentRegenerationTimer -= Time.deltaTime;
        }
        else
        {
            float tempHealth = Player_Stats.stats_instance.getCurrentHealthPoints() + (Player_Stats.stats_instance.getTotalHealthPoints() / regenerationDiviser); // Get the temp healthpoints by maths
            if (tempHealth > Player_Stats.stats_instance.getTotalHealthPoints()) // If player got more than total health points
            {
                Player_Stats.stats_instance.SetCurrentHealthPoints(Player_Stats.stats_instance.getTotalHealthPoints()); // set healthpoints to total
            }
            else
            {
                Player_Stats.stats_instance.SetCurrentHealthPoints((int)tempHealth); // else set by temp healthpoints
            }

            // Then refresh UI via playerStats class
            if (Player_Stats.stats_instance.playerStatsUI) // if its not null
            {
                Player_Stats.stats_instance.playerStatsUI.RefreshStatsDisplay();
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
        // int tempDamageAmount = Mathf.RoundToInt(amount - (Player_Stats.stats_instance.getArmor() / 9.9f)); TODO CHANGE
        int tempDamageAmount = amount;

        if (tempDamageAmount < 0)
        {
            tempDamageAmount = 0;
        }

        int tempcurrentHealthPoints = Player_Stats.stats_instance.getCurrentHealthPoints() - tempDamageAmount;

        if (tempcurrentHealthPoints <= 0) // To be sure we never get negative healthpoints
        {
            tempcurrentHealthPoints = 0;
        }

        Player_Stats.stats_instance.SetCurrentHealthPoints(tempcurrentHealthPoints); // Then set healthpoint

        // If player take damage when he's already in the stats panel
        Player_Stats.stats_instance.playerStatsUI.RefreshStatsDisplay();

        if (Player_Stats.stats_instance.getCurrentHealthPoints() <= 0)
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
