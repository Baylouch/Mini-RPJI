using UnityEngine;
using UnityEngine.UI;

public class UI_Player_Quest : MonoBehaviour
{
    [SerializeField] Button[] questsButtons; // To put buttons at the same index as quests config array

    [SerializeField] Text questTitle;
    [SerializeField] Text questDescription;

    [SerializeField] Text objective;
    [SerializeField] Text questObjective;
    [SerializeField] Text currentQuestObjective;

    private void Start()
    {
        if (objective.gameObject.activeSelf)
            objective.gameObject.SetActive(false);
    }

    private void OnDisable()
    {
        questTitle.text = "";
        questDescription.text = "";

        objective.text = "";
        questObjective.text = "";
        currentQuestObjective.text = "";

        if (objective.gameObject.activeSelf)
            objective.gameObject.SetActive(false);
    }

    // Method used on quests button 
    public void DisplayQuest(int questIndex)
    {
        if (QuestControl.questSlotsNumb - 1 < questIndex)
            return;

        if (QuestControl.quest_instance.GetQuest(questIndex) != null)
        {
            questTitle.text = QuestControl.quest_instance.GetQuest(questIndex).questTitle;
            questDescription.text = QuestControl.quest_instance.GetQuest(questIndex).questDescription;

            // If sprite isnt already set, set it.
            if (questsButtons[questIndex].GetComponent<Image>().sprite != QuestControl.quest_instance.GetQuest(questIndex).questSprite)
            {
                questsButtons[questIndex].GetComponent<Image>().sprite = QuestControl.quest_instance.GetQuest(questIndex).questSprite;
                questsButtons[questIndex].GetComponent<Image>().color = Color.white;
            }

            

            if (!QuestControl.quest_instance.GetQuest(questIndex).accomplished)
            {
                objective.text = "Objectif ";
                questObjective.text = QuestControl.quest_instance.GetQuest(questIndex).questObjective.ToString();
                currentQuestObjective.text = QuestControl.quest_instance.GetQuest(questIndex).currentQuestObjective.ToString();
                // Because we disable them when objective is accomplished, we need to enable them when it's not.
                for (int i = 0; i < objective.transform.childCount; i++)
                {
                    objective.transform.GetChild(i).gameObject.SetActive(true);
                }
            }
            else
            {
                objective.text = "Accompli ";
                questObjective.text = "";
                currentQuestObjective.text = "";
                // Because objective is parent of questObjective and currentQuestObjective text we can disable them from it
                for (int i = 0; i < objective.transform.childCount; i++)
                {
                    objective.transform.GetChild(i).gameObject.SetActive(false);
                }
            }

            if (!objective.gameObject.activeSelf)
            {
                objective.gameObject.SetActive(true);
            }
        }
    }

    public void SetupButton(int questIndex)
    {
        questsButtons[questIndex].onClick.AddListener(() => DisplayQuest(questIndex));
    }
}
