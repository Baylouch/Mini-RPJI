using UnityEngine;

[CreateAssetMenu(fileName = "QuestItemName", menuName = "ScriptableObjects/Items/QuestItem", order = 1)]
public class QuestItem : BaseItem
{
    public int questID;

    // Method used into Player_Inventory in GetNewItem(BaseItem)
    public void IncrementLinkedQuest()
    {
        if (Player_Quest_Control.quest_instance && Player_Quest_Control.quest_instance.GetPlayerQuestByID(questID))
        {
            // Security if player already got max objective item
            if (!Player_Quest_Control.quest_instance.GetPlayerQuestByID(questID).IsQuestAccomplished())
            {
                // If we're here, we got the quest linked to the item so increment quest's objective.
                Player_Quest_Control.quest_instance.GetPlayerQuestByID(questID).currentQuestObjective++;                

                if (UI_Player.ui_instance.playerQuestUI && UI_Player.ui_instance.playerQuestUI.enabled)
                    UI_Player.ui_instance.playerQuestUI.DisplayQuest(Player_Quest_Control.quest_instance.GetPlayerQuestByID(questID).questConfig.questIndexLog);
            }
        }
    }

    // Method used into UI_Player_Inventory if removing this item
    public void DecrementLinkedQuest()
    {
        if (Player_Quest_Control.quest_instance && Player_Quest_Control.quest_instance.GetPlayerQuestByID(questID))
        {
            if (Player_Quest_Control.quest_instance.GetPlayerQuestByID(questID).currentQuestObjective > 0)
            {
                Player_Quest_Control.quest_instance.GetPlayerQuestByID(questID).currentQuestObjective--;

                if (UI_Player.ui_instance.playerQuestUI && UI_Player.ui_instance.playerQuestUI.enabled)
                    UI_Player.ui_instance.playerQuestUI.DisplayQuest(Player_Quest_Control.quest_instance.GetPlayerQuestByID(questID).questConfig.questIndexLog);
            }
        }
    }
}
