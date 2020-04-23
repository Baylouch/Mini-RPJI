/* ItemDataBase.cs
 
Must contain all item in the game (EquipmentItem, QuestItem and UsableItem for now)
each of them must have an unique ID !

It's used to save and load inventory data.

+ Used to drop items by level and rarety type.

Récap: - A big ItemDataBase (in top hierarchy of Items folder in Assets) contains all item available in the game.
       - Smalls ItemDataBase will be created too in the same assets place of the big one, to contains all items by rarety. ("CommonItemsDataBase", "UnCommonItemsDataBase"....

*/
using UnityEngine;

[CreateAssetMenu(fileName = "ItemDataBase", menuName = "ScriptableObjects/Items/DataBase", order = 1)]
public class ItemDataBase : ScriptableObject
{
    // A list of items contained in the database.
    public BaseItem[] items;

    // Method to get objects by its unique ID
    public BaseItem GetItemById(int _itemID)
    {
        foreach(var item in items)
        {
            if (item != null && item.itemID == _itemID)
                return item;
        }
        return null;
    }

    // Method to get a random item in the database via its level (used into ItemDroper.cs)
    public BaseItem GetRandomItemByLevel(int level)
    {
        // First get the number of items in this level.
        int totalItemOfThisLevel = 0;

        foreach (BaseItem item in items)
        {
            if ((item as EquipmentItem).levelRequired == level)
            {
                totalItemOfThisLevel++;
            }
        }

        // Check if totalItemOfThisLevel is still 0. Then dont continue.
        if (totalItemOfThisLevel == 0)
            return null;

        // Create an array with the right amount of index
        BaseItem[] levelItems = new BaseItem[totalItemOfThisLevel];
        int levelItemsIndex = 0;

        // Then put BaseItem of this level in levelItems array.
        foreach (BaseItem item in items)
        {
            if ((item as EquipmentItem).levelRequired == level)
            {
                levelItems[levelItemsIndex] = item;
                levelItemsIndex++;
            }
        }

        // And return a random item from our levelItems array.
        int itemToDropIndex = Random.Range(0, levelItems.Length);
        if (itemToDropIndex < levelItems.Length && levelItems[itemToDropIndex])
        {
            return levelItems[itemToDropIndex];
        }

        return null;
    }

    // Same as below, but with a level range instead. Will drop items in the database between levelMin (included) and levelMax (included)
    public BaseItem GetRandomItemByLevel(int levelMin, int levelMax)
    {
        // First get the number of items in this level.
        int totalItemOfThisLevel = 0;

        foreach (BaseItem item in items)
        {
            if ((item as EquipmentItem).levelRequired >= levelMin && (item as EquipmentItem).levelRequired <= levelMax)
            {
                totalItemOfThisLevel++;
            }
        }

        // Check if totalItemOfThisLevel is still 0. Then dont continue.
        if (totalItemOfThisLevel == 0)
            return null;

        // Create an array with the right amount of index
        BaseItem[] levelItems = new BaseItem[totalItemOfThisLevel];
        int levelItemsIndex = 0;

        // Then put BaseItem of this level in levelItems array.
        foreach (BaseItem item in items)
        {
            if ((item as EquipmentItem).levelRequired >= levelMin && (item as EquipmentItem).levelRequired <= levelMax)
            {
                levelItems[levelItemsIndex] = item;
                levelItemsIndex++;
            }
        }

        // And return a random item from our levelItems array.
        int itemToDropIndex = Random.Range(0, levelItems.Length);
        if (itemToDropIndex < levelItems.Length && levelItems[itemToDropIndex])
        {
            return levelItems[itemToDropIndex];
        }

        return null;
    }
}
