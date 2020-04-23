/* Player_Quest.cs :
 * Permet de contenir la quête qu'un joueur a accepté.
 * Contient une référence vers la "QuestConfig" et l'avancé actuelle de la quête.
 * Est utilisé sous forme d'array dans Player_Quest_Control.cs
 * Sera crée directement quand le joueur accepte une quête.
 * */

using UnityEngine;

public class Player_Quest : MonoBehaviour
{
    public QuestConfig questConfig;

    public int currentQuestObjective;

    public bool IsQuestAccomplished()
    {
        return currentQuestObjective >= questConfig.totalQuestObjective;
    }
}
