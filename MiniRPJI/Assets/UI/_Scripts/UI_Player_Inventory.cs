using UnityEngine;
using UnityEngine.UI;

public class UI_Player_Inventory : MonoBehaviour
{
    [SerializeField] GameObject inventorySlotInteractionsUI; // Inventory interactions ui (bottom left of the bottom window in player's inventory. Use - Equip - Sell - Delete buttons)
    [SerializeField] GameObject armorySlotIntercationsUI; // Armory interactions ui (bottom left of the top window in player's inventory. Unequip - Sell - Delete buttons)

    // Inventory's buttons for player interactions
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

    [SerializeField] ArmorySlot[] armorySlots; // Contains all armory's slots

    [SerializeField] InventorySlot[] inventorySlots; // Contains all inventory's slots

    // Start is called before the first frame update
    void Start()
    {
        // Check if interactions are disable when game start
        if (armorySlotIntercationsUI.activeSelf)
            armorySlotIntercationsUI.SetActive(false);

        if (inventorySlotInteractionsUI.activeSelf)
            inventorySlotInteractionsUI.SetActive(false);
    }

    private void Update()
    {
        // Update playerTextmoney.
        if (Player_Inventory.instance)
        {
            if (Player_Inventory.instance.GetPlayerGold().ToString() != playerMoneyText.text)
            {
                playerMoneyText.text = Player_Inventory.instance.GetPlayerGold().ToString();
            }
        }
    }

    private void OnDisable()
    {
        // Reset all buttons
        RemoveAllButtonsListeners();

        // Reset slots index to not have a pre-set one when player re open inventory
        currentInventorySlotIndex = -1;
        currentArmorySlotIndex = -1;

        // Disable interactions UI
        if (armorySlotIntercationsUI.activeSelf)
            armorySlotIntercationsUI.SetActive(false);

        if (inventorySlotInteractionsUI.activeSelf)
            inventorySlotInteractionsUI.SetActive(false);

        // Hide and reset stats displayer
        if (itemStatsDisplay)
        {
            itemStatsDisplay.HideAndReset();
        }
    }

    // Method to remove all buttons listeners.
    void RemoveAllButtonsListeners()
    {
        inventoryRemoveButton.onClick.RemoveAllListeners();
        equipButton.onClick.RemoveAllListeners();
        armoryRemoveButton.onClick.RemoveAllListeners();
        unequipButton.onClick.RemoveAllListeners();
        sellInventoryButton.onClick.RemoveAllListeners();
        sellArmoryButton.onClick.RemoveAllListeners();
    }

    // Method to quickly refresh inventory
    public void RefreshInventory()
    {
        for (int i = 0; i < inventorySlots.Length; i++)
        {
            // Refresh inventory item
            inventorySlots[i].item = Player_Inventory.instance.GetInventoryItem(i);
            inventorySlots[i].RefreshSlot();
        }
    }

