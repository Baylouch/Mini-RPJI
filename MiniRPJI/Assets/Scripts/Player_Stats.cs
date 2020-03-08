/* Stats_Control.cs
 Gère les statistiques du joueur. Par ex: le taux de vitalité augmentera les points de vies etc
 Ce script doit être executé dans les premiers afin d'éviter null reference si le joueur a des objets pré-équipés avant de commencer
*/
using UnityEngine;


public enum StatsType { STRENGTH, AGILITY, VITALITY, INTELLECT }; // Enum pour différencier les différentes statistiques

public class Player_Stats : MonoBehaviour
{
    public static Player_Stats stats_instance;

    public UI_Player_Stats playerStatsUI;

    // Experience is for now set in "Projectile.cs" and "Player_Combat_Control.cs"
    [Header("Experience")]
    [SerializeField] private int level = 1;
    [SerializeField] private int totalLevelExp = 200;
    [SerializeField] private int currentExp = 0;

    [Header("Statistiques")]
    [SerializeField] private int baseStrength = 10; // Used as base to keep track on it for save and load functionnality 
    [SerializeField] private int baseAgility = 10; // Because if you lvl up, you win base points add.
    [SerializeField] private int baseVitality = 10; // And if you equip a weapon, its not base points but current points
    [SerializeField] private int baseIntellect = 10;
    [SerializeField] private float criticalRate = 3f;
    [SerializeField] private float rangedCriticalRate = 5f;

    // Current stats used in game. Addition of baseStats + weaponStats.
    private int currentStrength;
    private int currentAgility;
    private int currentVitality;
    private int currentIntellect;
    private int armor;

    [Header("Attack")]
    [SerializeField] private int damageMin = 1;
    [SerializeField] private int damageMax = 3;

    [Header("General")]
    [SerializeField] private int totalHealthPoints = 100; // Total player healthpoints
    [SerializeField] private int totalManaPoints = 100;

    private int baseHealthPoints = 0; // We need this base for know how much healthpoints (without vitality multiplier) player have (for refreshing stats)
    private int currentHealthPoints; // Player current healthpoints
    private int baseManaPoints = 0;
    private int currentManaPoints;

    private int[] statsTrack; // To keep a track from previous stats when player is upgrading. Usefull until player's validation
    private int currentStatsPoints = 0;

    private int currentDamageMin;
    private int currentDamageMax;
    private int currentRangedDamageMin;
    private int currentRangedDamageMax;

    private float strenghtMultiplier = .3f; // .3 Seems good for all numbers to calculation (strength * strenghtMultiplier)
    private float agilityMultiplier = .4f; // .4 Seems good for all numbers to calculation (agility * agilityMultiplier)
    private float vitalityMultiplier = 2;
    private float intellectMultiplier = 1.5f;

