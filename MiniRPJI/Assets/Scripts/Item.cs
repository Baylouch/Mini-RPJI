using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider2D))] // To set as trigger
public class Item : Interactable
{
    public ItemConfig itemConfig;

    // TODO, show player item stats via UI, then on this UI ask player if he want take this
    public override void Interact()
    {
        itemConfig.CheckItemStats();
        // Put item in inventory player
        Player_Inventory player_Inventory = FindObjectOfType<Player_Inventory>(); // TODO Change this for a more secure and relative to interacted player way
        if (player_Inventory)
        {
            bool isFull = player_Inventory.CheckInventoryIsFull();
            if (isFull)
                return;

            player_Inventory.PutNewItem(itemConfig);
            Destroy(gameObject);
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                Interact();
            }
        }
    }
}
