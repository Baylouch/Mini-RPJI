﻿using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class ArmorySlot : MonoBehaviour
{
    public EquipmentItem item;
    public ArmoryPart armoryPart;

    [SerializeField] Image imageSlot;
    [SerializeField] Color emptySlotColor;
    [SerializeField] Sprite emptySlotSprite; // The sprite when no item is on the slot

    public void RefreshSlot()
    {
        if (item != null)
        {
            // Set image slot
            imageSlot.sprite = item.inventoryImage;
            imageSlot.color = new Color(1, 1, 1, 1);
            // Set button as interactable now
            GetComponent<Button>().interactable = true;
        }
        else
        {
            imageSlot.sprite = null;
            imageSlot.color = emptySlotColor;
            imageSlot.sprite = emptySlotSprite;
            GetComponent<Button>().interactable = false;
        }
    }
}
