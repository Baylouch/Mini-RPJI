using UnityEngine;
using UnityEngine.UI;

public class UI_Abilities : MonoBehaviour
{
    [SerializeField] GameObject abilityInfoPanel;

    [SerializeField] Button abilityButtonLevel1;
    [SerializeField] Button abilityButtonLevel2;
    [SerializeField] Button abilityButtonLevel3;
    [SerializeField] Button abilityButtonLevel4;
    [SerializeField] Button abilityButtonLevel5;

    Ability_Config currentAbilityConfigDisplayed = null; // To know what ability is displayed when player clic on TreeAbilityButton
    int currentAbilityIDDisplayed = -1; // Second variable to know what ability is already displayed when player clic on level buttons

    [SerializeField] GameObject abilityDamageParent;
    [SerializeField] GameObject abilityHealthPointsParent;
    [SerializeField] Text abilityLevelText;
    [SerializeField] Text abilityNameText;
    [SerializeField] Text abilityDamageText;    
    [SerializeField] Text abilityHealthPointsText;
    [SerializeField] Text abilityPowerText;
    [SerializeField] Text abilityTimerText;
    [SerializeField] Text abilityEnergyCostText;
    [SerializeField] Text abilityDescriptionText;
    [SerializeField] Text abilityPointsAvailableText;

    [SerializeField] Button validateButton;
    [SerializeField] Button cancelButton;

    [SerializeField] Button quitButton;


    // Start is called before the first frame update
    void Start()
    {
        abilityInfoPanel.SetActive(false);

        ResetAbilitiesInfos();

        cancelButton.onClick.AddListener(() => ResetAbilitiesInfos());

        RefreshAbilitiesUI();

        if (quitButton)
        {
            quitButton.onClick.AddListener(() => UI_Player.instance.TogglePlayerAbilitiesUI());
        }
    }

    void ResetAbilityLevelButtons()
    {
        abilityButtonLevel1.GetComponent<Image>().color = Color.grey;
        abilityButtonLevel1.onClick.RemoveAllListeners();
        abilityButtonLevel1.interactable = false;

        abilityButtonLevel2.GetComponent<Image>().color = Color.grey;
        abilityButtonLevel2.onClick.RemoveAllListeners();
        abilityButtonLevel2.interactable = false;

        abilityButtonLevel3.GetComponent<Image>().color = Color.grey;
        abilityButtonLevel3.onClick.RemoveAllListeners();
        abilityButtonLevel3.interactable = false;

        abilityButtonLevel4.GetComponent<Image>().color = Color.grey;
        abilityButtonLevel4.onClick.RemoveAllListeners();
        abilityButtonLevel4.interactable = false;

        abilityButtonLevel5.GetComponent<Image>().color = Color.grey;
        abilityButtonLevel5.onClick.RemoveAllListeners();
        abilityButtonLevel5.interactable = false;
    }

