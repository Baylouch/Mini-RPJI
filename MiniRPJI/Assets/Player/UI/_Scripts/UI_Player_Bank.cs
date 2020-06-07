/* UI_Player_Bank.cs :
 * Gère la banque du joueur.
 * Il peut y stocker ses items et les retirer.
 * 
 * 
 * */

using UnityEngine;
using UnityEngine.UI;

public class UI_Player_Bank : MonoBehaviour
{
    public const int bankSlotsNumb = 36;

    [SerializeField] GameObject bankSlotInteractionUI;

    [SerializeField] Button getButton;
    [SerializeField] Button sellButton;

    [SerializeField] UI_DisplayItemStats itemStatsDisplay; // Stats displayer (share the same with UI_Player_Inventory)

    [SerializeField] InventorySlot[] bankSlots;

    int currentSlotInteraction = -1; // Important to set it -1 because bank index starts at 0

    // Start is called before the first frame update
    void Start()
    {
        if (bankSlotInteractionUI.activeSelf)
            bankSlotInteractionUI.SetActive(false);

        RefreshBank();
    }

    void OnDisable()
    {
        getButton.onClick.RemoveAllListeners();
        sellButton.onClick.RemoveAllListeners();

        if (bankSlotInteractionUI.activeSelf)
            bankSlotInteractionUI.SetActive(false);
      
    }

    void RemoveItem(int slotIndex)
    {
        if (bankSlots[slotIndex].item != null)
        {
            // If its a stackable item, decrement it or delete it.
            if (bankSlots[slotIndex].item.stackableItem)
            {
                if (bankSlots[slotIndex].itemNumb >= 1)
                {
                    bankSlots[slotIndex].itemNumb--;
                }

                if (bankSlots[slotIndex].itemNumb < 1)
                {
                    bankSlots[slotIndex].item = null;
                }
            }
            else
            {
                bankSlots[slotIndex].item = null;
            }

            RefreshBank();

            if (currentSlotInteraction == slotIndex)
                SetCurrentBankSlotInteractions(slotIndex);
        }
    }

    // Method to quickly refresh bank
    public void RefreshBank()
    {
        for (int i = 0; i < bankSlots.Length; i++)
        {
            // Refresh inventory item
            bankSlots[i].RefreshSlot();
        }
    }

    // False = slots available, true = full
    public bool CheckIfBankIsFull()
    {
        for (int i = 0; i < bankSlots.Length; i++)
        {
            if (bankSlots[i].item == null)
                return false;
        }
        // If we're here bank got no more space
        Debug.Log("No more available slot in the bank !");
        return true;
    }

    public void StoreNewItem(BaseItem item)
    {
        if (CheckIfBankIsFull())
            return;

        // Issue : If player got 5 item / 3 required for the quest. When he store one, he still got 4 item in inventory, 1 in bank, but quest objective display 2/3.
        // For now i'll just skip this because its not a big issue, player still got the item but in the bank instead of inventory.
        // Check if its a quest item
        //if (item as QuestItem)
        //{
        //    QuestItem questItem = (QuestItem)item;
        //    // Check if we got the quest linked to the item.
        //    if (Quests_Control.instance.GetPlayerQuestByID(questItem.questID))
        //    {
        //        questItem.DecrementLinkedQuest();
        //    }
        //}

        if (item.stackableItem) // Check if you can stack item in inventory
        {
            for (int i = 0; i < bankSlots.Length; i++) // Check in every items
            {
                if (bankSlots[i].item != null)
                {
                    if (bankSlots[i].item == item) // If its the same item than stackable one increment it
                    {
                        bankSlots[i].itemNumb++;
                        RefreshBank();
                        return; // dont continue
                    }
                }
            }
        }

        // Find the first available slot to put item
        for (int i = 0; i < bankSlots.Length; i++)
        {
            if (bankSlots[i].item == null)
            {
                // set new item in inventory
                bankSlots[i].item = item;
                // We need to check if its a stackable item here too.
                if (item.stackableItem)
                    bankSlots[i].itemNumb++;
                RefreshBank();
                return; // Get out of there
            }
        }
    }

