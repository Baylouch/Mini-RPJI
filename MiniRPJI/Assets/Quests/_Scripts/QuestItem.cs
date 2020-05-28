using UnityEngine;

[CreateAssetMenu(fileName = "QuestItemName", menuName = "ScriptableObjects/Items/QuestItem", order = 1)]
public class QuestItem : BaseItem
{
    [Header("Quest settings")]
    public int questID;

    // Method used into Player_Inventory in GetNewItem(BaseItem)
    public void IncrementLinkedQuest()
    {
        if (Quests_Control.instance && Quests_Control.instance.GetPlayerQuestByID(questID))
        {
            // Security if player already got max objective item
            if (!Quests_Control.instance.GetPlayerQuestByID(questID).IsQuestAccomplished())
            {
                // If we're here, we got the quest linked to the item so increment quest's objective.
                Quests_Control.instance.GetPlayerQuestByID(questID).currentQuestObjective++;

                if (UI_Player.instance.playerQuestUI && UI_Player.instance.playerQuestUI.enabled)
                    UI_Player.instance.playerQuestUI.DisplayQuest(Quests_Control.instance.GetPlayerQuestByID(questID).questConfig);
            }
        }
    }

    // Method used into UI_Player_Inventory if removing this item
    public void DecrementLinkedQuest()
    {
        if (Quests_Control.instance && Quests_Control.instance.GetPlayerQuestByID(questID))
        {
            if (Quests_Control.instance.GetPlayerQuestByID(questID).currentQuestObjective > 0)
            {
                Quests_Control.instance.GetPlayerQuestByID(questID).currentQuestObjective--;

                if (UI_Player.instance.playerQuestUI && UI_Player.instance.playerQuestUI.enabled)
                    UI_Player.instance.playerQuestUI.DisplayQuest(Quests_Control.instance.GetPlayerQuestByID(questID).questConfig);
            }
        }
    }
}
