using UnityEngine;
using UnityEngine.UI;

public class ItemSeller : Interactable
{
    // TODO Player interact, dialogue is launch, player click "ok", then display items to sell.
    // when item is sell, player gain money, item is added to player inventory and delete from seller inventory.
    [SerializeField] GameObject backPanel;

    [SerializeField] BaseItem[] itemsToSell;

    [SerializeField] Button buyButton; // This one need to be configure for every slots
    [SerializeField] Button okButton; // It can be configure at start
    [SerializeField] Button backButton; // It can be configure at start
    [SerializeField] Text textDialogue; // It can be configure at start

    [SerializeField] [TextArea] string dialogue;

    SellableSlot[] slots;

    UI_DisplayItemStats itemStatsDisplay; // Must have UI_DisplayItemStats on it

    private void Start()
    {
        interactionType = PlayerInteractionType.ItemSeller;

        textDialogue.text = dialogue;
        backButton.onClick.AddListener(UnInteract);
        okButton.onClick.AddListener(SetUIInventory);

        slots = GetComponentsInChildren<SellableSlot>();

        for (int i = 0; i < itemsToSell.Length; i++)
        {
            if (itemsToSell[i] != null)
            {
                slots[i].item = itemsToSell[i];
            }
        }

        UnActiveUI();
    }

    public override void Interact()
    {
        base.Interact();

        itemStatsDisplay = FindObjectOfType<UI_DisplayItemStats>();

        SetUIDialogue();
    }

    public override void UnInteract()
    {
        base.UnInteract();

        if (itemStatsDisplay)
            itemStatsDisplay.HideAndReset();

        UnActiveUI();
    }

    void UnActiveUI()
    {
        if (backPanel.activeSelf)
            backPanel.SetActive(false);

        for (int i = 0; i < slots.Length; i++)
        {
            if (slots[i].gameObject.activeSelf)
                slots[i].gameObject.SetActive(false);
        }

        if (buyButton.gameObject.activeSelf)
        {
            buyButton.onClick.RemoveAllListeners();
            buyButton.gameObject.SetActive(false);
        }
            

        if (okButton.gameObject.activeSelf)
        {
            okButton.gameObject.SetActive(false);
        }

        if (backButton.gameObject.activeSelf)
        {
            backButton.gameObject.SetActive(false);
        }

        if (textDialogue.gameObject.activeSelf)
        {
            textDialogue.gameObject.SetActive(false);
        }
    }

    void SetUIDialogue()
    {
        if (!backPanel.gameObject.activeSelf)
            backPanel.gameObject.SetActive(true);

        if (!textDialogue.gameObject.activeSelf)
            textDialogue.gameObject.SetActive(true);

        if (!okButton.gameObject.activeSelf)
            okButton.gameObject.SetActive(true);

        if (!backButton.gameObject.activeSelf)
            backButton.gameObject.SetActive(true);
    }

    void SetUIInventory()
    {
        if (textDialogue.gameObject.activeSelf)
            textDialogue.gameObject.SetActive(false);

        if (okButton.gameObject.activeSelf)
            okButton.gameObject.SetActive(false);

        for (int i = 0; i < slots.Length; i++)
        {
            if (slots[i].item != null)
            {
                slots[i].gameObject.SetActive(true);
                slots[i].RefreshSlot();
            }
        }
    }

    // Method used on every item seller slots to configure each when player click on it.
    public void SetSlot(int indexSlot)
    {
        if (slots[indexSlot].item != null)
        {
            buyButton.onClick.RemoveAllListeners();
            if (Player_Inventory.instance)
            {
                if (Player_Inventory.instance.GetPlayerGold() >= slots[indexSlot].item.itemSellPrice)
                {
                    buyButton.onClick.AddListener(() => Player_Inventory.instance.SetPlayerGold(-Mathf.RoundToInt(slots[indexSlot].item.itemSellPrice)));
                    buyButton.onClick.AddListener(() => Player_Inventory.instance.GetNewItem(slots[indexSlot].item));
                    buyButton.onClick.AddListener(() => Sound_Manager.instance.PlaySound(Sound_Manager.instance.asset.buy));
                    buyButton.onClick.AddListener(buyButton.onClick.RemoveAllListeners);
                    buyButton.onClick.AddListener(itemStatsDisplay.HideAndReset);
                    buyButton.onClick.AddListener(() => buyButton.gameObject.SetActive(false));                  
                }
                else
                {
                    buyButton.onClick.AddListener(() => Debug.Log("Pas assez d'argent pour acheter " + slots[indexSlot].item + " a " + slots[indexSlot].item.itemSellPrice + " pieces."));
                }
            }

            if (!buyButton.gameObject.activeSelf)
            {
                buyButton.gameObject.SetActive(true);
            }

            

            // If we are not displaying yet
            if (!itemStatsDisplay)
            {
                // Display item stats
                if (slots[indexSlot].item as EquipmentItem)
                    itemStatsDisplay.DisplayItemStats((EquipmentItem)slots[indexSlot].item);
                else if (slots[indexSlot].item as UsableItem)
                    itemStatsDisplay.DisplayItemStats((UsableItem)slots[indexSlot].item);
                else if (slots[indexSlot].item as QuestItem)
                    itemStatsDisplay.DisplayItemStats((QuestItem)slots[indexSlot].item);
            }
            else // Else we need to reset before display. Because player want to see another item's stats
            {
                itemStatsDisplay.HideAndReset();
                if (slots[indexSlot].item as EquipmentItem)
                    itemStatsDisplay.DisplayItemStats((EquipmentItem)slots[indexSlot].item);
                else if (slots[indexSlot].item as UsableItem)
                    itemStatsDisplay.DisplayItemStats((UsableItem)slots[indexSlot].item);
                else if (slots[indexSlot].item as QuestItem)
                    itemStatsDisplay.DisplayItemStats((QuestItem)slots[indexSlot].item);
            }
        }
    }
}
