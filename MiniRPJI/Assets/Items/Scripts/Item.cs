using UnityEngine;

public class Item : Interactable
{
    public BaseItem itemConfig;

    // TODO, show player item stats via UI, then on this UI ask player if he want take this
    public override void Interact()
    {
        // Put item in inventory player
        if (Player_Inventory.inventory_instance)
        {
            bool isFull = Player_Inventory.inventory_instance.CheckInventoryIsFull();
            if (isFull)
                return;

            Player_Inventory.inventory_instance.GetNewItem(itemConfig);
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
