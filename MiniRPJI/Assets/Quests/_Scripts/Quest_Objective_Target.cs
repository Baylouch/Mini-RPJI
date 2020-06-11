/* QuestObjectiveTarget.cs
 * A attaché sur chaque ennemis objectif de quêtes en précisant l'id de la quete liée.
 * a chaque fois que l'ennemi meurt l'objectif de la quête lié est incrementé
 */
using UnityEngine;

public class Quest_Objective_Target : MonoBehaviour
{
    public int questID = 0; // To know the quest ID of the script acces to.

    [SerializeField] int levelRequiredToApplyQuest = 0;

    public void IncrementQuestObjective()
    {
        // A check to know if this AI got the level required to apply the quest
        if (GetComponent<AI_Stats>())
        {
            if (levelRequiredToApplyQuest > 0)
            {
                if (GetComponent<AI_Stats>().GetLevel() < levelRequiredToApplyQuest)
                {
                    return;
                }
            }
        }

        if (Quests_Control.instance) // If there is a quest control in game
        {
            // Check if player got the quest linked
            if (Quests_Control.instance.GetPlayerQuestByID(questID))
            {
                // Check if quest isnt accomplished yet
                if (!Quests_Control.instance.GetPlayerQuestByID(questID).IsQuestAccomplished())
                {
                    // Then we increment current quest objective
                    Quests_Control.instance.GetPlayerQuestByID(questID).currentQuestObjective++;

                    // Refresh Quest UI if displayed
                    if (UI_Player.instance.playerQuestUI && UI_Player.instance.playerQuestUI.gameObject.activeSelf)
                        UI_Player.instance.playerQuestUI.DisplayQuest(Quests_Control.instance.GetPlayerQuestByID(questID).questConfig);

                }
            }
        }
    }
}
