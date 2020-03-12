using UnityEngine;

public class QuestControl : MonoBehaviour
{
    public static QuestControl quest_instance; // Make this singleton and dont destroy on load
    private void Awake()
    {
        if (quest_instance == null)
        {
            quest_instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
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
            UI_Player.ui_instance.playerQuestUI.SetupButton(quest.questIndexLog);
        }
        else
        {
            Debug.Log("You already are on the quest. Or on quest using this index atleast.");
        }
    }

    // To find a quest by index not by quest ID
    public QuestConfig GetQuest(int questIndex)
    {
        if (quests[questIndex] != null)
            return quests[questIndex];
        return null;
    }
}
