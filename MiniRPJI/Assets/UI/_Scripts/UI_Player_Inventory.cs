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

    [SerializeField] Button sellInventoryButton;
    [SerializeField] Button sellArmoryButton;

    [SerializeField] Text playerMoneyText;

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

    private void Update()
    {
        if (Player_Inventory.inventory_instance)
        {
            if (Player_Inventory.inventory_instance.GetPlayerGold().ToString() != playerMoneyText.text)
            {
                playerMoneyText.text = Player_Inventory.inventory_instance.GetPlayerGold().ToString();
            }
        }
    }

    private void OnDisable()
    {
        // Reset all buttons
        RemoveAllButtonsListeners();

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

    void RemoveAllButtonsListeners()
    {
        inventoryRemoveButton.onClick.RemoveAllListeners();
        equipButton.onClick.RemoveAllListeners();
        armoryRemoveButton.onClick.RemoveAllListeners();
        unequipButton.onClick.RemoveAllListeners();
        sellInventoryButton.onClick.RemoveAllListeners();
        sellArmoryButton.onClick.RemoveAllListeners();
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
            RemoveAllButtonsListeners();

            //  add right method with right index
            inventoryRemoveButton.onClick.AddListener(() => RemoveItem(indexSlot, true));

            // Hide usebutton if is active
            if (useButton.gameObject.activeSelf)
                useButton.gameObject.SetActive(false);
            if (equipButton.gameObject.activeSelf)
                equipButton.gameObject.SetActive(false);
            if (sellInventoryButton.gameObject.activeSelf)
                sellInventoryButton.gameObject.SetActive(false);

            // Detect if its an EquipmentItem or a UsableItem
            if (inventorySlots[indexSlot].item as EquipmentItem)
            {
                equipButton.onClick.AddListener(() => EquipItem((EquipmentItem)inventorySlots[indexSlot].item));

                if (!equipButton.gameObject.activeSelf)
                    equipButton.gameObject.SetActive(true);
            }
            if (inventorySlots[indexSlot].item as UsableItem)
            {
                useButton.onClick.AddListener(() => UseItem((UsableItem)inventorySlots[indexSlot].item));
                // Display usebutton
                if (!useButton.gameObject.activeSelf)
                    useButton.gameObject.SetActive(true);
            }

            // Set sell button
            if (inventorySlots[indexSlot].item.itemSellPrice > 0)
            {
                sellInventoryButton.onClick.AddListener(() => SellItem(indexSlot));

                if (!sellInventoryButton.gameObject.activeSelf)
                    sellInventoryButton.gameObject.SetActive(true);
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
                    itemStatsDisplay.DisplayItemStats((EquipmentItem)inventorySlots[indexSlot].item);
                else if (inventorySlots[indexSlot].item as UsableItem)
                    itemStatsDisplay.DisplayItemStats((UsableItem)inventorySlots[indexSlot].item);
                else if (inventorySlots[indexSlot].item as QuestItem)
                    itemStatsDisplay.DisplayItemStats((QuestItem)inventorySlots[indexSlot].item);
            }
            else // Else we need to reset before display. Because player want to see another item's stats
            {
                itemStatsDisplay.HideAndReset();
                if (inventorySlots[indexSlot].item as EquipmentItem)
                    itemStatsDisplay.DisplayItemStats((EquipmentItem)inventorySlots[indexSlot].item);
                else if (inventorySlots[indexSlot].item as UsableItem)
                    itemStatsDisplay.DisplayItemStats((UsableItem)inventorySlots[indexSlot].item);
                else if (inventorySlots[indexSlot].item as QuestItem)
                    itemStatsDisplay.DisplayItemStats((QuestItem)inventorySlots[indexSlot].item);
            }

        }
        else // If its equal, player clicked on the same item so we want to unshow slotIntercationsUI and reset buttons
        {
            RemoveAllButtonsListeners();

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
            RemoveAllButtonsListeners();

            armoryRemoveButton.onClick.AddListener(() => RemoveArmoryItem(indexPart, true));
            unequipButton.onClick.AddListener(() => UnequipItem(indexPart));

            // Set sell button
            if (armorySlots[indexPart].item.itemSellPrice > 0)
            {
                sellArmoryButton.onClick.AddListener(() => SellArmoryItem(indexPart));

                if (!sellArmoryButton.gameObject.activeSelf)
                    sellArmoryButton.gameObject.SetActive(true);
            }

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
                itemStatsDisplay.DisplayItemStats(armorySlots[indexPart].item);
            }
            else // Else we need to destroy before display. Because player want to see another item's stats
            {
                itemStatsDisplay.HideAndReset();
                itemStatsDisplay.DisplayItemStats(armorySlots[indexPart].item);
            }
        }
        else // If its equal, player clicked on the same item so we want to unshow slotIntercationsUI and reset buttons
        {
            RemoveAllButtonsListeners();

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

            // If its an item quest linked to quest we have, decrement current quest objective because we remove this item.
            if (Player_Inventory.inventory_instance.GetInventoryItem(itemIndex) as QuestItem)
            {
                QuestItem questItem = (QuestItem)Player_Inventory.inventory_instance.GetInventoryItem(itemIndex);
                if (Player_Quest_Control.quest_instance.GetPlayerQuestByID(questItem.questID))
                {
                    questItem.DecrementLinkedQuest();
                }
            }

            if (inventorySlots[currentInventorySlotIndex].item.stackableItem)
            {
                if (inventorySlots[currentInventorySlotIndex].itemNumb >= 1)
                {
                    inventorySlots[currentInventorySlotIndex].itemNumb--;
                }

                if (inventorySlots[currentInventorySlotIndex].itemNumb < 1)
                {
                    Player_Inventory.inventory_instance.SetInventoryIndex(itemIndex, -1);
                }
            }
            else
            {
                Player_Inventory.inventory_instance.SetInventoryIndex(itemIndex, -1);
            }

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
            if (!Player_Inventory.inventory_instance.CheckInventoryIsFull()) // If inventory isn't full put current armoryslot item in
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

        if (inventorySlots[currentInventorySlotIndex].item.stackableItem)
        {
            if (inventorySlots[currentInventorySlotIndex].itemNumb > 1)
            {
                inventorySlots[currentInventorySlotIndex].itemNumb--;
                RefreshInventory();

                return;
            }
        }

        RemoveItem(currentInventorySlotIndex, false);
        
    }

    // Method to sell an item in inventory
    public void SellItem(int inventoryIndex)
    {
        if (inventorySlots[inventoryIndex].item)
        {
            if (inventorySlots[inventoryIndex].item.itemSellPrice > 0)
            {
                if (Player_Inventory.inventory_instance)
                {
                    Player_Inventory.inventory_instance.SetPlayerGold(Mathf.RoundToInt(inventorySlots[inventoryIndex].item.itemSellPrice));
                    RemoveItem(inventoryIndex, false);
                }
            }
        }
    }

    // Method to sell an item in armory, same as the method above, just we need to use removeArmory insteand of just RemoveItem
    public void SellArmoryItem(int armoryIndex)
    {
        if (armorySlots[armoryIndex].item)
        {
            if (armorySlots[armoryIndex].item.itemSellPrice > 0)
            {
                if (Player_Inventory.inventory_instance)
                {
                    Player_Inventory.inventory_instance.SetPlayerGold(Mathf.RoundToInt(armorySlots[armoryIndex].item.itemSellPrice));
                    RemoveArmoryItem(armoryIndex, false);
                }
            }
        }
    }

    // Method used in Player_Inventory to increment stackable item numb in the slot.
    public InventorySlot GetInventorySlotByIndex(int indexSlot)
    {
        if (inventorySlots[indexSlot] != null)
            return inventorySlots[indexSlot];
        return null;
    }
}
