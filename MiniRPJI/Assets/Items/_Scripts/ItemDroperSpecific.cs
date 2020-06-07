/* ItemDroperSpecific.cs
 * 
 * Permet de choisir directement quel objets doit être drop par un monstre plutôt que de passer par une database comme ItemDroper.cs.
 * 
 * 
 * 
 * */

using UnityEngine;

public class ItemDroperSpecific : MonoBehaviour
{
    [SerializeField] Transform dropZone; // 2 Transform child of this make a zone, then we use Random.Range between theirs 2 position to get randomly spawn position

    [SerializeField] BaseItem[] itemsDropable;

    [SerializeField] float[] dropRate; // Drop rate is for now set in same index of the item in itemsDropable. dropRate[] need to have same index numbers of itemsDropable[]
    // TODO Think about Editor modification here to get -> item dropable : % rate drop

    [SerializeField] int maxItemDropable = 2;
    int itemsDropped = 0;

    public void DropItems()
    {
        Transform parentGameObject = null;

        if (GameObject.Find("Items"))
        {
            parentGameObject = GameObject.Find("Items").transform;
        }

        for (int i = 0; i < itemsDropable.Length; i++)
        {
            bool willDrop = dropRate[i] > Random.Range(0f, 100f); // If current dropRate is greater than random number between 0 / 100
            if (willDrop)
            {
                // Determine drop position
                Vector3 dropPose = new Vector3(Random.Range(dropZone.GetChild(0).position.x, dropZone.GetChild(1).position.x),
                                               Random.Range(dropZone.GetChild(0).position.y, dropZone.GetChild(1).position.y), 0f);


                // Just a check if its a quest item. If yes, just be sure player got the quest before instantiate it.
                if (itemsDropable[i] as QuestItem)
                {
                    if (Quests_Control.instance)
                    {
                        QuestItem questItem = (QuestItem)itemsDropable[i];
                        if (Quests_Control.instance.GetPlayerQuestByID(questItem.questID))
                        {
                            GameObject droppedGO = Instantiate(itemsDropable[i].itemPrefab, dropPose, Quaternion.identity);
                            Item droppedItem = droppedGO.GetComponent<Item>();
                            droppedItem.itemConfig = itemsDropable[i];


                            if (parentGameObject)
                                droppedGO.transform.parent = parentGameObject;

                            itemsDropped++;
                        }
                        else
                        {
                            // Do nothing
                        }
                    }
                }
                else
                {
                    GameObject droppedGO = Instantiate(itemsDropable[i].itemPrefab, dropPose, Quaternion.identity);
                    Item droppedItem = droppedGO.GetComponent<Item>();
                    droppedItem.itemConfig = itemsDropable[i];

                    if (parentGameObject)
                        droppedGO.transform.parent = parentGameObject;

                    itemsDropped++;
                }

            }

            if (itemsDropped >= maxItemDropable)
            {
                itemsDropped = 0; // useless because ItemDroper generally die when drop items.
                return;
            }
        }

        itemsDropped = 0;
    }
}
