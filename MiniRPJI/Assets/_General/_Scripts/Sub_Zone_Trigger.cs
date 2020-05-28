/* SubZoneTrigger.cs
 * 
 * Permet d'entrée dans une sous-level du level courant via trigger.
 * 
 * Contient un prefab de "informationsUI" étant instancié en tant qu'enfant de player_ui, permettant d'afficher une information
 * relative au requirement pour entrer dans la zone liée.
 * 
 * */

using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Collider2D))]
public class Sub_Zone_Trigger : Interactable
{
    // Use build index of scenes.
    [SerializeField] int levelToGoBuildIndex;

    [SerializeField] int levelRequired = 0; // Set a greater value to put a level required to go into a specific zone. (0 = unused)
    [SerializeField] int questRequiredID = -1; // Set a quest ID to link the quest. (-1 = unused)
    [SerializeField] bool bowEquipedRequired = false; // If player must have a bow to go into this zone.

    // TODO Add a UI to display why player can't interact.
    [SerializeField] GameObject informationsUI;

    GameObject currentInformationsUI;

    private void Start()
    {
        interactionType = PlayerInteractionType.SubZoneTrigger;

        if (informationsUI.activeSelf)
            informationsUI.SetActive(false);
    }

    void SetInformationsUI(string textToDisplay)
    {
        if (currentInformationsUI != null)
            return;

        if (UI_Player.instance)
        {
            // Instantiate informationsUI
            currentInformationsUI = Instantiate(informationsUI, UI_Player.instance.transform);
            // Set it unactive
            currentInformationsUI.SetActive(false);

            // Set the text
            currentInformationsUI.GetComponentInChildren<Text>().text = textToDisplay;

            // Set it active
            currentInformationsUI.SetActive(true);

            // Destroy timer
            Destroy(currentInformationsUI, 1.5f);
        }
        else // Must never be reached.
        {
            Debug.Log("No UI_Player instance in the scene to display infos.");
        }
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

                        SetInformationsUI("Tu dois être niveau " + levelRequired + " !");

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

                        SetInformationsUI("Tu dois accomplir la quête \"" + questToAccomplish.questTitle + "\"");

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

                        SetInformationsUI("Il te faut un arc !");

                        return;

                    }
                }
            }

            Scenes_Control.instance.SwitchGameLevel(levelToGoBuildIndex);
        }
    }
   
}