    private void Awake()
    {
        // Make it singleton
        if (!stats_instance)
        {
            stats_instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        statsTrack = new int[4];

        baseHealthPoints = totalHealthPoints; // first of all before all healthpoints maths
        baseManaPoints = totalManaPoints;

        RefreshPlayerStats(); // refresh stats
        SetCurrentHealthPoints(totalHealthPoints); // Set player healthpoints
        SetCurrentManaPoints(totalManaPoints);

        TrackCurrentStats(); // Get track of current stats at start
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F)) // To centralise in Player_Input.cs later
        {
            ToggleStatsMenu();
        }
    }

    public void ToggleStatsMenu()
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

    void LevelUp()
    {
        level++; // Add a lvl        

        // Set new base player stats
        baseStrength += 2; baseAgility += 2; baseVitality += 4; baseIntellect += 2; // See StatsBoard sheets for more information

        // Get track of the new stats
        TrackCurrentStats();

        // Give 5 stats points to the player
        currentStatsPoints += 5;
        SetCurrentHealthPoints(totalHealthPoints); // refresh currenthealthpoints to set it to max
        SetCurrentManaPoints(totalManaPoints);
        totalLevelExp *= 2; // Set next level XP

        // Refresh Stats
        RefreshPlayerStats();
    }

    // Methods used in Refresh Player Stats
    void ApplyItemStats(EquipmentItem item)
    {
        if (item.strength != 0)
        {
            currentStrength += item.strength;
        }
        if (item.agility != 0)
        {
            currentAgility += item.agility;
        }
        if (item.vitality != 0)
        {
            currentVitality += item.vitality;
        }
        if (item.intellect != 0)
        {
            currentIntellect += item.intellect;
        }
        if (item.armor != 0)
        {
            armor += item.armor;
        }
        if (item.criticalRate != 0)
        {
            criticalRate += item.criticalRate;
        }
        if (item.rangedCriticalRate != 0)
        {
            rangedCriticalRate += item.rangedCriticalRate;
        }
        if (item.damageMin != 0)
        {
            damageMin += item.damageMin;
        }
        if (item.damageMax != 0)
        {
            damageMax += item.damageMax;
        }
        if (item.healthpoints != 0)
        {
            baseHealthPoints += item.healthpoints;
        }

        // Get track of the new stats
        TrackCurrentStats();
    }

    public void RefreshPlayerStats()
    {
        // Refresh current statistiques
        currentStrength = baseStrength; currentAgility = baseAgility; currentVitality = baseVitality; currentIntellect = baseIntellect;
        armor = 0;
        // Now check armory slot for know if there is an item. If yes, apply stats item.
        // We know there is 6 armory slots because of the enum ArmoryPart. All armory slot got a unique name with this.
        for (int i = 0; i < 6; i++)
        {
            if (Player_Inventory.inventory_instance.GetArmorySlotItem(i) != null)
            {
                ApplyItemStats(Player_Inventory.inventory_instance.GetArmorySlotItem(i));
            }
        }

        float currentDamageMultiplier = (currentStrength * strenghtMultiplier);
        // Then use mathf methods to get min integer and max integer for the calcul
        currentDamageMin = (int)Mathf.Round(damageMin + currentDamageMultiplier);
        currentDamageMax = (int)Mathf.Round(damageMax + currentDamageMultiplier);

        // Do it only if player got bow equiped. Else player can't use ranged attack anyway
        if (Player_Inventory.inventory_instance.GetCurrentBow())
        {
            float currentRangedDamageMultiplier = (currentAgility * agilityMultiplier);
            currentRangedDamageMin = (int)Mathf.Round(Player_Inventory.inventory_instance.GetCurrentBow().rangedDamageMin + currentRangedDamageMultiplier);
            currentRangedDamageMax = (int)Mathf.Round(Player_Inventory.inventory_instance.GetCurrentBow().rangedDamageMax + currentRangedDamageMultiplier);
        }
        else
        {
            currentRangedDamageMin = 0;
            currentRangedDamageMax = 0;
        }

        // Vitality maths : works if you add or remove vitality points.
        if (baseHealthPoints + (currentVitality * vitalityMultiplier) != totalHealthPoints)
        {
            totalHealthPoints = (int)Mathf.Max(baseHealthPoints + (currentVitality * vitalityMultiplier));
        }

        // same as vitality maths
        if (baseManaPoints + (currentIntellect * intellectMultiplier) != totalManaPoints)
        {
            totalManaPoints = (int)Mathf.Max(baseManaPoints + (currentIntellect * intellectMultiplier));
        }

        if (playerStatsUI)
        {
            playerStatsUI.RefreshStatsDisplay();
        }
    }

    // Just for know what stats we got for a needed time (exemple, when player start to put new stats points before validation, if he wants to reset, he can)
    public void TrackCurrentStats()
    {
        statsTrack[0] = GetCurrentStatsByType(StatsType.STRENGTH);
        statsTrack[1] = GetCurrentStatsByType(StatsType.AGILITY);
        statsTrack[2] = GetCurrentStatsByType(StatsType.VITALITY);
        statsTrack[3] = GetCurrentStatsByType(StatsType.INTELLECT);
    }

    public void AddExperience(int amount)
    {
        int tempCurrentExperience = currentExp + amount; // Put in a temp variable currentExperience + amount
        if (tempCurrentExperience >= totalLevelExp) // Check if it's >= of required experience to lvl up
        {
            int tempNextLevelExperience = tempCurrentExperience - totalLevelExp; // Get the "too much" amount of experience
            currentExp = tempNextLevelExperience; // Set the "too much" into currentExperience
            LevelUp();
        }
        else
        {
            currentExp = tempCurrentExperience;
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

    // For later to use spell
    public void SetCurrentManaPoints(int newManaPoints)
    {
        currentManaPoints = newManaPoints;
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

    // Here, we need to increment baseStats AND currentStats
    public void IncrementStatsByType(StatsType type)
    {
        switch(type)
        {
            case StatsType.STRENGTH:
                baseStrength++;
                currentStrength++;
                break;
            case StatsType.AGILITY:
                baseAgility++;
                currentAgility++;
                break;
            case StatsType.VITALITY:
                baseVitality++;
                currentVitality++;
                break;
            case StatsType.INTELLECT:
                baseIntellect++;
                currentIntellect++;
                break;
            default:
                Debug.LogWarning("TYPE ERROR. Player_Stats.cs / + void IncrementStatsByType()");
                return;
        }
    }
    // Here, we need to decrement baseStats AND currentStats
    public void DecrementStatsByType(StatsType type)
    {
        switch (type)
        {
            case StatsType.STRENGTH:
                baseStrength--;
                currentStrength--;
                break;
            case StatsType.AGILITY:
                baseAgility--;
                currentAgility--;
                break;
            case StatsType.VITALITY:
                baseVitality--;
                currentVitality--;
                break;
            case StatsType.INTELLECT:
                baseIntellect--;
                currentIntellect--;
                break;
            default:
                Debug.LogWarning("TYPE ERROR. Player_Stats.cs / + void DecrementStatsByType()");
                return;
        }
    }
    #endregion

    #region getters

    public int GetCurrentStatsByType(StatsType type)
    {
        switch (type)
        {
            case StatsType.STRENGTH:
                return currentStrength;
            case StatsType.AGILITY:
                return currentAgility;
            case StatsType.VITALITY:
                return currentVitality;
            case StatsType.INTELLECT:
                return currentIntellect;
            default:
                Debug.Log("Le type demandé n'est pas reconnu. Stats_Control.cs");
                return -1;
        }
    }

    public int GetBaseStatsByType(StatsType type)
    {
        switch (type)
        {
            case StatsType.STRENGTH:
                return baseStrength;
            case StatsType.AGILITY:
                return baseAgility;
            case StatsType.VITALITY:
                return baseVitality;
            case StatsType.INTELLECT:
                return baseIntellect;
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
        return currentExp;
    }

    public int getTotalLevelExp()
    {
        return totalLevelExp;
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

    public int getArmor()
    {
        return armor;
    }

    public float getCriticalRate()
    {
        return criticalRate;
    }
    
    public float getRangedCriticalRate()
    {
        return rangedCriticalRate;
    }

    public int getTotalManaPoints()
    {
        return totalManaPoints;
    }

    public int getCurrentManaPoints()
    {
        return currentManaPoints;
    }

    #endregion

    #region setters
    // Next setters are use to LOAD DATA from GameControl.
    public void SetCurrentLevel(int _level)
    {
        level = _level;
    }

    public void SetTotalLevelExp(int amount)
    {
        totalLevelExp = amount;
    }

    public void SetCurrentLevelExp(int amount)
    {
        currentExp = amount;
    }

    public void SetBaseStatsByType(StatsType type, int statsAmount)
    {
        switch(type)
        {
            case StatsType.STRENGTH:
                baseStrength = statsAmount;
                break;
            case StatsType.AGILITY:
                baseAgility = statsAmount;
                break;
            case StatsType.VITALITY:
                baseVitality = statsAmount;
                break;
            case StatsType.INTELLECT:
                baseIntellect = statsAmount;
                break;
        }
    }

    public void SetCurrentStatsPoints(int amount)
    {
        currentStatsPoints = amount;
    }

    // End of "loader setters"

    #endregion
}
