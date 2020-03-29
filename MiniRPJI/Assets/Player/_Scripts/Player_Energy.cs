using UnityEngine;

public class Player_Energy : MonoBehaviour
{
    [SerializeField] private int totalEnergyPoints = 100; // This is the total energy points (item boost and energy stats included)
    public int GetTotalEnergyPoints()
    {
        return totalEnergyPoints;
    }

    private int baseEnergyPoints = 0; // Its the base points without item bost and energy stats points
    public int GetBaseEnergyPoints()
    {
        return baseEnergyPoints;
    }
    public void SetBaseEnergyPoints(int amount)
    {
        baseEnergyPoints = amount;
    }

    private int currentEnergyPoints;
    public int GetCurrentEnergyPoints()
    {
        return currentEnergyPoints;
    }

    [SerializeField] float energyRegenerationTimer = 2f;
    float currentRegenerationTimer;
    float regenerationMultiplier = 0.15f; // We regenerate 15% of our total energy

    // Start is called before the first frame update
    void Start()
    {
        SetCurrentEnergyPoints(totalEnergyPoints);
        currentRegenerationTimer = energyRegenerationTimer;
    }

    // Update is called once per frame
    void Update()
    {
        // Just a security if player decrement energy points for exemple.
        if (currentEnergyPoints > totalEnergyPoints)
        {
            SetCurrentEnergyPoints(totalEnergyPoints);
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
        if (currentEnergyPoints >= totalEnergyPoints)
        {
            return;
        }

        if (currentRegenerationTimer > 0f)
        {
            currentRegenerationTimer -= Time.deltaTime;
        }
        else
        {
            float tempEnergy = currentEnergyPoints + totalEnergyPoints * regenerationMultiplier; // Get the temp healthpoints
            if (tempEnergy > totalEnergyPoints) // If player got more than total health points
            {
                SetCurrentEnergyPoints(totalEnergyPoints); // set healthpoints to total
            }
            else
            {
                SetCurrentEnergyPoints((int)tempEnergy); // else set by temp healthpoints
            }

            // Then refresh UI via playerStats class
            if (UI_Player.ui_instance.playerStatsUI) // if its not null
            {
                UI_Player.ui_instance.playerStatsUI.RefreshStatsDisplay();
            }

            currentRegenerationTimer = energyRegenerationTimer;
        }
    }

    public void SetCurrentEnergyPoints(float newEnergyPoints)
    {
        int tempEnergy = Mathf.RoundToInt(newEnergyPoints);

        if (tempEnergy < 0)
        {
            currentEnergyPoints = 0;
        }
        else
        {
            currentEnergyPoints = tempEnergy;
        }
    }

    public void SetTotalEnergyPoints(int newTotal)
    {
        totalEnergyPoints = newTotal;
    }
}
