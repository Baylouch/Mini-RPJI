using UnityEngine;

public enum ArmoryPart { Helm, Chest, Pants, Gloves, Boots, Bow }; // Index of parts are same in armorySlots
public enum ItemRarety { Common, Uncommon, Rare, Epic, Legendary }; // All items rarety

public class Player_Inventory : MonoBehaviour
{  
    public static Player_Inventory inventory_instance;

    public ItemDataBase itemDataBase;

    [SerializeField] int playerGold;

    public const int armorySlotsNumb = 6; // Number of armory slots
    public const int inventorySlotsNumb = 18; // Number of inventory slots
    [Header("Player's armory")]
    [SerializeField] EquipmentItem[] armoryItems;
    [Header("Player's inventory")]
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

    private void Start()
    {
        // Give to the player 5 health potions
        // TODO see if next line get wrong when loading data
        GetNewItem(itemDataBase.GetItemById(100));
        GetNewItem(itemDataBase.GetItemById(100));
        GetNewItem(itemDataBase.GetItemById(100));
        GetNewItem(itemDataBase.GetItemById(100));
        GetNewItem(itemDataBase.GetItemById(100));

        UI_Player.ui_instance.playerInventoryUI.RefreshInventory();
        UI_Player.ui_instance.playerInventoryUI.RefreshArmory();
    }

    // TODO Delete
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            playerGold += 10;
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
            return;
        }
        
        // Check if its a quest item
        if (item as QuestItem)
        {
            QuestItem questItem = (QuestItem)item;
            // Check if we got the quest linked to the item.
            if (Player_Quest_Control.quest_instance.GetPlayerQuestByID(questItem.questID))
            {
                questItem.IncrementLinkedQuest();
            }
        }

        if (item.stackableItem) // Check if you can stack item in inventory
        {
            for (int i = 0; i < inventoryItems.Length; i++) // Check in every items
            {
                if (inventoryItems[i] != null)
                {
                    if (inventoryItems[i] == item) // If its the same item than stackable one increment it
                    {
                        UI_Player.ui_instance.playerInventoryUI.GetInventorySlotByIndex(i).itemNumb++;
                        UI_Player.ui_instance.playerInventoryUI.RefreshInventory();
                        return; // dont continue
                    }
                }
            }
        }

        // Find the first available slot to put item
        for (int i = 0; i < inventoryItems.Length; i++)
        {
            if (inventoryItems[i] == null)
            {
                // set new item in inventory
                inventoryItems[i] = item;
                // We need to check if its a stackable item here too.
                if (item.stackableItem)
                    UI_Player.ui_instance.playerInventoryUI.GetInventorySlotByIndex(i).itemNumb++;
                UI_Player.ui_instance.playerInventoryUI.RefreshInventory();
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

    // Used to set inventory item (GameControl.cs)
    public void SetInventoryIndex(int inventoryIndex, int _itemID)
    {
        // Next condition is used to remove item in the inventoryIndex slot. Because item's IDs will never be NEGATIVE
        if (_itemID == -1)
        {
            inventoryItems[inventoryIndex] = null;
        }

        inventoryItems[inventoryIndex] = itemDataBase.GetItemById(_itemID);
    }

    // used to set armory item (GameControl.cs)
    public void SetArmoryIndex(int armoryIndex, int _itemID)
    {
        if (_itemID == -1)
        {
            armoryItems[armoryIndex] = null;
        }

        armoryItems[armoryIndex] = (EquipmentItem)itemDataBase.GetItemById(_itemID);
    }

    public int GetPlayerGold()
    {
        return playerGold;
    }

    // you can put negative amount to decrease money, or a positive amount to increase it
    public void SetPlayerGold(int amount)
    {
        playerGold += amount;
    }
}
