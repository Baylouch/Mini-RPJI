using UnityEngine;
using UnityEngine.UI;

public class UI_Player_Stats : MonoBehaviour
{
    [Header("Stats Texts")]
    [SerializeField] Text playerLevel;
    [SerializeField] Text currentExp;
    [SerializeField] Text nextLevelExp;

    [SerializeField] Text maxHealthPoints;
    [SerializeField] Text currentHealthPoints;
    [SerializeField] Text maxEnergyPoints;
    [SerializeField] Text currentEnergyPoints;
    [SerializeField] Text armor;

    [SerializeField] Text primaryMinAttackDamage;
    [SerializeField] Text primaryMaxAttackDamage;
    [SerializeField] Text secondaryMinAttackDamage;
    [SerializeField] Text secondaryMaxAttackDamage;

    [SerializeField] Text strengthStatsPoints;
    [SerializeField] Text agilityStatsPoints;
    [SerializeField] Text vitalityStatsPoints;
    [SerializeField] Text intellectStatsPoints;

    [SerializeField] Text currentStatsPoints;

    [Header("Stats Buttons")]
    [SerializeField] Button strengthRemoveStatsButton;
    [SerializeField] Button agilityRemoveStatsButton;
    [SerializeField] Button vitalityRemoveStatsButton;
    [SerializeField] Button energyRemoveStatsButton;
    [SerializeField] Button strengthAddStatsButton;
    [SerializeField] Button agilityAddStatsButton;
    [SerializeField] Button vitalityAddStatsButton;
    [SerializeField] Button energyAddStatsButton;

    [SerializeField] Button statsCancel;
    [SerializeField] Button statsValidation;

    private int[] removableStatsPoints; // To know when player can remove points or not (an array for know exaclty what pts can be remove or not)

    private void Start()
    {
        removableStatsPoints = new int[4];
        for (int i = 0; i < removableStatsPoints.Length; i++)
        {
            removableStatsPoints[i] = 0;
        }

        RefreshStatsDisplay();
    }

    void RefreshStatsButtons()
    {
        // If we got stats points, we can add in each stats so
        if (Player_Stats.stats_instance.getCurrentStatsPoints() > 0)
        {
            strengthAddStatsButton.gameObject.SetActive(true);
            agilityAddStatsButton.gameObject.SetActive(true);
            vitalityAddStatsButton.gameObject.SetActive(true);
            energyAddStatsButton.gameObject.SetActive(true);
        }
        else
        {
            strengthAddStatsButton.gameObject.SetActive(false);
            agilityAddStatsButton.gameObject.SetActive(false);
            vitalityAddStatsButton.gameObject.SetActive(false);
            energyAddStatsButton.gameObject.SetActive(false);
        }

        if (removableStatsPoints != null) // Idk why but if we don't test it, we'll get error at start of the game.
        {
            // Then if we got removable stats points, we need to test each stats for know what we can remove or not
            if (removableStatsPoints[0] > 0) // strength
            {
                strengthRemoveStatsButton.gameObject.SetActive(true);
            }
            else
            {
                strengthRemoveStatsButton.gameObject.SetActive(false);
            }
            if (removableStatsPoints[1] > 0) // agility
            {
                agilityRemoveStatsButton.gameObject.SetActive(true);
            }
            else
            {
                agilityRemoveStatsButton.gameObject.SetActive(false);
            }
            if (removableStatsPoints[2] > 0) // Vitality
            {
                vitalityRemoveStatsButton.gameObject.SetActive(true);
            }
            else
            {
                vitalityRemoveStatsButton.gameObject.SetActive(false);
            }
            if (removableStatsPoints[3] > 0) // Intellect
            {
                energyRemoveStatsButton.gameObject.SetActive(true);
            }
            else
            {
                energyRemoveStatsButton.gameObject.SetActive(false);
            }

            // Now we check if player use a stats points to display validation or cancel button
            for (int i = 0; i < removableStatsPoints.Length; i++)
            {
                if (removableStatsPoints[i] > 0)
                {
                    if (!statsCancel.gameObject.activeSelf)
                        statsCancel.gameObject.SetActive(true);
                    if (!statsValidation.gameObject.activeSelf)
                        statsValidation.gameObject.SetActive(true);

                    return;
                }
            }
        }       
    }

