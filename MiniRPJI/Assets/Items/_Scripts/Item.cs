public class Item : Interactable
{
    public BaseItem itemConfig;

    bool used = false;

    private void Start()
    {
        interactionType = PlayerInteractionType.Item;
    }

    // TODO, show player item stats via UI, then on this UI ask player if he want take this
    public override void Interact()
    {
        // Put item in inventory player
        if (Player_Inventory.inventory_instance)
        {
            bool isFull = Player_Inventory.inventory_instance.CheckInventoryIsFull();
            if (isFull)
                return;

            if (!used)
            {
                used = true;
                Player_Inventory.inventory_instance.GetNewItem(itemConfig);
                Destroy(gameObject);
            }
        }
    }
}
