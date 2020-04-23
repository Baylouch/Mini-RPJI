/* BaseItem.cs
Utilisé pour les données communes des objets utilisable en jeu.
Doit être dérivé par d'autre classe comme WeaponItem.cs par exemple.
*/

using UnityEngine;

public class BaseItem : ScriptableObject
{
    [Header("Info generales")]
    public int itemID; // Unique on every item to get it in itemDataBase

    public string itemName;
    [TextArea]public string itemDescription;

    public float itemSellPrice;

    public GameObject itemPrefab; // Must have <Item> component on it, a sprite renderer and a collider set trigger.
    public Sprite prefabImage;
    public Sprite inventoryImage;

    public bool stackableItem; // If you can stack it in inventory. (Potions, quests items...)
}
