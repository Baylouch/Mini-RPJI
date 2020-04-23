/* Player_Intercations.cs :
 * Centralise les interactions possible du joueur. 
 * Par exemple :
 * Si le joueur peut ramasser un objet, ce script affiche une fenetre : Appuie sur e pour intéragir.
 * 
 * */

using UnityEngine;

public enum PlayerInteractionType { Item, QuestGiver, ItemSeller, SubZoneTrigger, None } // None added because of null issue for undefined interaction type.

[RequireComponent(typeof(Collider2D))]
public class Player_Intercations : MonoBehaviour
{
    UI_Player_Interactions interactionUI;

    Interactable interactableThing;

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

    // The way interactableThing is used here was because i tought it needed to get the final type of Interactable to use the right
    // Interact() method and not the one from Interactable script. After days, i added the None, and realize you can use the "final" Interact() method
    // even if you got the object as a "Interactable" and not "Item" for instance.
    private void Update()
    {
        if (interactionSet)
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                if (interactableThing)
                {
                    if (!interactableThing.GetIsInteracting())
                    {
                        switch (interactableThing.interactionType)
                        {
                            case PlayerInteractionType.Item:
                                Item item = interactableThing.GetComponent<Item>();
                                item.Interact();
                                break;
                            case PlayerInteractionType.ItemSeller:
                                ItemSeller itemSeller = interactableThing.GetComponent<ItemSeller>();
                                itemSeller.Interact();
                                break;
                            case PlayerInteractionType.QuestGiver:
                                QuestGiver questGiver = interactableThing.GetComponent<QuestGiver>();
                                questGiver.Interact();
                                break;
                            case PlayerInteractionType.SubZoneTrigger:
                                Sub_Zone_Trigger subZoneTrigger = interactableThing.GetComponent<Sub_Zone_Trigger>();
                                subZoneTrigger.Interact();
                                break;
                            default:
                                interactableThing.Interact();
                                break;
                        }
                    }

                    interactionUI.ResetInteractionUI();
                    interactableThing = null;
                    interactionSet = false;
                }
            }
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
                interactableThing = collision.GetComponent<Interactable>();
                interactionSet = true;
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
                default:
                    // Do nothing
                    break;
            }

            interactionUI.ResetInteractionUI();
            interactableThing = null;
            interactionSet = false;
        }
    }
}
