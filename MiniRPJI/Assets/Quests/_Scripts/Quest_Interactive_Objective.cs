/* Quest_Interactive_Objective.cs
 * 
 * A attacher sur un objet pour que le joueur puisse interagir avec et incrementer l'objectif de la quête.
 * 
 * 
 * */

using UnityEngine;

public class Quest_Interactive_Objective : Interactable
{
    public int questID = 0; // To know the quest ID of the script acces to.

    // Start is called before the first frame update
    void Start()
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
            if (Quests_Control.instance.GetQuestAchievement(questID))
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
            if (Quests_Control.instance.GetPlayerQuestByID(questID))
            {
                base.Interact();

                Quests_Control.instance.GetPlayerQuestByID(questID).currentQuestObjective++;

                if (Sound_Manager.instance)
                    Sound_Manager.instance.PlaySound(Sound_Manager.instance.asset.questInteraction);

                Destroy(gameObject);
                }
            }
        }
    }
