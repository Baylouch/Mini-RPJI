using UnityEngine;

public class QuestGiver : Interactable
{
    [SerializeField] private QuestConfig questToGive;

    [SerializeField] private bool alreadyGive = false;
    [SerializeField] private bool rewardGift = false;

    [SerializeField] private GameObject reward; // Reward you can give to player when quest is accomplished.
    // It'll be a GameObject spawn on the rewardPos position
    [SerializeField] private Transform rewardPosition;

    private void Start()
    {
        // Check if player already got the quest at start. Because of saving data for quest control.
        if (QuestControl.quest_instance)
        {
            // Check if there is a quest at the quest index log
            if (QuestControl.quest_instance.GetQuest(questToGive.questIndexLog)) 
            {
                // if this quest is the same than questToGive, we already gave this quest.
                if (QuestControl.quest_instance.GetQuest(questToGive.questIndexLog) == questToGive)
                {
                    alreadyGive = true;
                }
            }
        }
    }

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
        if (rewardGift)
        {
            Debug.Log("Thanks a lot for your help !");
            return;
        }

        // Give the quest
        if (!alreadyGive)
        {
            alreadyGive = true;
            QuestControl.quest_instance.GetNewQuest(questToGive);
            return;
        }
        // Check if quest is accomplished
        if (questToGive.accomplished)
        {
            Debug.Log("Great you accomplished the quest " + questToGive.questTitle + " !");
            if (!rewardGift)
            {
                Debug.Log("Take this reward.");
                SpawnReward();
                rewardGift = true;
            }
        }
        else
        {
            Debug.Log("Come back when you finished the quest " + questToGive.questTitle + ".");
        }
    }

    public void SpawnReward()
    {
        GameObject inst_Reward = Instantiate(reward, rewardPosition.position, Quaternion.identity);
    }

}
