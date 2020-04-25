/* Player_Abilities.cs 
 * Contient les différentes abilitées disponible pour le joueur.
 * Permet de savoir quelles abilitées sont débloquées
 * Permet de connaitre les abilitées courantes (primaire et secondaire)
 * 
 * */
using UnityEngine;

public enum AbilityType { Punch, Bow }; // Used in Ability_Config

public class Player_Abilities : MonoBehaviour
{
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

    public AbilitiesDataBase abilitiesDatabase; // player's abilities

    [SerializeField] Ability_Config primary_ability; // Left clic ability
    public Ability_Config GetPrimaryAbility()
    {
        return primary_ability;
    }

    [SerializeField] Ability_Config secondary_ability; // Right clic ability
    public Ability_Config GetSecondaryAbility()
    {
        return secondary_ability;
    }

    bool[] unlockAbilitiesID; // Array who determine unlock (true) abilities or locked (false) one. Index is linked to the unique ability's ID. (index 0 = ability's ID 0)

    // Start is called before the first frame update
    void Start()
    {
        // Set array size (Length = all player's abilities in game )
        unlockAbilitiesID = new bool[abilitiesDatabase.abilities.Length];

        // Unlock punch ability (ID = 0) and normalArrow (ID = 1)
        SetUnlockAbility(0, true);
        SetUnlockAbility(1, true);

        // Set base abilities
        SetPrimaryAbility(0);
        SetSecondaryAbility(1);
    }

    // Method to set primary ability
    public void SetPrimaryAbility(int _abilityID)
    {
        if (abilitiesDatabase.GetAbilityByID(_abilityID) != null)
        {
            if (unlockAbilitiesID[_abilityID] == true)
            {
                primary_ability = abilitiesDatabase.GetAbilityByID(_abilityID);
            }
            else
            {
                Debug.Log("Ability ID " + _abilityID + " isn't unlock yet!");
            }
        }
        else
        {
            Debug.Log("No ability linked to this ID : " + _abilityID);
        }
    }

    // Method to set secondary ability
    public void SetSecondaryAbility(int _abilityID)
    {
        if (abilitiesDatabase.GetAbilityByID(_abilityID) != null)
        {
            if (unlockAbilitiesID[_abilityID] == true)
            {
                secondary_ability = abilitiesDatabase.GetAbilityByID(_abilityID);
            }
            else
            {
                Debug.Log("Ability ID " + _abilityID + " isn't unlock yet!");
            }
        }
        else
        {
            Debug.Log("No ability linked to this ID : " + _abilityID);
        }
    }

    // Method to unlock an ability. We can use only one parameter as two (ability's ID) because array is set for it.
    public void SetUnlockAbility(int _abilityID, bool value)
    {
        // Check if index isnt out of range
        if (_abilityID <= unlockAbilitiesID.Length)
        {
            unlockAbilitiesID[_abilityID] = value;
        }
    }

    // Method to know if an ability is unlock. False = lock, true = unlock
    public bool GetUnlockAbility(int _abilityID)
    {
        // Check if index isnt out of range
        if (_abilityID <= unlockAbilitiesID.Length)
        {
            return unlockAbilitiesID[_abilityID];
        }

        Debug.Log("GetUnlockAbility() parameter is out of range. Ability ID doesn't exist.");
        return false;
    }
}
