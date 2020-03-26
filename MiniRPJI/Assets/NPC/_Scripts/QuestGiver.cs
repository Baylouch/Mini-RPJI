/* QuestGiver.cs :
 * Permet au joueur d'obtenir des quêtes par interactions UI.
 * 
 * Scenario : Le joueur interagit avec le "QuestGiver" celui-ci execute un premier dialogue, le joueur valide.
 * Le second dialogue sera la description de la quête que le "QuestGiver" souhaite actuellement donner.
 * Le "QuestGiver" peut donner plusieurs quêtes. Dans ce cas la, chaque QuestConfig a une valeur bool "questDone" qui sera évalué
 * pour savoir quelle quête n'as pas encore été donnée.
 * 
 * Les quêtes doivent être configurées dans l'ordre ou l'on souhaite les données au joueur. questsToGive[0] = 1ere quête, questsToGive[1] = 2eme quête...
 * 
 * le end dialogue permet de dire au joueur une derniere chose lorsqu'il n'a plus de quête a donner.
 * */

using UnityEngine;
using UnityEngine.UI;

public class QuestGiver : Interactable
{
    [Tooltip("If you put more than one quest in, quests will be get in ordrer its set.")]
    [SerializeField] private QuestConfig[] questsToGive; // Set it for quest suit. Quests[0] = first quest, Quests[1] = second quest, etc...

    [SerializeField] private Transform rewardPosition;

    [SerializeField] private bool isIntercating = false;

    [SerializeField] [TextArea] private string endDialogue;

    private Transform player;

    [Header("UI elements")]
    [SerializeField] GameObject dialoguePanel;
    [SerializeField] private Text dialogue;

    [SerializeField] private Button acceptButton;
    [SerializeField] private Button refuseButton;
    [SerializeField] private Button validationButton;

    private void Start()
    {
        UnActiveUI();

        // Le cas du bouton refuser est spécial : Il utilisera "UnInteract" a chaque fois, et comme la méthode ne reçoit pas de paramètres,
        // on peut configurer le bouton ici et ne plus toucher à sa configuration ensuite.
        refuseButton.onClick.AddListener(UnInteract);
    }

    private void Update()
    {
        // Security if player go to far from QuestGiver, we need to unset all.
        if (player)
        {
            if (Vector3.Distance(transform.position, player.position) > 5f)
            {
                UnInteract();
            }
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                if (!isIntercating)
                {
                    player = collision.gameObject.transform;
                    Interact();
                }                    
                else
                    UnInteract();
            }
        }
    }

    void UnActiveUI()
    {
        if (dialogue.gameObject.activeSelf)
        {
            dialogue.text = "";
            dialogue.gameObject.SetActive(false);
        }
            
        if (acceptButton.gameObject.activeSelf)
        {
            acceptButton.onClick.RemoveAllListeners();
            acceptButton.gameObject.SetActive(false);
        }
            
        if (refuseButton.gameObject.activeSelf)
            refuseButton.gameObject.SetActive(false);

        if (validationButton.gameObject.activeSelf)
        {
            validationButton.onClick.RemoveAllListeners();
            validationButton.gameObject.SetActive(false);
        }

        if (dialoguePanel.activeSelf)
            dialoguePanel.SetActive(false);
           
    }

    void SetNewQuestUI(int questToGiveIndex)
    {
        if (!dialoguePanel.activeSelf)
            dialoguePanel.SetActive(true);
        
        dialogue.text = questsToGive[questToGiveIndex].questDescription;

        if (!dialogue.gameObject.activeSelf)
            dialogue.gameObject.SetActive(true);

        acceptButton.onClick.AddListener(() => AcceptQuest(questsToGive[questToGiveIndex].questID));
        if (!acceptButton.gameObject.activeSelf)
            acceptButton.gameObject.SetActive(true);

        if (!refuseButton.gameObject.activeSelf)
            refuseButton.gameObject.SetActive(true);
    }

    void SetPreRewardUI(int questToGiveIndex)
    {
        if (!dialoguePanel.activeSelf)
            dialoguePanel.SetActive(true);

        dialogue.text = "Vous avez terminer la quête " + questsToGive[questToGiveIndex].questTitle + ".\n\n\nVoila votre récompense!";
        if (!dialogue.gameObject.activeSelf)
            dialogue.gameObject.SetActive(true);

        validationButton.onClick.AddListener(() => Player_Quest_Control.quest_instance.ValideQuest(questsToGive[questToGiveIndex].questIndexLog, rewardPosition.position));
        validationButton.onClick.AddListener(UnInteract);
        if (!validationButton.gameObject.activeSelf)
            validationButton.gameObject.SetActive(true);
    }

    void SetQuestInProgressUI()
    {
        if (!dialoguePanel.activeSelf)
            dialoguePanel.SetActive(true);

        dialogue.text = "Alors aventurier, comment avance la mission que je vous ai confié ?";
        if (!dialogue.gameObject.activeSelf)
            dialogue.gameObject.SetActive(true);

        validationButton.onClick.AddListener(UnInteract);
        if (!validationButton.gameObject.activeSelf)
            validationButton.gameObject.SetActive(true);
    }

    void SetEndDialogueUI()
    {
        if (!dialoguePanel.activeSelf)
            dialoguePanel.SetActive(true);

        dialogue.text = endDialogue;

        if (!dialogue.gameObject.activeSelf)
            dialogue.gameObject.SetActive(true);

        validationButton.onClick.AddListener(UnInteract);
        if (!validationButton.gameObject.activeSelf)
            validationButton.gameObject.SetActive(true);
    }

    void UnInteract()
    {
        UnActiveUI();
        player = null;
        isIntercating = false;
    }

    public override void Interact()
    {
        isIntercating = true;

        for (int i = 0; i < questsToGive.Length; i++)
        {
            // Si le joueur a déjà fait la quête
            if (questsToGive[i].questDone)
            {
                continue; // Move to the next quest
            }

            // Si le joueur a déjà cette quête
            if (Player_Quest_Control.quest_instance.GetPlayerQuestByID(questsToGive[i].questID))
            {
                // On verifie si il a terminé l'objectif
                if (Player_Quest_Control.quest_instance.GetPlayerQuestByID(questsToGive[i].questID).IsQuestAccomplished())
                {
                    // Si oui on valide la quête
                    SetPreRewardUI(i);

                    return;
                }
                else
                {
                    // Sinon on lui demande de revenir quand il aura terminé
                    SetQuestInProgressUI();

                    return;
                }
            }
            else // Sinon le joueur n'a pas cette quête
            {
                // On verifie qu'il n'a pas de quête se trouvant au meme index log
                if (Player_Quest_Control.quest_instance.GetPlayerQuestByIndex(questsToGive[i].questIndexLog) == null)
                {
                    SetNewQuestUI(i);

                    return;
                }         
                else
                {
                    // Sinon on dit au joueur qu'il doit effectuer la quête se trouvant à l'index log avant (ce cas ne devrait pas arriver)
                }
            }
        }

        // Si le joueur a fait toutes les quêtes qu'il y avait, affiche "endDialogue"
        SetEndDialogueUI();
    }

    public void AcceptQuest(int questID)
    {        
        Player_Quest_Control.quest_instance.GetNewQuest(questID);
        UnInteract();
    }
}
