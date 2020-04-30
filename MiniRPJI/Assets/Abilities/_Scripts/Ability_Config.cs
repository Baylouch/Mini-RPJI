/* Abilities_config.cs :
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
    // TODO Think about punch ability can use an abilityPrefab too ?
    public GameObject abilityPrefab; // For this game, it consist of a prefab with Player_Projectile on it or its children. (Keep it null for punch ability)

    public int energyCost;

    public AbilityType abilityType; // PunchAttack when its the punch. Bow when its the bow. (I keep it simple because this game will not have more than the "Ranger" who use only a punch and a bow.

}
