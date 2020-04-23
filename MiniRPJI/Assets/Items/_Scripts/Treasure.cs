using UnityEngine;

[RequireComponent(typeof(ItemDroper))]
[RequireComponent(typeof(SpriteRenderer))]
public class Treasure : Interactable
{
    [Tooltip("Set it to 0 if you want a relative to player level drop")]
    [SerializeField] int itemLevel = 0;

    [SerializeField] Sprite chestOpenSprite;

    ItemDroper droper;
    SpriteRenderer rend;

    // Start is called before the first frame update
    void Start()
    {
        interactionType = PlayerInteractionType.None;

        droper = GetComponent<ItemDroper>();
        rend = GetComponent<SpriteRenderer>();
    }

    public override void Interact()
    {
        base.Interact();

        // First get player level
        if (itemLevel <= 0)
        {
            itemLevel = Player_Stats.instance.GetCurrentLevel();
        }

        // Drop items
        droper.DropItems(itemLevel);

        // Change chest sprite
        rend.sprite = chestOpenSprite;

        // Clean useless things.
        Destroy(this);
        Destroy(droper);
        Destroy(transform.GetChild(0).gameObject);
    }
}
