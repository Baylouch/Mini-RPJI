/* QuestItemSpawner.cs :
 * 
 * A le même fonctionnement que QuestItemDroper mais plutot que d'être attaché à un monstre,
 * il faut attacher ce script a un objet pour que le joueur puisse intéragir avec et obtenir l'objet de quête.
 * 
 * 
 * */

using UnityEngine;

public class QuestItemSpawner : Interactable
{
    [SerializeField] QuestItem questItem;
    [SerializeField] Transform spawnPos;

    bool used = false;

    private void Start()
    {
        interactionType = PlayerInteractionType.None;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            CheckIfPlayerDidTheQuest();
        }
    }

    private void CheckIfPlayerDidTheQuest()
    {
        // If player already did the quest, Destroy(this)
        if (Quests_Control.instance)
        {
            if (Quests_Control.instance.GetQuestAchievement(questItem.questID))
            {
                Destroy(this);
            }
        }
    }

    public override void Interact()
    {
        // If we got QuestControl instance
        if (Quests_Control.instance)
        {
            // Check if player got the quest
            if (Quests_Control.instance.GetPlayerQuestByID(questItem.questID))
            {
                if (!used)
                {
                    base.Interact();

                    used = true;

                    // If yes, spawn questItem prefab and delete this script.
                    GameObject questGO = Instantiate(questItem.itemPrefab, spawnPos.position, Quaternion.identity);

                    questGO.GetComponent<SpriteRenderer>().sprite = questItem.prefabImage;
                    questGO.GetComponent<Item>().itemConfig = questItem;

                    if (GameObject.Find("Items"))
                    {
                        questGO.transform.parent = GameObject.Find("Items").transform;
                    }

                    if (Sound_Manager.instance)
                        Sound_Manager.instance.PlaySound(Sound_Manager.instance.asset.questInteraction);

                    Destroy(this);
                }
            }
        }
    }
}
