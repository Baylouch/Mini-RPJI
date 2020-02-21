/* Stats_Control.cs
 Gère les statistiques du joueur. Par ex: le taux de vitalité augmentera les points de vies etc

*/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum StatsType { STRENGTH, AGILITY, VITALITY, INTELLECT }; // Enum pour différencier les différentes statistiques

public class Player_Stats : MonoBehaviour
{

    public UI_Player_Stats playerStatsUI;

    // Experience is for now set in "Projectile.cs" and "Player_Combat_Control.cs"
    [Header("Experience")]
    [SerializeField] private int level = 1;
    [SerializeField] private int NextLevelExperience = 200;
    [SerializeField] private int currentExperience = 0;

    [Header("Statistiques")]
    [SerializeField] private int strength = 10;
    [SerializeField] private int agility = 10;
    [SerializeField] private int vitality = 10;
    [SerializeField] private int intellect = 10;
    [SerializeField] private int armor = 10;
    [SerializeField] private float criticalRate = 3f;
    [SerializeField] private float rangedCriticalRate = 5f;

    [Header("Attack")]
    [SerializeField] private int damageMin = 10;
    [SerializeField] private int damageMax = 15;

    [Header("Ranged Attack")] // To move or delete
    [SerializeField] private int rangedDamageMin = 5;
    [SerializeField] private int rangedDamageMax = 10;

    [Header("General")]
    [SerializeField] private int totalHealthPoints = 100; // Total player healthpoints
    private int baseHealthPoints = 0; // We need this base for know how much healthpoints without vitality player have (for refreshing stats)
    private int currentHealthPoints; // Player current healthpoints

    private int[] statsTrack; // To keep a track from previous stats when player is upgrading. Usefull until player's validation
    private int currentStatsPoints = 0;

    private int currentDamageMin;
    private int currentDamageMax;
    private int currentRangedDamageMin;
    private int currentRangedDamageMax;

    private float strengthDiviser = .8f; // .8 Seems good for all numbers to calculation (strength / strengthDiviser)
    private float agilityDiviser = .87f; // .87 Seems good for all numbers to calculation (agility / agilityDiviser)
    private int vitalityMultiplier = 2;
    private float intellectMultiplier = 2;