    public void RefreshStatsDisplay()
    {
        if (!Player_Stats.stats_instance)
            return;

        // Experience panel
        playerLevel.text = Player_Stats.stats_instance.getCurrentLevel().ToString();
        currentExp.text = Player_Stats.stats_instance.getCurrentExp().ToString();
        nextLevelExp.text = Player_Stats.stats_instance.getTotalLevelExp().ToString();

        // Health and armor panel
        maxHealthPoints.text = Player_Stats.stats_instance.playerHealth.GetTotalHealthPoints().ToString();
        currentHealthPoints.text = Player_Stats.stats_instance.playerHealth.GetCurrentHealthPoints().ToString();
        maxEnergyPoints.text = Player_Stats.stats_instance.playerEnergy.GetTotalEnergyPoints().ToString();
        currentEnergyPoints.text = Player_Stats.stats_instance.playerEnergy.GetCurrentEnergyPoints().ToString();
        armor.text = Player_Stats.stats_instance.getArmor().ToString();

        // Attack panel
        primaryMinAttackDamage.text = Player_Stats.stats_instance.getCurrentMinDamage().ToString();
        primaryMaxAttackDamage.text = Player_Stats.stats_instance.getCurrentMaxDamage().ToString();
        secondaryMinAttackDamage.text = Player_Stats.stats_instance.getCurrentRangedMinDamage().ToString();
        secondaryMaxAttackDamage.text = Player_Stats.stats_instance.getCurrentRangedMaxDamage().ToString();

        // Stats panel
        strengthStatsPoints.text = Player_Stats.stats_instance.GetCurrentStatsByType(StatsType.STRENGTH).ToString();
        agilityStatsPoints.text = Player_Stats.stats_instance.GetCurrentStatsByType(StatsType.AGILITY).ToString();
        vitalityStatsPoints.text = Player_Stats.stats_instance.GetCurrentStatsByType(StatsType.VITALITY).ToString();
        intellectStatsPoints.text = Player_Stats.stats_instance.GetCurrentStatsByType(StatsType.ENERGY).ToString();

        currentStatsPoints.text = Player_Stats.stats_instance.getCurrentStatsPoints().ToString();

        // Refresh stats buttons
        RefreshStatsButtons();
    }

    public void StatsValidation()
    {

        // Player has validate so no removable points
        for (int i = 0; i < removableStatsPoints.Length; i++)
        {
            removableStatsPoints[i] = 0;
        }
        // Hide validation&cancel buttons
        statsCancel.gameObject.SetActive(false);
        statsValidation.gameObject.SetActive(false);

        // track current stats
        Player_Stats.stats_instance.TrackCurrentStats();
        Player_Stats.stats_instance.RefreshPlayerStats();
    }

    public void StatsCancel()
    {
        for (int i = 0; i < removableStatsPoints.Length; i++)
        {
            if (removableStatsPoints[i] > 0)
            {
                Player_Stats.stats_instance.AddCurrentStatsPoints(removableStatsPoints[i]);
                for (int j = 0; j < removableStatsPoints[i]; j++)
                {
                    Player_Stats.stats_instance.DecrementStatsByType((StatsType)i); // While its enum, and we were strict on how we code, its ok to use index of an enum
                }
                removableStatsPoints[i] = 0;
            }
        }
        // Hide validation&cancel buttons
        statsValidation.gameObject.SetActive(false);
        statsCancel.gameObject.SetActive(false);
        Player_Stats.stats_instance.RefreshPlayerStats();
    }

    // We can't use StatsType via OnClick from a UI button.
    // So we use int for choose stats : 0 = strength, 1 = agility, 2 = vitality, 3 = intellect
    public void AddStatsPoints(int statsType)
    {
        if (Player_Stats.stats_instance.getCurrentStatsPoints() > 0)
        {
            switch (statsType)
            {
                case 0:
                    Player_Stats.stats_instance.IncrementStatsByType(StatsType.STRENGTH);
                    removableStatsPoints[0]++;
                    break;
                case 1:
                    Player_Stats.stats_instance.IncrementStatsByType(StatsType.AGILITY);
                    removableStatsPoints[1]++;
                    break;
                case 2:
                    Player_Stats.stats_instance.IncrementStatsByType(StatsType.VITALITY);
                    removableStatsPoints[2]++;
                    break;
                case 3:
                    Player_Stats.stats_instance.IncrementStatsByType(StatsType.ENERGY);
                    removableStatsPoints[3]++;
                    break;
                default:
                    Debug.LogWarning("Don't know what stats to upgrade !");
                    return;
            }

            Player_Stats.stats_instance.RemoveCurrentStatsPoints(1);
            Player_Stats.stats_instance.RefreshPlayerStats();
        }
    }

    public void RemoveStatsPoints(int statsType)
    {
        switch(statsType)
        {
            case 0:
                if (removableStatsPoints[0] > 0)
                {
                    Player_Stats.stats_instance.DecrementStatsByType(StatsType.STRENGTH);
                    removableStatsPoints[0]--;
                    Player_Stats.stats_instance.AddCurrentStatsPoints(1);           
                }
                break;
            case 1:
                if (removableStatsPoints[1] > 0)
                {
                    Player_Stats.stats_instance.DecrementStatsByType(StatsType.AGILITY);
                    removableStatsPoints[1]--;
                    Player_Stats.stats_instance.AddCurrentStatsPoints(1);
                }
                break;
            case 2:
                if (removableStatsPoints[2] > 0)
                {
                    Player_Stats.stats_instance.DecrementStatsByType(StatsType.VITALITY);
                    removableStatsPoints[2]--;
                    Player_Stats.stats_instance.AddCurrentStatsPoints(1);
                }
                break;
            case 3:
                if (removableStatsPoints[3] > 0)
                {
                    Player_Stats.stats_instance.DecrementStatsByType(StatsType.ENERGY);
                    removableStatsPoints[3]--;
                    Player_Stats.stats_instance.AddCurrentStatsPoints(1);
                }
                break;
            default:
                Debug.LogWarning("Attention la valeur \"" + statsType + "\" n'est pas reconnu ");
                return;
        }

        Player_Stats.stats_instance.RefreshPlayerStats();
    }
}
