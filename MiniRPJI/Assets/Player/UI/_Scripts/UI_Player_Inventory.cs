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
    [SerializeField] Button storeButton;

    [SerializeField] Text playerMoneyText;

    UI_DisplayItemStats itemStatsDisplay; // Stats displayer

    int currentInventorySlotIndex = -1; // Important to set it -1 because inventory index starts at 0
    int currentArmorySlotIndex = -1;

    [SerializeField] ArmorySlot[] armorySlots; // Contains all armory's slots manually set

    [SerializeField] InventorySlot[] inventorySlots; // Contains all inventory's slots manually set

    float clicDelayToEquip = .3f;
    float lastTimeClickedOnItem = 0f;

    [SerializeField] Button quitButton;

    // Start is called before the first frame update
    void Start()
    {
        // Check if interactions are disable when game start
        if (armorySlotIntercationsUI.activeSelf)
            armorySlotIntercationsUI.SetActive(false);

        if (inventorySlotInteractionsUI.activeSelf)
            inventorySlotInteractionsUI.SetActive(false);

        RefreshInventory();
        RefreshArmory();

        itemStatsDisplay = FindObjectOfType<UI_DisplayItemStats>();

        if (quitButton)
        {
            quitButton.onClick.AddListener(() => UI_Player.instance.ToggleInventoryMenu());
        }
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

    private void OnDestroy()
    {
        // Hide and reset stats displayer
        if (itemStatsDisplay)
            itemStatsDisplay.HideAndReset();
        
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
        storeButton.onClick.RemoveAllListeners();
    }

    // Method to quickly refresh inventory
    public void RefreshInventory()
    {
        for (int i = 0; i < inventorySlots.Length; i++)
        {
            // Refresh inventory item
            inventorySlots[i].item = Player_Inventory.instance.GetInventoryItem(i);

            if (inventorySlots[i].item && inventorySlots[i].item.stackableItem)
            {
                inventorySlots[i].itemNumb = Player_Inventory.instance.inventoryItemsNumb[i];
            }

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

            // Check if UI_Player_Bank is in use. If yes reset interactions parameters.
            if (UI_Player.instance.playerBankUI)
            {
                UI_Player.instance.playerBankUI.ResetInteractionsParameters();
            }

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
            if (storeButton.gameObject.activeSelf)
                storeButton.gameObject.SetActive(false);

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

            // Check if we can store the item (if player is using the bank)
            if (UI_Player.instance.playerBankUI && UI_Player.instance.playerBankUI.gameObject.activeSelf)
            {
                // add listener on the storeButton to store item in the bank.
                 storeButton.onClick.AddListener(() => StoreItem(indexSlot));

                if (!storeButton.gameObject.activeSelf)
                    storeButton.gameObject.SetActive(true);
            }

            // Set currentInventorySlotIndex with the right indexSlot. It'll be used if player click on the same index, so we can hide it.
            currentInventorySlotIndex = indexSlot;
            currentArmorySlotIndex = -1;

            // Enable inventory interactions because its now set. Then disable armory interactions because player cant work on these 2 simultaneously
            if (!inventorySlotInteractionsUI.activeSelf)
                inventorySlotInteractionsUI.SetActive(true);
            if (armorySlotIntercationsUI.activeSelf)
                armorySlotIntercationsUI.SetActive(false);

            // Then display item's stats
            if (itemStatsDisplay)
            {
                itemStatsDisplay.HideAndReset();

                if (inventorySlots[indexSlot].item as EquipmentItem)
                    itemStatsDisplay.DisplayItemStats((EquipmentItem)inventorySlots[indexSlot].item);
                else if (inventorySlots[indexSlot].item as UsableItem)
                    itemStatsDisplay.DisplayItemStats((UsableItem)inventorySlots[indexSlot].item);
                else if (inventorySlots[indexSlot].item as QuestItem)
                    itemStatsDisplay.DisplayItemStats((QuestItem)inventorySlots[indexSlot].item);
            }
            // Set the last time player clicked on it to check if click again
            lastTimeClickedOnItem = Time.time;
        }
        else // If currentInventorySlotIndex == indexSlot, player clicked on the same item so we want to unshow slotIntercationsUI and reset buttons OR equip / use the item
        {
            // Check if player double clicked
            if (Time.time < lastTimeClickedOnItem + clicDelayToEquip)
            {
                if (inventorySlots[indexSlot].item as EquipmentItem) // Equip item
                {
                    EquipItem((EquipmentItem)inventorySlots[indexSlot].item);
                }
                else if (inventorySlots[indexSlot].item as UsableItem) // Use item
                {
                    UseItem((UsableItem)inventorySlots[indexSlot].item);
                }
            }

            RemoveAllButtonsListeners();

            currentInventorySlotIndex = -1;

            if (inventorySlotInteractionsUI.activeSelf)
                inventorySlotInteractionsUI.SetActive(false);

            if (itemStatsDisplay)
                itemStatsDisplay.HideAndReset();
        }
    }

    // Used on "OnClick" method of every ArmorySlot to set right armory item
    public void SetCurrentArmorySlotInteractions(int indexPart) // Refer to ArmoryPart for know number of each part
    {
        if (currentArmorySlotIndex != indexPart)
        {
            RemoveAllButtonsListeners();

            // Check if UI_Player_Bank is in use. If yes reset interactions parameters.
            if (UI_Player.instance.playerBankUI)
            {
                UI_Player.instance.playerBankUI.ResetInteractionsParameters();
            }

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

            // Display item's stats
            if (itemStatsDisplay)
            {
                itemStatsDisplay.HideAndReset();
                itemStatsDisplay.DisplayItemStats(armorySlots[indexPart].item);
            }

            lastTimeClickedOnItem = Time.time;
        }
        else // If its equal, player clicked on the same item so we want to unshow slotIntercationsUI and reset buttons
        {
            // Check if player double clicked
            if (Time.time < lastTimeClickedOnItem + clicDelayToEquip)
            {
                UnequipItem(indexPart);
            }

            RemoveAllButtonsListeners();

            currentArmorySlotIndex = -1;

            if (armorySlotIntercationsUI.activeSelf)
                armorySlotIntercationsUI.SetActive(false);

            if (itemStatsDisplay)
                itemStatsDisplay.HideAndReset();
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
                if (Quests_Control.instance.GetPlayerQuestByID(questItem.questID))
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
                    Player_Inventory.instance.inventoryItemsNumb[currentInventorySlotIndex]--;
                }

                if (inventorySlots[currentInventorySlotIndex].itemNumb < 1)
                {
                    Player_Inventory.instance.SetInventoryIndex(itemIndex, -1);

                    // Check if this potion was on a fast potion use input, if yes remove it.
                    if (UI_Player_Potions.instance)
                    {
                        if (UI_Player_Potions.instance.firstPotion && 
                            UI_Player_Potions.instance.firstPotion.itemID == inventorySlots[currentInventorySlotIndex].item.itemID)
                        {
                            UI_Player_Potions.instance.firstPotion = null;
                        }

                        if (UI_Player_Potions.instance.secondPotion && 
                            UI_Player_Potions.instance.secondPotion.itemID == inventorySlots[currentInventorySlotIndex].item.itemID)
                        {
                            UI_Player_Potions.instance.secondPotion = null;
                        }
                    }
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
                itemStatsDisplay.HideAndReset();
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
                itemStatsDisplay.HideAndReset();
        }
    }

    // Method to equip a player on armory slot
    public void EquipItem(EquipmentItem item)
    {
        if (item.levelRequired > Player_Stats.instance.GetCurrentLevel())
        {
            // Debug.Log("You have not the level required for this item.");

            UI_Player_Informations.instance.DisplayInformation("Tu n'as pas le niveau requis.");
           
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
                // Debug.Log("No more slots in inventory !!!!");
                UI_Player_Informations.instance.DisplayInformation("L'inventaire est plein.");
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
            itemStatsDisplay.HideAndReset();
    }

    // Method to use a usable item
    public void UseItem(UsableItem item)
    {
        if (item == null) // I saw only one time an error when player used its last potion leading me here with a null reference. I suppose this will fix it.
            return;

        if (!item.CanUse())
            return;

        item.Use();

        for (int i = 0; i < inventorySlots.Length; i++) // Loop over inventory slots to find the item used to decrease the right one.
        {
            // Goal with line 2 next lines is to avoid null reference when player use fast potion when there is no more potions available.
            if (inventorySlots[i].item == null)
                continue;

            if (inventorySlots[i].item.itemID == item.itemID) // We found it.
            {
                // If player already using inventory, we need to put currentInventorySlotIndex in a temp variable because UseItem use currentInventorySlotIndex to use potion.
                int tempCurrentInventorySlotIndex = currentInventorySlotIndex;

                currentInventorySlotIndex = i; // put the right inventorySlots index into currentInventorySlotIndex to have the right action

                if (inventorySlots[currentInventorySlotIndex].item.stackableItem)
                {
                    if (inventorySlots[currentInventorySlotIndex].itemNumb > 1)
                    {
                        inventorySlots[currentInventorySlotIndex].itemNumb--;
                        Player_Inventory.instance.inventoryItemsNumb[currentInventorySlotIndex]--;
                        RefreshInventory();
                    }
                    else
                    {
                        RemoveItem(currentInventorySlotIndex, false);
                    }
                }
                else
                {
                    RemoveItem(currentInventorySlotIndex, false);
                }

                currentInventorySlotIndex = tempCurrentInventorySlotIndex; // Reset currentInventorySlotIndex as it was
                return;
            }
        }   
    }

    // Method used to store item in the bank
    public void StoreItem(int inventoryIndex)
    {
        if (inventorySlots[inventoryIndex].item)
        {
            // Check if bank isn't full
            if (UI_Player.instance.playerBankUI)
            {
                if (!Player_Inventory.instance.CheckIfBankIsFull())
                {
                    // Put item in the bank
                    UI_Player.instance.playerBankUI.StoreNewItem(inventorySlots[inventoryIndex].item);

                    if (Sound_Manager.instance)
                    {
                        Sound_Manager.instance.PlaySound(Sound_Manager.instance.asset.bankStoreAndGet);
                    }

                    // Remove item from inventory
                    RemoveItem(inventoryIndex, false);
                }
                else
                {
                    UI_Player_Informations.instance.DisplayInformation("La banque est pleine !");
                }
            }           
        }
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

    // Method to reset all interactions parameters. Used in UI_Player_Bank who got a mirror method used here.
    public void ResetInteractionsParameters()
    {
        // Check if currentInventorySlotIndex is in use ( != 1)
        if (currentInventorySlotIndex != -1)
        {
            RemoveAllButtonsListeners();

            currentInventorySlotIndex = -1;

            if (inventorySlotInteractionsUI.activeSelf)
                inventorySlotInteractionsUI.SetActive(false);

            if (itemStatsDisplay)
                itemStatsDisplay.HideAndReset();
        }

        // Check if currentArmorySlotIndex is in use
        if (currentArmorySlotIndex != -1)
        {
            RemoveAllButtonsListeners();

            currentArmorySlotIndex = -1;

            if (armorySlotIntercationsUI.activeSelf)
                armorySlotIntercationsUI.SetActive(false);

            if (itemStatsDisplay)
                itemStatsDisplay.HideAndReset();
        }
    }
}
