using UnityEngine;

[RequireComponent(typeof(Collider2D))] // To set as trigger
public class Item : Interactable
{
    public BaseItem itemConfig;

    // TODO, show player item stats via UI, then on this UI ask player if he want take this
    public override void Interact()
    {
        // Put item in inventory player
        Player_Inventory player_Inventory = Player_Inventory.inventory_instance;
        if (player_Inventory)
        {
            bool isFull = player_Inventory.CheckInventoryIsFull();
            if (isFull)
                return;

            player_Inventory.GetNewItem(itemConfig);
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
