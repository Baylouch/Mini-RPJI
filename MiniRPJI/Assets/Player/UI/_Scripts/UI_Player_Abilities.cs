/* UI_Player_Abilities.cs
 * 
 * Attaché au top hierarchie de l'UI du player (sur le meme gameobject que UI_Player)
 * 
 * Permet d'afficher les abilités disponibles lorsque le joueur clic sur les boutons d'attaque primaire ou secondaire
 * 
 * 
 * */

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class UI_Player_Abilities : MonoBehaviour
{
    // We make this script singleton to have an easy acces from Game_Data_Control and UI_Player_Potions (to unshow ability panel)
    public static UI_Player_Abilities instance;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(this); // Should never happen
        }
    }

    // To change image relatively of the current abilities using.
    public Image primaryAbilityImage;
    public Image secondaryAbilityImage;

    [SerializeField] RectTransform AbilitiesPanel; // The RectTransform who'll be parent of ability's buttons
    // AbilitiesPanel need to upgrade its width by 30 for each ability button instantiate.

    // abilityButtonPrefab are instantiate for each ability's available.
    // /!\ Must have a Ability_Button script on it. /!\
    [SerializeField] GameObject abilityButtonPrefab; // Spawn as a child of AbilitiesPanel, the first is spawn with 15 on x pos, then for all next buttons, increment x pos by 30.

    // Ability description variables
    [SerializeField] GameObject abilityDescriptionPanelPrefab; // The panel who contain ability's name, description and cost

    GameObject currentDescriptionPanel; // To register an instantiate gameobject and manipulate it.

    Ability_Button currentAbilityDescription; // To know the current ability description

    bool displayingAbilities = false; // To know when player is displaying abilities.

    // Start is called before the first frame update
    void Start()
    {
        ResetAbilitiesPanel();
    }

    private void Update()
    {
        IsMouseOverAbilityButton();
    }

    // Method to reset abilities panel
    public void ResetAbilitiesPanel()
    {
        // Check if there is children (abilities buttons), if there are destroy them.
        if (AbilitiesPanel.childCount > 0)
        {
            for (int i = 0; i < AbilitiesPanel.childCount; i++)
            {
                Destroy(AbilitiesPanel.GetChild(i).gameObject);
            }
        }

        // Reset abilitiesPanel width (x = 0, y = 33).
        AbilitiesPanel.sizeDelta = new Vector2(0f, 33f); // Height is always 33f.

        if (AbilitiesPanel.gameObject.activeSelf)
        {
            AbilitiesPanel.gameObject.SetActive(false);
        }

        displayingAbilities = false;
    }

    // Method to know when mouse is over Ability button to display its description
    void IsMouseOverAbilityButton()
    {
        PointerEventData pointerEventData = new PointerEventData(EventSystem.current);
        pointerEventData.position = Input.mousePosition;

        List<RaycastResult> raycastResultList = new List<RaycastResult>();
        EventSystem.current.RaycastAll(pointerEventData, raycastResultList);

        for (int i = 0; i < raycastResultList.Count; i++)
        {
            if (raycastResultList[i].gameObject.GetComponent<Ability_Button>())
            {
                if (currentAbilityDescription != raycastResultList[i].gameObject.GetComponent<Ability_Button>())
                {
                    // Display ability's description
                    currentAbilityDescription = raycastResultList[i].gameObject.GetComponent<Ability_Button>();

                    DisplayAbilityDescription(currentAbilityDescription.abilityID);
                }
                return;
            }
        }

        // If we're here mouse isn't over Ability_Button so destroy abilityDescriptionPanel if there is one
        if (currentDescriptionPanel)
        {
            Destroy(currentDescriptionPanel);
            currentAbilityDescription = null;
        }            
    }

    void DisplayAbilityDescription(int _abilityID)
    {
        Ability_Config abilityToDisplay = Player_Abilities.instance.abilitiesDatabase.GetAbilityByID(_abilityID);

        if (abilityToDisplay != null)
        {
            if (currentDescriptionPanel == null)
            {
                currentDescriptionPanel = Instantiate(abilityDescriptionPanelPrefab);

                currentDescriptionPanel.transform.parent = AbilitiesPanel.transform;
                currentDescriptionPanel.transform.localScale = Vector3.one;
                currentDescriptionPanel.transform.localPosition = new Vector2(0f, 58f);

                // We know the first child of abilityDescriptionPanel is the name
                // second child is the description
                // and third (last child) is the energy cost

                currentDescriptionPanel.transform.GetChild(0).GetComponent<Text>().text = abilityToDisplay.abilityName;
                currentDescriptionPanel.transform.GetChild(1).GetComponent<Text>().text = abilityToDisplay.abilityShortDescription;
                currentDescriptionPanel.transform.GetChild(2).GetComponent<Text>().text = "Energie : " + abilityToDisplay.energyCost;


            }
            else
            {
                currentDescriptionPanel.transform.GetChild(0).GetComponent<Text>().text = abilityToDisplay.abilityName;
                currentDescriptionPanel.transform.GetChild(1).GetComponent<Text>().text = abilityToDisplay.abilityShortDescription;
                currentDescriptionPanel.transform.GetChild(2).GetComponent<Text>().text = "Energie : " + abilityToDisplay.energyCost;
            }
        }
        else
        {
            Debug.Log("Wrong _abilityID in DisplayAbilityDescription() from UI_Player_Abilities.");
        }
    }

    // Method to display abilities available when player clic on the Primary or Secondary ability's UI.
    // indexAttack means : (0) Primary ability, (1) Secondary ability.
    public void DisplayAbilitiesAvailable(int indexAttack)
    {
        if (UI_Player_Potions.instance)
        {
            UI_Player_Potions.instance.ResetPotionsPanel();
        }

        if (!displayingAbilities)
        {
            displayingAbilities = true;

            // We need to check if we have acces to player abilities
            if (Player_Abilities.instance)
            {
                // Now get available abilities
                // for this we first create a bool array intialize with the number of total abilities (accessed by abilities database).
                // Array index = Ability's ID
                int statesArraySize = Player_Abilities.instance.abilitiesDatabase.abilities.Length;
                bool[] abilitiesStates = new bool[statesArraySize];

                for (int i = 0; i < statesArraySize; i++)
                {
                    abilitiesStates[i] = Player_Abilities.instance.GetUnlockAbility(i);
                }

                // Then we can create a Ability_Config array with size of available abilities and put it with their ID in
                int abilitiesArraySize = 0;
                for (int i = 0; i < abilitiesStates.Length; i++)
                {
                    if (abilitiesStates[i] == true)
                    {
                        abilitiesArraySize++;
                    }
                }

                Ability_Config[] abilitiesAvailable = new Ability_Config[abilitiesArraySize];
                int currentAbilityIndex = 0; // To know index of abilitiesAvailable array. Because we loop trough abilitiesStates who havn't the same size.

                for (int i = 0; i < abilitiesStates.Length; i++)
                {
                    if (abilitiesStates[i] == true)
                    {
                        abilitiesAvailable[currentAbilityIndex] = Player_Abilities.instance.abilitiesDatabase.GetAbilityByID(i);
                        currentAbilityIndex++;
                    }
                }

                // Now we got our array containing all available abilities. (abilitiesAvailable[])
                // We can start displaying UI and set interactions.

                // First display AbilitiesPanel
                if (!AbilitiesPanel.gameObject.activeSelf)
                {
                    AbilitiesPanel.gameObject.SetActive(true);
                }

                // Now for each abilities available, increase width of AbilitiesPanel by 30f, instantiate an ability button then set it.
                for (int i = 0; i < abilitiesAvailable.Length; i++)
                {
                    // /!\ Because of UNITY issue, we must create a new int to store value of i. If not, onClick,AddListener will not WORK /!\
                    int x = i;

                    AbilitiesPanel.sizeDelta = new Vector2(AbilitiesPanel.sizeDelta.x + 30f, 33f);

                    // Instantiate and set Parent
                    GameObject currentAbilityGO = Instantiate(abilityButtonPrefab);
                    currentAbilityGO.transform.parent = AbilitiesPanel.transform;
                    // And reset localScale
                    currentAbilityGO.transform.localScale = Vector3.one;

                    // Now set the currentAbilityButton
                    Ability_Button currentAbilityButton = currentAbilityGO.GetComponent<Ability_Button>();

                    // Set the onClick
                    currentAbilityButton.GetComponent<Button>().onClick.AddListener(() => ChangeAbility(abilitiesAvailable[x].abilityID, indexAttack));

                    // Set the ability's image
                    currentAbilityButton.abilityImage.sprite = abilitiesAvailable[i].abilitySprite;

                    // Set the ability's ID
                    currentAbilityButton.abilityID = abilitiesAvailable[i].abilityID;
                }
            }
        }
        else
        {
            ResetAbilitiesPanel();
        }       
    }

    // Method used on each Ability_Button from theirs onClick event to change primary or secondary ability (indexAttack same of the DisplayingAbilities method)
    public void ChangeAbility(int newAbilityID, int indexAttack)
    {
        Ability_Config newAbility = Player_Abilities.instance.abilitiesDatabase.GetAbilityByID(newAbilityID);

        if (indexAttack == 0) // if its the primary ability
        {
            primaryAbilityImage.sprite = newAbility.abilitySprite;

            Player_Abilities.instance.SetPrimaryAbility(newAbility.abilityID);
        }
        else if (indexAttack == 1) // if its the secondary ability
        {
            secondaryAbilityImage.sprite = newAbility.abilitySprite;

            Player_Abilities.instance.SetSecondaryAbility(newAbility.abilityID);
        }
        else
        {
            // There is an error with indexAttack parameter
            Debug.Log("ChangeAbility indexAttack from UI_Player_Abilities isnt right. Please fix.");
        }

        ResetAbilitiesPanel();
    }
}
