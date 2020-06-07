/* SubZoneTrigger.cs
 * 
 * Permet d'entrée dans une sous-level du level courant via trigger.
 * 
 * Contient un prefab de "informationsUI" étant instancié en tant qu'enfant de player_ui, permettant d'afficher une information
 * relative au requirement pour entrer dans la zone liée.
 * 
 * */

using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class Sub_Zone_Trigger : Interactable
{
    // Use build index of scenes.
    [SerializeField] int levelToGoBuildIndex;

    [SerializeField] int levelRequired = 0; // Set a greater value to put a level required to go into a specific zone. (0 = unused)
    [SerializeField] int questRequiredID = -1; // Set a quest ID to link the quest. (-1 = unused)
    [SerializeField] bool bowEquipedRequired = false; // If player must have a bow to go into this zone.

    private void Start()
    {
        interactionType = PlayerInteractionType.SubZoneTrigger;

    }

    public override void Interact()
    {
        if (Scenes_Control.instance)
        {
            // Check if this zone required min level
            if (levelRequired > 0)
            {
                if (Player_Stats.instance)
                {
                    if (Player_Stats.instance.GetCurrentLevel() >= levelRequired)
                    {
                        // Player can enter into the game level
                        // Debug.Log("You have the level required to enter.");
                    }
                    else
                    {
                        // Tell player he can't enter while he's not levelRequired.
                        // Debug.Log("You must be lvl " + levelRequired + " to enter.");

                        UI_Player_Informations.instance.DisplayInformation("Tu dois être niveau " + levelRequired + " !");

                        return;
                    }
                }
            }
            // Check if this zone required a quest to be done
            else if (questRequiredID > -1)
            {
                if (Quests_Control.instance)
                {
                    if (Quests_Control.instance.GetQuestAchievement(questRequiredID))
                    {
                        // Quest is done, player can enter
                        // Debug.Log("The quest is done. You can enter.");
                    }
                    else
                    {
                        // Tell player he must accomplish this quest before enter
                        // Debug.Log("You must accomplish the quest(ID) : " + questRequiredID + ". To enter.");

                        QuestConfig questToAccomplish = Quests_Control.instance.questDataBase.GetQuestByID(questRequiredID);

                        UI_Player_Informations.instance.DisplayInformation("Tu dois accomplir la quête \"" + questToAccomplish.questTitle + "\"");

                        return;

                    }
                }
            }
            // If player must have a bow to enter
            else if (bowEquipedRequired == true)
            {
                if (Player_Inventory.instance)
                {
                    if (Player_Inventory.instance.GetCurrentBow() != null)
                    {
                        // Player has a bow, he can enter
                        // Debug.Log("You got a bow.");
                    }
                    else
                    {
                        // Tell player he need a bow to enter.
                        // Debug.Log("You must have a bow.");

                        UI_Player_Informations.instance.DisplayInformation("Il te faut un arc !");

                        return;

                    }
                }
            }

            Scenes_Control.instance.SwitchGameLevel(levelToGoBuildIndex);
        }
    }
   
}
