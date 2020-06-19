/* Abilities_config.cs :
 * 
 * Contient les données d'une abilité
 * 
 * 
 * */

using UnityEngine;

[CreateAssetMenu(fileName = "AbilityConfig", menuName = "ScriptableObjects/Abilities/AbilityConfig", order = 1)]
public class Ability_Config : ScriptableObject
{
    public int abilityID;

    public string abilityName;

    [TextArea]
    public string abilityDescription;

    public Sprite abilitySprite;

    public GameObject abilityPrefab; // For this game, it consist of a prefab reprensenting the ability itself. Player_Projectile required on bow ability type

    public int energyCost;

    public AbilityType abilityType; // PunchAttack when its the punch. Bow when its the bow. Other when its something else (no animation for this one)
    // (I keep it simple because this game will not have more than the "Ranger" who use only a punch and a bow).

}
