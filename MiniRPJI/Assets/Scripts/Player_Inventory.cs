using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum ArmoryPart { Helm, Armor, Pants, Gloves, Boots }; // Index of parts are same in armoryItems

public class Player_Inventory : MonoBehaviour
{
    public GameObject inventorySlotInteractionsUI;
    public GameObject armorySlotIntercationsUI;
    [SerializeField] Button inventoryRemoveButton;
    [SerializeField] Button armoryRemoveButton;
    [SerializeField] Button equipButton;
    [SerializeField] Button unequipButton;

    int currentSlotIndexSet = -1; // Important to set it -1 because inventory index starts at 0

    [SerializeField] ArmorySlot[] armoryItems;

    [SerializeField] InventorySlot[] inventoryItems;

    // Start is called before the first frame update
    void Start()
    {
        if (armorySlotIntercationsUI.activeSelf)
            armorySlotIntercationsUI.SetActive(false);

        if (inventorySlotInteractionsUI.activeSelf)
            inventorySlotInteractionsUI.SetActive(false);

    }

    // Update is called once per frame
    void Update()
    {

    }

    // Used for now only in Equip method to clear a bit her code
    void ClearInventoryCurrentSlot()
    {
        inventoryItems[currentSlotIndexSet].item = null;
        RefreshInventory();
        SetCurrentInventorySlotInteractions(currentSlotIndexSet);
    }

    void RefreshInventory()
    {
        for (int i = 0; i < inventoryItems.Length; i++)
        {
            // Refresh inventory item
            inventoryItems[i].RefreshSlot();
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
        }
    }

    // Used on "OnClick" method of every InventorySlot to set button right slot index
    public void SetCurrentInventorySlotInteractions(int indexSlot)
    {
        if (currentSlotIndexSet != indexSlot)
        {
            inventoryRemoveButton.onClick.RemoveAllListeners(); // Remove previous listeners
            inventoryRemoveButton.onClick.AddListener(() => RemoveItem(indexSlot)); //  add right method with right index

            equipButton.onClick.RemoveAllListeners();
            equipButton.onClick.AddListener(() => EquipItem(inventoryItems[indexSlot].item));

            currentSlotIndexSet = indexSlot;

            if (!inventorySlotInteractionsUI.activeSelf)
                inventorySlotInteractionsUI.SetActive(true);
        }
        else // If its equal, player clicked on same item so we want to unshow slotIntercationsUI and reset buttons
        {
            inventoryRemoveButton.onClick.RemoveAllListeners();
            equipButton.onClick.RemoveAllListeners();

            currentSlotIndexSet = -1;

            if (inventorySlotInteractionsUI.activeSelf)
                inventorySlotInteractionsUI.SetActive(false);
        }
    }

    // Used on "OnClick" method of every ArmorySlot to set right armory item
    public void SetCurrentArmorySlotInteractions(int indexPart) // Ref to ArmoryPart for know number of each part
    {
        armoryRemoveButton.onClick.RemoveAllListeners();
        unequipButton.onClick.RemoveAllListeners();

        if (armoryItems[indexPart].item != null)
        {
            armoryRemoveButton.onClick.AddListener(() => RemoveArmoryItem(armoryItems[indexPart].item));
            unequipButton.onClick.AddListener(() => UnequipItem(armoryItems[indexPart].item));
        }
        

        if (!armorySlotIntercationsUI.activeSelf)
            armorySlotIntercationsUI.SetActive(true);
        else
            armorySlotIntercationsUI.SetActive(false);
    }

    // For remove and spawn item via Armory
    public void RemoveArmoryItem(ItemConfig item)
    {
        for (int i = 0; i < armoryItems.Length; i++)
        {
            if (armoryItems[i].item.armoryPart == item.armoryPart)
            {
                Instantiate(item.itemPrefab, GameObject.FindGameObjectWithTag("Player").transform.position, Quaternion.identity);
                armoryItems[i].item = null;
                armoryItems[i].RefreshSlot();
                return;
            }
        }

        // If we dont disable gameobject here, error will happened because player still can click on "remove" and will get error
        if (armorySlotIntercationsUI.activeSelf)
            armorySlotIntercationsUI.SetActive(false);
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
                    if (!CheckInventoryIsFull()) // If inventory isn't full put current slot item in
                    {
                        PutNewItem(armoryItems[i].item);
                    }
                    else // Else remove it
                    {
                        RemoveArmoryItem(armoryItems[i].item);
                    }
                    // Then equip wanted item
                    armoryItems[i].item = item;
                    // And remove it from inventory (dont use RemoveItem(int) method because of instanciation)
                    ClearInventoryCurrentSlot();
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
                    }
                    else // Else you can only remove it.
                    {
                        Debug.Log("No more slots in inventory !!!!");
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
            }


        }
    }
}
