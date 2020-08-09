/* Player_Stats.cs
 Gère les statistiques du joueur. Par ex: le taux de vitalité augmentera les points de vies etc
 Ce script doit être executé dans les premiers afin d'éviter null reference si le joueur a des objets pré-équipés avant de commencer
 Permet l'accès aux éléments Player_Energy et Player_Health pour les autres scripts
*/
using UnityEngine;


public enum StatsType { STRENGTH, AGILITY, VITALITY, ENERGY }; // Enum pour différencier les différentes statistiques

[RequireComponent(typeof(Player_Energy))]
[RequireComponent(typeof(Player_Health))]
public class Player_Stats : MonoBehaviour
{
    public static Player_Stats instance; // Singleton + persistent

    public const int level_max = 25;

    // Experience is for now set in "Projectile.cs" and "Player_Combat_Control.cs"
    [Header("Experience")]
    [SerializeField] private int level = 1;
    [SerializeField] private int totalLevelExp = 200;
    [SerializeField] private int currentExp = 0;
    [SerializeField] private GameObject levelUpEffect;

    [Header("Statistiques")]
    [SerializeField] private int baseStrength = 10; // Used as base to keep track on it for save and load functionnality 
    [SerializeField] private int baseAgility = 10; // Because if you lvl up, you win base points add.
    [SerializeField] private int baseVitality = 10; // And if you equip a weapon, its not base points but current points
    [SerializeField] private int baseEnergy = 10;
    [SerializeField] private float criticalRate = 3f;
    [SerializeField] private float rangedCriticalRate = 5f;
    [SerializeField] float movementSpeed = 4;

    // Current stats used in game. Addition of baseStats + weaponStats.
    private int currentStrength;
    private int currentAgility;
    private int currentVitality;
    private int currentEnergy;
    private int armor;

    [Header("Attack")]
    [SerializeField] private int damageMin = 1;
    [SerializeField] private int damageMax = 3;

    private int tempItemHealthPoints = 0; // If an item give us healthpoints, we need a temp variable to be reset each time we enter in RefreshPlayerStats()
    // then add to the total healthpoints after vitality calculation

    private int[] statsTrack; // To keep a track from previous stats when player is upgrading. Usefull until player's validation
    private int currentStatsPoints = 0;

    private int currentDamageMin;
    private int currentDamageMax;
    // We need a temp variable to set currentDamage with item additionnal damage. Same as tempItemHealthPoints.
    private int tempItemDamageMin = 0;
    private int tempItemDamageMax = 0;

    // Current ranged attack is based on the agility and bow damage
    private int currentRangedDamageMin;
    private int currentRangedDamageMax;

    private float strenghtMultiplier = .30f; // 30% of our strength is convert into damage
    private float agilityMultiplier = .25f; // 25% of our agility is convert into ranged damage
    private float vitalityMultiplier = 3.5f;
    private float energyMultiplier = 2.5f;
    private float armorMultiplier = .10f; // Used in Player_Health to reduce damage taken

    // TODO hideininspector
    public Player_Energy playerEnergy;
    public Player_Health playerHealth;

