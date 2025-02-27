﻿/* Quests_Control.cs
 * 
 * 
 * 
 * */

using UnityEngine;

public class Quests_Control : MonoBehaviour
{
    public static Quests_Control instance; // Make this singleton
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

    public QuestDataBase questDataBase; // The data base containing all the quests.

    // In this little game, we'll never get more than 100 Quests. So we can set the array to 200 size, this will not cause any issue (memory) and will simplify a bit the code.
    [SerializeField] Player_Quest[] playerQuests = new Player_Quest[100];

    private void Start()
    {
        ResetAllQuestsAchievement();
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

                        if (UI_Player.instance.playerInventoryUI)
                            UI_Player.instance.playerInventoryUI.RefreshInventory();
                    }
                }
            }
        }

        // Check if they're QuestObjectiveTarget.cs link to this quest in the scene (if its a killing quest). Then delete it
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

    void CheckForQuestsSuccess()
    {
        // Display a success pop up. We know the succes ID 0, 1, 2, 3 are for quests.
        // We want to display them in the right order because they're all require quests to be done.
        // So for instance : we'll check if the succes ID 0 is done, if yes, then display pop up for succes ID 1 and not for other.
        if (Player_Success.instance)
        {
            if (!Player_Success.instance.successDatabase.GetSuccessByID(0).isDone)
            {
                Player_Success.instance.IncrementSuccessObjectiveByID(0);
                Player_Success.instance.IncrementSuccessObjectiveByID(1, false);
                Player_Success.instance.IncrementSuccessObjectiveByID(2, false);
                Player_Success.instance.IncrementSuccessObjectiveByID(3, false);
            }
            else if (!Player_Success.instance.successDatabase.GetSuccessByID(1).isDone)
            {
                Player_Success.instance.IncrementSuccessObjectiveByID(1);
                Player_Success.instance.IncrementSuccessObjectiveByID(0, false);
                Player_Success.instance.IncrementSuccessObjectiveByID(2, false);
                Player_Success.instance.IncrementSuccessObjectiveByID(3, false);
            }
            else if (!Player_Success.instance.successDatabase.GetSuccessByID(2).isDone)
            {
                Player_Success.instance.IncrementSuccessObjectiveByID(2);
                Player_Success.instance.IncrementSuccessObjectiveByID(0, false);
                Player_Success.instance.IncrementSuccessObjectiveByID(1, false);
                Player_Success.instance.IncrementSuccessObjectiveByID(3, false);
            }
            else if (!Player_Success.instance.successDatabase.GetSuccessByID(3).isDone)
            {
                Player_Success.instance.IncrementSuccessObjectiveByID(3);
                Player_Success.instance.IncrementSuccessObjectiveByID(0, false);
                Player_Success.instance.IncrementSuccessObjectiveByID(1, false);
                Player_Success.instance.IncrementSuccessObjectiveByID(2, false);
            }
        }
    }

    public void GetNewQuest(int _questID)
    {
        // Get the first available quest index
        int questIndex = -1;
        
        // Loop trough the playerQuests array to know the first index available
        for (int i = 0; i < playerQuests.Length; i++)
        {
            if (playerQuests[i] == null)
            {
                questIndex = i;
                break;
            }
        }
        
        // If questIndex still -1 here, an error happened
        if (questIndex == -1)
        {
            Debug.LogWarning("Error with the player quests index to get a new quest.");
            return;
        }

        // Set new quest
        Player_Quest _tempPlayerQuest = gameObject.AddComponent(typeof(Player_Quest)) as Player_Quest;
        _tempPlayerQuest.questConfig = questDataBase.GetQuestByID(_questID);

        playerQuests[questIndex] = _tempPlayerQuest;
    }

    // To find a quest by INDEX
    public Player_Quest GetPlayerQuestByIndex(int questIndex)
    {
        if (playerQuests[questIndex] != null)
            return playerQuests[questIndex];
        return null;
    }

    // To find a quest by ID
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
    public void ValideQuest(int _questID, Vector3 rewardSpawnPos)
    {
        int _questIndex = -1;

        for (int i = 0; i < playerQuests.Length; i++)
        {
            if (playerQuests[i] == null)
                continue;

            if (playerQuests[i].questConfig.questID == _questID)
            {
                _questIndex = i;
                break;
            }
        }

        if (_questIndex == -1)
        {
            return;
        }

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

            // Check if the quest was the pet linked one to unlock pets.
            if (playerQuests[_questIndex].questConfig.questID == Player_Pets.questIDToUnlockPets)
            {
                Player_Pets.instance.SetPetsUnlocked(true);
            }

            if (UI_Player.instance.playerQuestUI)
                UI_Player.instance.playerQuestUI.RemoveQuestButton(playerQuests[_questIndex].questConfig);

            // Play sound
            if (Sound_Manager.instance)
                Sound_Manager.instance.PlaySound(Sound_Manager.instance.asset.achievement);

            // Player rewarded, now we need to delete everything used by the quest into the world. No more use for them.
            RemoveAccomplishedQuestStuffInScene(playerQuests[_questIndex].questConfig.questID);

            CheckForQuestsSuccess();

            // Now we can set questDone to true and delete it from our quest log (quests array)
            playerQuests[_questIndex].questConfig.questDone = true;

            Destroy(playerQuests[_questIndex]);
            playerQuests[_questIndex] = null;

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

            // Check if the quest was the pet linked one to unlock pets.
            if (questID == Player_Pets.questIDToUnlockPets)
            {
                Player_Pets.instance.SetPetsUnlocked(true);
            }

            CheckForQuestsSuccess();
        }
    }
}