    void SetAbilityLevelButtons(int _currentAbilityLevel, Ability_Config[] _abilityLevels)
    {
        ResetAbilityLevelButtons();

        // Set ability levels buttons
        switch (_currentAbilityLevel)
        {
            case 0:
                abilityButtonLevel1.GetComponent<Image>().color = Color.white;
                for (int i = 0; i < _abilityLevels.Length; i++)
                {
                    if (_abilityLevels[i].abilityLevel == 1)
                    {
                        int x = i;
                        abilityButtonLevel1.onClick.AddListener(() => DisplayAbilityInformationsByID(_abilityLevels[x].abilityID));
                        break;
                    }
                }
                abilityButtonLevel1.interactable = true;
                break;
            case 1:
                abilityButtonLevel1.GetComponent<Image>().color = Color.green;
                for (int i = 0; i < _abilityLevels.Length; i++)
                {
                    if (_abilityLevels[i].abilityLevel == 1)
                    {
                        int x = i;
                        abilityButtonLevel1.onClick.AddListener(() => DisplayAbilityInformationsByID(_abilityLevels[x].abilityID));
                        break;
                    }
                }
                abilityButtonLevel1.interactable = true;

                abilityButtonLevel2.GetComponent<Image>().color = Color.white;
                for (int i = 0; i < _abilityLevels.Length; i++)
                {
                    if (_abilityLevels[i].abilityLevel == 2)
                    {
                        int x = i;
                        abilityButtonLevel2.onClick.AddListener(() => DisplayAbilityInformationsByID(_abilityLevels[x].abilityID));
                        break;
                    }
                }
                abilityButtonLevel2.interactable = true;
                break;
            case 2:
                abilityButtonLevel1.GetComponent<Image>().color = Color.green;
                for (int i = 0; i < _abilityLevels.Length; i++)
                {
                    if (_abilityLevels[i].abilityLevel == 1)
                    {
                        int x = i;
                        abilityButtonLevel1.onClick.AddListener(() => DisplayAbilityInformationsByID(_abilityLevels[x].abilityID));
                        break;
                    }
                }
                abilityButtonLevel1.interactable = true;

                abilityButtonLevel2.GetComponent<Image>().color = Color.green;
                for (int i = 0; i < _abilityLevels.Length; i++)
                {
                    if (_abilityLevels[i].abilityLevel == 2)
                    {
                        int x = i;
                        abilityButtonLevel2.onClick.AddListener(() => DisplayAbilityInformationsByID(_abilityLevels[x].abilityID));
                        break;
                    }
                }
                abilityButtonLevel2.interactable = true;

                abilityButtonLevel3.GetComponent<Image>().color = Color.white;
                for (int i = 0; i < _abilityLevels.Length; i++)
                {
                    if (_abilityLevels[i].abilityLevel == 3)
                    {
                        int x = i;
                        abilityButtonLevel3.onClick.AddListener(() => DisplayAbilityInformationsByID(_abilityLevels[x].abilityID));
                        break;
                    }
                }
                abilityButtonLevel3.interactable = true;
                break;
            case 3:
                abilityButtonLevel1.GetComponent<Image>().color = Color.green;
                for (int i = 0; i < _abilityLevels.Length; i++)
                {
                    if (_abilityLevels[i].abilityLevel == 1)
                    {
                        int x = i;
                        abilityButtonLevel1.onClick.AddListener(() => DisplayAbilityInformationsByID(_abilityLevels[x].abilityID));
                        break;
                    }
                }
                abilityButtonLevel1.interactable = true;

                abilityButtonLevel2.GetComponent<Image>().color = Color.green;
                for (int i = 0; i < _abilityLevels.Length; i++)
                {
                    if (_abilityLevels[i].abilityLevel == 2)
                    {
                        int x = i;
                        abilityButtonLevel2.onClick.AddListener(() => DisplayAbilityInformationsByID(_abilityLevels[x].abilityID));
                        break;
                    }
                }
                abilityButtonLevel2.interactable = true;

                abilityButtonLevel3.GetComponent<Image>().color = Color.green;
                for (int i = 0; i < _abilityLevels.Length; i++)
                {
                    if (_abilityLevels[i].abilityLevel == 3)
                    {
                        int x = i;
                        abilityButtonLevel3.onClick.AddListener(() => DisplayAbilityInformationsByID(_abilityLevels[x].abilityID));
                        break;
                    }
                }
                abilityButtonLevel3.interactable = true;

                abilityButtonLevel4.GetComponent<Image>().color = Color.white;
                for (int i = 0; i < _abilityLevels.Length; i++)
                {
                    if (_abilityLevels[i].abilityLevel == 4)
                    {
                        int x = i;
                        abilityButtonLevel4.onClick.AddListener(() => DisplayAbilityInformationsByID(_abilityLevels[x].abilityID));
                        break;
                    }
                }
                abilityButtonLevel4.interactable = true;
                break;
            case 4:
                abilityButtonLevel1.GetComponent<Image>().color = Color.green;
                for (int i = 0; i < _abilityLevels.Length; i++)
                {
                    if (_abilityLevels[i].abilityLevel == 1)
                    {
                        int x = i;
                        abilityButtonLevel1.onClick.AddListener(() => DisplayAbilityInformationsByID(_abilityLevels[x].abilityID));
                        break;
                    }
                }
                abilityButtonLevel1.interactable = true;

                abilityButtonLevel2.GetComponent<Image>().color = Color.green;
                for (int i = 0; i < _abilityLevels.Length; i++)
                {
                    if (_abilityLevels[i].abilityLevel == 2)
                    {
                        int x = i;
                        abilityButtonLevel2.onClick.AddListener(() => DisplayAbilityInformationsByID(_abilityLevels[x].abilityID));
                        break;
                    }
                }
                abilityButtonLevel2.interactable = true;

                abilityButtonLevel3.GetComponent<Image>().color = Color.green;
                for (int i = 0; i < _abilityLevels.Length; i++)
                {
                    if (_abilityLevels[i].abilityLevel == 3)
                    {
                        int x = i;
                        abilityButtonLevel3.onClick.AddListener(() => DisplayAbilityInformationsByID(_abilityLevels[x].abilityID));
                        break;
                    }
                }
                abilityButtonLevel3.interactable = true;

                abilityButtonLevel4.GetComponent<Image>().color = Color.green;
                for (int i = 0; i < _abilityLevels.Length; i++)
                {
                    if (_abilityLevels[i].abilityLevel == 4)
                    {
                        int x = i;
                        abilityButtonLevel4.onClick.AddListener(() => DisplayAbilityInformationsByID(_abilityLevels[x].abilityID));
                        break;
                    }
                }
                abilityButtonLevel4.interactable = true;

                abilityButtonLevel5.GetComponent<Image>().color = Color.white;
                for (int i = 0; i < _abilityLevels.Length; i++)
                {
                    if (_abilityLevels[i].abilityLevel == 5)
                    {
                        int x = i;
                        abilityButtonLevel5.onClick.AddListener(() => DisplayAbilityInformationsByID(_abilityLevels[x].abilityID));
                        break;
                    }
                }
                abilityButtonLevel5.interactable = true;
                break;
            case 5:
                abilityButtonLevel1.GetComponent<Image>().color = Color.green;
                for (int i = 0; i < _abilityLevels.Length; i++)
                {
                    if (_abilityLevels[i].abilityLevel == 1)
                    {
                        int x = i;
                        abilityButtonLevel1.onClick.AddListener(() => DisplayAbilityInformationsByID(_abilityLevels[x].abilityID));
                        break;
                    }
                }
                abilityButtonLevel1.interactable = true;

                abilityButtonLevel2.GetComponent<Image>().color = Color.green;
                for (int i = 0; i < _abilityLevels.Length; i++)
                {
                    if (_abilityLevels[i].abilityLevel == 2)
                    {
                        int x = i;
                        abilityButtonLevel2.onClick.AddListener(() => DisplayAbilityInformationsByID(_abilityLevels[x].abilityID));
                        break;
                    }
                }
                abilityButtonLevel2.interactable = true;

                abilityButtonLevel3.GetComponent<Image>().color = Color.green;
                for (int i = 0; i < _abilityLevels.Length; i++)
                {
                    if (_abilityLevels[i].abilityLevel == 3)
                    {
                        int x = i;
                        abilityButtonLevel3.onClick.AddListener(() => DisplayAbilityInformationsByID(_abilityLevels[x].abilityID));
                        break;
                    }
                }
                abilityButtonLevel3.interactable = true;

                abilityButtonLevel4.GetComponent<Image>().color = Color.green;
                for (int i = 0; i < _abilityLevels.Length; i++)
                {
                    if (_abilityLevels[i].abilityLevel == 4)
                    {
                        int x = i;
                        abilityButtonLevel4.onClick.AddListener(() => DisplayAbilityInformationsByID(_abilityLevels[x].abilityID));
                        break;
                    }
                }
                abilityButtonLevel4.interactable = true;

                abilityButtonLevel5.GetComponent<Image>().color = Color.green;
                for (int i = 0; i < _abilityLevels.Length; i++)
                {
                    if (_abilityLevels[i].abilityLevel == 5)
                    {
                        int x = i;
                        abilityButtonLevel5.onClick.AddListener(() => DisplayAbilityInformationsByID(_abilityLevels[x].abilityID));
                        break;
                    }
                }
                abilityButtonLevel5.interactable = true;
                break;
            default:
                Debug.Log("Don't know what ability to display. Wront ability level.");
                return;
        }
    }

