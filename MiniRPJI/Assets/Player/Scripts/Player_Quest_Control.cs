using UnityEngine;

public class Player_Quest_Control : MonoBehaviour
{
    public static Player_Quest_Control quest_instance; // Make this singleton
    private void Awake()
    {
        if (quest_instance == null)
        {
            quest_instance = this;
        }
        else
        {
            Destroy(this);
        }
    }

    public const int questSlotsNumb = 6; // Max quest by act
    [SerializeField] QuestConfig[] quests; // To setup 6 index

    private void Start()
    {
        for (int i = 0; i < quests.Length; i++)
        {
            if (quests[i] != null)
            {
                UI_Player.ui_instance.playerQuestUI.SetupButton(quests[i].questIndexLog);
            }
        }
    }

    public void GetNewQuest(QuestConfig quest)
    {
        // Check if slot is empty
        if (quests[quest.questIndexLog] == null)
        {
            // Set new quest
            quests[quest.questIndexLog] = quest;

            UI_Player.ui_instance.playerQuestUI.DisplayQuest(quest.questIndexLog);
            UI_Player.ui_instance.playerQuestUI.SetupButton(quest.questIndexLog);
        }
        else
        {
            Debug.Log("You already are on the quest. Or on quest using this index atleast.");
        }
    }

    // To find a quest by INDEX NOT by its ID
    public QuestConfig GetQuestWithIndex(int questIndex)
    {
        if (quests[questIndex] != null)
            return quests[questIndex];
        return null;
    }

    // To find a quest we have by its ID !
    public QuestConfig GetQuestWithID(int _questID)
    {
        for (int i = 0; i < quests.Length; i++)
        {
            if (quests[i] != null)
            {
                if (quests[i].questID == _questID)
                    return quests[i];
            }
        }

        return null;
    }

    // Method used in QuestGiver to valide a quest
    public void ValideQuest(int _questIndex, Vector3 rewardSpawnPos)
    {
        if (quests[_questIndex].accomplished)
        {
            // Check if quest got a reward
            if (quests[_questIndex].questReward != null)
            {
                GameObject _reward = Instantiate(quests[_questIndex].questReward, rewardSpawnPos, Quaternion.identity);
                _reward.transform.parent = GameObject.Find("Items").transform;
            }
            // Check if quest got xp amount
            if (quests[_questIndex].xpAmount > 0)
            {
                if (Player_Stats.stats_instance)
                {
                    Player_Stats.stats_instance.AddExperience(quests[_questIndex].xpAmount);
                }
            }

            // Player rewarded, now we need to delete everything used by the quest into the world. No more use for these.
            RemoveAccomplishedQuestStuffInScene(quests[_questIndex].questID);

            // Now we can set questDone to true and delete it from our quest log (quests array)
            quests[_questIndex].questDone = true;
            quests[_questIndex].playerIsOnThisQuest = false;
            quests[_questIndex] = null;

            UI_Player.ui_instance.playerQuestUI.DisplayQuest(_questIndex);
        }
    }

    void RemoveAccomplishedQuestStuffInScene(int _questID)
    {
        // Check if player got quest item link to this quest in his inventory. Then delete it.
        for (int i = 0; i < Player_Inventory.inventorySlotsNumb; i++)
        {
            if (Player_Inventory.inventory_instance.GetInventoryItem(i)) // If there is item in this inventory slot
            {
                if (Player_Inventory.inventory_instance.GetInventoryItem(i) as QuestItem) // If item is QuestItem
                {
                    // Check if its link to the quest ID.
                    QuestItem questItem = (QuestItem)Player_Inventory.inventory_instance.GetInventoryItem(i);
                    if (_questID == questItem.questID)
                    {
                        Player_Inventory.inventory_instance.SetInventoryIndex(i, -1); // Delete item.
                    }
                }
            }
        }

        // Check if they're QuestObjectiveTarget.cs link to this quest in the scene (if its a killing quest). Thene delete it
        if (FindObjectsOfType<QuestObjectiveTarget>().Length > 0)
        {
            QuestObjectiveTarget[] questObjectiveTargets = FindObjectsOfType<QuestObjectiveTarget>();
            for (int i = 0; i < questObjectiveTargets.Length; i++)
            {
                if (_questID == questObjectiveTargets[i].questID)
                {
                    Destroy(questObjectiveTargets[i]);
                }
            }
        }
    }
}
