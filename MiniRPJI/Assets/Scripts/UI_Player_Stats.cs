using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_Player_Stats : MonoBehaviour
{
    [SerializeField] Text playerLevel;
    [SerializeField] Text currentExp;
    [SerializeField] Text nextLevelExp;

    [SerializeField] Text minAttackDamage;
    [SerializeField] Text maxAttackDamage;

    [SerializeField] Text strengthStatsPoints;
    [SerializeField] Text agilityStatsPoints;
    [SerializeField] Text vitalityStatsPoints;
    [SerializeField] Text intellectStatsPoints;

    [SerializeField] Text currentStatsPoints;

    [SerializeField] GameObject quitValidation; // Confirmation if player didnt valid stats before quit stats UI

    private int[] removableStatsPoints; // To know when player can remove points or not (an array for know exaclty what pts can be remove or not)

    private void Start()
    {
        removableStatsPoints = new int[4];
        for (int i = 0; i < removableStatsPoints.Length; i++)
        {
            removableStatsPoints[i] = 0;
        }
    }

    public void RefreshStatsText(Player_Stats stats)
    {
        playerLevel.text = stats.getCurrentLevel().ToString();
        currentExp.text = stats.getCurrentExp().ToString();
        nextLevelExp.text = stats.getNextLevelExperience().ToString();

        minAttackDamage.text = stats.getMinDamage().ToString();
        maxAttackDamage.text = stats.getMaxDamage().ToString();

        strengthStatsPoints.text = stats.GetStatsByType(StatsType.STRENGTH).ToString();
        agilityStatsPoints.text = stats.GetStatsByType(StatsType.AGILITY).ToString();
        vitalityStatsPoints.text = stats.GetStatsByType(StatsType.VITALITY).ToString();
        intellectStatsPoints.text = stats.GetStatsByType(StatsType.INTELLECT).ToString();

        currentStatsPoints.text = stats.getCurrentStatsPoints().ToString();
    }

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

    //public void StatsValidation()
    //{
    //    // Player has validate so no removable points
    //    for (int i = 0; i < removableStatsPoints.Length; i++)
    //    {
    //        removableStatsPoints[i] = 0;
    //    }
    //    // track current stats
    //    TrackCurrentStats();
    //    // refresh UI
    //    playerStatsUI.RefreshStatsText(this); // Maybe useless
    //}

    //public void StatsCancel()
    //{
    //    for (int i = 0; i < removableStatsPoints.Length; i++)
    //    {
    //        if (removableStatsPoints[i] > 0)
    //        {
    //            currentStatsPoints += removableStatsPoints[i];
    //            removableStatsPoints[i] = 0;
    //        }
    //    }

    //    // Reset stats as tracked
    //    strength = statsTrack[0];
    //    agility = statsTrack[1];
    //    vitality = statsTrack[2];
    //    intellect = statsTrack[3];
    //    // Track stats then refresh UI
    //    TrackCurrentStats();
    //    playerStatsUI.RefreshStatsText(this);
    //}

    //// We can't use StatsType via OnClick from a button UI.
    //// So we use int for choose stats : 0 = strength, 1 = agility, 2 = vitality, 3 = intellect
    //public void AddStatsPoints(int statsType)
    //{
    //    if (currentStatsPoints > 0)
    //    {
    //        switch (statsType)
    //        {
    //            case 0:
    //                strength++;
    //                removableStatsPoints[0]++;
    //                break;
    //            case 1:
    //                agility++;
    //                removableStatsPoints[1]++;
    //                break;
    //            case 2:
    //                vitality++;
    //                removableStatsPoints[2]++;
    //                break;
    //            case 3:
    //                intellect++;
    //                removableStatsPoints[3]++;
    //                break;
    //            default:
    //                Debug.LogWarning("Don't know what stats to upgrade !");
    //                return;
    //        }

    //        currentStatsPoints--;

    //        // If we're here we're sure we got playerStatsUI not null
    //        playerStatsUI.RefreshStatsText(this);
    //    }

    //}

    //public void RemoveStatsPoints(int statsType)
    //{
    //    if (statsType == 0)
    //    {
    //        if (removableStatsPoints[0] > 0)
    //        {
    //            strength--;
    //            removableStatsPoints[0]--;
    //            currentStatsPoints++;
    //        }
    //    }
    //    else if (statsType == 1)
    //    {
    //        if (removableStatsPoints[1] > 0)
    //        {
    //            agility--;
    //            removableStatsPoints[1]--;
    //            currentStatsPoints++;
    //        }
    //    }
    //    else if (statsType == 2)
    //    {
    //        if (removableStatsPoints[2] > 0)
    //        {
    //            vitality--;
    //            removableStatsPoints[2]--;
    //            currentStatsPoints++;
    //        }
    //    }
    //    else if (statsType == 3)
    //    {
    //        if (removableStatsPoints[3] > 0)
    //        {
    //            intellect--;
    //            removableStatsPoints[3]--;
    //            currentStatsPoints++;
    //        }
    //    }
    //    else
    //    {
    //        Debug.LogWarning("Attention la valeur \"" + statsType + "\" n'est pas reconnu ");
    //    }

    //    // If we're here we're sure we got playerStatsUI not null
    //    playerStatsUI.RefreshStatsText(this);

    //}
}