    public void RefreshAbilitiesUI()
    {
        AbilityTree_Button[] abilityTree_Buttons = GetComponentsInChildren<AbilityTree_Button>();

        foreach (AbilityTree_Button abilityTreeButton in abilityTree_Buttons)
        {
            abilityTreeButton.RefreshButton();
        }

        if (Player_Abilities.instance)
            abilityPointsAvailableText.text = Player_Abilities.instance.GetAbilityPoints().ToString();
    }

    public void ResetAbilitiesInfos()
    {
        ResetAbilityLevelButtons();

        validateButton.onClick.RemoveAllListeners();
        validateButton.gameObject.SetActive(false);

        currentAbilityConfigDisplayed = null;
        currentAbilityIDDisplayed = -1;

        abilityInfoPanel.SetActive(false);
    }

    // Method to put on every AbilityTree button. /!\ The parameter to set is the sub ID who regroup abilities level.
    public void SetAbilityInformations(int _abilitySubID)
    {
        // Check in player abilities array if he already got the ability. If yes, get what level of the ability.
        int currentAbilityLevel = 0;

        for (int i = 0; i < Player_Abilities.totalPlayerAbilities; i++)
        {
            if (Player_Abilities.instance.GetPlayerAbilityConfig(i) != null && Player_Abilities.instance.GetPlayerAbilityConfig(i).abilitySubID == _abilitySubID)
            {
                // If we are not displaying this ability
                if (currentAbilityConfigDisplayed != Player_Abilities.instance.GetPlayerAbilityConfig(i))
                {
                    currentAbilityConfigDisplayed = Player_Abilities.instance.GetPlayerAbilityConfig(i);
                    currentAbilityLevel = Player_Abilities.instance.GetPlayerAbilityConfig(i).abilityLevel;
                    currentAbilityIDDisplayed = -1;
                    break;
                }
                else // We can just return because we're already displaying this ability
                {
                    //Debug.Log("Ability already displayed.");
                    return;
                }
            }
        }

        // If currentAbilityLevel still 0 here, we must set currentAbilityConfigDisplayed as the level 1 ability.
        if (currentAbilityLevel == 0)
        {
            for (int i = 0; i < Player_Abilities.instance.abilitiesDatabase.abilities.Length; i++)
            {
                if (Player_Abilities.instance.abilitiesDatabase.abilities[i].abilitySubID == _abilitySubID)
                {
                    if (Player_Abilities.instance.abilitiesDatabase.abilities[i].abilityLevel == 1)
                    {
                        currentAbilityConfigDisplayed = Player_Abilities.instance.abilitiesDatabase.abilities[i];
                        currentAbilityIDDisplayed = -1;
                        break;
                    }
                }
            }
        }

        // We need all levels of the ability in a array. We know all abilities got 5 levels.
        Ability_Config[] abilityLevels = new Ability_Config[5];
        int abilityLevelsIndex = 0;

        // Get all levels of the ability in the abilityLevels array by checking abilitySubID.
        for (int i = 0; i < Player_Abilities.instance.abilitiesDatabase.abilities.Length; i++)
        {
            if (Player_Abilities.instance.abilitiesDatabase.abilities[i].abilitySubID == _abilitySubID)
            {
                abilityLevels[abilityLevelsIndex] = Player_Abilities.instance.abilitiesDatabase.abilities[i];
                abilityLevelsIndex++;
            }
        }

        // Set ability level buttons
        SetAbilityLevelButtons(currentAbilityLevel, abilityLevels);

        // Now we can simply display the informations of the level of the ability
        if (currentAbilityLevel == 0)
        {
            for (int i = 0; i < abilityLevels.Length; i++)
            {
                if (abilityLevels[i].abilityLevel == 1)
                {
                    DisplayAbilityInformationsByID(abilityLevels[i].abilityID);
                    break;
                }
            }
        }
        else
        {
            for (int i = 0; i < abilityLevels.Length; i++)
            {
                if (abilityLevels[i].abilityLevel == currentAbilityLevel)
                {
                    DisplayAbilityInformationsByID(abilityLevels[i].abilityID);
                    break;
                }
            }
        }

        // To end this method, we finally need to set the validationButton.
        // This one is accessible only if player show the next level ability informations.


        abilityInfoPanel.SetActive(true);
    }

