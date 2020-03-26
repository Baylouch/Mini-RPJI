/* ItemDataBase.cs
 
Must contain all item in the game (EquipmentItem, QuestItem and UsableItem for now)
each of them must have an unique ID !

It's used to save and load inventory data.

*/
using UnityEngine;

[CreateAssetMenu(fileName = "ItemDataBase", menuName = "ScriptableObjects/Items/DataBase", order = 1)]
public class ItemDataBase : ScriptableObject
{
    public BaseItem[] items;

    public BaseItem GetItemById(int _itemID)
    {
        foreach(var item in items)
        {
            if (item != null && item.itemID == _itemID)
                return item;
        }
        return null;
    }
}
