/* EquipmentItem.cs
Utilisé pour dérivé de BaseItem tout objets équipable par le joueur 
(sera aussi considéré comme un objet d'inventaire par défaut)
*/
using UnityEngine;

[CreateAssetMenu(fileName = "EquipmentItemName", menuName = "ScriptableObjects/Items/EquipmentItem", order = 1)]
public class EquipmentItem : BaseItem
{
    public ArmoryPart armoryPart;
    public ItemRarety rarety;

    [Tooltip("You set projectile ONLY if you're creating a BOW !")]
    public GameObject projectile;

    public int strength;
    public int agility;
    public int vitality;
    public int intellect;
    public int armor;
    public float criticalRate;
    public float rangedCriticalRate;

    public int damageMin;
    public int damageMax;

    public int rangedDamageMin;
    public int rangedDamageMax;

    public int healthpoints;

    public int levelRequired;
    public int strenghtRequired;
    public int agilityRequired;
}
