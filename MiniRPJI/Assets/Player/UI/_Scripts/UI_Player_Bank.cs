/* UI_Player_Bank.cs :
 * 
 * Gère l'interface de la banque du joueur.
 * Il peut y stocker ses items et les retirer.
 * 
 * 
 * */

using UnityEngine;
using UnityEngine.UI;

public class UI_Player_Bank : MonoBehaviour
{
    [SerializeField] GameObject bankSlotInteractionUI;

    [SerializeField] Button getButton;
    [SerializeField] Button sellButton;

    UI_DisplayItemStats itemStatsDisplay; // Stats displayer (share the same with UI_Player_Inventory)

    [SerializeField] InventorySlot[] bankSlots;

    int currentSlotInteraction = -1; // Important to set it -1 because bank index starts at 0

    [SerializeField] Button quitButton;

    // Start is called before the first frame update
    void Start()
    {
        if (bankSlotInteractionUI.activeSelf)
            bankSlotInteractionUI.SetActive(false);

        RefreshBankSlots();

        itemStatsDisplay = FindObjectOfType<UI_DisplayItemStats>();

        if (quitButton)
        {
            quitButton.onClick.AddListener(() => UI_Player.instance.ToggleBankUI(false));
        }
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
                    Player_Inventory.instance.bankItemsNumb[slotIndex]--;
                }

                if (bankSlots[slotIndex].itemNumb < 1)
                {
                    bankSlots[slotIndex].item = null;
                    Player_Inventory.instance.SetBankItemSlot(slotIndex, -1);
                }
            }
            else
            {
                bankSlots[slotIndex].item = null;
                Player_Inventory.instance.SetBankItemSlot(slotIndex, -1);
            }

            RefreshBankSlots();

            if (currentSlotInteraction == slotIndex)
                SetCurrentBankSlotInteractions(slotIndex);
        }
    }

    // Method to quickly refresh bank
    public void RefreshBankSlots()
    {
        for (int i = 0; i < Player_Inventory.bankSlotsNumb; i++)
        {
            if (Player_Inventory.instance.GetBankItem(i) != null)
            {
                bankSlots[i].item = Player_Inventory.instance.GetBankItem(i);

                if (bankSlots[i].item.stackableItem)
                {
                    bankSlots[i].itemNumb = Player_Inventory.instance.bankItemsNumb[i];
                }

            }

            bankSlots[i].RefreshSlot();

        }
    }



    public void StoreNewItem(BaseItem item)
    {
        if (Player_Inventory.instance.CheckIfBankIsFull())
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
                        RefreshBankSlots();
                        Player_Inventory.instance.bankItemsNumb[i]++;
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
                Player_Inventory.instance.SetBankItemSlot(i, item.itemID);

                // We need to check if its a stackable item here too.
                if (item.stackableItem)
                {
                    bankSlots[i].itemNumb++;
                    Player_Inventory.instance.bankItemsNumb[i]++;
                }

                RefreshBankSlots();
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
            if (UI_Player.instance.playerInventoryUI)
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
            if (itemStatsDisplay)
            {
                itemStatsDisplay.HideAndReset();

                if (bankSlots[indexSlot].item as EquipmentItem)
                    itemStatsDisplay.DisplayItemStats((EquipmentItem)bankSlots[indexSlot].item);
                else if (bankSlots[indexSlot].item as UsableItem)
                    itemStatsDisplay.DisplayItemStats((UsableItem)bankSlots[indexSlot].item);
                else if (bankSlots[indexSlot].item as QuestItem)
                    itemStatsDisplay.DisplayItemStats((QuestItem)bankSlots[indexSlot].item);
            }
        }
        else // If currentInventorySlotIndex == indexSlot, player clicked on the same item so we want to unshow slotIntercationsUI and reset buttons
        {
            getButton.onClick.RemoveAllListeners();
            sellButton.onClick.RemoveAllListeners();

            currentSlotInteraction = -1;

            if (bankSlotInteractionUI.activeSelf)
                bankSlotInteractionUI.SetActive(false);

            if (itemStatsDisplay)
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

            if (itemStatsDisplay)
                itemStatsDisplay.HideAndReset();
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
