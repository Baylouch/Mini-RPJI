using UnityEngine;

public class QuestGiver : Interactable
{
    [Tooltip("If you put more than one quest in, quests will be get in ordrer its set.")]
    [SerializeField] private QuestConfig[] questsToGive; // Set it for quest suit. Quests[0] = first quest, Quests[1] = second quest, etc...

    [SerializeField] private Transform rewardPosition;

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                Interact();
            }
        }
    }

    public override void Interact()
    {
        for (int i = 0; i < questsToGive.Length; i++)
        {
            // Check if quest is already done
            if (questsToGive[i].questDone)
            {
                continue; // Move to the next quest.
            }

            // Check if the quest is accomplished
            if (questsToGive[i].accomplished)
            {
                Debug.Log("Thanks a lot for your help !");

                Player_Quest_Control.quest_instance.ValideQuest(questsToGive[i].questIndexLog, rewardPosition.position);
                return;
            }
            else // Else quest isnt accomplished yet
            {
                if (questsToGive[i].playerIsOnThisQuest) // Check if player already got the quest
                {
                    Debug.Log("Come back when you finished the quest " + questsToGive[i].questTitle + ".");
                }
                else // Else give the quest to the player
                {
                    questsToGive[i].playerIsOnThisQuest = true;
                    Player_Quest_Control.quest_instance.GetNewQuest(questsToGive[i]);
                }
                return;
            }                            
        }       
    }

    public void SpawnReward(GameObject _reward)
    {
        GameObject inst_Reward = Instantiate(_reward, rewardPosition.position, Quaternion.identity);
    }

}
