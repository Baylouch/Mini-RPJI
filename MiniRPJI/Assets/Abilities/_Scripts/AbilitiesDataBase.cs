using UnityEngine;

[CreateAssetMenu(fileName = "AbilitiesDataBase", menuName = "ScriptableObjects/Abilities/DataBase", order = 0)]
public class AbilitiesDataBase : ScriptableObject
{
    public Ability_Config[] abilities;

    // Method to get objects by its unique ID
    public Ability_Config GetAbilityByID(int _abilityID)
    {
        foreach (var ability in abilities)
        {
            if (ability != null && ability.abilityID == _abilityID)
                return ability;
        }
        return null;
    }
}