    private void Awake()
    {
        // Make it singleton
        if (!instance)
        {
            instance = this;
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

        if (GetComponent<Player_Energy>())
        {
            playerEnergy = GetComponent<Player_Energy>();
            playerEnergy.SetBaseEnergyPoints(playerEnergy.GetTotalEnergyPoints());
        }
            
        if (GetComponent<Player_Health>())
        {
            playerHealth = GetComponent<Player_Health>();
            playerHealth.SetBaseHealthPoints(playerHealth.GetTotalHealthPoints());

        }

        RefreshPlayerStats(); // refresh stats

        TrackCurrentStats(); // Get track of current stats at start      
    }

    void SetLevelXP()
    {
        switch (level)
        {
            case 1:
                totalLevelExp = 400;
                break;
            case 2:
                totalLevelExp = 800;
                break;
            case 3:
                totalLevelExp = 1600;
                break;
            case 4:
                totalLevelExp = 3200;
                break;
            case 5:
                totalLevelExp = 5100;
                Player_Success.instance.IncrementSuccessObjectiveByID(14);
                break;
            case 6:
                totalLevelExp = 7500;
                break;
            case 7:
                totalLevelExp = 10400;
                break;
            case 8:
                totalLevelExp = 14000;
                break;
            case 9:
                totalLevelExp = 18500;
                break;
            case 10:
                totalLevelExp = 24400;
                Player_Success.instance.IncrementSuccessObjectiveByID(15);
                break;
            case 11:
                totalLevelExp = 31000;
                break;
            case 12:
                totalLevelExp = 39500;
                break;
            case 13:
                totalLevelExp = 50000;
                break;
            case 14:
                totalLevelExp = 75000;
                break;
            case 15:
                totalLevelExp = 90000;
                break;
            case 16:
                totalLevelExp = 107000;
                break;
            case 17:
                totalLevelExp = 127000;
                break;
            case 18:
                totalLevelExp = 149000;
                break;
            case 19:
                totalLevelExp = 173000;
                break;
            case 20:
                totalLevelExp = 190000;
                Player_Success.instance.IncrementSuccessObjectiveByID(16);
                break;
            case 21:
                totalLevelExp = 200000;
                break;
            case 22:
                totalLevelExp = 225000;
                break;
            case 23:
                totalLevelExp = 250000;
                break;
            case 24:
                totalLevelExp = 285000;
                break;
            case 25: // Last level
                totalLevelExp = 0;
                Player_Success.instance.IncrementSuccessObjectiveByID(17);
                break;
        }
    }

    void LevelUp()
    {
        if (level >= level_max)
            return;

        level++; // Add a lvl        
        Player_Abilities.instance.SetAbilityPoints(Player_Abilities.instance.GetAbilityPoints() + 1); // Add an ability points

        if (UI_Player.instance.playerAbilitiesUI)
            UI_Player.instance.playerAbilitiesUI.RefreshAbilitiesUI();

        // Set new base player stats
        baseStrength += 2; baseAgility += 2; baseVitality += 4; baseEnergy += 2; // See StatsBoard sheets for more information
        damageMin++; damageMax++;

        // Get track of the new stats
        TrackCurrentStats();

        // Give 5 stats points to the player
        currentStatsPoints += 5;
        playerHealth.SetCurrentHealthPoints(playerHealth.GetTotalHealthPoints()); // refresh currenthealthpoints to set it to max
        playerEnergy.SetCurrentEnergyPoints(playerEnergy.GetTotalEnergyPoints());

        SetLevelXP();

        // Refresh Stats
        RefreshPlayerStats();

        // Effects
        if (levelUpEffect)
        {
            GameObject effect = Instantiate(levelUpEffect, transform.position, Quaternion.identity);
            effect.transform.SetParent(transform);
            Destroy(effect, 1f);
        }

        Sound_Manager.instance.PlaySound(Sound_Manager.instance.asset.levelUp);
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
        if (item.energy != 0)
        {
            currentEnergy += item.energy;
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
            tempItemDamageMin += item.damageMin;
        }
        if (item.damageMax != 0)
        {
            tempItemDamageMax += item.damageMax;
        }
        if (item.healthpoints != 0)
        {
            tempItemHealthPoints += item.healthpoints;
        }

        // Get track of the new stats
        TrackCurrentStats();
    }

    public void RefreshPlayerStats()
    {
        // Refresh current statistiques
        currentStrength = baseStrength; currentAgility = baseAgility; currentVitality = baseVitality; currentEnergy = baseEnergy;
        armor = 0;
        tempItemHealthPoints = 0;
        tempItemDamageMin = 0;
        tempItemDamageMax = 0;

        // Now check armory slot for know if there is an item. If yes, apply stats item.
        // We know there are 6 armory slots because of the enum ArmoryPart. Every armory slot got a unique name with this.
        for (int i = 0; i < 6; i++)
        {
            if (Player_Inventory.instance.GetArmoryItem(i) != null)
            {
                ApplyItemStats(Player_Inventory.instance.GetArmoryItem(i));
            }
        }

        float currentDamageMultiplier = (currentStrength * strenghtMultiplier);
        // Then use mathf methods to get min integer and max integer for the calculation
        currentDamageMin = (int)Mathf.Round(damageMin + currentDamageMultiplier);
        currentDamageMax = (int)Mathf.Round(damageMax + currentDamageMultiplier);

        currentDamageMin += tempItemDamageMin;
        currentDamageMax += tempItemDamageMax;

        // Do it only if player got bow equiped. Else player can't use ranged attack anyway
        if (Player_Inventory.instance.GetCurrentBow())
        {
            float currentRangedDamageMultiplier = (currentAgility * agilityMultiplier);
            currentRangedDamageMin = (int)Mathf.Round(Player_Inventory.instance.GetCurrentBow().rangedDamageMin + currentRangedDamageMultiplier);
            currentRangedDamageMax = (int)Mathf.Round(Player_Inventory.instance.GetCurrentBow().rangedDamageMax + currentRangedDamageMultiplier);
        }
        else
        {
            currentRangedDamageMin = 0;
            currentRangedDamageMax = 0;
        }

        // Vitality maths : works if you add or remove vitality points.
        if (playerHealth.GetBaseHealthPoints() + (currentVitality * vitalityMultiplier) != playerHealth.GetTotalHealthPoints())
        {
            playerHealth.SetTotalHealthPoints((int)Mathf.Max(playerHealth.GetBaseHealthPoints() + (currentVitality * vitalityMultiplier)));
        }

        playerHealth.SetTotalHealthPoints(playerHealth.GetTotalHealthPoints() + tempItemHealthPoints);

        // same as vitality maths
        if (playerEnergy.GetBaseEnergyPoints() + (currentEnergy * energyMultiplier) != playerEnergy.GetTotalEnergyPoints())
        {
            playerEnergy.SetTotalEnergyPoints((int)Mathf.Max(playerEnergy.GetBaseEnergyPoints() + (currentEnergy * energyMultiplier)));
        }

        if (UI_Player.instance.playerStatsUI)
        {
            UI_Player.instance.playerStatsUI.RefreshStatsDisplay();
        }
    }

    // Just for know what stats we got for a needed time (exemple, when player start to put new stats points before validation, if he wants to reset, he can)
    public void TrackCurrentStats()
    {
        statsTrack[0] = GetCurrentStatsByType(StatsType.STRENGTH);
        statsTrack[1] = GetCurrentStatsByType(StatsType.AGILITY);
        statsTrack[2] = GetCurrentStatsByType(StatsType.VITALITY);
        statsTrack[3] = GetCurrentStatsByType(StatsType.ENERGY);
    }

    public void AddExperience(int amount)
    {
        if (level >= level_max)
            return;

        int tempCurrentExperience = currentExp + amount; // Put in a temp variable currentExperience + amount

        if (tempCurrentExperience >= totalLevelExp) // Check if it's >= of required experience to lvl up
        {            
            int tempNextLevelExperience = tempCurrentExperience - totalLevelExp; // Get the "too much" amount of experience

            if (tempNextLevelExperience >= totalLevelExp) // If current exp - totalLevelExp still greater player's won 2 lvl or more
            {
                while (tempNextLevelExperience >= totalLevelExp) // While tempNextLevelExperience is >= totalLevelExp, lvl up and decrement tempNextLevelExperience
                {
                    tempNextLevelExperience -= totalLevelExp;

                    if (tempNextLevelExperience <= 0)
                    {
                        tempNextLevelExperience = 0;

                        break;
                    }

                    LevelUp();
                }
            }
            else  // Else player just lvl up once
            {
                LevelUp();
            }

            currentExp = tempNextLevelExperience; // Set the "too much" into currentExperience

        }
        else
        {
            currentExp = tempCurrentExperience;
            if (UI_Player.instance.playerStatsUI)
            {
                UI_Player.instance.playerStatsUI.RefreshStatsDisplay();
            }
        }
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
            case StatsType.ENERGY:
                baseEnergy++;
                currentEnergy++;
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
            case StatsType.ENERGY:
                baseEnergy--;
                currentEnergy--;
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
            case StatsType.ENERGY:
                return currentEnergy;
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
            case StatsType.ENERGY:
                return baseEnergy;
            default:
                Debug.Log("Le type demandé n'est pas reconnu. Stats_Control.cs");
                return -1;
        }
    }

