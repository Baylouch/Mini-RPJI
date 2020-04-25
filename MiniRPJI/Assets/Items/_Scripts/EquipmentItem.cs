/* EquipmentItem.cs
Utilisé pour dérivé de BaseItem tout objets équipable par le joueur 
(sera aussi considéré comme un objet d'inventaire par défaut)
*/
using UnityEngine;

[CreateAssetMenu(fileName = "EquipmentItemName", menuName = "ScriptableObjects/Items/EquipmentItem", order = 1)]
public class EquipmentItem : BaseItem
{
    [Header("Categorie")]
    public ArmoryPart armoryPart;
    public ItemRarety rarety;

    [Header("Statistiques")]
    public int strength;
    public int agility;
    public int vitality;
    public int energy;
    public int armor;
    public float criticalRate;
    public float rangedCriticalRate;
    public int healthpoints;

    [Header("Degats")]
    public int damageMin;
    public int damageMax;

    public int rangedDamageMin;
    public int rangedDamageMax;

    [Header("Required")]
    public int levelRequired;
    public int strenghtRequired;
    public int agilityRequired;
}
