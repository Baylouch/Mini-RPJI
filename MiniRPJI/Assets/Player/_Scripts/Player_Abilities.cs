/* Player_Abilities.cs 
 * 
 * Contient les différentes abilitées disponible pour le joueur.
 * 
 * Permet de savoir quelles abilitées sont débloquées
 * 
 * Permet de connaitre les abilitées courantes (primaire et secondaire)
 * 
 * */

using UnityEngine;

public enum AbilityType { Punch, Bow, Other }; // Used in Ability_Config

public class Player_Abilities : MonoBehaviour
{
    public const int totalPlayerAbilities = 10;

    public static Player_Abilities instance;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject); // This must not happen.
        }
    }

    public AbilitiesDataBase abilitiesDatabase; // player's abilities database

    // Array representing current abilities player has unlocked
    [SerializeField] Ability_Config[] player_AbilityConfigs = new Ability_Config[totalPlayerAbilities]; // Initialize to 10 (the max number of ability player can has)
    public Ability_Config GetPlayerAbilityConfig(int index)
    {
        if (index < player_AbilityConfigs.Length && index >= 0)
            return player_AbilityConfigs[index];
        else
            return null;
    }
    public void SetPlayerAbilityConfig(int index, Ability_Config _config)
    {
        if (index < player_AbilityConfigs.Length && index >= 0)
            player_AbilityConfigs[index] = _config;
        else
            Debug.Log("Index out of range.");
    }

    // Player can use "F", "T", "W", "X", "C" as shortcuts to fastly get access to an ability
    // Each time player change a shortcut, the couple of arrays are checked to delete the precedent input using to apply on the right ability in the right array.
    // Index array are the same as player_AbilityConfigs
    [SerializeField] string[] primaryAbilityShortCuts = new string[totalPlayerAbilities]; // represent the primary ability shortcuts.
    public string GetPrimaryAbilityShortCutByAbilityID(int _abilityID)
    {
        for (int i = 0; i < player_AbilityConfigs.Length; i++)
        {
            if (player_AbilityConfigs[i].abilityID == _abilityID)
            {
                return primaryAbilityShortCuts[i];
            }
        }

        return "";
    }
    public string GetPrimaryAbilityShortCutByIndex(int _index)
    {
        return primaryAbilityShortCuts[_index];
    }
    [SerializeField] string[] secondaryAbilityShortCuts = new string[totalPlayerAbilities]; // represent the secondary ability shortcuts.
    public string GetSecondaryAbilityShortCutByAbilityID(int _abilityID)
    {
        for (int i = 0; i < player_AbilityConfigs.Length; i++)
        {
            if (player_AbilityConfigs[i].abilityID == _abilityID)
            {
                return secondaryAbilityShortCuts[i];
            }
        }

        return "";
    }
    public string GetSecondaryAbilityShortCutByIndex(int _index)
    {
        return secondaryAbilityShortCuts[_index];
    }

    Ability_Config primary_ability; // Left clic ability
    public Ability_Config GetPrimaryAbility()
    {
        return primary_ability;
    }

    Ability_Config secondary_ability; // Right clic ability
    public Ability_Config GetSecondaryAbility()
    {
        return secondary_ability;
    }

    private int abilityPoints = 0; // Incremented each time player level up (Player_Stats.cs)
    public int GetAbilityPoints()
    {
        return abilityPoints;
    }
    public void SetAbilityPoints(int value)
    {
        abilityPoints = value;
    }

    // Start is called before the first frame update
    void Start()
    {
        // Unlock punch ability (ID = 0) and normalArrow (ID = 1)
        SetPlayerAbilityConfig(0, abilitiesDatabase.GetAbilityByID(0));
        SetPlayerAbilityConfig(1, abilitiesDatabase.GetAbilityByID(1));

        // Set base abilities
        SetPrimaryAbility(abilitiesDatabase.GetAbilityByID(0));
        SetSecondaryAbility(abilitiesDatabase.GetAbilityByID(1));
    }

    // Method to set primary ability
    public void SetPrimaryAbility(Ability_Config _newAbility)
    {
        if (abilitiesDatabase.GetAbilityByID(_newAbility.abilityID) != null)
        {
            primary_ability = _newAbility;
        }
        else
        {
            Debug.Log("No ability linked to this ID : " + _newAbility.abilityID);
        }
    }

    // Method to set secondary ability
    public void SetSecondaryAbility(Ability_Config _newAbility)
    {
        if (abilitiesDatabase.GetAbilityByID(_newAbility.abilityID) != null)
        {
            secondary_ability = _newAbility;
        }
        else
        {
            Debug.Log("No ability linked to this ID : " + _newAbility.abilityID);
        }
    }

    public void SetAbilityShortCut(string _shortCut, int _abilityID, bool _primaryAbility) // bool primaryAbility is just a switch to not have 2 method indentic with only the last line different
    {
        int abilityIndex = -1;

        // Check of the index ability in player_AbilityConfigs to get the right index of the ability
        for (int i = 0; i < player_AbilityConfigs.Length; i++)
        {
            if (player_AbilityConfigs[i].abilityID == _abilityID)
            {
                abilityIndex = i;
                break;
            }
        }

        // If shortCutIndex still -1 here, we havn't the ability linked to this ID.
        if (abilityIndex == -1)
        {
            Debug.Log("Wront ability ID");
            return;
        }

        // Check if we already use the short cut by looping trough all index of primaryAbilityShortCuts and secondaryAbilityShortcuts.
        for (int i = 0; i < totalPlayerAbilities; i++) // We know these two arrays are set with totalPlayerAbilities
        {
            if (primaryAbilityShortCuts[i] == _shortCut)
            {
                primaryAbilityShortCuts[i] = "";

                break;
            }
            if (secondaryAbilityShortCuts[i] == _shortCut)
            {
                secondaryAbilityShortCuts[i] = "";

                break;
            }
        }

        // If we're here, everything is preset, we can set the new shortcut to the right index of the ability in the primary ability array
        if (_primaryAbility)
            primaryAbilityShortCuts[abilityIndex] = _shortCut;
        else
            secondaryAbilityShortCuts[abilityIndex] = _shortCut;
    }
}
