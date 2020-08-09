/* Player_Inventory.cs :
 * 
 * Gère la partie non UI de l'inventaire.
 * Contient l'inventaire du joueur et son armurerie (objets équipés)
 * 
 * Permet de savoir si l'inventaire est plein, de recevoir un nouvel item, de configurer (load) l'inventaire directement
 * et de connaître son état actuel (save).
 * 
 * Beaucoup de méthodes comme vendre, équiper, jeter sont elles placées dans UI_Player_Inventory.cs car elles sont directement lié à l'UI et n'ont pas d'utilités
 * lorsque celle-ci n'est pas activée.
 * Cela permet aussi de séparé ce script (qui été un seul script de base) et de le rendre plus simple.
 * 
 * 
 * */

using UnityEngine;

public enum ArmoryPart { Helm, Chest, Pants, Gloves, Boots, Bow }; // Index of parts are same in armorySlots
public enum ItemRarety { Common, Uncommon, Rare, Epic, Legendary }; // All items rarety

public class Player_Inventory : MonoBehaviour
{  
    public static Player_Inventory instance;

    public ItemDataBase itemDataBase; // The database that contain ALL items

    [SerializeField] int playerGold;
    public int GetPlayerGold()
    {
        return playerGold;
    }

    // you can put negative amount to decrease money, or a positive amount to increase it
    public void SetPlayerGold(int amount)
    {
        if (playerGold + amount > 1000000000) // Security to not break the game because of int limit.
        {
            playerGold = 1000000000;
        }
        else
        {
            playerGold += amount;
        }
    }

    public const int armorySlotsNumb = 6; // Number of armory slots
    public const int inventorySlotsNumb = 18; // Number of inventory slots
    public const int bankSlotsNumb = 36;

    [Header("Player's armory")]
    [SerializeField] EquipmentItem[] armoryItems;
    [Header("Player's inventory")]
    [SerializeField] BaseItem[] inventoryItems;
    [Header("Player's bank")]
    [SerializeField] BaseItem[] bankItems;

    [HideInInspector] public int[] inventoryItemsNumb; // To keep a track when its a stackable item
    [HideInInspector] public int[] bankItemsNumb;

    private void Awake()
    {
        // Make this singleton
        if (!instance)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        inventoryItemsNumb = new int[inventoryItems.Length];
        bankItemsNumb = new int[bankItems.Length];
    }

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

    // False = slots available, true = full
    public bool CheckIfBankIsFull()
    {
        for (int i = 0; i < bankItems.Length; i++)
        {
            if (bankItems[i] == null)
                return false;
        }
        // If we're here bank got no more space
        Debug.Log("No more available slot in the bank !");
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
            if (Quests_Control.instance.GetPlayerQuestByID(questItem.questID))
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
                    if (inventoryItems[i].itemID == item.itemID) // If its the same item than stackable one increment it
                    {
                        inventoryItemsNumb[i]++;

                        if (UI_Player.instance.playerInventoryUI)
                        {
                            UI_Player.instance.playerInventoryUI.RefreshInventory();
                        }

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
                {
                    inventoryItemsNumb[i]++;

                    if (UI_Player.instance.playerInventoryUI)
                        UI_Player.instance.playerInventoryUI.GetInventorySlotByIndex(i).itemNumb++;

                }

                if (UI_Player.instance.playerInventoryUI)
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

    // Used to set inventory item (GameControl.cs)
    public void SetInventoryIndex(int inventoryIndex, int _itemID, int _itemNumb = 0)
    {
        // Next condition is used to remove item in the inventoryIndex slot. Because item's IDs will never be NEGATIVE
        if (_itemID == -1)
        {
            inventoryItems[inventoryIndex] = null;
        }

        inventoryItems[inventoryIndex] = itemDataBase.GetItemById(_itemID);

        if (_itemNumb > 0)
        {
            inventoryItemsNumb[inventoryIndex] = _itemNumb;
        }
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

    // used in GameControl.cs to save bank index item if there is an item in
    public BaseItem GetBankItem(int bankIndex)
    {
        if (bankItems[bankIndex] != null)
            return bankItems[bankIndex];
        return null;
    }

    // Used to set inventory item (Game_Data_Control.cs)
    public void SetBankItemSlot(int bankIndex, int _itemID, int _itemNumb = 0)
    {
        // Next condition is used to remove item in the inventoryIndex slot. Because item's IDs will never be NEGATIVE
        if (_itemID == -1)
        {
            bankItems[bankIndex] = null;
        }

        bankItems[bankIndex] = itemDataBase.GetItemById(_itemID);

        if (_itemNumb > 0)
        {
            bankItemsNumb[bankIndex] = _itemNumb;
        }
    }
}
