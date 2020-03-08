using UnityEngine;
using UnityEngine.UI;

public enum ArmoryPart { Helm, Chest, Pants, Gloves, Boots, Bow }; // Index of parts are same in armorySlots
public enum ItemRarety { Common, Uncommon, Rare, Epic, Legendary }; // All items rarety

public class Player_Inventory : MonoBehaviour
{  
    public static Player_Inventory inventory_instance;

    public GameObject inventoryUI;

    [SerializeField] GameObject inventorySlotInteractionsUI;
    [SerializeField] GameObject armorySlotIntercationsUI;
    [SerializeField] Button inventoryRemoveButton;
    [SerializeField] Button armoryRemoveButton;
    [SerializeField] Button equipButton;
    [SerializeField] Button unequipButton;
    [SerializeField] Button useButton;

    [SerializeField] UI_DisplayItemStats itemStatsDisplay; // Must have UI_DisplayItemStats on it

    int currentInventorySlotIndex = -1; // Important to set it -1 because inventory index starts at 0
    int currentArmorySlotIndex = -1;

    public ItemDataBase itemDataBase;

    public const int armorySlotsNumb = 6; // Used into GameControl to know number of slots
    [SerializeField] ArmorySlot[] armorySlots;

    public const int inventorySlotsNumb = 18; // Used into GameControl to know number of slots
    [SerializeField] InventorySlot[] inventorySlots;

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

    // Start is called before the first frame update
    void Start()
    {
        if (armorySlotIntercationsUI.activeSelf)
            armorySlotIntercationsUI.SetActive(false);

        if (inventorySlotInteractionsUI.activeSelf)
            inventorySlotInteractionsUI.SetActive(false);

        if (inventoryUI.activeSelf)
            inventoryUI.SetActive(false);

        RefreshInventory();
        RefreshArmory();
    }

    // Update is called once per frame
    void Update()
    {
        // TODO Centralise later ?
        // Toggle inventoryUI
        if (Input.GetKeyDown(KeyCode.B))
        {
            ToggleInventoryMenu();
        }
    }

    public void ToggleInventoryMenu()
    {
        if (!inventoryUI.activeSelf)
        {
            inventoryUI.SetActive(true);
        }
        else
        {
            // We need to reset slotIntercationsUIs too
            inventoryRemoveButton.onClick.RemoveAllListeners();
            equipButton.onClick.RemoveAllListeners();
            armoryRemoveButton.onClick.RemoveAllListeners();
            unequipButton.onClick.RemoveAllListeners();
            currentInventorySlotIndex = -1;
            currentArmorySlotIndex = -1;

            if (armorySlotIntercationsUI.activeSelf)
                armorySlotIntercationsUI.SetActive(false);

            if (inventorySlotInteractionsUI.activeSelf)
                inventorySlotInteractionsUI.SetActive(false);

            if (itemStatsDisplay)
            {
                itemStatsDisplay.HideAndReset();
            }

            inventoryUI.SetActive(false);
        }
    }

    public void RefreshInventory()
    {
        for (int i = 0; i < inventorySlots.Length; i++)
        {
            // Refresh inventory item
            inventorySlots[i].RefreshSlot();
        }
    }

    public void RefreshArmory()
    {
        for (int i = 0; i < armorySlots.Length; i++)
        {
            armorySlots[i].RefreshSlot();   
        }
    }

    // False = slots available, true = full
    public bool CheckInventoryIsFull()
    {
        for (int i = 0; i < inventorySlots.Length; i++)
        {
            if (inventorySlots[i].item == null)
                return false;
        }
        // If we're here inventory got no more space
        Debug.Log("No more available slot in Inventory !");
        return true;
    }

