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

}