    // Method to quickly refresh armory
    public void RefreshArmory()
    {
        for (int i = 0; i < armorySlots.Length; i++)
        {
            armorySlots[i].item = Player_Inventory.instance.GetArmoryItem(i);
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

            // add right method with right index
            // every items can be removed. So we dont need to particulary check for removebutton
            inventoryRemoveButton.onClick.AddListener(() => RemoveItem(indexSlot, true));

            // Instead of remove buttons, others need to be disabled because not every items can be used, sell or equip.
            if (useButton.gameObject.activeSelf)
                useButton.gameObject.SetActive(false);
            if (equipButton.gameObject.activeSelf)
                equipButton.gameObject.SetActive(false);
            if (sellInventoryButton.gameObject.activeSelf)
                sellInventoryButton.gameObject.SetActive(false);

            // Detect if its an EquipmentItem
            if (inventorySlots[indexSlot].item as EquipmentItem)
            {
                equipButton.onClick.AddListener(() => EquipItem((EquipmentItem)inventorySlots[indexSlot].item));

                if (!equipButton.gameObject.activeSelf)
                    equipButton.gameObject.SetActive(true);
            }
            // Detect if its a UsableItem
            if (inventorySlots[indexSlot].item as UsableItem)
            {
                useButton.onClick.AddListener(() => UseItem((UsableItem)inventorySlots[indexSlot].item));
                // Display usebutton
                if (!useButton.gameObject.activeSelf)
                    useButton.gameObject.SetActive(true);
            }

            // Set sell button if the item price is > than 0
            if (inventorySlots[indexSlot].item.itemSellPrice > 0)
            {
                sellInventoryButton.onClick.AddListener(() => SellItem(indexSlot));

                if (!sellInventoryButton.gameObject.activeSelf)
                    sellInventoryButton.gameObject.SetActive(true);
            }

            // Set currentInventorySlotIndex with the right indexSlot. It'll be used if player click on the same index, so we can hide it.
            currentInventorySlotIndex = indexSlot;
            currentArmorySlotIndex = -1;

            // Enable inventory interactions because its now set. Then disable armory interactions because player cant work on these 2 simultaneously
            if (!inventorySlotInteractionsUI.activeSelf)
                inventorySlotInteractionsUI.SetActive(true);
            if (armorySlotIntercationsUI.activeSelf)
                armorySlotIntercationsUI.SetActive(false);

            // If we are not displaying stats yet 
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
        else // If currentInventorySlotIndex == indexSlot, player clicked on the same item so we want to unshow slotIntercationsUI and reset buttons
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

    // To remove and spawn item via inventory index
    public void RemoveItem(int itemIndex, bool instantiateItem)
    {
        // First check if there is an item (it must be because of logic when interactions UI appear but its safer to check)
        if (Player_Inventory.instance.GetInventoryItem(itemIndex) != null)
        {
            if (instantiateItem) // If we want to instantiate the item
            {
                // We can instantiate on Player Inventory instance position (because its the player position)
                GameObject itemGO = Instantiate(Player_Inventory.instance.GetInventoryItem(itemIndex).itemPrefab, Player_Inventory.instance.transform.position, Quaternion.identity);

                itemGO.GetComponent<SpriteRenderer>().sprite = Player_Inventory.instance.GetInventoryItem(itemIndex).prefabImage;
                itemGO.GetComponent<Item>().itemConfig = Player_Inventory.instance.GetInventoryItem(itemIndex);

                if (GameObject.Find("Items"))
                {
                    itemGO.transform.parent = GameObject.Find("Items").transform;
                }

                Sound_Manager.instance.PlaySound(Sound_Manager.instance.asset.itemTrash);
            }

            // If its an item quest linked to quest we have, decrement current quest objective because we removed this item.
            if (Player_Inventory.instance.GetInventoryItem(itemIndex) as QuestItem)
            {
                QuestItem questItem = (QuestItem)Player_Inventory.instance.GetInventoryItem(itemIndex);
                if (Player_Quest_Control.instance.GetPlayerQuestByID(questItem.questID))
                {
                    questItem.DecrementLinkedQuest();
                }
            }

            // If its a stackable item, decrement it or delete it.
            if (inventorySlots[currentInventorySlotIndex].item.stackableItem)
            {
                if (inventorySlots[currentInventorySlotIndex].itemNumb >= 1)
                {
                    inventorySlots[currentInventorySlotIndex].itemNumb--;
                }

                if (inventorySlots[currentInventorySlotIndex].itemNumb < 1)
                {
                    Player_Inventory.instance.SetInventoryIndex(itemIndex, -1);
                }
            }
            else
            {
                Player_Inventory.instance.SetInventoryIndex(itemIndex, -1);
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

    // For remove and spawn item via Armory index. Same method as RemoveItem but we refresh player stats because its an equiped item.
    public void RemoveArmoryItem(int armoryIndex, bool instantiateItem)
    {
        if (Player_Inventory.instance.GetArmoryItem(armoryIndex) != null)
        {
            if (instantiateItem)
            {
                GameObject itemGO = Instantiate(Player_Inventory.instance.GetArmoryItem(armoryIndex).itemPrefab, Player_Inventory.instance.transform.position, Quaternion.identity);

                itemGO.GetComponent<SpriteRenderer>().sprite = Player_Inventory.instance.GetArmoryItem(armoryIndex).prefabImage;
                itemGO.GetComponent<Item>().itemConfig = Player_Inventory.instance.GetArmoryItem(armoryIndex);

                if (GameObject.Find("Items"))
                {
                    itemGO.transform.parent = GameObject.Find("Items").transform;
                }

                Sound_Manager.instance.PlaySound(Sound_Manager.instance.asset.itemTrash);
            }

            Player_Inventory.instance.SetArmoryIndex(armoryIndex, -1);
            RefreshArmory();

            if (currentArmorySlotIndex == armoryIndex)
                SetCurrentArmorySlotInteractions(armoryIndex);

            Player_Stats.instance.RefreshPlayerStats();

            // If we dont disable gameobject here, error will happened when loading data. Because of clearing all items before load saved ones.
            if (armorySlotIntercationsUI.activeSelf)
                armorySlotIntercationsUI.SetActive(false);

            if (itemStatsDisplay)
            {
                itemStatsDisplay.HideAndReset();
            }
        }
    }

    // Method to equip a player on armory slot
    public void EquipItem(EquipmentItem item)
    {
        if (item.levelRequired > Player_Stats.instance.GetCurrentLevel())
        {
            // TODO Display an UI ?
            Debug.Log("You have not the level required for this item.");
            return;
        }

        for (int i = 0; i < Player_Inventory.armorySlotsNumb; i++)
        {
            if (armorySlots[i].armoryPart == item.armoryPart) // Find the right slot
            {
                // Set ArmorySlot
                if (Player_Inventory.instance.GetArmoryItem(i) != null) // If current slot isn't empty, then switch item
                {
                    // Keep a track of current item on the slot (too put it on the same place in inventory)
                    EquipmentItem tempItemOnSlot = Player_Inventory.instance.GetArmoryItem(i);
                    // Then equip wanted item
                    Player_Inventory.instance.SetArmoryIndex(i, item.itemID);
                    // And remove it from inventory (without instantiation) 
                    // /!\ CAREFULL HERE WE NEED TO USE currentInventorySlotIndex to know current index in inventory item /!\
                    RemoveItem(currentInventorySlotIndex, false);
                    // Then put tempItemOnSlot in inventory
                    Player_Inventory.instance.GetNewItem(tempItemOnSlot);
                }
                else
                {
                    // Else slot is alreay empty so just equip
                    Player_Inventory.instance.SetArmoryIndex(i, item.itemID);
                    // And remove it from inventory (without instantiation)
                    RemoveItem(currentInventorySlotIndex, false);
                }

                if (Sound_Manager.instance)
                {
                    Sound_Manager.instance.PlaySound(Sound_Manager.instance.asset.itemEquip);
                }

                RefreshArmory();
                Player_Stats.instance.RefreshPlayerStats();
                return;
            }
        }
    }

    // Method to unequip an armory item
    public void UnequipItem(int armoryIndex)
    {
        if (Player_Inventory.instance.GetArmoryItem(armoryIndex) != null) // If slot isn't empty, then continue
        {
            if (!Player_Inventory.instance.CheckInventoryIsFull()) // If inventory isn't full put current armoryslot item in
            {
                Player_Inventory.instance.GetNewItem(armorySlots[armoryIndex].item); // Put item in inventory
                Player_Inventory.instance.SetArmoryIndex(armoryIndex, -1); // Remove it from armory items

                if (Sound_Manager.instance)
                {
                    Sound_Manager.instance.PlaySound(Sound_Manager.instance.asset.itemUnequip);
                }

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
        Player_Stats.instance.RefreshPlayerStats();

        // If we dont disable gameobject here, error will happened because player still can click on "remove" and will get error
        if (armorySlotIntercationsUI.activeSelf)
            armorySlotIntercationsUI.SetActive(false);
        if (itemStatsDisplay)
        {
            itemStatsDisplay.HideAndReset();
        }
    }

    // Method to use a usable item
    public void UseItem(UsableItem item)
    {
        if (!item.CanUse())
            return;

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
                if (Player_Inventory.instance)
                {
                    Player_Inventory.instance.SetPlayerGold(Mathf.RoundToInt(inventorySlots[inventoryIndex].item.itemSellPrice));

                    if (Sound_Manager.instance)
                    {
                        Sound_Manager.instance.PlaySound(Sound_Manager.instance.asset.sell);
                    }

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
                if (Player_Inventory.instance)
                {
                    Player_Inventory.instance.SetPlayerGold(Mathf.RoundToInt(armorySlots[armoryIndex].item.itemSellPrice));

                    if (Sound_Manager.instance)
                    {
                        Sound_Manager.instance.PlaySound(Sound_Manager.instance.asset.sell);
                    }

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

    // TODO Upgrade to use more than the potion ID 100
    public void FastPotionUse()
    {
        for (int i = 0; i < inventorySlots.Length; i++) // Loop over inventory slots
        {
            if (inventorySlots[i].item) // if there is an item in this slot
            {
                if (inventorySlots[i].item.itemID == 350) // test it to check if its a potion
                {
                    UsableItem potion = inventorySlots[i].item as UsableItem; // Convert it into a UsableItem

                    // If player already using inventory, we need to put currentInventorySlotIndex in a temp variable because UseItem use currentInventorySlotIndex to use potion.
                    int tempCurrentInventorySlotIndex = currentInventorySlotIndex;
                    currentInventorySlotIndex = i; // put the right inventorySlots index into currentInventorySlotIndex to have the right action

                    UseItem(potion); // use potion (will remove one from inventory too)

                    currentInventorySlotIndex = tempCurrentInventorySlotIndex; // Reset currentInventorySlotIndex as it was
                    return; // Dont continue because potion is used.
                }
            }
        }
    }
}
