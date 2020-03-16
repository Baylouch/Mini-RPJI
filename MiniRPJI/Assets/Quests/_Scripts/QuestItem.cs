using UnityEngine;

[CreateAssetMenu(fileName = "QuestItemName", menuName = "ScriptableObjects/Items/QuestItem", order = 1)]
public class QuestItem : BaseItem
{
    public int questID;

    // Method used into Player_Inventory in GetNewItem(BaseItem)
    public void IncrementLinkedQuest()
    {
        if (Player_Quest_Control.quest_instance && Player_Quest_Control.quest_instance.GetQuestWithID(questID))
        {
            // Security if player already got max objective item
            if (Player_Quest_Control.quest_instance.GetQuestWithID(questID).currentQuestObjective <
                Player_Quest_Control.quest_instance.GetQuestWithID(questID).questObjective)
            {
                // If we're here, we got the quest linked to the item so increment quest's objective.
                Player_Quest_Control.quest_instance.GetQuestWithID(questID).currentQuestObjective++;
                // Check if we accomplished quest objective
                if (Player_Quest_Control.quest_instance.GetQuestWithID(questID).currentQuestObjective >=
                    Player_Quest_Control.quest_instance.GetQuestWithID(questID).questObjective)
                {
                    Player_Quest_Control.quest_instance.GetQuestWithID(questID).accomplished = true;
                }

                if (UI_Player.ui_instance.playerQuestUI && UI_Player.ui_instance.playerQuestUI.enabled)
                    UI_Player.ui_instance.playerQuestUI.DisplayQuest(Player_Quest_Control.quest_instance.GetQuestWithID(questID).questIndexLog);
            }
        }
    }

    // Method used into UI_Player_Inventory if removing this item
    public void DecrementLinkedQuest()
    {
        if (Player_Quest_Control.quest_instance && Player_Quest_Control.quest_instance.GetQuestWithID(questID))
        {
            if (Player_Quest_Control.quest_instance.GetQuestWithID(questID).currentQuestObjective > 0)
            {
                Player_Quest_Control.quest_instance.GetQuestWithID(questID).currentQuestObjective--;
                // If quest was accomplished, its no more then
                if (Player_Quest_Control.quest_instance.GetQuestWithID(questID).accomplished)
                {
                    Player_Quest_Control.quest_instance.GetQuestWithID(questID).accomplished = false;
                }

                if (UI_Player.ui_instance.playerQuestUI && UI_Player.ui_instance.playerQuestUI.enabled)
                    UI_Player.ui_instance.playerQuestUI.DisplayQuest(Player_Quest_Control.quest_instance.GetQuestWithID(questID).questIndexLog);
            }
        }
    }
}
