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
    [SerializeField] private Transform questLinkedItemSpawnPosition;

    [SerializeField] [TextArea] private string endDialogue;

    [Header("UI elements")]
    [SerializeField] GameObject dialoguePanel;
    [SerializeField] private Text dialogue;

    [SerializeField] private Button acceptButton;
    [SerializeField] private Button refuseButton;
    [SerializeField] private Button validationButton;

    private void Start()
    {
        interactionType = PlayerInteractionType.QuestGiver;

        UnActiveUI();

        // Le cas du bouton refuser est spécial : Il utilisera "UnInteract" a chaque fois, et comme la méthode ne reçoit pas de paramètres,
        // on peut configurer le bouton ici et ne plus toucher à sa configuration ensuite.
        refuseButton.onClick.AddListener(UnInteract);

        // Etant donné que l'objet de quête lié spawn au moment ou le joueur prend la quête, si il sauvegarde alors qu'il a accepté la quête, mais n'as pas
        // encore récupéré l'objet, il disparaîtra a jamais. Pour palier à ce problème, il faut s'assurer que le QuestGiver donnant cette quête soit dans la 
        // même chose que l'objet lié. De ce fait au start de celui-ci on peut spawn l'objet si le joueur a déjà accepté la quête sans le ramasser auparavant.
        if (questLinkedItemSpawnPosition != null)
        {
            // Check for every quests to give
            for (int i = 0; i < questsToGive.Length; i++)
            {
                // Check if player is actually on the quest
                if (Quests_Control.instance && Quests_Control.instance.GetPlayerQuestByID(questsToGive[i].questID) != null)
                {
                    // If this quest got a GO linked
                    if (questsToGive[i].questGOToSpawn)
                    {
                        GameObject questGO = Instantiate(questsToGive[i].questGOToSpawn, GameObject.Find("Items").transform);
                        questGO.transform.position = questLinkedItemSpawnPosition.position;
                    }
                }
            }
        }
    }


    public override void UnInteract()
    {
        base.UnInteract();

        UnActiveUI();
    }

    public override void Interact()
    {
        base.Interact();

        for (int i = 0; i < questsToGive.Length; i++)
        {
            // Si le joueur a déjà fait la quête
            if (questsToGive[i].questDone)
            {
                continue; // Move to the next quest
            }

            // Verifier que le joueur a le niveau requis pour effectuer la quête
            if (Player_Stats.instance)
            {
                if (questsToGive[i].levelRequired > 0)
                {
                    if (Player_Stats.instance.GetCurrentLevel() < questsToGive[i].levelRequired)
                    {
                        SetDialogueUI("Reviens me voir quand tu sera niveau " + questsToGive[i].levelRequired + ".");
                        return;
                    }
                }
            }

            // Si le joueur a déjà cette quête
            if (Quests_Control.instance.GetPlayerQuestByID(questsToGive[i].questID))
            {
                // On verifie si il a terminé l'objectif
                if (Quests_Control.instance.GetPlayerQuestByID(questsToGive[i].questID).IsQuestAccomplished())
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
                SetNewQuestUI(i);

                return;
            }
        }

        // Si le joueur a fait toutes les quêtes qu'il y avait, affiche "endDialogue"
        SetDialogueUI(endDialogue);
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

        dialogue.text = "Vous avez terminer la quête " + questsToGive[questToGiveIndex].questTitle + ".\n\nVoila votre récompense!";
        if (!dialogue.gameObject.activeSelf)
            dialogue.gameObject.SetActive(true);

        validationButton.onClick.AddListener(() => Quests_Control.instance.ValideQuest(questsToGive[questToGiveIndex].questID, rewardPosition.position));
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

    void SetDialogueUI(string _dialogue)
    {
        if (!dialoguePanel.activeSelf)
            dialoguePanel.SetActive(true);

        dialogue.text = _dialogue;

        if (!dialogue.gameObject.activeSelf)
            dialogue.gameObject.SetActive(true);

        validationButton.onClick.AddListener(UnInteract);

        if (!validationButton.gameObject.activeSelf)
            validationButton.gameObject.SetActive(true);
    }

    public void AcceptQuest(int questID)
    {
        Quests_Control.instance.GetNewQuest(questID);

        if (questLinkedItemSpawnPosition != null)
        {
            if (Quests_Control.instance.questDataBase.GetQuestByID(questID).questGOToSpawn)
            {
                GameObject questGO = Instantiate(Quests_Control.instance.questDataBase.GetQuestByID(questID).questGOToSpawn, GameObject.Find("Items").transform);
                questGO.transform.position = questLinkedItemSpawnPosition.position;
            }
        }

        UnInteract();
    }
}
