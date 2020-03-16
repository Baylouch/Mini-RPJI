/* QuestObjectiveTarget.cs
 * A attaché sur chaque ennemis objectif de quêtes en précisant l'id de la quete liée.
 * a chaque fois que ce script est détruit (que l'ennemi meurt) l'objectif de la quête lié est incrementé
 */
using UnityEngine;

public class QuestObjectiveTarget : MonoBehaviour
{
    public int questID = 0; // To know the quest ID (not index) of the script acces to.

    // We use OnDestroy method when quest link to this script is set with "KillObjective" to increment quest's objective.
    private void OnDestroy()
    {
        if (Player_Quest_Control.quest_instance) // If there is a quest control in game
        {
            // Check if player got the quest linked
            if (Player_Quest_Control.quest_instance.GetQuestWithID(questID))
            {
                // Check if quest isnt accomplished yet
                if (!Player_Quest_Control.quest_instance.GetQuestWithID(questID).accomplished)
                {
                    if (Player_Quest_Control.quest_instance.GetQuestWithID(questID).currentQuestObjective < Player_Quest_Control.quest_instance.GetQuestWithID(questID).questObjective)
                    {
                        // Then we increment current quest objective
                        Player_Quest_Control.quest_instance.GetQuestWithID(questID).currentQuestObjective++;

                        if (Player_Quest_Control.quest_instance.GetQuestWithID(questID).currentQuestObjective >= Player_Quest_Control.quest_instance.GetQuestWithID(questID).questObjective)
                        {
                            Player_Quest_Control.quest_instance.GetQuestWithID(questID).accomplished = true;
                        }
                        // Refresh Quest UI if displayed
                        if (UI_Player.ui_instance.playerQuestUI && UI_Player.ui_instance.playerQuestUI.gameObject.activeSelf)
                            UI_Player.ui_instance.playerQuestUI.DisplayQuest(Player_Quest_Control.quest_instance.GetQuestWithID(questID).questIndexLog);
                    }                    
                }
            }
        }
    }
}
