using UnityEngine;

public class Player_Quest_Control : MonoBehaviour
{
    public static Player_Quest_Control instance; // Make this singleton
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(this);
        }
    }

    public QuestDataBase questDataBase;

    public const int questSlotsNumb = 6; // Max quest by act
    [SerializeField] Player_Quest[] playerQuests; // To setup 6 index

    private void Start()
    {
        ResetAllQuestsAchievement();

        for (int i = 0; i < playerQuests.Length; i++)
        {
            if (playerQuests[i] != null)
            {
                UI_Player.instance.playerQuestUI.SetupButton(i);
            }
        }
    }

    void RemoveAccomplishedQuestStuffInScene(int _questID)
    {
        // Check if player got quest item link to this quest in his inventory. Then delete it.
        for (int i = 0; i < Player_Inventory.inventorySlotsNumb; i++)
        {
            if (Player_Inventory.instance.GetInventoryItem(i)) // If there is item in this inventory slot
            {
                if (Player_Inventory.instance.GetInventoryItem(i) as QuestItem) // If item is QuestItem
                {
                    // Check if its link to the quest ID.
                    QuestItem questItem = (QuestItem)Player_Inventory.instance.GetInventoryItem(i);
                    if (_questID == questItem.questID)
                    {
                        Player_Inventory.instance.SetInventoryIndex(i, -1); // Delete item.
                        UI_Player.instance.playerInventoryUI.RefreshInventory();
                    }
                }
            }
        }

        // Check if they're QuestObjectiveTarget.cs link to this quest in the scene (if its a killing quest). Thene delete it
        if (FindObjectsOfType<Quest_Objective_Target>().Length > 0)
        {
            Quest_Objective_Target[] questObjectiveTargets = FindObjectsOfType<Quest_Objective_Target>();
            for (int i = 0; i < questObjectiveTargets.Length; i++)
            {
                if (_questID == questObjectiveTargets[i].questID)
                {
                    Destroy(questObjectiveTargets[i]);
                }
            }
        }
    }

    public void GetNewQuest(int _questID)
    {
        // Check if slot is empty
        if (playerQuests[questDataBase.GetQuestByID(_questID).questIndexLog] == null)
        {
            // Set new quest
            Player_Quest _tempPlayerQuest = gameObject.AddComponent(typeof(Player_Quest)) as Player_Quest;
            _tempPlayerQuest.questConfig = questDataBase.GetQuestByID(_questID);

            playerQuests[questDataBase.GetQuestByID(_questID).questIndexLog] = _tempPlayerQuest;
            

            UI_Player.instance.playerQuestUI.DisplayQuest(questDataBase.GetQuestByID(_questID).questIndexLog);
            UI_Player.instance.playerQuestUI.SetupButton(questDataBase.GetQuestByID(_questID).questIndexLog);
        }
        else // It must never go this part.
        {
            Debug.Log("You already are on the quest. Or on quest using this index atleast.");
        }
    }

    // To find a quest by INDEX NOT by its ID
    public Player_Quest GetPlayerQuestByIndex(int questIndex)
    {
        if (playerQuests[questIndex] != null)
            return playerQuests[questIndex];
        return null;
    }

    public Player_Quest GetPlayerQuestByID(int _questID)
    {
        for (int i = 0; i < playerQuests.Length; i++)
        {
            if (playerQuests[i] != null)
            {
                if (playerQuests[i].questConfig.questID == _questID)
                    return playerQuests[i];
            }
        }

        return null;
    }

    // Method used in QuestGiver to valide a quest
    public void ValideQuest(int _questIndex, Vector3 rewardSpawnPos)
    {
        if (playerQuests[_questIndex].IsQuestAccomplished())
        {
            // Check if quest got a reward
            if (playerQuests[_questIndex].questConfig.questReward != null)
            {
                GameObject _reward = Instantiate(playerQuests[_questIndex].questConfig.questReward.itemPrefab, rewardSpawnPos, Quaternion.identity);

                _reward.GetComponent<SpriteRenderer>().sprite = playerQuests[_questIndex].questConfig.questReward.prefabImage;
                _reward.GetComponent<Item>().itemConfig = playerQuests[_questIndex].questConfig.questReward;

                if (GameObject.Find("Items"))
                {
                    _reward.transform.parent = GameObject.Find("Items").transform;
                }
            }
            // Check if quest got xp amount
            if (playerQuests[_questIndex].questConfig.xpAmount > 0)
            {
                if (Player_Stats.instance)
                {
                    Player_Stats.instance.AddExperience(playerQuests[_questIndex].questConfig.xpAmount);
                }
            }

            // Play sound
            if (Sound_Manager.instance)
                Sound_Manager.instance.PlaySound(Sound_Manager.instance.asset.questAchievement);

            // Player rewarded, now we need to delete everything used by the quest into the world. No more use for them.
            RemoveAccomplishedQuestStuffInScene(playerQuests[_questIndex].questConfig.questID);

            // Now we can set questDone to true and delete it from our quest log (quests array)
            playerQuests[_questIndex].questConfig.questDone = true;

            Destroy(playerQuests[_questIndex]);
            playerQuests[_questIndex] = null;

            UI_Player.instance.playerQuestUI.DisplayQuest(_questIndex);
        }
    }   

    // Method used for load data from GameDataControl.
    public void RemoveQuestByIndex(int questIndex)
    {
        if (playerQuests[questIndex] != null)
        {
            Destroy(playerQuests[questIndex]);
            playerQuests[questIndex] = null;
        }
    }

    // Method to reset all quest already done by player. Must be used on each start game, and loaded game.
    public void ResetAllQuestsAchievement()
    {
        if (questDataBase)
        {
            for (int i = 0; i < questDataBase.quests.Length; i++)
            {
                if (questDataBase.quests[i])
                    questDataBase.quests[i].questDone = false;
            }
        }
    }

    // Methods mirror to the one below "ResetAllQuestsAchievement" to know which quests are done when data are saved
    public bool GetQuestAchievement(int questID)
    {
        if (questDataBase.GetQuestByID(questID))
        {
            if (questDataBase.GetQuestByID(questID).questDone)
            {
                return true;
            }
        }

        return false;
    }

    // to set quests when loaded, second parameter isnt required but its more explicit.
    public void SetQuestAchievement(int questID, bool done)
    {
        if (questDataBase.GetQuestByID(questID))
        {
            questDataBase.GetQuestByID(questID).questDone = done;
        }
    }
}
