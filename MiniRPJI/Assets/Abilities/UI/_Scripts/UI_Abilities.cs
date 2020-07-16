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

    Ability_Config currentAbilityConfigDisplayed = null;

    [SerializeField] GameObject abilityDamageParent;
    [SerializeField] GameObject abilityHealthPointsParent;
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


    // Start is called before the first frame update
    void Start()
    {
        abilityInfoPanel.SetActive(false);
    }

    // Method to put on every AbilityTree button. /!\ The parameter to set is the ID of the LEVEL 1 ABILITY. A search at the beggin will define if player already got the next level of the ability.
    public void SetAbilityInformations(int abilityID)
    {
        currentAbilityConfigDisplayed = Player_Abilities.instance.abilitiesDatabase.GetAbilityByID(abilityID);

        // Check if player already has the ability, and if he's go the next level.
        Ability_Config[] abilityLevels = new Ability_Config[5];
        int abilityLevelsIndex = 0;

        // Get all levels of the ability in the abilityLevels array by checking abilitySubID.
        for (int i = 0; i < Player_Abilities.instance.abilitiesDatabase.abilities.Length; i++)
        {
            if (Player_Abilities.instance.abilitiesDatabase.abilities[i].abilitySubID == Player_Abilities.instance.abilitiesDatabase.GetAbilityByID(abilityID).abilitySubID)
            {
                abilityLevels[abilityLevelsIndex] = Player_Abilities.instance.abilitiesDatabase.abilities[i];
                abilityLevelsIndex++;
            }
        }

        // Check if player already unlock the ability by checking with all level ability ID if player got this ability ID


        // Set all UI element from currentAbilityConfigDisplayed's infos.


    }
}
