/* Abilities_config.cs :
 * 
 * Contient les données d'une abilité
 * 
 * Dans la hierarchie d'unity (dans les dossiers du projet) le schema du nom d'une abilité est : "subID_ID_abilityname abilitylevel"
 * 
 * 
 * */

using UnityEngine;

[CreateAssetMenu(fileName = "AbilityConfig", menuName = "ScriptableObjects/Abilities/AbilityConfig", order = 1)]
public class Ability_Config : ScriptableObject
{
    public int abilityID; // The unique ID of all abilities

    public int abilitySubID; // To regroup abilities because there are multiple level for the same ability

    public int abilityLevel; // Represent the level of the ability (5 max)

    public string abilityName;

    [TextArea]
    public string abilityShortDescription; // Applied on ability's button from bottom bar

    [TextArea]
    public string abilityLongDescription; // Applied on ability's button from ability tree

    public Sprite abilitySprite;

    public GameObject abilityPrefab; // For this game, it consist of a prefab reprensenting the ability itself. Player_Projectile required on bow ability type

    public int abilityBonus; // Represent damage for attack ability and healthpoints for decoy.

    public float abilityPower; // Represent the push force when enemy gets hitted

    public float abilityTimer; // For others ability, to get a time before the prefab of the ability has been destroy. Determine the malus timer for attacks abilities with defined type.

    public int energyCost;

    public AbilityType abilityType; // PunchAttack when its the punch. Bow when its the bow. Other when its something else (no animation for this one)
    // (I keep it simple because this game will not have more than the "Ranger" who use only a punch and a bow).

    public int levelRequired;
}
