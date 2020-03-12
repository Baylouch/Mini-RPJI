using UnityEngine;

public class Player_Energy : MonoBehaviour
{
    [SerializeField] float energyRegenerationTimer = 2f;
    float currentRegenerationTimer;
    float regenerationMultiplier = 0.15f; // We regenerate 15% of our total energy

    // Start is called before the first frame update
    void Start()
    {
        currentRegenerationTimer = energyRegenerationTimer;
    }

    // Update is called once per frame
    void Update()
    {
        // Just a security if player decrement energy points for exemple.
        if (Player_Stats.stats_instance.getCurrentEnergyPoints() > Player_Stats.stats_instance.getTotalEnergyPoints())
        {
            Player_Stats.stats_instance.SetCurrentEnergyPoints(Player_Stats.stats_instance.getTotalEnergyPoints());
            // Then refresh UI via playerStats class
            if (UI_Player.ui_instance.playerStatsUI) // if its not null
            {
                UI_Player.ui_instance.playerStatsUI.RefreshStatsDisplay();
            }
            return;
        }

        RegenerateEnergy();
    }

    void RegenerateEnergy()
    {
        // If player got max healthpoints, just return.
        if (Player_Stats.stats_instance.getCurrentEnergyPoints() >= Player_Stats.stats_instance.getTotalEnergyPoints())
        {
            return;
        }

        if (currentRegenerationTimer > 0f)
        {
            currentRegenerationTimer -= Time.deltaTime;
        }
        else
        {
            float tempEnergy = Player_Stats.stats_instance.getCurrentEnergyPoints() + Player_Stats.stats_instance.getTotalEnergyPoints() * regenerationMultiplier; // Get the temp healthpoints
            if (tempEnergy > Player_Stats.stats_instance.getTotalEnergyPoints()) // If player got more than total health points
            {
                Player_Stats.stats_instance.SetCurrentEnergyPoints(Player_Stats.stats_instance.getTotalEnergyPoints()); // set healthpoints to total
            }
            else
            {
                Player_Stats.stats_instance.SetCurrentEnergyPoints((int)tempEnergy); // else set by temp healthpoints
            }

            // Then refresh UI via playerStats class
            if (UI_Player.ui_instance.playerStatsUI) // if its not null
            {
                UI_Player.ui_instance.playerStatsUI.RefreshStatsDisplay();
            }

            currentRegenerationTimer = energyRegenerationTimer;
        }
    }
}
