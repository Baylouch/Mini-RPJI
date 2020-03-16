using UnityEngine;

public class DeadTreeQuest : Interactable
{
    [SerializeField] QuestItem questItem;
    [SerializeField] Transform spawnPos;

    private void OnTriggerStay2D(Collider2D collision)
    {
        // First check if collision is the player
        if (collision.gameObject.tag == "Player")
        {
            // Then if player input e
            if (Input.GetKeyDown(KeyCode.E))
            {
                // If we got QuestControl instance
                if (Player_Quest_Control.quest_instance)
                {
                    // Check if player got the quest
                    if (Player_Quest_Control.quest_instance.GetQuestWithID(questItem.questID))
                    {
                        // If yes, spawn questItem prefab and delete this script.
                        GameObject inst_questItem = Instantiate(questItem.itemPrefab, spawnPos.position, Quaternion.identity);
                        Destroy(this);
                    }
                }
            }
        }
    }
}
