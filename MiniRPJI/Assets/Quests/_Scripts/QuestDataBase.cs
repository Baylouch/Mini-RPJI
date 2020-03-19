/* QuestDataBase.cs
 
Must contain all quests in the game
each of them must have an unique ID !

It's used to save and load inventory data.

*/
using UnityEngine;

[CreateAssetMenu(fileName = "QuestDataBase", menuName = "ScriptableObjects/Quest/QuestDataBase", order = 1)]
public class QuestDataBase : ScriptableObject
{
    public QuestConfig[] quests;

    public QuestConfig GetQuestByID(int _questID)
    {
        for (int i = 0; i < quests.Length; i++)
        {
            if (quests[i] != null)
            {
                if (quests[i].questID == _questID)
                    return quests[i];
            }
        }

        return null;
    }
}
