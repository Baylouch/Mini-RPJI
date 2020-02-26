using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum ArmoryPart { Helm, Chest, Pants, Gloves, Boots, Bow }; // Index of parts are same in armoryItems
public enum ItemRarety { Common, Uncommon, Rare, Epic, Legendary }; // All items rarety

public class Player_Inventory : MonoBehaviour
{
    [SerializeField] GameObject inventoryUI;

    public GameObject inventorySlotInteractionsUI;
    public GameObject armorySlotIntercationsUI;
    [SerializeField] Button inventoryRemoveButton;
    [SerializeField] Button armoryRemoveButton;
    [SerializeField] Button equipButton;
    [SerializeField] Button unequipButton;

    // To display item's stats
    [SerializeField] GameObject statsItemPanel; // This gameobject prefab must be set 0 width and 0 height on his rectTransform 
    [SerializeField] GameObject itemName; // Automatictly scaled by Vertical Layout group on statsItemPanel
    [SerializeField] GameObject statsNameAndPoints; // Same as itemName
    // statsItemPanel will contains all others UI elements who display item's stats. statsBackgroundPanelRect is rect of current statsItemPanel display
    // so just destroy gameobject attach to statsBackgroundPanelRect to remove stats display
    RectTransform statsBackgroundPanelRect; // To set hierarchy and item's stats stuff

    int currentInventorySlotIndex = -1; // Important to set it -1 because inventory index starts at 0
    int currentArmorySlotIndex = -1; 

    [SerializeField] ArmorySlot[] armoryItems;

    [SerializeField] InventorySlot[] inventoryItems;

    Player_Stats playerStats; // MUST BE SET !! Its in parent of player

    // Start is called before the first frame update
    void Start()
    {
        if (armorySlotIntercationsUI.activeSelf)
            armorySlotIntercationsUI.SetActive(false);

        if (inventorySlotInteractionsUI.activeSelf)
            inventorySlotInteractionsUI.SetActive(false);

        if (inventoryUI.activeSelf)
            inventoryUI.SetActive(false);

        playerStats = GetComponentInParent<Player_Stats>();
    }

    // Update is called once per frame
    void Update()
    {
        // TODO Centralise later ?
        // Toggle inventoryUI
        if (Input.GetKeyDown(KeyCode.B))
        {
            if (!inventoryUI.activeSelf)
                inventoryUI.SetActive(true);
            else
            {
                // We need to reset slotIntercationsUIs too
                inventoryRemoveButton.onClick.RemoveAllListeners();
                equipButton.onClick.RemoveAllListeners();
                armoryRemoveButton.onClick.RemoveAllListeners();
                unequipButton.onClick.RemoveAllListeners();
                currentInventorySlotIndex = -1;

                if (armorySlotIntercationsUI.activeSelf)
                    armorySlotIntercationsUI.SetActive(false);

                if (inventorySlotInteractionsUI.activeSelf)
                    inventorySlotInteractionsUI.SetActive(false);

                if (statsBackgroundPanelRect)
                {
                    Destroy(statsBackgroundPanelRect.gameObject);
                }

                inventoryUI.SetActive(false);
            }              
        }
    }

    // Used for now only in Equip method to clear a bit her code
    void ClearInventoryCurrentSlot()
    {
        inventoryItems[currentInventorySlotIndex].item = null;
        RefreshInventory();
        SetCurrentInventorySlotInteractions(currentInventorySlotIndex);
    }

    void RefreshInventory()
    {
        for (int i = 0; i < inventoryItems.Length; i++)
        {
            // Refresh inventory item
            inventoryItems[i].RefreshSlot();
        }
    }

    // Method to create new Text element in DisplayItemStats(ItemConfig)
    void CreateItemStatsText(string statsName, float statsPoints)
    {
        GameObject _statsText = Instantiate(statsNameAndPoints); // Instatiate text element prefab
        // Check if statsPoints is > or < to 0 for set green or red text
        if (statsPoints > 0)
        {
            _statsText.GetComponent<Text>().text = statsName + ": " + "<color=green>" + statsPoints + "</color>"; // Set text with greend pts
        }
        else
        {
            _statsText.GetComponent<Text>().text = statsName + ": " + "<color=red>" + statsPoints + "</color>"; // Set text with red pts
        }

        _statsText.transform.SetParent(statsBackgroundPanelRect.transform); // Set hierarchy
        _statsText.GetComponent<RectTransform>().localScale = Vector3.one; // Reset scale (idk why but without scale do weird things)
        // Add 30 to the height of the panel for each new element
        statsBackgroundPanelRect.sizeDelta = new Vector2(statsBackgroundPanelRect.sizeDelta.x, statsBackgroundPanelRect.sizeDelta.y + 30); 
    }

