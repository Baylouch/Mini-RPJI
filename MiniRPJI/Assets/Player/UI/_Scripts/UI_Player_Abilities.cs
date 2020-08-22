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

    bool primaryAbilityDisplaying = false; // Just to know if its primary or secondary ability to set shortcut in the right array in Player_Abilities.cs

    // Start is called before the first frame update
    void Start()
    {
        ResetAbilitiesPanel();
    }

    private void Update()
    {
        IsMouseOverAbilityButton();

        if (Player_Shortcuts.GetShortCuts() == 0)
        {
            // Process ZQSD Shortcuts
            ProcessZQSDShortcuts();
        }
        else
        {
            // Process Arrows Shortcuts
            ProcessARROWSShortcuts();
        }
    }

    void ProcessZQSDShortcuts()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            // If we're displaying abilities description, we want to change the bind of the ability
            if (currentAbilityDescription)
            {
                SetCurrentAbilityShortCut("1");
            }
            else // Else we want to use the bind, so change the current ability to the one we binded later on.
            {
                UseAbilityShortCut("1");
            }
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            // If we're displaying abilities description, we want to change the bind of the ability
            if (currentAbilityDescription)
            {
                SetCurrentAbilityShortCut("2");
            }
            else // Else we want to use the bind, so change the current ability to the one we binded later on.
            {
                UseAbilityShortCut("2");
            }
        }
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            // If we're displaying abilities description, we want to change the bind of the ability
            if (currentAbilityDescription)
            {
                SetCurrentAbilityShortCut("3");
            }
            else // Else we want to use the bind, so change the current ability to the one we binded later on.
            {
                UseAbilityShortCut("3");
            }
        }
        if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            // If we're displaying abilities description, we want to change the bind of the ability
            if (currentAbilityDescription)
            {
                SetCurrentAbilityShortCut("4");
            }
            else // Else we want to use the bind, so change the current ability to the one we binded later on.
            {
                UseAbilityShortCut("4");
            }
        }
        if (Input.GetKeyDown(KeyCode.Alpha5))
        {
            // If we're displaying abilities description, we want to change the bind of the ability
            if (currentAbilityDescription)
            {
                SetCurrentAbilityShortCut("5");
            }
            else // Else we want to use the bind, so change the current ability to the one we binded later on.
            {
                UseAbilityShortCut("5");
            }
        }
    }

    void ProcessARROWSShortcuts()
    {
        if (Input.GetKeyDown(KeyCode.Keypad1))
        {
            // If we're displaying abilities description, we want to change the bind of the ability
            if (currentAbilityDescription)
            {
                SetCurrentAbilityShortCut("1");
            }
            else // Else we want to use the bind, so change the current ability to the one we binded later on.
            {
                UseAbilityShortCut("1");
            }
        }
        if (Input.GetKeyDown(KeyCode.Keypad2))
        {
            // If we're displaying abilities description, we want to change the bind of the ability
            if (currentAbilityDescription)
            {
                SetCurrentAbilityShortCut("2");
            }
            else // Else we want to use the bind, so change the current ability to the one we binded later on.
            {
                UseAbilityShortCut("2");
            }
        }
        if (Input.GetKeyDown(KeyCode.Keypad3))
        {
            // If we're displaying abilities description, we want to change the bind of the ability
            if (currentAbilityDescription)
            {
                SetCurrentAbilityShortCut("3");
            }
            else // Else we want to use the bind, so change the current ability to the one we binded later on.
            {
                UseAbilityShortCut("3");
            }
        }
        if (Input.GetKeyDown(KeyCode.Keypad4))
        {
            // If we're displaying abilities description, we want to change the bind of the ability
            if (currentAbilityDescription)
            {
                SetCurrentAbilityShortCut("4");
            }
            else // Else we want to use the bind, so change the current ability to the one we binded later on.
            {
                UseAbilityShortCut("4");
            }
        }
        if (Input.GetKeyDown(KeyCode.Keypad5))
        {
            // If we're displaying abilities description, we want to change the bind of the ability
            if (currentAbilityDescription)
            {
                SetCurrentAbilityShortCut("5");
            }
            else // Else we want to use the bind, so change the current ability to the one we binded later on.
            {
                UseAbilityShortCut("5");
            }
        }
    }

    void SetCurrentAbilityShortCut(string _shortCut)
    {
        Ability_Button[] abilityButtons = FindObjectsOfType<Ability_Button>();

        for (int i = 0; i < abilityButtons.Length; i++)
        {
            for (int j = 0; j < Player_Abilities.totalPlayerAbilities; j++)
            {
                if (Player_Abilities.instance.GetPlayerAbilityConfig(j) != null && abilityButtons[i].abilityID == Player_Abilities.instance.GetPlayerAbilityConfig(j).abilityID)
                {
                    if (Player_Abilities.instance.GetPrimaryAbilityShortCutByIndex(j) == _shortCut)
                    {
                        abilityButtons[i].SetInput("");
                        break;
                    }
                    if (Player_Abilities.instance.GetSecondaryAbilityShortCutByIndex(j) == _shortCut)
                    {
                        abilityButtons[i].SetInput("");
                        break;
                    }
                }
            }
        }

        if (primaryAbilityDisplaying)
        {
            Player_Abilities.instance.SetAbilityShortCut(_shortCut, currentAbilityDescription.abilityID, true);
        }
        else
        {
            Player_Abilities.instance.SetAbilityShortCut(_shortCut, currentAbilityDescription.abilityID, false);
        }

        currentAbilityDescription.SetInput(_shortCut);
    }

    void UseAbilityShortCut(string _shortCut)
    {
        for (int i = 0; i < Player_Abilities.totalPlayerAbilities; i++)
        {
            if (Player_Abilities.instance.GetPrimaryAbilityShortCutByIndex(i) == _shortCut)
            {
                if (Player_Abilities.instance.GetPlayerAbilityConfig(i) != null)
                {
                    ChangeAbility(Player_Abilities.instance.GetPlayerAbilityConfig(i).abilityID, 0);
                }

                return;
            }

            if (Player_Abilities.instance.GetSecondaryAbilityShortCutByIndex(i) == _shortCut)
            {
                if (Player_Abilities.instance.GetPlayerAbilityConfig(i) != null)
                {
                    ChangeAbility(Player_Abilities.instance.GetPlayerAbilityConfig(i).abilityID, 1);
                }

                return;
            }
        }
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
        // We first need to unactive potions panel because its using the same spot on the screen.
        if (UI_Player_Potions.instance)
        {
            UI_Player_Potions.instance.ResetPotionsPanel();
        }

        // Check if we are not already displaying abilities
        if (!displayingAbilities)
        {
            displayingAbilities = true;

            // We need to check if we have acces to player abilities
            if (Player_Abilities.instance)
            {
                // Set primaryAbilityDisplaying for shortCuts input
                if (indexAttack == 0)
                    primaryAbilityDisplaying = true;
                else
                    primaryAbilityDisplaying = false;

                // Now get available abilities
                Ability_Config[] abilitiesAvailable = new Ability_Config[Player_Abilities.totalPlayerAbilities];
                int currentIndex = 0;

                for (int i = 0; i < Player_Abilities.totalPlayerAbilities; i++)
                {
                    if (Player_Abilities.instance.GetPlayerAbilityConfig(i) != null)
                    {
                        abilitiesAvailable[currentIndex] = Player_Abilities.instance.GetPlayerAbilityConfig(i);
                        currentIndex++;
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
                    // If we're at the end of player abilities (because the array chechked is initialize by 10 -> the max ability player can hold).
                    if (abilitiesAvailable[i] == null)
                        break;

                    // /!\ Because of UNITY issue, we must create a new int to store the value of i. If not, onClick.AddListener() will not WORK /!\
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

                    // Set the ability's shortcut
                    if (indexAttack == 0)
                    {
                        currentAbilityButton.SetInput(Player_Abilities.instance.GetPrimaryAbilityShortCutByAbilityID(currentAbilityButton.abilityID));
                    }
                    else if (indexAttack == 1)
                    {
                        currentAbilityButton.SetInput(Player_Abilities.instance.GetSecondaryAbilityShortCutByAbilityID(currentAbilityButton.abilityID));
                    }
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

            Player_Abilities.instance.SetPrimaryAbility(newAbility);
        }
        else if (indexAttack == 1) // if its the secondary ability
        {
            secondaryAbilityImage.sprite = newAbility.abilitySprite;

            Player_Abilities.instance.SetSecondaryAbility(newAbility);
        }
        else
        {
            // There is an error with indexAttack parameter
            Debug.Log("ChangeAbility indexAttack from UI_Player_Abilities isnt right. Please fix.");
        }

        ResetAbilitiesPanel();
    }
}
