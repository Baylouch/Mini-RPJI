﻿using UnityEngine;

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
    public BaseItem questLinkedItem; // An item who's objective of the quest, and should be spawn only when player get the quest.
    public GameObject questGOToSpawn; // A gameobject who need to spawn for do the quest

    public int xpAmount;

    public int levelRequired;
}
