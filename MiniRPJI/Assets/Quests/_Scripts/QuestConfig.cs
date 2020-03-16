using UnityEngine;

[CreateAssetMenu(fileName = "QuestConfig", menuName = "ScriptableObjects/QuestConfig", order = 1)]
public class QuestConfig : ScriptableObject
{
    // DO not forget to reset quest state. It's automatically saved because its a ScriptableObject. but i'll save and reset all data via GameDataControl

    public int questID; // Unique ID of the quest to save and load.
    // There are 6 quests per act maximum.
    public int questIndexLog; // the index of the quest into QuestControl questsconfig array. To displaying quest at the right UI element.

    public string questTitle;
    [TextArea] public string questDescription;
    public Sprite questSprite;

    public int questObjective; // if currentQuestObjective == questObjective, then the quest is accomplished
    public int currentQuestObjective;

    public bool playerIsOnThisQuest;
    public bool accomplished;

    public bool questDone; // To set true in QuestGiver.cs when questReward is give
    public GameObject questReward;

    public int xpAmount;
}
