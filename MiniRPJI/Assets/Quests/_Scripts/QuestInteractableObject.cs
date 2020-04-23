using UnityEngine;

public class QuestInteractableObject : Interactable
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
        // If player already did the first quest, Destroy(this)
        if (Player_Quest_Control.instance)
        {
            if (Player_Quest_Control.instance.GetQuestAchievement(questItem.questID))
            {
                Destroy(this);
            }
        }
    }

    public override void Interact()
    {
        base.Interact();

        // If we got QuestControl instance
        if (Player_Quest_Control.instance)
        {
            // Check if player got the quest
            if (Player_Quest_Control.instance.GetPlayerQuestByID(questItem.questID))
            {
                if (!used)
                {
                    used = true;

                    // If yes, spawn questItem prefab and delete this script.
                    GameObject questGO = Instantiate(questItem.itemPrefab, spawnPos.position, Quaternion.identity);

                    questGO.GetComponent<SpriteRenderer>().sprite = questItem.prefabImage;
                    questGO.GetComponent<Item>().itemConfig = questItem;

                    if (GameObject.Find("Items"))
                    {
                        questGO.transform.parent = GameObject.Find("Items").transform;
                    }

                    Destroy(this);
                }
            }
        }
    }
}
