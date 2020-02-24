/* ItemConfig.cs
Utilisé pour les données des objets utilisable en jeu.
Pour l'instant seule les valeures non égales à 0 seront utilisées.
*/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ItemName", menuName = "ScriptableObjects/ItemConfig", order = 1)]
public class ItemConfig : ScriptableObject
{
    public string itemName;

    public GameObject itemPrefab;
    public Sprite inventoryImage;
    public ArmoryPart armoryPart;
    public ItemRarety rarety;

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

    // Method for check every stats from itemConfig for know what's item stats upgrade
    void OldCheckItemStats()
    {
        if (strength != 0)
        {
            Debug.Log("strength of " + itemName + " = " + strength);
        }
        if (agility != 0)
        {
            Debug.Log("agility of " + itemName + " = " + agility);
        }
        if (vitality != 0)
        {
            Debug.Log("vitality of " + itemName + " = " + vitality);
        }
        if (intellect != 0)
        {
            Debug.Log("intellect of " + itemName + " = " + intellect);
        }
        if (armor != 0)
        {
            Debug.Log("armor of " + itemName + " = " + armor);
        }
        if (criticalRate != 0)
        {
            Debug.Log("criticalRate of " + itemName + " = " + criticalRate);
        }
        if (rangedCriticalRate != 0)
        {
            Debug.Log("rangedCriticalRate of " + itemName + " = " + rangedCriticalRate);
        }
        if (damageMin != 0)
        {
            Debug.Log("damageMin of " + itemName + " = " + damageMin);
        }
        if (damageMax != 0)
        {
            Debug.Log("damageMax of " + itemName + " = " + damageMax);
        }
        if (rangedDamageMin != 0)
        {
            Debug.Log("rangedDamageMin of " + itemName + " = " + rangedDamageMin);
        }
        if (rangedDamageMax != 0)
        {
            Debug.Log("rangedDamageMax of " + itemName + " = " + rangedDamageMax);
        }
        if (healthpoints != 0)
        {
            Debug.Log("healthpoints of " + itemName + " = " + healthpoints);
        }
        if (levelRequired != 0)
        {
            Debug.Log("levelRequired of " + itemName + " = " + levelRequired);
        }
        if (strenghtRequired != 0)
        {
            Debug.Log("strenghtRequired of " + itemName + " = " + strenghtRequired);
        }
        if (agilityRequired != 0)
        {
            Debug.Log("agilityRequired of " + itemName + " = " + agilityRequired);
        }
    }
}