    public int GetCurrentMinDamage()
    {
        return currentDamageMin;
    }

    public int GetCurrentMaxDamage()
    {
        return currentDamageMax;
    }

    public int GetCurrentRangedMinDamage()
    {
        return currentRangedDamageMin;
    }

    public int GetCurrentRangedMaxDamage()
    {
        return currentRangedDamageMax;
    }

    public int GetCurrentExp()
    {
        return currentExp;
    }

    public int GetTotalLevelExp()
    {
        return totalLevelExp;
    }

    public int GetCurrentStatsPoints()
    {
        return currentStatsPoints;
    }

    public int GetCurrentLevel()
    {
        return level;
    }

    public int GetArmor()
    {
        return armor;
    }

    public float GetArmorMultiplier() // For Player_Health to deal with damage taken
    {
        return armorMultiplier;
    }

    public float GetCriticalRate()
    {
        return criticalRate;
    }
    
    public float GetRangedCriticalRate()
    {
        return rangedCriticalRate;
    }

    public float GetSpeed()
    {
        return movementSpeed;
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
            case StatsType.ENERGY:
                baseEnergy = statsAmount;
                break;
        }
    }

    public void SetCurrentStatsPoints(int amount)
    {
        currentStatsPoints = amount;
    }

    public void SetSpeed(float amount)
    {
        movementSpeed = amount;
    }

    // End of "loader setters"

    #endregion

    // Non using in game methods (most used in Cheats.cs)
    
    // Method to level up by an input
    public void CheatLevelUp()
    {
        LevelUp();
    }
}