    // Because we need an other way to display information via levels button. Directly by ID seems perfect.
    public void DisplayAbilityInformationsByID(int abilityID)
    {
        if (Player_Abilities.instance.abilitiesDatabase.GetAbilityByID(abilityID) != null)
        {
            if (currentAbilityIDDisplayed != abilityID)
            {
                currentAbilityIDDisplayed = abilityID;

                Ability_Config abilityToDisplay = Player_Abilities.instance.abilitiesDatabase.GetAbilityByID(abilityID);

                // Display informations
                //Debug.Log("Displaying ability's ID " + abilityID + " informations.");

                abilityLevelText.text = abilityToDisplay.abilityLevel.ToString();

                abilityNameText.text = abilityToDisplay.abilityName;

                abilityPowerText.text = abilityToDisplay.abilityPower.ToString();

                abilityTimerText.text = abilityToDisplay.abilityTimer.ToString();

                abilityEnergyCostText.text = abilityToDisplay.energyCost.ToString();

                abilityDescriptionText.text = abilityToDisplay.abilityLongDescription;

                // Special condition if we display the decoy ability (SubID 7 family), we need to switch damage text to healthpoints text.
                if (abilityToDisplay.abilitySubID == 7)
                {
                    abilityDamageParent.SetActive(false);
                    abilityHealthPointsParent.SetActive(true);

                    int decoyHealthPoints = Player_Stats.instance.playerHealth.GetTotalHealthPoints() + abilityToDisplay.abilityBonus;

                    abilityHealthPointsText.text = decoyHealthPoints.ToString();
                }
                else
                {
                    abilityDamageParent.SetActive(true);
                    abilityHealthPointsParent.SetActive(false);

                    abilityDamageText.text = abilityToDisplay.abilityBonus.ToString();
                }

                // Set Validate button
                // we must check if the level of the player - the level required + the current level of the ability - 1 
                // (because we want to be able to get an ability level 10 when we're level 10 for instance) still greater or equal to 0.
                if (Player_Stats.instance.GetCurrentLevel() - (abilityToDisplay.levelRequired + (abilityToDisplay.abilityLevel - 1)) >= 0)
                {
                    if (Player_Abilities.instance.GetAbilityPoints() > 0)
                    {
                        validateButton.onClick.RemoveAllListeners();
                        validateButton.gameObject.SetActive(false);

                        bool validateButtonSet = false;
                        // Now we need to check if player already unlocked this level of the ability.
                        for (int i = 0; i < Player_Abilities.totalPlayerAbilities; i++)
                        {
                            if (Player_Abilities.instance.GetPlayerAbilityConfig(i) != null && Player_Abilities.instance.GetPlayerAbilityConfig(i).abilitySubID == abilityToDisplay.abilitySubID)
                            {
                                if (abilityToDisplay.abilityLevel > Player_Abilities.instance.GetPlayerAbilityConfig(i).abilityLevel)
                                {
                                    validateButton.onClick.AddListener(() => GetNewAbility(abilityToDisplay));
                                    validateButton.onClick.AddListener(() => UI_Player_Abilities.instance.ResetAbilitiesPanel());
                                    validateButton.onClick.AddListener(() => ResetAbilitiesInfos());
                                    validateButton.gameObject.SetActive(true);
                                    validateButtonSet = true;                                  
                                    break;
                                }
                                else
                                {
                                    validateButtonSet = true;
                                    break;
                                }
                            }
                        }

                        // Player hasn't the ability yet
                        if (!validateButtonSet)
                        {
                            validateButton.onClick.AddListener(() => GetNewAbility(abilityToDisplay));
                            validateButton.onClick.AddListener(() => UI_Player_Abilities.instance.ResetAbilitiesPanel());
                            validateButton.onClick.AddListener(() => ResetAbilitiesInfos());
                            validateButton.gameObject.SetActive(true);
                            validateButtonSet = true;
                        }
                    }
                }

            }
            else
            {
                Debug.Log("Ability already displayed.");
            }           
        }
    }

