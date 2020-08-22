/* UI_Player_Pets.cs
 * 
 * Permet de gérer la partie UI des pets du joueur.
 * 
 * Répertorie les pets du joueur dans une liste de boutons dans une UI adaptée pour, lorsque le joueur passe le curseur sur un de ces boutons
 * les informations de l'animal sont affichées en dessous. Si le joueur clic sur un bouton l'animal est "invoqué". Si le joueur clic sur un bouton alors
 * qu'il a déjà l'animal, ce dernier est "renvoyé".
 * 
 * */

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class UI_Player_Pets : MonoBehaviour
{
    [SerializeField] GameObject petButtonPrefab; // Pet button to set when created with the right pet config -> Must have a Pet_Button component on it

    [SerializeField] RectTransform buttonsContainer; // The parent of each pet buttons.

    // Displaying stuff
    [SerializeField] GameObject informationsPanel; // Contains all descriptions stuff

    [SerializeField] Image petImage;
    [SerializeField] Text petName;
    [SerializeField] Text petCategory;
    [SerializeField] Text petWeight;
    [SerializeField] Text petDescription;
    [SerializeField] Text noPetText;

    Pet_Button currentPetButtonDescription;

    // Start is called before the first frame update
    void Start()
    {
        if (informationsPanel.activeSelf)
            informationsPanel.SetActive(false);

        // To active/unactive noPetText
        if (Player_Pets.instance)
        {
            if (!noPetText.gameObject.activeSelf)
                noPetText.gameObject.SetActive(true);

            for (int i = 0; i < Player_Pets.instance.petsDataBase.pets.Length; i++)
            {
                if (Player_Pets.instance.GetPlayerPetByIndex(i) != null)
                {
                    if (noPetText.gameObject.activeSelf)
                        noPetText.gameObject.SetActive(false);

                    AddPetButton(Player_Pets.instance.GetPlayerPetByIndex(i));
                }
            }


        }
    }

    void Update()
    {
        IsMouseOverPetButton();
    }

    // Method to know when mouse is over Pet button to display its description
    void IsMouseOverPetButton()
    {
        PointerEventData pointerEventData = new PointerEventData(EventSystem.current);
        pointerEventData.position = Input.mousePosition;

        List<RaycastResult> raycastResultList = new List<RaycastResult>();
        EventSystem.current.RaycastAll(pointerEventData, raycastResultList);

        for (int i = 0; i < raycastResultList.Count; i++)
        {
            if (raycastResultList[i].gameObject.GetComponent<Pet_Button>())
            {
                if (currentPetButtonDescription != raycastResultList[i].gameObject.GetComponent<Pet_Button>())
                {
                    // Display pet's description
                    currentPetButtonDescription = raycastResultList[i].gameObject.GetComponent<Pet_Button>();

                    DisplayPetInfo(currentPetButtonDescription.petConfig);
                }
                return;
            }
        }

        if (currentPetButtonDescription != null)
        {
            currentPetButtonDescription = null;
            ResetPetInfo();
        }
    }

    // Method to display pet information when player put cursor on pet's button
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

        if (!informationsPanel.activeSelf)
            informationsPanel.SetActive(true);
    }

    public void ResetPetInfo()
    {
        if (informationsPanel.activeSelf)
            informationsPanel.SetActive(false);
    }

    // Method to add a pet button when player gets new pet
    public void AddPetButton(PetConfig linkedPet)
    {
        GameObject newPetButtonGO = Instantiate(petButtonPrefab, buttonsContainer);

        Pet_Button newPetButton = newPetButtonGO.GetComponent<Pet_Button>();

        newPetButton.petConfig = linkedPet;
        newPetButton.petImage.sprite = linkedPet.petSprite;

        newPetButtonGO.GetComponent<Button>().onClick.AddListener(() => Player_Pets.instance.InvokePet(linkedPet));
  
    }

    // To set a pet button listener to return / invoke a chosen pet
    public void SetPetButtonListener(PetConfig linkedPet)
    {
        // Check if we got a button with the linked pet.
        // To do this we loop trough buttonsContainer childs who are pet buttons
        Button buttonToSet = null;

        for (int i = 0; i < buttonsContainer.childCount; i++)
        {
            if (buttonsContainer.GetChild(i).GetComponent<Pet_Button>())
            {
                if (buttonsContainer.GetChild(i).GetComponent<Pet_Button>().petConfig.petID == linkedPet.petID)
                {
                    // We found the right button
                    buttonToSet = buttonsContainer.GetChild(i).GetComponent<Button>();
                    break;
                }
            }
        }

        if (buttonToSet == null)
        {
            // We dont found the right button
            return;
        }

        buttonToSet.onClick.RemoveAllListeners();

        // Set buttonToSet onclick with the right listener
        if (Player_Pets.instance)
        {
            if (Player_Pets.instance.currentPlayerPet && Player_Pets.instance.currentPlayerPet.petID == linkedPet.petID)
            {
                buttonToSet.onClick.AddListener(() => Player_Pets.instance.ReturnPet());
            }
            else
            {
                buttonToSet.onClick.AddListener(() => Player_Pets.instance.InvokePet(linkedPet));
            }
        }
    }
}
