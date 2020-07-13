using UnityEngine;
using UnityEngine.UI;

public class Item : Interactable
{
    public BaseItem itemConfig;

    [SerializeField] GameObject panelUI; // A UI Panel who act as a background for display the item's name. (Item's name text is a child of this panel)

    bool used = false;

    private void Start()
    {
        interactionType = PlayerInteractionType.Item;

        if (panelUI.activeSelf)
        {
            panelUI.SetActive(false);
        }
    }

    // TODO : For now, its manually here we activating the panelUI to display the item name when player trigger it.
    // we must centralise all these kind of things into Player_Interactions later.
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.GetComponent<Player_Stats>())
        {
            if (!panelUI.activeSelf)
            {
                panelUI.SetActive(true);
                panelUI.GetComponentInChildren<Text>().text = itemConfig.itemName;

                // If its an EquipmentItem, then display his name linked to this rarety color
                if (itemConfig as EquipmentItem)
                {
                    EquipmentItem currentItem = itemConfig as EquipmentItem;
                    switch (currentItem.rarety)
                    {
                        case ItemRarety.Common:
                            panelUI.GetComponentInChildren<Text>().color = Color.white;
                            break;
                        case ItemRarety.Uncommon:
                            panelUI.GetComponentInChildren<Text>().color = Color.cyan;
                            break;
                        case ItemRarety.Rare:
                            panelUI.GetComponentInChildren<Text>().color = Color.yellow;
                            break;
                        case ItemRarety.Epic:
                            panelUI.GetComponentInChildren<Text>().color = Color.magenta;
                            break;
                        case ItemRarety.Legendary:
                            panelUI.GetComponentInChildren<Text>().color = Color.red; // TODO modify
                            break;
                    }
                }
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.GetComponent<Player_Stats>())
        {
            if (panelUI.activeSelf)
            {
                panelUI.SetActive(false);
            }
        }
    }

    public override void Interact()
    {
        base.Interact();

        // We need to verify if itemConfig is set. If not, its an error.
        if (itemConfig == null)
        {
            Debug.LogError("itemConfig isnt set !");
            return;
        }

        // Put item in inventory player
        if (Player_Inventory.instance)
        {
            bool isFull = Player_Inventory.instance.CheckInventoryIsFull();

            if (isFull)
                return;

            if (!used)
            {
                used = true;
                Player_Inventory.instance.GetNewItem(itemConfig);
                Sound_Manager.instance.PlaySound(Sound_Manager.instance.asset.itemPickup);
                Destroy(gameObject);
            }
        }
    }
}
