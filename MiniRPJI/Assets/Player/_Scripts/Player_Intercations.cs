/* Player_Intercations.cs :
 * 
 * Centralise les interactions possible du joueur. 
 * Par exemple :
 * Si le joueur peut ramasser un objet, ce script affiche une fenetre : Appuie sur e pour intéragir.
 * 
 * 
 * Le code a été simplifié pour l'instant car je n'effectue pas d'actions spéciales pour chaque type d'interactions différentes.
 * Pour l'instant, Interact() et UnInteract() sont utilisés de manières universelle.
 * Tout les nouveaux Scripts "Interactable" peuvent définir leur PlayerInteractionType en "None".
 * 
 * */

using UnityEngine;

public enum PlayerInteractionType { Item, QuestGiver, ItemSeller, PetSeller, SubZoneTrigger, None } // None added because of null issue for undefined interaction type.

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
    // So todo, use only Interactable component to use Interact() and UnInteract() methods
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
                        interactableThing.Interact();

                        //switch (interactableThing.interactionType)
                        //{
                        //    case PlayerInteractionType.Item:
                        //        Item item = interactableThing.GetComponent<Item>();
                        //        item.Interact();
                        //        break;
                        //    case PlayerInteractionType.ItemSeller:
                        //        ItemSeller itemSeller = interactableThing.GetComponent<ItemSeller>();
                        //        itemSeller.Interact();
                        //        break;
                        //    case PlayerInteractionType.QuestGiver:
                        //        QuestGiver questGiver = interactableThing.GetComponent<QuestGiver>();
                        //        questGiver.Interact();
                        //        break;
                        //        // Add PetSeller part
                        //    case PlayerInteractionType.SubZoneTrigger:
                        //        Sub_Zone_Trigger subZoneTrigger = interactableThing.GetComponent<Sub_Zone_Trigger>();
                        //        subZoneTrigger.Interact();
                        //        break;
                        //    default:
                        //        interactableThing.Interact();
                        //        break;
                        //}
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
            collision.GetComponent<Interactable>().UnInteract();

            //switch (collision.GetComponent<Interactable>().interactionType)
            //{
            //    case PlayerInteractionType.Item:
            //        // Item doesnt require to use Uninteract method.
            //        break;
            //    case PlayerInteractionType.ItemSeller:
            //        ItemSeller itemSeller = collision.GetComponent<ItemSeller>();
            //        itemSeller.UnInteract();
            //        break;
            //    case PlayerInteractionType.QuestGiver:
            //        QuestGiver questGiver = collision.GetComponent<QuestGiver>();
            //        questGiver.UnInteract();
            //        break;
            //    // Add PetSeller part
            //    case PlayerInteractionType.SubZoneTrigger:
            //        // Dosnt require to use Uninteract method.
            //        break;
            //    default:
            //        collision.GetComponent<Interactable>().UnInteract();
            //        break;
            //}

            interactionUI.ResetInteractionUI();
            interactableThing = null;
            interactionSet = false;
        }
    }
}
