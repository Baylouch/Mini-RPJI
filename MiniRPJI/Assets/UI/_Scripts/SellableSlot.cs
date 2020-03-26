using UnityEngine;
using UnityEngine.UI;

public class SellableSlot : MonoBehaviour
{
    public BaseItem item;

    public void RefreshSlot()
    {
        if (item != null)
        {
            // Set image slot
            GetComponent<Image>().sprite = item.inventoryImage;
            gameObject.SetActive(true);
        }
        else
        {
            gameObject.SetActive(false);
        }
    }
}
