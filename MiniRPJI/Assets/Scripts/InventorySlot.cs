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

    public void RefreshSlot()
    {
        if (item != null)
        {
            // Set image slot
            GetComponent<Image>().sprite = item.inventoryImage;
            GetComponent<Image>().color = new Color(1, 1, 1, 1);
            // Set button as interactable now
            GetComponent<Button>().interactable = true;
        }
        else
        {
            GetComponent<Image>().sprite = null;
            GetComponent<Image>().color = new Color(1, 1, 1, 0);
            GetComponent<Button>().interactable = false;
        }
    }
}