    // Method used on validate button to get the next ability's level.
    public void GetNewAbility(Ability_Config newAbility)
    {
        bool isSet = false;

        // Check if player has the level under the new ability to delete it and remplace with newAbility
        for (int i = 0; i < Player_Abilities.totalPlayerAbilities; i++)
        {
            if (Player_Abilities.instance.GetPlayerAbilityConfig(i) != null)
            {
                if (Player_Abilities.instance.GetPlayerAbilityConfig(i).abilitySubID == newAbility.abilitySubID)
                {
                    Player_Abilities.instance.SetPlayerAbilityConfig(i, newAbility);

                    // Last step is : maybe player put this ability on its primary or second ability. So we must check to change to the new ability
                    if (Player_Abilities.instance.GetPrimaryAbility().abilitySubID == newAbility.abilitySubID)
                    {
                        Player_Abilities.instance.SetPrimaryAbility(newAbility);
                    }
                    else if (Player_Abilities.instance.GetSecondaryAbility().abilitySubID == newAbility.abilitySubID)
                    {
                        Player_Abilities.instance.SetSecondaryAbility(newAbility);
                    }

                    isSet = true;
                    break;
                }
            }
        }

        if (!isSet)
        {
            for (int i = 0; i < Player_Abilities.totalPlayerAbilities; i++)
            {
                if (Player_Abilities.instance.GetPlayerAbilityConfig(i) == null)
                {
                    Player_Abilities.instance.SetPlayerAbilityConfig(i, newAbility);
                    isSet = true;
                    break;            
                }
            }
        }

        Player_Abilities.instance.SetAbilityPoints(Player_Abilities.instance.GetAbilityPoints() - 1);
        abilityPointsAvailableText.text = Player_Abilities.instance.GetAbilityPoints().ToString();
    }
}
