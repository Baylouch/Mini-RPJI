/* Stats_Control.cs
 Gère les statistiques du joueur. Par ex: le taux de vitalité augmentera les points de vies gérés eux dans Health.cs

*/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum StatsType { STRENGTH, AGILITY, VITALITY, INTELLECT }; // Enum pour différencier les différentes statistiques

public class Player_Stats : MonoBehaviour
{
    // Think about centralize everything here, as healthpoint, armor...

    [SerializeField] private UI_Player_Stats playerStatsUI;
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

    [Header("Attack")]
    [SerializeField] private int damageMin = 10;
    [SerializeField] private int damageMax = 15;

    [Header("Ranged Attack")] // To move or delete
    [SerializeField] private int rangedDamageMin = 5;
    [SerializeField] private int rangedDamageMax = 10;

    [Header("General")]
    [SerializeField] private int healthPoints = 100;

    private int[] statsTrack; // To keep a track from previous stats when player is upgrading. Usefull until player's validation
    private int currentStatsPoints = 0;
    private int[] removableStatsPoints; // To know when player can remove points or not (an array for know exaclty what pts can be remove or not)
    private bool hasStatsPoints = false;

    Player_Health health;

    // Start is called before the first frame update
    void Start()
    {
        if (GetComponent<Player_Health>())
            health = GetComponent<Player_Health>();

        statsTrack = new int[4];
        TrackCurrentStats();

        removableStatsPoints = new int[4];
        for (int i = 0; i < removableStatsPoints.Length; i++)
        {
            removableStatsPoints[i] = 0;
        }
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
                    playerStatsUI.RefreshStatsText(this);
                }
            }
        }
    }

    void LevelUp()
    {
        level++;
        // Reset some stats
        // healthpoints = maxHealthpoints
        if (health)
        {
            health.SetHealthPoints(healthPoints);
        }

        // Give 5 stats points to the player
        currentStatsPoints += 5;
        NextLevelExperience *= level; // Too change later

        if (playerStatsUI)
        {
            playerStatsUI.RefreshStatsText(this);
        }
    }

    // Just for know what stats we got for a needed time (exemple, when player start to put new stats points before validation, if he wants to reset, he can)
    void TrackCurrentStats()
    {
        statsTrack[0] = GetStatsByType(StatsType.STRENGTH);
        statsTrack[1] = GetStatsByType(StatsType.AGILITY);
        statsTrack[2] = GetStatsByType(StatsType.VITALITY);
        statsTrack[3] = GetStatsByType(StatsType.INTELLECT);

        Debug.Log("current strength is : " + statsTrack[0]);
        Debug.Log("current agility is : " + statsTrack[1]);
        Debug.Log("current vitality is : " + statsTrack[2]);
        Debug.Log("current intellect is : " + statsTrack[3]);
    }

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

    public int GetAttackDamage()
    {
        int currAttack = (Random.Range(damageMin, damageMax) + (strength / 2));
        Debug.Log("Dommage infligés : " + currAttack);
        return currAttack;
    }

    public int GetRangedAttackDamage()
    {
        int currRangedattack = (Random.Range(rangedDamageMin, rangedDamageMax) + (agility / 2));
        Debug.Log("Dommage a distance infligés : " + currRangedattack);
        return currRangedattack;
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
        }
    }

    #region UI_Methods

    public void StatsValidation()
    {
        // Player has validate so no removable points
        for (int i = 0; i < removableStatsPoints.Length; i++)
        {
            removableStatsPoints[i] = 0;
        }
        // track current stats
        TrackCurrentStats();
        // refresh UI
        playerStatsUI.RefreshStatsText(this); // Maybe useless
    }

    public void StatsCancel()
    {
        for (int i = 0; i < removableStatsPoints.Length; i++)
        {
            if (removableStatsPoints[i] > 0)
            {
                currentStatsPoints += removableStatsPoints[i];
                removableStatsPoints[i] = 0;
            }
        }

        // Reset stats as tracked
        strength = statsTrack[0];
        agility = statsTrack[1];
        vitality = statsTrack[2];
        intellect = statsTrack[3];
        // Track stats then refresh UI
        TrackCurrentStats();
        playerStatsUI.RefreshStatsText(this);
    }

    // We can't use StatsType via OnClick from a button UI.
    // So we use int for choose stats : 0 = strength, 1 = agility, 2 = vitality, 3 = intellect
    public void AddStatsPoints(int statsType)
    {
        if (currentStatsPoints > 0)
        {
            switch (statsType)
            {
                case 0:
                    strength++;
                    removableStatsPoints[0]++;
                    break;
                case 1:
                    agility++;
                    removableStatsPoints[1]++;
                    break;
                case 2:
                    vitality++;
                    removableStatsPoints[2]++;
                    break;
                case 3:
                    intellect++;
                    removableStatsPoints[3]++;
                    break;
                default:
                    Debug.LogWarning("Don't know what stats to upgrade !");
                    return;
            }

            currentStatsPoints--;

            // If we're here we're sure we got playerStatsUI not null
            playerStatsUI.RefreshStatsText(this);
        }

    }

    public void RemoveStatsPoints(int statsType)
    {
        if (statsType == 0)
        {
            if (removableStatsPoints[0] > 0)
            {
                strength--;
                removableStatsPoints[0]--;
                currentStatsPoints++;
            }
        }
        else if (statsType == 1)
        {
            if (removableStatsPoints[1] > 0)
            {
                agility--;
                removableStatsPoints[1]--;
                currentStatsPoints++;
            }
        }
        else if (statsType == 2)
        {
            if (removableStatsPoints[2] > 0)
            {
                vitality--;
                removableStatsPoints[2]--;
                currentStatsPoints++;
            }
        }
        else if (statsType == 3)
        {
            if (removableStatsPoints[3] > 0)
            {
                intellect--;
                removableStatsPoints[3]--;
                currentStatsPoints++;
            }
        }
        else
        {
            Debug.LogWarning("Attention la valeur \"" + statsType + "\" n'est pas reconnu ");
        }

         // If we're here we're sure we got playerStatsUI not null
         playerStatsUI.RefreshStatsText(this);
        
    }

    public int getMinDamage()
    {
        return damageMin;
    }

    public int getMaxDamage()
    {
        return damageMax;
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

    #endregion
}
