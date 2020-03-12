/* QuestObjective.cs
 * A attaché sur chaque objectif de quete, item ou ennemies, afin de pouvoir accomplir la quête lié à l'ID
 * Si le joueur a débloquer la quête, a chaque fois que cette objet est détruit, l'objectif de la quête s'actualise.
 * Si le joueur a terminé la quête, tout les scripts relié a l'ID de cette quete sont détruits.
 * 
 * */
using UnityEngine;

public class QuestObjective : MonoBehaviour
{
    [SerializeField] private int questID = 0; // To know the quest ID (not index) of the script acces to.

    private void Start()
    {
        // If quest is already accomplished at start of the game. Because loading stuff
        if (QuestControl.quest_instance)
        {
            for (int i = 0; i < QuestControl.questSlotsNumb; i++)
            {
                if (QuestControl.quest_instance.GetQuest(i))
                {
                    if (QuestControl.quest_instance.GetQuest(i).questID == questID)
                    {
                        if (QuestControl.quest_instance.GetQuest(i).accomplished)
                        {
                            QuestObjective[] questObjective = FindObjectsOfType<QuestObjective>();
                            for (int j = 0; j < questObjective.Length; j++)
                            {
                                if (questObjective[j].questID == questID)
                                    Destroy(questObjective[j]);
                            }
                        }
                    }
                    return;
                }
            }
        }
    }

    private void OnDestroy()
    {
        if (QuestControl.quest_instance) // If there is a quest control in game
        {
            for (int i = 0; i < QuestControl.questSlotsNumb; i++) // Check all element
            {
                if (QuestControl.quest_instance.GetQuest(i))
                {
                    if (QuestControl.quest_instance.GetQuest(i).questID == questID) // If this element its the right one
                    {
                        if (!QuestControl.quest_instance.GetQuest(i).accomplished) // If quest isnt accomplished yet
                        {
                            // Then we are the right objective of the quest and we're destroyed so currentQuestObjective++ !
                            QuestControl.quest_instance.GetQuest(i).currentQuestObjective++;

                            // Then check if we accomplished the quest. If yes find all MoleQuest scripts and destroy them because no more need of them.
                            if (QuestControl.quest_instance.GetQuest(i).questObjective <= QuestControl.quest_instance.GetQuest(i).currentQuestObjective)
                            {
                                QuestControl.quest_instance.GetQuest(i).accomplished = true;

                                QuestObjective[] questObjective = FindObjectsOfType<QuestObjective>();
                                for (int j = 0; j < questObjective.Length; j++)
                                {
                                    if (questObjective[j].questID == questID)
                                        Destroy(questObjective[j]);
                                }
                            }

                            // Refresh Quest UI if displayed
                            if (UI_Player.ui_instance.playerQuestUI.enabled)
                                UI_Player.ui_instance.playerQuestUI.DisplayQuest(QuestControl.quest_instance.GetQuest(i).questIndexLog);
                        }
                        return; // Dont continue
                    }
                }             
            }
        }
    }
}