    // Method to display item stats on UI (used on buttons configurations methods)
    void DisplayItemStats(ItemConfig item)
    {
        // First of all, instantiate statsItemPanel
        GameObject statsBackgroundPanel = Instantiate(statsItemPanel); // Create background panel
        statsBackgroundPanelRect = statsBackgroundPanel.GetComponent<RectTransform>(); // Get current rectransform
        statsBackgroundPanel.transform.SetParent(this.transform.parent); // Set background panel hierarchy
        statsBackgroundPanel.GetComponent<RectTransform>().localScale = Vector3.one; // Reset scale

        // Second add item name text and set it
        GameObject itemName = Instantiate(this.itemName); // Create item name text
        Text itemNameText = itemName.GetComponent<Text>();
        // Switch on every rarety to set correct color on item name
        switch (item.rarety) {
            case ItemRarety.Common:
                itemNameText.color = Color.white;
                break;
            case ItemRarety.Uncommon:
                itemNameText.color = Color.blue;
                break;
            case ItemRarety.Rare:
                itemNameText.color = Color.yellow;
                break;
            case ItemRarety.Epic:
                itemNameText.color = Color.magenta;
                break;
            case ItemRarety.Legendary:
                itemNameText.color = Color.cyan; // TODO modify
                break;
        }
        itemName.GetComponent<Text>().text = item.itemName; // Set item name text
        itemName.transform.SetParent(statsBackgroundPanel.transform); // Set item name text hierarchy
        itemName.GetComponent<RectTransform>().localScale = Vector3.one;
        // Add 50 to panel's height
        statsBackgroundPanelRect.sizeDelta = new Vector2(statsBackgroundPanelRect.sizeDelta.x, statsBackgroundPanelRect.sizeDelta.y + 30); 

        // Stats are tested in order we want to display them

        if (item.damageMin != 0) // We only check damageMin and rangedDamageMin. If min is set max must be set /!!!!!\ 
        {
            GameObject _damageText = Instantiate(statsNameAndPoints);                                                                 
            if (item.damageMin > 0)
            {
                _damageText.GetComponent<Text>().text = "Degats: <color=green>" + item.damageMin + "</color> - <color=green>" + item.damageMax + "</color>"; 
            }
            else
            {
                _damageText.GetComponent<Text>().text = "Degats: <color=red>" + item.damageMin + "</color> - <color=red>" + item.damageMax + "</color>";
            }
            _damageText.transform.SetParent(statsBackgroundPanelRect.transform);
            _damageText.GetComponent<RectTransform>().localScale = Vector3.one;
            statsBackgroundPanelRect.sizeDelta = new Vector2(statsBackgroundPanelRect.sizeDelta.x, statsBackgroundPanelRect.sizeDelta.y + 30);
        }
        if (item.rangedDamageMin != 0)
        {
            GameObject _rangedDamageText = Instantiate(statsNameAndPoints);
            if (item.rangedDamageMin > 0)
            {
                _rangedDamageText.GetComponent<Text>().text = "Dist. Dgt: <color=green>" + item.rangedDamageMin + "</color> - <color=green>" + item.rangedDamageMax + "</color>";
            }
            else
            {
                _rangedDamageText.GetComponent<Text>().text = "Dist. Dgt: <color=red>" + item.rangedDamageMin + "</color> - <color=red>" + item.rangedDamageMax + "</color>";
            }
            _rangedDamageText.transform.SetParent(statsBackgroundPanelRect.transform);
            _rangedDamageText.GetComponent<RectTransform>().localScale = Vector3.one;
            statsBackgroundPanelRect.sizeDelta = new Vector2(statsBackgroundPanelRect.sizeDelta.x, statsBackgroundPanelRect.sizeDelta.y + 30);
        }
        if (item.healthpoints != 0)
        {
            CreateItemStatsText("Vie", item.healthpoints);
        }
        if (item.armor != 0)
        {
            CreateItemStatsText("Armure", item.armor);
        }
        if (item.strength != 0)
        {
            CreateItemStatsText("Force", item.strength);
            
        }
        if (item.agility != 0)
        {
            CreateItemStatsText("Agilité", item.agility);
        }
        if (item.vitality != 0)
        {
            CreateItemStatsText("Vitalité", item.vitality);
        }
        if (item.intellect != 0)
        {
            CreateItemStatsText("Intellect", item.intellect);
        }
        if (item.criticalRate != 0)
        {
            GameObject _criticalRateText = Instantiate(statsNameAndPoints);
            if (item.criticalRate > 0)
            {
                _criticalRateText.GetComponent<Text>().text = "Crit: <color=green>" + item.criticalRate + "</color>%";
            }
            else
            {
                _criticalRateText.GetComponent<Text>().text = "Crit: <color=red>" + item.criticalRate + "</color>%";
            }
            _criticalRateText.transform.SetParent(statsBackgroundPanelRect.transform);
            _criticalRateText.GetComponent<RectTransform>().localScale = Vector3.one;
            statsBackgroundPanelRect.sizeDelta = new Vector2(statsBackgroundPanelRect.sizeDelta.x, statsBackgroundPanelRect.sizeDelta.y + 30);
        }
        if (item.rangedCriticalRate != 0)
        {
            GameObject _rangedCriticalRateText = Instantiate(statsNameAndPoints);
            if (item.rangedCriticalRate > 0)
            {
                _rangedCriticalRateText.GetComponent<Text>().text = "Dist. Crit: <color=green>" + item.rangedCriticalRate + "</color>%";
            }
            else
            {
                _rangedCriticalRateText.GetComponent<Text>().text = "Dist. Crit: <color=red>" + item.rangedCriticalRate + "</color>%";
            }
            _rangedCriticalRateText.transform.SetParent(statsBackgroundPanelRect.transform);
            _rangedCriticalRateText.GetComponent<RectTransform>().localScale = Vector3.one;
            statsBackgroundPanelRect.sizeDelta = new Vector2(statsBackgroundPanelRect.sizeDelta.x, statsBackgroundPanelRect.sizeDelta.y + 30);
        }
    }