    // Used on "OnClick" method of every InventorySlot to set button right slot index
    public void SetCurrentInventorySlotInteractions(int indexSlot)
    {
        if (currentInventorySlotIndex != indexSlot)
        {
            // Remove previous listeners
            armoryRemoveButton.onClick.RemoveAllListeners();
            unequipButton.onClick.RemoveAllListeners();
            inventoryRemoveButton.onClick.RemoveAllListeners();
            equipButton.onClick.RemoveAllListeners();
            useButton.onClick.RemoveAllListeners();
            //  add right method with right index
            inventoryRemoveButton.onClick.AddListener(() => RemoveItem(indexSlot, true));

            // Hide usebutton if is active
            if (useButton.gameObject.activeSelf)
                useButton.gameObject.SetActive(false);
            if (inventorySlots[indexSlot].item as EquipmentItem)
            {
                equipButton.onClick.AddListener(() => EquipItem((EquipmentItem)inventorySlots[indexSlot].item));
            }
            if (inventorySlots[indexSlot].item as UsableItem)
            {
                useButton.onClick.AddListener(() => UseItem((UsableItem)inventorySlots[indexSlot].item));
                // Display usebutton
                if (!useButton.gameObject.activeSelf)
                    useButton.gameObject.SetActive(true);
            }

            currentInventorySlotIndex = indexSlot;
            currentArmorySlotIndex = -1;

            if (!inventorySlotInteractionsUI.activeSelf)
                inventorySlotInteractionsUI.SetActive(true);
            if (armorySlotIntercationsUI.activeSelf)
                armorySlotIntercationsUI.SetActive(false);

            // If we are not displaying yet
            if (!itemStatsDisplay)
            {
                // Display item stats
                if (inventorySlots[indexSlot].item as EquipmentItem)
                itemStatsDisplay.DisplayEquipmentItemStats((EquipmentItem)inventorySlots[indexSlot].item);
                else if (inventorySlots[indexSlot].item as UsableItem)
                    itemStatsDisplay.DisplayUsableItemStats((UsableItem)inventorySlots[indexSlot].item);
            }
            else // Else we need to reset before display. Because player want to see another item's stats
            {
                itemStatsDisplay.HideAndReset();
                if (inventorySlots[indexSlot].item as EquipmentItem)
                    itemStatsDisplay.DisplayEquipmentItemStats((EquipmentItem)inventorySlots[indexSlot].item);
                else if (inventorySlots[indexSlot].item as UsableItem)
                    itemStatsDisplay.DisplayUsableItemStats((UsableItem)inventorySlots[indexSlot].item);
            }

        }
        else // If its equal, player clicked on the same item so we want to unshow slotIntercationsUI and reset buttons
        {
            inventoryRemoveButton.onClick.RemoveAllListeners();
            equipButton.onClick.RemoveAllListeners();
            useButton.onClick.RemoveAllListeners();

            currentInventorySlotIndex = -1;

            if (inventorySlotInteractionsUI.activeSelf)
                inventorySlotInteractionsUI.SetActive(false);

            if (itemStatsDisplay)
            {
                itemStatsDisplay.HideAndReset();
            }
        }
    }

    // Used on "OnClick" method of every ArmorySlot to set right armory item
    public void SetCurrentArmorySlotInteractions(int indexPart) // Refer to ArmoryPart for know number of each part
    {
        if (currentArmorySlotIndex != indexPart)
        {
            inventoryRemoveButton.onClick.RemoveAllListeners();
            equipButton.onClick.RemoveAllListeners();
            armoryRemoveButton.onClick.RemoveAllListeners();
            unequipButton.onClick.RemoveAllListeners();

            armoryRemoveButton.onClick.AddListener(() => RemoveArmoryItem(indexPart, true));
            unequipButton.onClick.AddListener(() => UnequipItem(indexPart));

            currentArmorySlotIndex = indexPart;
            currentInventorySlotIndex = -1;

            if (inventorySlotInteractionsUI.activeSelf)
                inventorySlotInteractionsUI.SetActive(false);
            if (!armorySlotIntercationsUI.activeSelf)
                armorySlotIntercationsUI.SetActive(true);

            // If we are not displaying yet
            if (!itemStatsDisplay)
            {
                // Display item stats
                itemStatsDisplay.DisplayEquipmentItemStats(armorySlots[indexPart].item);
            }
            else // Else we need to destroy before display. Because player want to see another item's stats
            {
                itemStatsDisplay.HideAndReset();
                itemStatsDisplay.DisplayEquipmentItemStats(armorySlots[indexPart].item);
            }
        }
        else // If its equal, player clicked on the same item so we want to unshow slotIntercationsUI and reset buttons
        {
            armoryRemoveButton.onClick.RemoveAllListeners();
            unequipButton.onClick.RemoveAllListeners();

            currentArmorySlotIndex = -1;

            if (armorySlotIntercationsUI.activeSelf)
                armorySlotIntercationsUI.SetActive(false);

            if (itemStatsDisplay)
            {
                itemStatsDisplay.HideAndReset();
            }
        }            
    }

    public void PutNewItem(BaseItem item)
    {
        // Find the first available slot to put item
        for (int i = 0; i < inventorySlots.Length; i++)
        {
            if (inventorySlots[i].item == null)
            {
                // set new item in inventory
                inventorySlots[i].item = item;
                RefreshInventory();
                return; // Get out of there
            }
        }
    }

    // To remove and spawn item by inventory index
    public void RemoveItem(int itemIndex, bool instantiateItem)
    {
        if (inventorySlots[itemIndex].item != null)
        {
            if (instantiateItem)
            {
                // We need to instantiate on Player_Stats.stats_instance position
                Instantiate(inventorySlots[itemIndex].item.itemPrefab, Player_Stats.stats_instance.transform.position, Quaternion.identity);
            }
            inventorySlots[itemIndex].item = null;
            RefreshInventory();

            // we must check if currentInventorySlotIndex == itemIndex before else we'll set wrong informations
            if (currentInventorySlotIndex == itemIndex)
                SetCurrentInventorySlotInteractions(itemIndex);

            if (itemStatsDisplay)
            {
                itemStatsDisplay.HideAndReset();
            }
        }
    }