    // Start is called before the first frame update
    void Start()
    {
        statsTrack = new int[4];
        TrackCurrentStats(); // Get track of current stats at start
        baseHealthPoints = totalHealthPoints; // first of all before all healthpoints maths

        RefreshStats(); // refresh stats
        SetCurrentHealthPoints(totalHealthPoints); // Set player healthpoints
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F)) // To centralise in Player_Input.cs later
        {
            if (playerStatsUI) // To active/disable playerStatsUI
            {
                if (playerStatsUI.gameObject.activeSelf)
                {
                    playerStatsUI.gameObject.SetActive(false);
                }
                else
                {
                    playerStatsUI.gameObject.SetActive(true);
                    // playerStatsUI Must have UI_Player_Stats component !
                    playerStatsUI.RefreshStatsDisplay();
                }
            }
        }
    }

    void LevelUp()
    {
        level++; // Add a lvl        

        // Set new player stats
        strength += 2; agility += 2; vitality += 4; intellect += 2; // See StatsBoard sheets for more information
        damageMin += 2; damageMax += 2; rangedDamageMin += 2; rangedDamageMax += 2;
        criticalRate += .5f; rangedCriticalRate += .5f;
        baseHealthPoints += 10; // Basehealthpoints is used for calculation to set player total healthpoints with vitality

        // Get track of the new stats
        TrackCurrentStats();

        // Give 5 stats points to the player
        currentStatsPoints += 5;
        SetCurrentHealthPoints(totalHealthPoints); // refresh currenthealthpoints to set it to max
        NextLevelExperience *= 2; // Set next level XP

        // Refresh Stats
        RefreshStats();
    }

    public void RefreshStats()
    {
        float currentDamageMultiplier = (strength / strengthDiviser);
        // Then use mathf methods to get min integer and max integer for the calcul
        currentDamageMin = (int)Mathf.Min(damageMin + currentDamageMultiplier);
        currentDamageMax = (int)Mathf.Max(damageMax + currentDamageMultiplier);

        float currentRangedDamageMultiplier = (agility / agilityDiviser);
        currentRangedDamageMin = (int)Mathf.Min(rangedDamageMin + currentRangedDamageMultiplier);
        currentRangedDamageMax = (int)Mathf.Max(rangedDamageMax + currentRangedDamageMultiplier);

        // Vitality maths : works if you add or remove vitality points.
        if (baseHealthPoints + (vitality * vitalityMultiplier) != totalHealthPoints)
        {
            totalHealthPoints = baseHealthPoints + (vitality * vitalityMultiplier);
        }

        // To deal with intellect later

        if (playerStatsUI)
        {
            playerStatsUI.RefreshStatsDisplay();
        }
    }

    // Just for know what stats we got for a needed time (exemple, when player start to put new stats points before validation, if he wants to reset, he can)
    public void TrackCurrentStats()
    {
        statsTrack[0] = GetStatsByType(StatsType.STRENGTH);
        statsTrack[1] = GetStatsByType(StatsType.AGILITY);
        statsTrack[2] = GetStatsByType(StatsType.VITALITY);
        statsTrack[3] = GetStatsByType(StatsType.INTELLECT);
    }

    public int GetAttackDamage()
    {
        float tempCritCondition = Random.Range(0, 100);
        if (tempCritCondition <= criticalRate) // Do critical strike
        {
            int criticalAttack = Mathf.RoundToInt((Random.Range(currentDamageMin, currentDamageMax) * 1.5f));
            return criticalAttack;
        }
        else
        {
            int currAttack = (Random.Range(currentDamageMin, currentDamageMax));
            return currAttack;
        }

    }

    public int GetRangedAttackDamage()
    {
        float tempCritCondition = Random.Range(0, 100);
        if (tempCritCondition <= rangedCriticalRate)
        {
            int criticalRangedAttack = Mathf.RoundToInt((Random.Range(currentRangedDamageMin, currentRangedDamageMax) * 1.5f));
            return criticalRangedAttack;
        }
        else
        {
            int currRangedattack = (Random.Range(currentRangedDamageMin, currentRangedDamageMax));
            return currRangedattack;
        }
    }

    public void GetExperience(int amount)
    {
        int tempCurrentExperience = currentExperience + amount; // Put in a temp variable currentExperience + amount
        if (tempCurrentExperience >= NextLevelExperience) // Check if it's >= of required experience to lvl up
        {
            int tempNextLevelExperience = tempCurrentExperience - NextLevelExperience; // Get the "too much" amount of experience
            currentExperience = tempNextLevelExperience; // Set the "too much" into currentExperience
            LevelUp();
        }
        else
        {
            currentExperience = tempCurrentExperience;
            if (playerStatsUI)
            {
                playerStatsUI.RefreshStatsDisplay();
            }
        }
    }

    // Method use in Player_Health (its the way player taking damage)
    public void SetCurrentHealthPoints(int newHealthPoints)
    {
        currentHealthPoints = newHealthPoints;
    }

    #region UI_Player_Stats relative methods

    public void AddCurrentStatsPoints(int amount)
    {
        currentStatsPoints += amount;
    }

    public void RemoveCurrentStatsPoints(int amount)
    {
        currentStatsPoints -= amount;
    }

    public void UseTrackForResetStats()
    {
        // Reset stats as tracked
        strength = statsTrack[0];
        agility = statsTrack[1];
        vitality = statsTrack[2];
        intellect = statsTrack[3];
    }

    public void IncrementStatsByType(StatsType type)
    {
        switch(type)
        {
            case StatsType.STRENGTH:
                strength++;
                break;
            case StatsType.AGILITY:
                agility++;
                break;
            case StatsType.VITALITY:
                vitality++;
                break;
            case StatsType.INTELLECT:
                intellect++;
                break;
            default:
                Debug.LogWarning("TYPE ERROR. Player_Stats.cs / + void IncrementStatsByType()");
                return;
        }
    }

    public void DecrementStatsByType(StatsType type)
    {
        switch (type)
        {
            case StatsType.STRENGTH:
                strength--;
                break;
            case StatsType.AGILITY:
                agility--;
                break;
            case StatsType.VITALITY:
                vitality--;
                break;
            case StatsType.INTELLECT:
                intellect--;
                break;
            default:
                Debug.LogWarning("TYPE ERROR. Player_Stats.cs / + void DecrementStatsByType()");
                return;
        }
    }
    #endregion

    #region getters

    public int GetStatsByType(StatsType type)
    {
        switch (type)
        {
            case StatsType.STRENGTH:
                return strength;
            case StatsType.AGILITY:
                return agility;
            case StatsType.VITALITY:
                return vitality;
            case StatsType.INTELLECT:
                return intellect;
            default:
                Debug.Log("Le type demandé n'est pas reconnu. Stats_Control.cs");
                return -1;
        }
    }

    public int getCurrentMinDamage()
    {
        return currentDamageMin;
    }

    public int getCurrentMaxDamage()
    {
        return currentDamageMax;
    }

    public int getCurrentRangedMinDamage()
    {
        return currentRangedDamageMin;
    }

    public int getCurrentRangedMaxDamage()
    {
        return currentRangedDamageMax;
    }

    public int getCurrentExp()
    {
        return currentExperience;
    }

    public int getNextLevelExperience()
    {
        return NextLevelExperience;
    }

    public int getCurrentStatsPoints()
    {
        return currentStatsPoints;
    }

    public int getCurrentLevel()
    {
        return level;
    }

    public int getCurrentHealthPoints()
    {
        return currentHealthPoints;
    }

    public int getTotalHealthPoints()
    {
        return totalHealthPoints;
    }

    public int getArmorPoints()
    {
        return armor;
    }

    #endregion
}