    // False = slots available, true = full
    public bool CheckInventoryIsFull()
    {
        for (int i = 0; i < inventoryItems.Length; i++)
        {
            if (inventoryItems[i].item == null)
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
            //  add right method with right index
            inventoryRemoveButton.onClick.AddListener(() => RemoveItem(indexSlot));  
            equipButton.onClick.AddListener(() => EquipItem(inventoryItems[indexSlot].item));

            currentInventorySlotIndex = indexSlot;
            currentArmorySlotIndex = -1;

            if (!inventorySlotInteractionsUI.activeSelf)
                inventorySlotInteractionsUI.SetActive(true);
            if (armorySlotIntercationsUI.activeSelf)
                armorySlotIntercationsUI.SetActive(false);

            // If we are not displaying yet
            if (!statsBackgroundPanelRect)
            {
                // Display item stats
                DisplayItemStats(inventoryItems[indexSlot].item);
            }
            else // Else we need to destroy before display. Because player want to see another item's stats
            {
                Destroy(statsBackgroundPanelRect.gameObject);
                DisplayItemStats(inventoryItems[indexSlot].item);
            }

        }
        else // If its equal, player clicked on the same item so we want to unshow slotIntercationsUI and reset buttons
        {
            inventoryRemoveButton.onClick.RemoveAllListeners();
            equipButton.onClick.RemoveAllListeners();

            currentInventorySlotIndex = -1;

            if (inventorySlotInteractionsUI.activeSelf)
                inventorySlotInteractionsUI.SetActive(false);

            if (statsBackgroundPanelRect)
            {
                Destroy(statsBackgroundPanelRect.gameObject);
            }
        }
    }

    // Used on "OnClick" method of every ArmorySlot to set right armory item
    public void SetCurrentArmorySlotInteractions(int indexPart) // Ref to ArmoryPart for know number of each part
    {
        if (currentArmorySlotIndex != indexPart)
        {
            inventoryRemoveButton.onClick.RemoveAllListeners();
            equipButton.onClick.RemoveAllListeners();
            armoryRemoveButton.onClick.RemoveAllListeners();
            unequipButton.onClick.RemoveAllListeners();

            armoryRemoveButton.onClick.AddListener(() => RemoveArmoryItem(armoryItems[indexPart].item));
            unequipButton.onClick.AddListener(() => UnequipItem(armoryItems[indexPart].item));

            currentArmorySlotIndex = indexPart;

            if (inventorySlotInteractionsUI.activeSelf)
                inventorySlotInteractionsUI.SetActive(false);
            if (!armorySlotIntercationsUI.activeSelf)
                armorySlotIntercationsUI.SetActive(true);

            // If we are not displaying yet
            if (!statsBackgroundPanelRect)
            {
                // Display item stats
                DisplayItemStats(armoryItems[indexPart].item);
            }
            else // Else we need to destroy before display. Because player want to see another item's stats
            {
                Destroy(statsBackgroundPanelRect.gameObject);
                DisplayItemStats(armoryItems[indexPart].item);
            }
        }
        else // If its equal, player clicked on the same item so we want to unshow slotIntercationsUI and reset buttons
        {
            armoryRemoveButton.onClick.RemoveAllListeners();
            unequipButton.onClick.RemoveAllListeners();

            currentArmorySlotIndex = -1;

            if (armorySlotIntercationsUI.activeSelf)
                armorySlotIntercationsUI.SetActive(false);

            if (statsBackgroundPanelRect)
            {
                Destroy(statsBackgroundPanelRect.gameObject);
            }
        }            
    }

