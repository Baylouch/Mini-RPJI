/* BaseItem.cs
Utilisé pour les données communes des objets utilisable en jeu.
Doit être dérivé par d'autre classe comme WeaponItem.cs par exemple.
*/

using UnityEngine;

public class BaseItem : ScriptableObject
{
    public int itemID; // Unique on every item to get it in itemDataBase

    public string itemName;

    public GameObject itemPrefab;
    public Sprite inventoryImage;
}
