/* ItemDroper.cs
 * Va gérer les possible items dropable et le % drop par l'ennemie à qui le script est attaché. 
 * 
 * 
 * */
using UnityEngine;

public class ItemDroper : MonoBehaviour
{
    [Header("Drop zone")]
    [SerializeField] Transform[] dropZone = new Transform[2]; // 2 Transform child of this make a zone, then we use Random.Range between theirs 2 position to get randomly spawn position

    [Header("Data bases")]
    [SerializeField] ItemDataBase commonItems;
    [SerializeField] ItemDataBase uncommonItems;
    [SerializeField] ItemDataBase rareItems;
    [SerializeField] ItemDataBase epicItems;
    // TODO Add lengedary one ?

    [Header("Drop rate")]
    [SerializeField] float commonItemDropRate;
    [SerializeField] float uncommonItemDropRate;
    [SerializeField] float rareItemDropRate;
    [SerializeField] float epicItemDropRate;

    [Header("Up to how many objects")] // Because for some ennemies we want to be able to drop 2 common items, 2 rare items etc...
    [SerializeField] int commonItemTryDropNumb;
    [SerializeField] int uncommonItemTryDropNumb;
    [SerializeField] int rareItemTryDropNumb;
    [SerializeField] int epicItemTryDropNumb;

    [Header("Max items droppable")]
    [SerializeField]int maxItemDropable = 2;

    [Header("Level difference between npc and objects")]
    [SerializeField] int levelRange = 1; // The range between our level and item's level who could drop (AI_Stats.level - levelRange && AI_Stats.level + levelRange)

    int itemsDropped = 0;

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.black;

        // Calculation line by line to draw a cube who show spawn zone limits
        Vector3 TopRightCorner = new Vector3(dropZone[1].position.x, dropZone[0].position.y, 0f);
        Vector3 BottomLeftCorner = new Vector3(dropZone[0].position.x, dropZone[1].position.y, 0f);

        Vector3 BottomRightCorner = dropZone[1].position;
        Vector3 TopLeftCorner = dropZone[0].position;

