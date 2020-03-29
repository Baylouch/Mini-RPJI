﻿/* Player_Intercations.cs :
 * Centralise les interactions possible du joueur. Par exemple
 * si le joueur peut ramasser un objet, ce script affiche une fenetre : Appuie sur e pour intéragir.
 * 
 * */

using UnityEngine;

public enum PlayerInteractionType { Item, QuestGiver, ItemSeller, SubZoneTrigger }

[RequireComponent(typeof(Collider2D))]
public class Player_Intercations : MonoBehaviour
{
    UI_Player_Interactions interactionUI;

    bool interactionSet = false;
    bool isInteracting;

    private void Start()
    {
        if (FindObjectOfType<UI_Player_Interactions>())
        {
            interactionUI = FindObjectOfType<UI_Player_Interactions>();
            interactionUI.ResetInteractionUI();
        }
        else
        {
            Debug.Log("No UI_Player_Interactions found.");
        }
    }

    // TODO Test all type of interactable to display an adaptable UI.
    // "Appuie sur E pour ramasser cet objet", "Appuie sur E pour parler avec ...".
    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.GetComponent<Interactable>())
        {
            if (!interactionSet)
            {
                interactionUI.SetInteractionUI("Appuie sur E pour intéragir");
                interactionSet = true;
            }

            if (Input.GetKeyDown(KeyCode.E))
            {
                if (!collision.GetComponent<Interactable>().GetIsInteracting())
                {
                    switch (collision.GetComponent<Interactable>().interactionType)
                    {
                        case PlayerInteractionType.Item:
                            Item item = collision.GetComponent<Item>();
                            item.Interact();
                            break;
                        case PlayerInteractionType.ItemSeller:
                            ItemSeller itemSeller = collision.GetComponent<ItemSeller>();
                            itemSeller.Interact();
                            break;
                        case PlayerInteractionType.QuestGiver:
                            QuestGiver questGiver = collision.GetComponent<QuestGiver>();
                            questGiver.Interact();
                            break;
                        case PlayerInteractionType.SubZoneTrigger:
                            Sub_Zone_Trigger subZoneTrigger = collision.GetComponent<Sub_Zone_Trigger>();
                            subZoneTrigger.Interact();
                            break;
                    }
                }

                interactionUI.ResetInteractionUI();
                interactionSet = false;
            }               
            
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.GetComponent<Interactable>())
        {
            switch (collision.GetComponent<Interactable>().interactionType)
            {
                case PlayerInteractionType.Item:
                    // Item doesnt require to use Uninteract method.
                    break;
                case PlayerInteractionType.ItemSeller:
                    ItemSeller itemSeller = collision.GetComponent<ItemSeller>();
                    itemSeller.UnInteract();
                    break;
                case PlayerInteractionType.QuestGiver:
                    QuestGiver questGiver = collision.GetComponent<QuestGiver>();
                    questGiver.UnInteract();
                    break;
                case PlayerInteractionType.SubZoneTrigger:
                    // Dosnt require to use Uninteract method.
                    break;
            }

            interactionUI.ResetInteractionUI();
            interactionSet = false;
        }
    }
}
