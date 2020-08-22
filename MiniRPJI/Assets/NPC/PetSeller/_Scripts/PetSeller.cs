using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class PetSeller : Interactable
{
    [SerializeField] NPC_VOICE voice;

    [SerializeField] PetConfig[] petsToSell;

    [SerializeField] GameObject globalPanel;
    [SerializeField] GameObject petsList;

    [SerializeField] Button buyButton; // This one need to be configure for every slots
    [SerializeField] Button okButton; // It can be configure at start
    [SerializeField] Button backButton; // It can be configure at start
    [SerializeField] Text textDialogue; // It can be configure at start

    [SerializeField] [TextArea] string dialogue;

    [SerializeField] GameObject petButtonPrefab; // Pet button to set when created with the right pet config -> Must have a Pet_Sellable_Button component on it

    [SerializeField] RectTransform buttonsContainer; // The parent of each pet buttons.

    // Displaying stuff
    [SerializeField] GameObject informationsPanel; // Contains all descriptions stuff

    [SerializeField] Image petImage;
    [SerializeField] Text petName;
    [SerializeField] Text petCategory;
    [SerializeField] Text petWeight;
    [SerializeField] Text petDescription;
    [SerializeField] Text petPrice;

    Pet_Sellable_Button currentPetSellableButtonDescription;

    bool sellerSet = false;

    // Start is called before the first frame update
    void Start()
    {
        interactionType = PlayerInteractionType.PetSeller;

        backButton.onClick.AddListener(UnInteract);
        okButton.onClick.AddListener(SetUIInventory);

        UnActiveUI();

        SetSeller();
    }

    // Update is called once per frame
    void Update()
    {
        if (Player_Stats.instance)
            IsMouseOverPetSellableButton();
    }

    // Method to set the pet seller. It'll check for each sellable pets if player got the pet, sellable button doesnt be created and pet config is removed from 
    // petsToSell array
    void SetSeller()
    {
        // Loop trough all sellable pets, and compare each one with all player's pet. If player got the sellable pet, dont make it sellable.
        if (Player_Pets.instance)
        {
            for (int i = 0; i < petsToSell.Length; i++)
            {
                // If we're here, player dont have the pet to sell. We can create a new sell button and link the pet to it.
                AddPetSellableButton(petsToSell[i]);

                for (int j = 0; j < Player_Pets.playerPetsLength; j++)
                {
                    if (Player_Pets.instance.GetPlayerPetByIndex(j) != null && petsToSell[i] != null)
                    {
                        if (Player_Pets.instance.GetPlayerPetByIndex(j).petID == petsToSell[i].petID)
                        {
                            // Player already got the current pet to sell.
                            petsToSell[i] = null;

                            // TODO Next line is maybe wrong/useless, need check
                            RemovePetSellableButton(petsToSell[i]);

                            continue;
                        }
                    }
                }
            }

            sellerSet = true;
            UnActiveUI();
        }
        else
        {
            //Debug.Log("No Player_Pets instance.");
        }
    }

    // Method to know when mouse is over Pet Sellable button to display its description
    void IsMouseOverPetSellableButton()
    {
        PointerEventData pointerEventData = new PointerEventData(EventSystem.current);
        pointerEventData.position = Input.mousePosition;

        List<RaycastResult> raycastResultList = new List<RaycastResult>();
        EventSystem.current.RaycastAll(pointerEventData, raycastResultList);

        for (int i = 0; i < raycastResultList.Count; i++)
        {
            if (raycastResultList[i].gameObject.GetComponent<Pet_Sellable_Button>())
            {
                if (currentPetSellableButtonDescription != raycastResultList[i].gameObject.GetComponent<Pet_Sellable_Button>())
                {
                    // Display pet's description
                    currentPetSellableButtonDescription = raycastResultList[i].gameObject.GetComponent<Pet_Sellable_Button>();

                    DisplayPetInfo(currentPetSellableButtonDescription.petConfig);
                }
                return;
            }
        }

        if (currentPetSellableButtonDescription != null)
        {
            currentPetSellableButtonDescription = null;
            ResetPetInfo();
        }
    }

    // To unshow the UI.
    void UnActiveUI()
    {
        if (globalPanel.activeSelf)
            globalPanel.SetActive(false);

        if (petsList.activeSelf)
            petsList.SetActive(false);

        if (buyButton.gameObject.activeSelf)
        {
            buyButton.onClick.RemoveAllListeners();
            buyButton.gameObject.SetActive(false);
        }

        if (okButton.gameObject.activeSelf)
        {
            okButton.gameObject.SetActive(false);
        }

        if (backButton.gameObject.activeSelf)
        {
            backButton.gameObject.SetActive(false);
        }

        if (textDialogue.gameObject.activeSelf)
        {
            textDialogue.gameObject.SetActive(false);
        }

        if (informationsPanel.activeSelf)
            informationsPanel.SetActive(false);
    }

    // Method to show a dialogue when player hasnt unlocked pets
    void SetUIPreDialogue()
    {
        if (!globalPanel.gameObject.activeSelf)
            globalPanel.gameObject.SetActive(true);

        if (!textDialogue.gameObject.activeSelf)
        {
            textDialogue.text = "Oui ? On se connait ?";
            textDialogue.gameObject.SetActive(true);
        }

        if (!backButton.gameObject.activeSelf)
            backButton.gameObject.SetActive(true);
    }

    // Set the dialogue UI. First thing player see when interact with.
    void SetUIDialogue()
    {
        if (!globalPanel.gameObject.activeSelf)
            globalPanel.gameObject.SetActive(true);

        if (!textDialogue.gameObject.activeSelf)
        {
            textDialogue.text = dialogue;
            textDialogue.gameObject.SetActive(true);
        }

        if (!okButton.gameObject.activeSelf)
            okButton.gameObject.SetActive(true);

        if (!backButton.gameObject.activeSelf)
            backButton.gameObject.SetActive(true);
    }

    // Unshow text and ok button to display the list of pets
    void SetUIInventory()
    {
        if (textDialogue.gameObject.activeSelf)
            textDialogue.gameObject.SetActive(false);

        if (okButton.gameObject.activeSelf)
            okButton.gameObject.SetActive(false);

        if (!petsList.activeSelf)
            petsList.SetActive(true);
    }

    public override void Interact()
    {
        base.Interact();

        if (voice != NPC_VOICE.None)
        {
            if (Sound_Manager.instance)
            {
                Sound_Manager.instance.PlayNPCSound(voice, NPC_Interaction.Greetings);
            }
        }

        if (!sellerSet)
            SetSeller();

        if (Player_Pets.instance && !Player_Pets.instance.GetPetsUnlocked())
        {
            SetUIPreDialogue();
        }
        else
        {
            SetUIDialogue();
        }
    }

    public override void UnInteract()
    {
        base.UnInteract();

        if (globalPanel.activeSelf)
        {
            if (voice != NPC_VOICE.None)
            {
                if (Sound_Manager.instance)
                {
                    Sound_Manager.instance.PlayNPCSound(voice, NPC_Interaction.Farewell);
                }
            }
        }

        UnActiveUI();
    }

    // Method to display pet's informations when player put cursor on pet's button
    public void DisplayPetInfo(PetConfig config)
    {
        petImage.sprite = config.petSprite;

        petName.text = config.petName;

        // Pet Category
        if (config.petCategory == PetCategory.Cat)
        {
            petCategory.text = "Chat ";
        }
        else if (config.petCategory == PetCategory.Dog)
        {
            petCategory.text = "Chien ";
        }
        else if (config.petCategory == PetCategory.Alien)
        {
            petCategory.text = "Alien ";
        }

        // Pet sex (added to the category text)
        if (config.petSex == PetSex.Female)
        {
            petCategory.text += "Femelle";
        }
        else if (config.petSex == PetSex.Male)
        {
            petCategory.text += "Male";
        }
        else if (config.petSex == PetSex.Undefined)
        {
            petCategory.text += "Indéfini";
        }

        petWeight.text = config.petWeight.ToString() + " kg";

        petDescription.text = config.petDescription;

        petPrice.text = config.petPrice.ToString();

        if (!informationsPanel.activeSelf)
            informationsPanel.SetActive(true);
    }

    // To unshow informations panel
    public void ResetPetInfo()
    {
        if (informationsPanel.activeSelf)
            informationsPanel.SetActive(false);
    }

    // Method to add a pet sellable button and set its onClick event
    public void AddPetSellableButton(PetConfig linkedPet)
    {
        GameObject newPetButtonGO = Instantiate(petButtonPrefab, buttonsContainer);

        Pet_Sellable_Button newPetSellableButton = newPetButtonGO.GetComponent<Pet_Sellable_Button>();

        newPetSellableButton.petConfig = linkedPet;

        newPetSellableButton.petImage.sprite = linkedPet.petSprite;

        newPetButtonGO.GetComponent<Button>().onClick.AddListener(() => SetPetSellableButton(linkedPet));

    }

    // Method to configure a pet sellable button. Added on the pet sellable button event, when player click on it,
    // it configure the buy button to make player able to buy the pet linked when clicks on Buy button.
    public void SetPetSellableButton(PetConfig linkedPet)
    {
        buyButton.onClick.RemoveAllListeners();

        if (Player_Inventory.instance && Player_Pets.instance)
        {
            if (Player_Inventory.instance.GetPlayerGold() >= linkedPet.petPrice)
            {
                buyButton.onClick.AddListener(() => Player_Inventory.instance.SetPlayerGold(-Mathf.RoundToInt(linkedPet.petPrice)));
                buyButton.onClick.AddListener(() => Player_Pets.instance.GetNewPet(linkedPet));
                buyButton.onClick.AddListener(() => RemovePetSellableButton(linkedPet));
                buyButton.onClick.AddListener(() => Sound_Manager.instance.PlaySound(Sound_Manager.instance.asset.buy));
                buyButton.onClick.AddListener(buyButton.onClick.RemoveAllListeners);
                buyButton.onClick.AddListener(() => buyButton.gameObject.SetActive(false));

                // Success unlock part
                // Success 8 consist of buying 1 pet
                if (!Player_Success.instance.successDatabase.GetSuccessByID(8).isDone)
                {
                    buyButton.onClick.AddListener(() => Player_Success.instance.IncrementSuccessObjectiveByID(8));
                    buyButton.onClick.AddListener(() => Player_Success.instance.IncrementSuccessObjectiveByID(9)); // We want to increment the nine success aswell
                    // Because we dont care if the first pet bought was a cat or a dog.
                }
                else if (!Player_Success.instance.successDatabase.GetSuccessByID(9).isDone)
                {
                    // Here we want to accomplish the success 9 who means player must buy a cat and a dog.
                    // For do this we'll check if player got already a cat. If its not the case, he previously bought a dog.
                    // So we can check now if he buy the other category.
                    bool hasCat = false;

                    for (int i = 0; i < Player_Pets.playerPetsLength; i++)
                    {
                        if (Player_Pets.instance.GetPlayerPetByIndex(i).petCategory == PetCategory.Cat)
                        {
                            hasCat = true;
                            break;
                        }
                    }

                    if (hasCat)
                    {
                        if (linkedPet.petCategory == PetCategory.Dog)
                        {
                            buyButton.onClick.AddListener(() => Player_Success.instance.IncrementSuccessObjectiveByID(9));
                        }
                    }
                    else
                    {
                        if (linkedPet.petCategory == PetCategory.Cat)
                        {
                            buyButton.onClick.AddListener(() => Player_Success.instance.IncrementSuccessObjectiveByID(9));
                        }
                    }
                }
            }
            else
            {
                int amountNeeded = linkedPet.petPrice - Player_Inventory.instance.GetPlayerGold();
                buyButton.onClick.AddListener(() => UI_Player_Informations.instance.DisplayInformation("Il te manque " + amountNeeded.ToString() + " pieces !"));
            }
        }

        if (!buyButton.gameObject.activeSelf)
        {
            buyButton.gameObject.SetActive(true);
        }
    }

    // Method to remove a pet sellable button with a linked petconfig
    public void RemovePetSellableButton(PetConfig linkedPet)
    {
        for (int i = 0; i < petsToSell.Length; i++)
        {            
            if (linkedPet != null)
            {
                if (petsToSell[i] != null && petsToSell[i].petID == linkedPet.petID)
                {
                    // remove linked pet to sell
                    petsToSell[i] = null;

                    // Then remove the linked button
                    for (int j = 0; j < buttonsContainer.childCount; j++)
                    {
                        if (buttonsContainer.GetChild(j).GetComponent<Pet_Sellable_Button>())
                        {
                            if (buttonsContainer.GetChild(j).GetComponent<Pet_Sellable_Button>().petConfig.petID == linkedPet.petID)
                            {
                                Destroy(buttonsContainer.GetChild(j).gameObject);
                            }
                        }
                    }
                }
            }
        }
    }
}