    // Used on "OnClick" method of every InventorySlot to set button right slot index
    public void SetCurrentBankSlotInteractions(int indexSlot)
    {
        if (currentSlotInteraction != indexSlot)
        {
            // Remove previous listeners
            getButton.onClick.RemoveAllListeners();
            sellButton.onClick.RemoveAllListeners();

            // If UI_Player_Inventory is in use, check if its interactions are set. If yes reset it.
            if (UI_Player.instance.playerInventoryUI.gameObject.activeSelf)
            {
                UI_Player.instance.playerInventoryUI.ResetInteractionsParameters();
            }

            // Set getButton
            getButton.onClick.AddListener(() => GetItemInInventory(indexSlot));
           
            // Set sell button if the item price is > than 0
            if (bankSlots[indexSlot].item.itemSellPrice > 0)
            {
                sellButton.onClick.AddListener(() => SellItem(indexSlot));
                sellButton.gameObject.SetActive(true);
            }
            else
            {
                sellButton.gameObject.SetActive(false);
            }

            // Set currentInventorySlotIndex with the right indexSlot. It'll be used if player click on the same index, so we can hide it.
            currentSlotInteraction = indexSlot;

            // Enable inventory interactions because its now set. Then disable armory interactions because player cant work on these 2 simultaneously
            if (!bankSlotInteractionUI.activeSelf)
                bankSlotInteractionUI.SetActive(true);

            // Then display item's stats
            itemStatsDisplay.HideAndReset();

            if (bankSlots[indexSlot].item as EquipmentItem)
                itemStatsDisplay.DisplayItemStats((EquipmentItem)bankSlots[indexSlot].item);
            else if (bankSlots[indexSlot].item as UsableItem)
                itemStatsDisplay.DisplayItemStats((UsableItem)bankSlots[indexSlot].item);
            else if (bankSlots[indexSlot].item as QuestItem)
                itemStatsDisplay.DisplayItemStats((QuestItem)bankSlots[indexSlot].item);
        }
        else // If currentInventorySlotIndex == indexSlot, player clicked on the same item so we want to unshow slotIntercationsUI and reset buttons
        {
            getButton.onClick.RemoveAllListeners();
            sellButton.onClick.RemoveAllListeners();

            currentSlotInteraction = -1;

            if (bankSlotInteractionUI.activeSelf)
                bankSlotInteractionUI.SetActive(false);

            itemStatsDisplay.HideAndReset();
        }
    }

    public void SellItem(int inventoryIndex)
    {
        if (bankSlots[inventoryIndex].item)
        {
            if (bankSlots[inventoryIndex].item.itemSellPrice > 0)
            {
                if (Player_Inventory.instance)
                {
                    Player_Inventory.instance.SetPlayerGold(Mathf.RoundToInt(bankSlots[inventoryIndex].item.itemSellPrice));

                    if (Sound_Manager.instance)
                    {
                        Sound_Manager.instance.PlaySound(Sound_Manager.instance.asset.sell);
                    }

                    RemoveItem(inventoryIndex);
                }
            }
        }
    }

    public void GetItemInInventory(int inventoryIndex)
    {
        if (bankSlots[inventoryIndex].item != null)
        {
            // Check if player inventory isnt full
            if (!Player_Inventory.instance.CheckInventoryIsFull())
            {
                // Put item in player inventory
                Player_Inventory.instance.GetNewItem(bankSlots[inventoryIndex].item);

                if (Sound_Manager.instance)
                {
                    Sound_Manager.instance.PlaySound(Sound_Manager.instance.asset.bankStoreAndGet);
                }

                // Remove item from bank
                RemoveItem(inventoryIndex);
            }

        }
    }

    public void ResetInteractionsParameters()
    {
        if (currentSlotInteraction != -1)
        {
            getButton.onClick.RemoveAllListeners();
            sellButton.onClick.RemoveAllListeners();

            currentSlotInteraction = -1;

            if (bankSlotInteractionUI.activeSelf)
                bankSlotInteractionUI.SetActive(false);

            itemStatsDisplay.HideAndReset();
        }
    }

    // used in GameControl.cs to save bank index item if there is an item in
    public BaseItem GetBankItem(int bankIndex)
    {
        if (bankSlots[bankIndex].item != null)
            return bankSlots[bankIndex].item;
        return null;
    }

    // Used to set inventory item (Game_Data_Control.cs)
    public void SetBankItemSlot(int bankIndex, int _itemID, int _itemNumb = 0)
    {
        // Next condition is used to remove item in the inventoryIndex slot. Because item's IDs will never be NEGATIVE
        if (_itemID == -1)
        {
            bankSlots[bankIndex].item = null;
        }

        bankSlots[bankIndex].item = Player_Inventory.instance.itemDataBase.GetItemById(_itemID);

        if (_itemNumb > 0)
        {
            bankSlots[bankIndex].itemNumb = _itemNumb;
        }
    }

    // Method used in Game_Data_Control to get item numb
    public InventorySlot GetBankSlotByIndex(int indexSlot)
    {
        if (bankSlots[indexSlot] != null)
            return bankSlots[indexSlot];
        return null;
    }
}