        Gizmos.DrawLine(TopLeftCorner, TopRightCorner);
        Gizmos.DrawLine(TopLeftCorner, BottomLeftCorner);
        Gizmos.DrawLine(TopRightCorner, BottomRightCorner);
        Gizmos.DrawLine(BottomLeftCorner, BottomRightCorner);

    }

    public void DropItems(int itemLevel)
    {
        // Get the parent of all items gameobjects
        Transform parentGameObject = null;

        if (GameObject.Find("Items"))
        {
            parentGameObject = GameObject.Find("Items").transform;
        }

        // TODO Make legendary ones.

        // Epic items
        for (int i = 0; i < epicItemTryDropNumb; i++)
        {
            bool willEpicDrop = epicItemDropRate > Random.Range(0, 101);

            if (willEpicDrop)
            {
                // Determine drop position
                Vector3 dropPose = new Vector3(Random.Range(dropZone[0].position.x, dropZone[1].position.x),
                                               Random.Range(dropZone[0].position.y, dropZone[1].position.y), 0f);

                // Get the item to drop
                BaseItem epicItemToDrop = epicItems.GetRandomItemByLevel(itemLevel - levelRange, itemLevel + levelRange);
                if (epicItemToDrop == null) // There is no item of this rarety for this level.
                {
                    break;
                }

                // Set the item
                GameObject droppedGO = Instantiate(epicItemToDrop.itemPrefab, dropPose, Quaternion.identity);

                droppedGO.GetComponent<SpriteRenderer>().sprite = epicItemToDrop.prefabImage;
                droppedGO.GetComponent<Item>().itemConfig = epicItemToDrop;

                // Parent it to clean up hierarchy
                if (parentGameObject)
                    droppedGO.transform.parent = parentGameObject;

                // Increment itemsDropped and check if we dropped the max amount of items.
                itemsDropped++;
                if (itemsDropped >= maxItemDropable)
                {
                    return;
                }
            }     
        }

        // Rare items
        for (int i = 0; i < rareItemTryDropNumb; i++)
        {
            bool willRareDrop = rareItemDropRate >= Random.Range(0, 101);

            if (willRareDrop)
            {
                // Determine drop position
                Vector3 dropPose = new Vector3(Random.Range(dropZone[0].position.x, dropZone[1].position.x),
                                               Random.Range(dropZone[0].position.y, dropZone[1].position.y), 0f);

                // Get the item to drop
                BaseItem rareItemToDrop = rareItems.GetRandomItemByLevel(itemLevel - levelRange, itemLevel + levelRange);
                if (rareItemToDrop == null) // There is no item of this rarety for this level.
                {
                    break;
                }

                // Set the item
                GameObject droppedGO = Instantiate(rareItemToDrop.itemPrefab, dropPose, Quaternion.identity);

                droppedGO.GetComponent<SpriteRenderer>().sprite = rareItemToDrop.prefabImage;
                droppedGO.GetComponent<Item>().itemConfig = rareItemToDrop;

                // Parent it to clean up hierarchy
                if (parentGameObject)
                    droppedGO.transform.parent = parentGameObject;

                // Increment itemsDropped and check if we dropped the max amount of items.
                itemsDropped++;
                if (itemsDropped >= maxItemDropable)
                {
                    return;
                }
            }
        }

        // Uncommon items
        for (int i = 0; i < uncommonItemTryDropNumb; i++)
        {
            bool willUncommonDrop = uncommonItemDropRate > Random.Range(0, 101);

            if (willUncommonDrop)
            {
                // Determine drop position
                Vector3 dropPose = new Vector3(Random.Range(dropZone[0].position.x, dropZone[1].position.x),
                                               Random.Range(dropZone[0].position.y, dropZone[1].position.y), 0f);

                // Get the item to drop
                BaseItem uncommonItemToDrop = uncommonItems.GetRandomItemByLevel(itemLevel - levelRange, itemLevel + levelRange);
                if (uncommonItemToDrop == null) // There is no item of this rarety for this level.
                {
                    break;
                }

                // Set the item
                GameObject droppedGO = Instantiate(uncommonItemToDrop.itemPrefab, dropPose, Quaternion.identity);

                droppedGO.GetComponent<SpriteRenderer>().sprite = uncommonItemToDrop.prefabImage;
                droppedGO.GetComponent<Item>().itemConfig = uncommonItemToDrop;

                // Parent it to clean up hierarchy
                if (parentGameObject)
                    droppedGO.transform.parent = parentGameObject;

                // Increment itemsDropped and check if we dropped the max amount of items.
                itemsDropped++;
                if (itemsDropped >= maxItemDropable)
                {
                    return;
                }
            }
        }

        // Common items
        for (int i = 0; i < commonItemTryDropNumb; i++)
        {
            bool willCommonItemDrop = commonItemDropRate > Random.Range(0, 101);

            if (willCommonItemDrop)
            {
                // Determine drop position
                Vector3 dropPose = new Vector3(Random.Range(dropZone[0].position.x, dropZone[1].position.x),
                                               Random.Range(dropZone[0].position.y, dropZone[1].position.y), 0f);

                // Get the item to drop
                BaseItem commonItemToDrop = commonItems.GetRandomItemByLevel(itemLevel - levelRange, itemLevel + levelRange);
                if (commonItemToDrop == null) // There is no item of this rarety for this level.
                {
                    break;
                }

                // Set the item
                GameObject droppedGO = Instantiate(commonItemToDrop.itemPrefab, dropPose, Quaternion.identity);

                droppedGO.GetComponent<SpriteRenderer>().sprite = commonItemToDrop.prefabImage;
                droppedGO.GetComponent<Item>().itemConfig = commonItemToDrop;

                // Parent it to clean up hierarchy
                if (parentGameObject)
                    droppedGO.transform.parent = parentGameObject;

                // Increment itemsDropped and check if we dropped the max amount of items.
                itemsDropped++;
                if (itemsDropped >= maxItemDropable)
                {
                    return;
                }
            }
        }
    }
}
