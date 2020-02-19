using System.Collections;
using System.Collections.Generic;
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
    [SerializeField] Button intellectRemoveStatsButton;
    [SerializeField] Button strengthAddStatsButton;
    [SerializeField] Button agilityAddStatsButton;
    [SerializeField] Button vitalityAddStatsButton;
    [SerializeField] Button intellectAddStatsButton;

    [SerializeField] GameObject quitValidation; // Confirmation if player didnt valid stats before quit stats UI

    Player_Stats playerStats;

    private int[] removableStatsPoints; // To know when player can remove points or not (an array for know exaclty what pts can be remove or not)

    private void Start()
    {
        removableStatsPoints = new int[4];
        for (int i = 0; i < removableStatsPoints.Length; i++)
        {
            removableStatsPoints[i] = 0;
        }

        // We suppose we have Player_Stats in parent because UI canvas is attach to player and UI_Player_Stats is on the canvas
        playerStats = GetComponentInParent<Player_Stats>();

        RefreshStatsDisplay();
    }

    void RefreshStatsButtons()
    {
        // If we got stats points, we can add in each stats so
        if (playerStats.getCurrentStatsPoints() > 0)
        {
            strengthAddStatsButton.gameObject.SetActive(true);
            agilityAddStatsButton.gameObject.SetActive(true);
            vitalityAddStatsButton.gameObject.SetActive(true);
            intellectAddStatsButton.gameObject.SetActive(true);
        }
        else
        {
            strengthAddStatsButton.gameObject.SetActive(false);
            agilityAddStatsButton.gameObject.SetActive(false);
            vitalityAddStatsButton.gameObject.SetActive(false);
            intellectAddStatsButton.gameObject.SetActive(false);
        }

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
            intellectRemoveStatsButton.gameObject.SetActive(true);
        }
        else
        {
            intellectRemoveStatsButton.gameObject.SetActive(false);
        }
    }

    public void RefreshStatsDisplay()
    {
        if (!playerStats)
            return;

        // Experience panel
        playerLevel.text = playerStats.getCurrentLevel().ToString();
        currentExp.text = playerStats.getCurrentExp().ToString();
        nextLevelExp.text = playerStats.getNextLevelExperience().ToString();

        // Health and armor panel
        maxHealthPoints.text = playerStats.getHealthPoints().ToString();
        currentHealthPoints.text = playerStats.getCurrentHealthPoints().ToString();
        armor.text = playerStats.getArmorPoints().ToString();

        // Attack panel
        primaryMinAttackDamage.text = playerStats.getCurrentMinDamage().ToString();
        primaryMaxAttackDamage.text = playerStats.getCurrentMaxDamage().ToString();
        secondaryMinAttackDamage.text = playerStats.getCurrentRangedMinDamage().ToString();
        secondaryMaxAttackDamage.text = playerStats.getCurrentRangedMaxDamage().ToString();

        // Stats panel
        strengthStatsPoints.text = playerStats.GetStatsByType(StatsType.STRENGTH).ToString();
        agilityStatsPoints.text = playerStats.GetStatsByType(StatsType.AGILITY).ToString();
        vitalityStatsPoints.text = playerStats.GetStatsByType(StatsType.VITALITY).ToString();
        intellectStatsPoints.text = playerStats.GetStatsByType(StatsType.INTELLECT).ToString();

        currentStatsPoints.text = playerStats.getCurrentStatsPoints().ToString();

        // Refresh stats buttons
        RefreshStatsButtons();
    }

    // To finish
    public void ToggleQuitValidation()
    {
        if (!quitValidation.activeSelf)
        {
            quitValidation.SetActive(true);
        }
        else
        {
            quitValidation.SetActive(false);
        }
    }

    public void StatsValidation()
    {

        // Player has validate so no removable points
        for (int i = 0; i < removableStatsPoints.Length; i++)
        {
            removableStatsPoints[i] = 0;
        }
        // track current stats
        playerStats.TrackCurrentStats();
        playerStats.RefreshStats(); // important to put it before RefreshStatsDisplay for avoid text bug 
        // refresh UI
        RefreshStatsDisplay();
    }

    public void StatsCancel()
    {
        for (int i = 0; i < removableStatsPoints.Length; i++)
        {
            if (removableStatsPoints[i] > 0)
            {
                playerStats.AddCurrentStatsPoints(removableStatsPoints[i]);
                removableStatsPoints[i] = 0;
            }
        }

        // Reset stats as tracked
        playerStats.UseTrackForResetStats();
        playerStats.RefreshStats();// important to put it before RefreshStatsDisplay for avoid text bug 

        RefreshStatsDisplay();
    }

    // We can't use StatsType via OnClick from a button UI.
    // So we use int for choose stats : 0 = strength, 1 = agility, 2 = vitality, 3 = intellect
    public void AddStatsPoints(int statsType)
    {
        if (playerStats.getCurrentStatsPoints() > 0)
        {
            switch (statsType)
            {
                case 0:
                    playerStats.IncrementStatsByType(StatsType.STRENGTH);
                    removableStatsPoints[0]++;
                    break;
                case 1:
                    playerStats.IncrementStatsByType(StatsType.AGILITY);
                    removableStatsPoints[1]++;
                    break;
                case 2:
                    playerStats.IncrementStatsByType(StatsType.VITALITY);
                    removableStatsPoints[2]++;
                    break;
                case 3:
                    playerStats.IncrementStatsByType(StatsType.INTELLECT);
                    removableStatsPoints[3]++;
                    break;
                default:
                    Debug.LogWarning("Don't know what stats to upgrade !");
                    return;
            }

            playerStats.RemoveCurrentStatsPoints(1);
            playerStats.RefreshStats();// important to put it before RefreshStatsDisplay for avoid text bug 

            RefreshStatsDisplay();
        }

    }

    public void RemoveStatsPoints(int statsType)
    {
        if (statsType == 0)
        {
            if (removableStatsPoints[0] > 0)
            {
                playerStats.DecrementStatsByType(StatsType.STRENGTH);
                removableStatsPoints[0]--;
                playerStats.AddCurrentStatsPoints(1);
            }
        }
        else if (statsType == 1)
        {
            if (removableStatsPoints[1] > 0)
            {
                playerStats.DecrementStatsByType(StatsType.AGILITY);
                removableStatsPoints[1]--;
                playerStats.AddCurrentStatsPoints(1);
            }
        }
        else if (statsType == 2)
        {
            if (removableStatsPoints[2] > 0)
            {
                playerStats.DecrementStatsByType(StatsType.VITALITY);
                removableStatsPoints[2]--;
                playerStats.AddCurrentStatsPoints(1);
            }
        }
        else if (statsType == 3)
        {
            if (removableStatsPoints[3] > 0)
            {
                playerStats.DecrementStatsByType(StatsType.INTELLECT);
                removableStatsPoints[3]--;
                playerStats.AddCurrentStatsPoints(1);
            }
        }
        else
        {
            Debug.LogWarning("Attention la valeur \"" + statsType + "\" n'est pas reconnu ");
        }

        playerStats.RefreshStats(); // important to put it before RefreshStatsDisplay for avoid text bug
        RefreshStatsDisplay();
    }
}
