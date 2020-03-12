using UnityEngine;

public enum ArmoryPart { Helm, Chest, Pants, Gloves, Boots, Bow }; // Index of parts are same in armorySlots
public enum ItemRarety { Common, Uncommon, Rare, Epic, Legendary }; // All items rarety

public class Player_Inventory : MonoBehaviour
{  
    public static Player_Inventory inventory_instance;

    public ItemDataBase itemDataBase;

    public const int armorySlotsNumb = 6; // Number of armory slots
    public const int inventorySlotsNumb = 18; // Number of inventory slots

    [SerializeField] EquipmentItem[] armoryItems;  
    [SerializeField] BaseItem[] inventoryItems;

    private void Awake()
    {
        // Make this singleton
        if (!inventory_instance)
        {
            inventory_instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // TODO Modify after split Player_Inventory with UI_Player_Inventory (to check THIS array of item?).
    // False = slots available, true = full
    public bool CheckInventoryIsFull()
    {
        for (int i = 0; i < inventoryItems.Length; i++)
        {
            if (inventoryItems[i] == null)
                return false;
        }
        // If we're here inventory got no more space
        Debug.Log("No more available slot in Inventory !");
        return true;
    }

    public void GetNewItem(BaseItem item)
    {
        if (CheckInventoryIsFull()) // If inventory is full return
        {
            Debug.Log("No more available slot in Inventory !");
            return;
        }

        // Find the first available slot to put item
        for (int i = 0; i < inventoryItems.Length; i++)
        {
            if (inventoryItems[i] == null)
            {
                // set new item in inventory
                inventoryItems[i] = item;
                UI_Player.instance.playerInventoryUI.RefreshInventory();
                return; // Get out of there
            }
        }
    }

    // Method use in Player_Combat_Control to get projectile of current bow.
    public EquipmentItem GetCurrentBow()
    {
        if (armoryItems[5])
        {
            return armoryItems[5];
        }

        return null;
    }

    // Used in Player Stats to apply item stats. And in GameControl.cs to save current EquipmentItems
    public EquipmentItem GetArmoryItem(int armoryIndex)
    {
        if (armoryItems[armoryIndex])
            return armoryItems[armoryIndex];
        return null;
    }

    // used in GameControl.cs to save inventory index item if there is an item in
    public BaseItem GetInventoryItem(int inventoryIndex)
    {
        if (inventoryItems[inventoryIndex])
            return inventoryItems[inventoryIndex];
        return null;
    }

    // used in GameControl.cs to load inventory items
    public void SetInventoryIndex(int inventoryIndex, int _itemID)
    {
        // Next is used in game control to remove items. Because item's IDs will never be NEGATIVE
        if (_itemID == -1)
        {
            inventoryItems[inventoryIndex] = null;
        }

        inventoryItems[inventoryIndex] = itemDataBase.GetItemById(_itemID);
    }

    // used in GameControl.cs to load armory items
    public void SetArmoryIndex(int armoryIndex, int _itemID)
    {
        if (_itemID == -1)
        {
            armoryItems[armoryIndex] = null;
        }

        armoryItems[armoryIndex] = (EquipmentItem)itemDataBase.GetItemById(_itemID);
    }
}
