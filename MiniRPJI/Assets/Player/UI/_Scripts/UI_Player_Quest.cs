/* UI_Player_Quest.cs
 * 
 * 
 * 
 * 
 * 
 * */


using UnityEngine;
using UnityEngine.UI;

public class UI_Player_Quest : MonoBehaviour
{
    [SerializeField] GameObject questButtonPrefab; // The button who'll spawn each time player get a new quest

    [SerializeField] RectTransform buttonsContainer; // The parent of each quest buttons.

    [SerializeField] GameObject questDisplayer; // The panel to display quest description, current objective...

    // Displaying stuff
    [SerializeField] Text questTitle;
    [SerializeField] Text questDescription;
    [SerializeField] Text objective;
    [SerializeField] Text questObjective;
    [SerializeField] Text currentQuestObjective;
    [SerializeField] Image questImage;
    [SerializeField] Text noQuestText;

    private void OnDisable()
    {
        if (questDisplayer.activeSelf)
            questDisplayer.SetActive(false);
    }

    private void OnEnable()
    {
        // OLD content, check if new below works, then delete it.
        // We check if there is a quest at first index
        //if (Quests_Control.instance && Quests_Control.instance.GetPlayerQuestByIndex(0))
        //{
        //    if (noQuestText.gameObject.activeSelf)
        //        noQuestText.gameObject.SetActive(false);
        //}
        //else
        //{
        //    if (!noQuestText.gameObject.activeSelf)
        //        noQuestText.gameObject.SetActive(true);
        //}

        // We check if there is a quest, else we show "noQuestText"
        if (Quests_Control.instance)
        {
            for (int i = 0; i < 200; i++) // We know 200 because its the max length of the Player_Quest array in Quests_Control.
            {
                if (Quests_Control.instance.GetPlayerQuestByIndex(i) != null) // We check if there is a quest
                {
                    // If yes we can desactive noQuestText if its active and return.
                    if (noQuestText.gameObject.activeSelf)
                        noQuestText.gameObject.SetActive(false);

                    return;
                }
            }
            // If we're here, there is no quest in Player_Quest array
            if (!noQuestText.gameObject.activeSelf)
                noQuestText.gameObject.SetActive(true);

        }
    }

    // Start is called before the first frame update
    void Start()
    {
        if (questDisplayer.activeSelf)
            questDisplayer.SetActive(false);
    }

    public void DisplayQuest(QuestConfig questToDisplay)
    {
        questTitle.text = questToDisplay.questTitle;
        questDescription.text = questToDisplay.questDescription;

        if (questToDisplay.questSprite != null)
        {
            questImage.sprite = questToDisplay.questSprite;
            questImage.gameObject.SetActive(true);
        }
        else
        {
            questImage.gameObject.SetActive(false);
        }

        if (!Quests_Control.instance.GetPlayerQuestByID(questToDisplay.questID).IsQuestAccomplished())
        {
            objective.text = "Objectif ";
            questObjective.text = questToDisplay.totalQuestObjective.ToString();
            currentQuestObjective.text = Quests_Control.instance.GetPlayerQuestByID(questToDisplay.questID).currentQuestObjective.ToString();
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

        if (!questDisplayer.activeSelf)
            questDisplayer.SetActive(true);
    }

    public void AddQuestButton(QuestConfig linkedQuest)
    {
        GameObject newButton = Instantiate(questButtonPrefab, buttonsContainer);

        newButton.GetComponent<Quest_Button>().questLinkedID = linkedQuest.questID;
        newButton.GetComponentInChildren<Text>().text = linkedQuest.questTitle;

        newButton.GetComponent<Button>().onClick.AddListener(() => DisplayQuest(linkedQuest));
    }

    public void RemoveQuestButton(QuestConfig linkedQuest)
    {
        for (int i = 0; i < buttonsContainer.childCount; i++)
        {
            Quest_Button cur_QuestButton = buttonsContainer.GetChild(i).GetComponent<Quest_Button>();

            if (cur_QuestButton && cur_QuestButton.questLinkedID == linkedQuest.questID)
            {
                Destroy(cur_QuestButton.gameObject);
                return;
            }
        }

        // We check if there is a quest at first index
        if (Quests_Control.instance.GetPlayerQuestByIndex(0))
        {
            if (noQuestText.gameObject.activeSelf)
                noQuestText.gameObject.SetActive(false);
        }
        else
        {
            if (!noQuestText.gameObject.activeSelf)
                noQuestText.gameObject.SetActive(true);
        }
    }

    public void CloseQuestDisplayer()
    {
        if (questDisplayer.activeSelf)
            questDisplayer.SetActive(false);
    }
}