    public void PutNewItem(ItemConfig item)
    {
        // Find the first available slot to put item
        for (int i = 0; i < inventoryItems.Length; i++)
        {
            if (inventoryItems[i].item == null)
            {
                // set new item in inventory
                inventoryItems[i].item = item;
                RefreshInventory();
                return; // Get out of there
            }
        }
    }

    // To remove and spawn item by inventory index
    public void RemoveItem(int itemIndex)
    {
        if (inventoryItems[itemIndex].item != null)
        {
            Instantiate(inventoryItems[itemIndex].item.itemPrefab, GameObject.FindGameObjectWithTag("Player").transform.position, Quaternion.identity);
            inventoryItems[itemIndex].item = null;
            RefreshInventory();
            SetCurrentInventorySlotInteractions(itemIndex);
            if (statsBackgroundPanelRect)
            {
                Destroy(statsBackgroundPanelRect.gameObject);
            }
        }
    }

    // For remove and spawn item via Armory
    public void RemoveArmoryItem(ItemConfig item)
    {
        for (int i = 0; i < armoryItems.Length; i++)
        {
            if (armoryItems[i].armoryPart == item.armoryPart)
            {
                if (armoryItems[i].item != null)
                {
                    Instantiate(item.itemPrefab, GameObject.FindGameObjectWithTag("Player").transform.position, Quaternion.identity);
                    armoryItems[i].item = null;
                    armoryItems[i].RefreshSlot();
                    playerStats.RemoveItemStats(item); 
                    break;
                }
            }
        }

        // If we dont disable gameobject here, error will happened because player still can click on "remove" and will get error
        if (armorySlotIntercationsUI.activeSelf)
            armorySlotIntercationsUI.SetActive(false);

        if (statsBackgroundPanelRect)
        {
            Destroy(statsBackgroundPanelRect.gameObject);
        }
    }

    // TODO find how to set armory button UI
    public void EquipItem(ItemConfig item)
    {
        for (int i = 0; i < armoryItems.Length; i++)
        {
            if (armoryItems[i].armoryPart == item.armoryPart)
            {
                // Set ArmorySlot
                if (armoryItems[i].item != null) // If current slot isn't empty, then switch item
                {
                    // Keep a track of current item on the slot (too put it on the same place in inventory of the new object)
                    ItemConfig tempItemOnSlot = armoryItems[i].item;
                    // Then equip wanted item
                    armoryItems[i].item = item;
                    // And remove it from inventory (dont use RemoveItem(int) method because of instanciation)
                    ClearInventoryCurrentSlot();
                    // Then put tempItemOnSlot in inventory
                    PutNewItem(tempItemOnSlot);
                    // Dont forget to remove old item's stats
                    playerStats.RemoveItemStats(tempItemOnSlot);
                }
                else
                {
                    // Else slot is alreay empty so just equip
                    // TODO check required stats
                    armoryItems[i].item = item;
                    // And remove it from inventory (dont use RemoveItem(int) method because of instanciation)
                    ClearInventoryCurrentSlot();
                }
                armoryItems[i].RefreshSlot();
                playerStats.ApplyItemStats(item);
                return;
            }
        }
    }

    public void UnequipItem(ItemConfig item)
    {
        for (int i = 0; i < armoryItems.Length; i++)
        {
            if (armoryItems[i].armoryPart == item.armoryPart)
            {
                if (armoryItems[i].item != null) // If slot isn't empty, then continue
                {
                    if (!CheckInventoryIsFull()) // If inventory isn't full put current helmslot item in
                    {
                        PutNewItem(armoryItems[i].item);
                        armoryItems[i].item = null;
                        playerStats.RemoveItemStats(item);
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
                armoryItems[i].RefreshSlot();

                // If we dont disable gameobject here, error will happened because player still can click on "remove" and will get error
                if (armorySlotIntercationsUI.activeSelf)
                    armorySlotIntercationsUI.SetActive(false);
                if (statsBackgroundPanelRect)
                {
                    Destroy(statsBackgroundPanelRect.gameObject);
                }
            }
        }
    }
}