    // For remove and spawn item via Armory
    public void RemoveArmoryItem(int armoryIndex, bool instantiateItem)
    {
        if (armorySlots[armoryIndex].item != null)
        {
            if (instantiateItem)
            {
                Instantiate(armorySlots[armoryIndex].item.itemPrefab, Player_Stats.stats_instance.transform.position, Quaternion.identity);
            }

            armorySlots[armoryIndex].item = null;
            armorySlots[armoryIndex].RefreshSlot();

            if (currentArmorySlotIndex == armoryIndex)
                SetCurrentArmorySlotInteractions(armoryIndex);

            Player_Stats.stats_instance.RefreshPlayerStats();

            // If we dont disable gameobject here, error will happened when loading data. Because of clearing all items before load saved ones.
            if (armorySlotIntercationsUI.activeSelf)
                armorySlotIntercationsUI.SetActive(false);

            if (itemStatsDisplay)
            {
                itemStatsDisplay.HideAndReset();
            }
        }
    }

    public void EquipItem(EquipmentItem item)
    {
        for (int i = 0; i < armorySlots.Length; i++)
        {
            if (armorySlots[i].armoryPart == item.armoryPart)
            {
                // Set ArmorySlot
                if (armorySlots[i].item != null) // If current slot isn't empty, then switch item
                {
                    // Keep a track of current item on the slot (too put it on the same place in inventory)
                    EquipmentItem tempItemOnSlot = armorySlots[i].item;
                    // Then equip wanted item
                    armorySlots[i].item = item;
                    // And remove it from inventory (without instantiation) 
                    // /!\ CAREFULL HERE WE NEED TO USE currentInventorySlotIndex to know current index in inventory item /!\
                    RemoveItem(currentInventorySlotIndex, false);
                    // Then put tempItemOnSlot in inventory
                    PutNewItem(tempItemOnSlot);
                }
                else
                {
                    // Else slot is alreay empty so just equip
                    // TODO check required stats
                    armorySlots[i].item = item;
                    // And remove it from inventory (without instantiation)
                    RemoveItem(currentInventorySlotIndex, false);
                }
                armorySlots[i].RefreshSlot();
                Player_Stats.stats_instance.RefreshPlayerStats();
                return;
            }
        }
    }

    public void UnequipItem(int armoryIndex)
    {
        if (armorySlots[armoryIndex].item != null) // If slot isn't empty, then continue
        {
            if (!CheckInventoryIsFull()) // If inventory isn't full put current helmslot item in
            {
                PutNewItem(armorySlots[armoryIndex].item);
                armorySlots[armoryIndex].item = null;
                Player_Stats.stats_instance.RefreshPlayerStats();
            }
            else // Else you can only remove it.
            {
                Debug.Log("No more slots in inventory !!!!");
                return;
            }
        }
        else
        {
            // If there is no item in slot
        }
        armorySlots[armoryIndex].RefreshSlot();

        // If we dont disable gameobject here, error will happened because player still can click on "remove" and will get error
        if (armorySlotIntercationsUI.activeSelf)
            armorySlotIntercationsUI.SetActive(false);
        if (itemStatsDisplay)
        {
            itemStatsDisplay.HideAndReset();
        }               
    }

    public void UseItem(UsableItem item)
    {
        item.Use();
        if (item.used)
        {
            RemoveItem(currentInventorySlotIndex, false);
        }
    }

    // Method use in Player_Combat_Control to get projectile of current bow.
    public EquipmentItem GetCurrentBow()
    {
        if (armorySlots[5].item)
        {
            return armorySlots[5].item;
        }

        return null;
    }

    // Used in Player Stats to apply item stats. And in GameControl.cs to save current EquipmentItems
    public EquipmentItem GetArmorySlotItem(int slotIndex)
    {
        if (armorySlots[slotIndex].item)
            return armorySlots[slotIndex].item;
        return null;
    }

    // used in GameControl.cs to save inventory index item if there is an item in
    public BaseItem GetInventorySlotItem(int slotIndex)
    {
        if (inventorySlots[slotIndex].item)
            return inventorySlots[slotIndex].item;
        return null;
    }

    // used in GameControl.cs to load inventory items
    public void SetInventorySlots(int slotIndex, int _itemID)
    {
        inventorySlots[slotIndex].item = itemDataBase.GetItemById(_itemID);
    }

    // used in GameControl.cs to load armory items
    public void SetArmorySlots(int slotIndex, int _itemID)
    {
        armorySlots[slotIndex].item = (EquipmentItem)itemDataBase.GetItemById(_itemID);
    }
}
