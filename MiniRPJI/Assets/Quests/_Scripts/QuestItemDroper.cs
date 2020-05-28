/* QuestItemDroper.cs :
 * Permet a un enemy de drop un objet de quête.
 * A attaché sur un enemy susceptible de drop un objet de quête défini.
 * Le script déclenchera "DropQuestItem" au moment de la mort de l'ennemi par défaut. (Peut être modifié au besoin en déplaçant l'exécution de la méthode).
 * La méthode "DropQuestItem" testera si le joueur possède bien la quête lié a l'objet, sans quoi l'objet ne dropera pas.
 * 
 * */
using UnityEngine;

public class QuestItemDroper : MonoBehaviour
{
    [Header("Drop zone")]
    [SerializeField] Transform[] dropZone = new Transform[2]; // 2 Transform child of this make a zone, then we use Random.Range between theirs 2 position to get randomly spawn position

    [Header("Quest items")]
    [SerializeField] QuestItem[] items;

    [Header("Drop rate")]
    [SerializeField] float dropRate = 10f;

    public void DropQuestItems()
    {
        // We have the possibility of set multiple items. Maybe each are not for the same quest.
        // So we need to test each object to know if player got the linked quest.
        for (int i = 0; i < items.Length; i++)
        {
            // Check if there is a quest instance for the player.
            if (Quests_Control.instance)
            {
                // Now check if player got the linked quest
                if (Quests_Control.instance.GetPlayerQuestByID(items[i].questID))
                {
                    // Then we can try to drop this quest item
                    bool questItemWillDrop = dropRate > Random.Range(0, 101);
                    if (questItemWillDrop)
                    {
                        // Determine drop position
                        Vector3 dropPose = new Vector3(Random.Range(dropZone[0].position.x, dropZone[1].position.x),
                                                       Random.Range(dropZone[0].position.y, dropZone[1].position.y), 0f);

                        // Set the item
                        GameObject droppedGO = Instantiate(items[i].itemPrefab, dropPose, Quaternion.identity);

                        droppedGO.GetComponent<SpriteRenderer>().sprite = items[i].prefabImage;
                        droppedGO.GetComponent<Item>().itemConfig = items[i];

                        if (GameObject.Find("Items"))
                        {
                            droppedGO.transform.parent = GameObject.Find("Items").transform;
                        }
                    }
                }
            }
        }
    }
}
