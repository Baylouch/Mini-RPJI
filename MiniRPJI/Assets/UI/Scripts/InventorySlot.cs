/* InventorySlot.cs
Used on UI Element
*/

using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
[RequireComponent(typeof(Image))]
public class InventorySlot : MonoBehaviour
{
    public BaseItem item;

    public GameObject stackableItemTextUI;
    public Text stackableNumbText;

    public int itemNumb; // Only use if its a stackable item. Increment in Player_Inventory. Decrement in UI_Player_Inventory

    public void RefreshSlot()
    {
        if (item != null)
        {
            // Set image slot
            GetComponent<Image>().sprite = item.inventoryImage;
            GetComponent<Image>().color = new Color(1, 1, 1, 1);
            // Active stackable numb text if its a stackableItem
            if (item.stackableItem)
            {
                if (itemNumb > 1)
                {
                    stackableNumbText.text = itemNumb.ToString();

                    if (!stackableItemTextUI.activeSelf)
                    {
                        stackableItemTextUI.SetActive(true);
                    }
                }
                else
                {
                    if (stackableItemTextUI.activeSelf)
                    {
                        stackableItemTextUI.SetActive(false);
                    }
                }
            }
            else
            {
                if (stackableItemTextUI.activeSelf)
                {
                    stackableItemTextUI.SetActive(false);
                }
            }

            // Set button as interactable now
            GetComponent<Button>().interactable = true;
        }
        else
        {
            GetComponent<Image>().sprite = null;
            GetComponent<Image>().color = new Color(1, 1, 1, 0);

            itemNumb = 0;

            if (stackableItemTextUI.activeSelf)
            {
                stackableItemTextUI.SetActive(false);
            }

            GetComponent<Button>().interactable = false;
        }
    }
}
