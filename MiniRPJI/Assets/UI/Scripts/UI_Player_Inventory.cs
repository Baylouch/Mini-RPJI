using UnityEngine;
using UnityEngine.UI;

public class UI_Player_Inventory : MonoBehaviour
{
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

    [SerializeField] ArmorySlot[] armorySlots;

    [SerializeField] InventorySlot[] inventorySlots;

    // Start is called before the first frame update
    void Start()
    {
        if (armorySlotIntercationsUI.activeSelf)
            armorySlotIntercationsUI.SetActive(false);

        if (inventorySlotInteractionsUI.activeSelf)
            inventorySlotInteractionsUI.SetActive(false);
    }

    private void OnDisable()
    {
        // Reset all
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
    }

    public void RefreshInventory()
    {
        for (int i = 0; i < inventorySlots.Length; i++)
        {
            // Refresh inventory item
            inventorySlots[i].item = Player_Inventory.inventory_instance.GetInventoryItem(i);
            inventorySlots[i].RefreshSlot();
        }
    }

    public void RefreshArmory()
    {
        for (int i = 0; i < armorySlots.Length; i++)
        {
            armorySlots[i].item = Player_Inventory.inventory_instance.GetArmoryItem(i);
            armorySlots[i].RefreshSlot();
        }
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

    // To remove and spawn item by inventory index
    public void RemoveItem(int itemIndex, bool instantiateItem)
    {
        if (Player_Inventory.inventory_instance.GetInventoryItem(itemIndex) != null)
        {
            if (instantiateItem)
            {
                // We need can instantiate on Player Inventory instance position
                Instantiate(Player_Inventory.inventory_instance.GetInventoryItem(itemIndex).itemPrefab, Player_Inventory.inventory_instance.transform.position, Quaternion.identity);
            }

            Player_Inventory.inventory_instance.SetInventoryIndex(itemIndex, -1);
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
        if (Player_Inventory.inventory_instance.GetArmoryItem(armoryIndex) != null)
        {
            if (instantiateItem)
            {
                Instantiate(Player_Inventory.inventory_instance.GetArmoryItem(armoryIndex).itemPrefab, Player_Inventory.inventory_instance.transform.position, Quaternion.identity);
            }

            Player_Inventory.inventory_instance.SetArmoryIndex(armoryIndex, -1);
            RefreshArmory();

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
        if (item.levelRequired > Player_Stats.stats_instance.getCurrentLevel())
        {
            Debug.Log("You have not the level required for this item.");
            return;
        }

        for (int i = 0; i < Player_Inventory.armorySlotsNumb; i++)
        {
            if (armorySlots[i].armoryPart == item.armoryPart) // Find the right slot
            {
                // Set ArmorySlot
                if (Player_Inventory.inventory_instance.GetArmoryItem(i) != null) // If current slot isn't empty, then switch item
                {
                    // Keep a track of current item on the slot (too put it on the same place in inventory)
                    EquipmentItem tempItemOnSlot = Player_Inventory.inventory_instance.GetArmoryItem(i);
                    // Then equip wanted item
                    Player_Inventory.inventory_instance.SetArmoryIndex(i, item.itemID);
                    // And remove it from inventory (without instantiation) 
                    // /!\ CAREFULL HERE WE NEED TO USE currentInventorySlotIndex to know current index in inventory item /!\
                    RemoveItem(currentInventorySlotIndex, false);
                    // Then put tempItemOnSlot in inventory
                    Player_Inventory.inventory_instance.GetNewItem(tempItemOnSlot);
                }
                else
                {
                    // Else slot is alreay empty so just equip
                    Player_Inventory.inventory_instance.SetArmoryIndex(i, item.itemID);
                    // And remove it from inventory (without instantiation)
                    RemoveItem(currentInventorySlotIndex, false);
                }

                RefreshArmory();
                Player_Stats.stats_instance.RefreshPlayerStats();
                return;
            }
        }
    }

    public void UnequipItem(int armoryIndex)
    {
        if (Player_Inventory.inventory_instance.GetArmoryItem(armoryIndex) != null) // If slot isn't empty, then continue
        {
            if (!Player_Inventory.inventory_instance.CheckInventoryIsFull()) // If inventory isn't full put current helmslot item in
            {
                Player_Inventory.inventory_instance.GetNewItem(armorySlots[armoryIndex].item); // Put item in inventory
                Player_Inventory.inventory_instance.SetArmoryIndex(armoryIndex, -1); // Remove it from armory items
                
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

        RefreshArmory();
        Player_Stats.stats_instance.RefreshPlayerStats();

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
}
