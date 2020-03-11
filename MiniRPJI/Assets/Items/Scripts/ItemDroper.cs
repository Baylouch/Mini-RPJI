/* ItemDroper.cs
 * Va gérer les possible items dropable et le % drop par l'ennemie à qui le script est attaché. 
 * 
 * 
 * */
using UnityEngine;

public class ItemDroper : MonoBehaviour
{
    [SerializeField] Transform dropZone; // 2 Transform child of this make a zone, then we use Random.Range between their 2 position to get randomly spawn position

    [SerializeField] BaseItem[] itemsDropable;

    [SerializeField] float[] dropRate; // Drop rate is for now set in same index of the item in itemsDropable. dropRate[] need to have same index numbers of itemsDropable[]
    // TODO Think about Editor modification here to get -> item dropable : % rate drop

    [SerializeField]int maxItemDropable = 2;
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
            bool willDrop = dropRate[i] > Random.Range(0, 100); // If current dropRate is greater than random number between 0 / 100
            if (willDrop)
            {
                Vector3 dropPose = new Vector3(Random.Range(dropZone.GetChild(0).position.x, dropZone.GetChild(1).position.x), 
                                               Random.Range(dropZone.GetChild(0).position.y, dropZone.GetChild(1).position.y), 0f);

                GameObject droppedItem = Instantiate(itemsDropable[i].itemPrefab, dropPose, Quaternion.identity);
                if (parentGameObject)
                    droppedItem.transform.parent = parentGameObject;

                itemsDropped++;
            }

            if (itemsDropped >= maxItemDropable)
            {
                itemsDropped = 0;
                return;
            }
        }

        itemsDropped = 0; // not a must because items drops often when enemy die. will see if there is an evolution
    }
}
