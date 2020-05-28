using UnityEngine;

[CreateAssetMenu(fileName = "QuestConfig", menuName = "ScriptableObjects/Quest/QuestConfig", order = 1)]
public class QuestConfig : ScriptableObject
{

    public int questID; // Unique ID of the quest to save and load.

    public string questTitle;
    [TextArea] public string questDescription;
    public Sprite questSprite;

    public int totalQuestObjective; // if Player_Quest.currentQuestObjective == totalQuestObjective, then the quest is accomplished

    public bool questDone; // To set true in QuestGiver.cs when questReward is give
    public BaseItem questReward;

    public int xpAmount;
}
