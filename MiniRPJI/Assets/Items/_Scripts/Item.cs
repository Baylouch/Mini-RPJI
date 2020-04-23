using UnityEngine;

public class Item : Interactable
{
    public BaseItem itemConfig;

    bool used = false;

    private void Start()
    {
        interactionType = PlayerInteractionType.Item;
    }

    public override void Interact()
    {
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
