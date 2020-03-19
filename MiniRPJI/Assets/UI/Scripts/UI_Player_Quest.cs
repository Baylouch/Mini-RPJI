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

    [SerializeField] Sprite noQuestSprite;
    [SerializeField] Color noQuestColor;

    private void Start()
    {
        if (objective.gameObject.activeSelf)
            objective.gameObject.SetActive(false);
    }

    private void OnDisable()
    {
        ResetQuestsDisplay();
    }

    private void OnEnable()
    {
        for (int i = 0; i < questsButtons.Length; i++)
        {
            if (Player_Quest_Control.quest_instance)
            {
                if (Player_Quest_Control.quest_instance.GetPlayerQuestByIndex(i) != null) // If we have a quest on this button index we can set sprite only
                {
                    questsButtons[i].GetComponent<Image>().sprite = Player_Quest_Control.quest_instance.GetPlayerQuestByIndex(i).questConfig.questSprite;
                    questsButtons[i].GetComponent<Image>().color = Color.white;
                }
            }
        }
    }

    void ResetQuestsDisplay()
    {
        questTitle.text = "";
        questDescription.text = "";

        objective.text = "";
        questObjective.text = "";
        currentQuestObjective.text = "";

        for (int i = 0; i < questsButtons.Length; i++)
        {
            questsButtons[i].GetComponent<Image>().sprite = noQuestSprite;
            questsButtons[i].GetComponent<Image>().color = noQuestColor;
        }

        if (objective.gameObject.activeSelf)
            objective.gameObject.SetActive(false);
    }

    // Method used on quests button 
    public void DisplayQuest(int questIndex)
    {
        if (Player_Quest_Control.questSlotsNumb - 1 < questIndex)
            return;

        if (Player_Quest_Control.quest_instance.GetPlayerQuestByIndex(questIndex) != null)
        {
            questTitle.text = Player_Quest_Control.quest_instance.GetPlayerQuestByIndex(questIndex).questConfig.questTitle;
            questDescription.text = Player_Quest_Control.quest_instance.GetPlayerQuestByIndex(questIndex).questConfig.questDescription;

            // If sprite isnt already set, set it.
            if (questsButtons[questIndex].GetComponent<Image>().sprite != Player_Quest_Control.quest_instance.GetPlayerQuestByIndex(questIndex).questConfig.questSprite)
            {
                questsButtons[questIndex].GetComponent<Image>().sprite = Player_Quest_Control.quest_instance.GetPlayerQuestByIndex(questIndex).questConfig.questSprite;
                questsButtons[questIndex].GetComponent<Image>().color = Color.white;
            }

            if (!Player_Quest_Control.quest_instance.GetPlayerQuestByIndex(questIndex).IsQuestAccomplished())
            {
                objective.text = "Objectif ";
                questObjective.text = Player_Quest_Control.quest_instance.GetPlayerQuestByIndex(questIndex).questConfig.totalQuestObjective.ToString();
                currentQuestObjective.text = Player_Quest_Control.quest_instance.GetPlayerQuestByIndex(questIndex).currentQuestObjective.ToString();
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
        else // There is no quest in the slot
        {
            ResetQuestsDisplay();
        }
    }

    public void SetupButton(int questIndex)
    {
        questsButtons[questIndex].onClick.AddListener(() => DisplayQuest(questIndex));
    }
}
