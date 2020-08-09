using UnityEngine;
using UnityEngine.UI;

public class UI_Player_Stats : MonoBehaviour
{
    [Header("Stats Texts")]
    [SerializeField] Text playerLevel;
    [SerializeField] Text currentExp;
    [SerializeField] Text nextLevelExp;

    [SerializeField] Text maxHealthPoints;
    [SerializeField] Text currentHealthPoints;
    [SerializeField] Text maxEnergyPoints;
    [SerializeField] Text currentEnergyPoints;
    [SerializeField] Text armor;

    [SerializeField] Text primaryAbilityText;
    [SerializeField] Text secondaryAbilityText;
    [SerializeField] Text primaryMinAttackDamage;
    [SerializeField] Text primaryMaxAttackDamage;
    [SerializeField] Text secondaryMinAttackDamage;
    [SerializeField] Text secondaryMaxAttackDamage;

    [SerializeField] Text strengthStatsPoints;
    [SerializeField] Text agilityStatsPoints;
    [SerializeField] Text vitalityStatsPoints;
    [SerializeField] Text intellectStatsPoints;

    [SerializeField] Text currentStatsPoints;

    [Header("Stats Buttons")]
    [SerializeField] Button strengthRemoveStatsButton;
    [SerializeField] Button agilityRemoveStatsButton;
    [SerializeField] Button vitalityRemoveStatsButton;
    [SerializeField] Button energyRemoveStatsButton;
    [SerializeField] Button strengthAddStatsButton;
    [SerializeField] Button agilityAddStatsButton;
    [SerializeField] Button vitalityAddStatsButton;
    [SerializeField] Button energyAddStatsButton;

    [SerializeField] Button statsCancel;
    [SerializeField] Button statsValidation;
    [SerializeField] Button quitButton;

    private int[] removableStatsPoints; // To know when player can remove points or not (an array for know exaclty what pts can be remove or not)

    private void Start()
    {
        removableStatsPoints = new int[4];
        for (int i = 0; i < removableStatsPoints.Length; i++)
        {
            removableStatsPoints[i] = 0;
        }

        RefreshStatsDisplay();

        if (quitButton)
        {
            quitButton.onClick.AddListener(() => UI_Player.instance.ToggleStatsMenu());
        }
    }

    void RefreshStatsButtons()
    {
        // If we got stats points, we can add in each stats so
        if (Player_Stats.instance.GetCurrentStatsPoints() > 0)
        {
            strengthAddStatsButton.gameObject.SetActive(true);
            agilityAddStatsButton.gameObject.SetActive(true);
            vitalityAddStatsButton.gameObject.SetActive(true);
            energyAddStatsButton.gameObject.SetActive(true);
        }
        else
        {
            strengthAddStatsButton.gameObject.SetActive(false);
            agilityAddStatsButton.gameObject.SetActive(false);
            vitalityAddStatsButton.gameObject.SetActive(false);
            energyAddStatsButton.gameObject.SetActive(false);
        }

        if (removableStatsPoints != null) // Idk why but if we don't test it, we'll get error at start of the game.
        {
            // Then if we got removable stats points, we need to test each stats for know what we can remove or not
            if (removableStatsPoints[0] > 0) // strength
            {
                strengthRemoveStatsButton.gameObject.SetActive(true);
            }
            else
            {
                strengthRemoveStatsButton.gameObject.SetActive(false);
            }
            if (removableStatsPoints[1] > 0) // agility
            {
                agilityRemoveStatsButton.gameObject.SetActive(true);
            }
            else
            {
                agilityRemoveStatsButton.gameObject.SetActive(false);
            }
            if (removableStatsPoints[2] > 0) // Vitality
            {
                vitalityRemoveStatsButton.gameObject.SetActive(true);
            }
            else
            {
                vitalityRemoveStatsButton.gameObject.SetActive(false);
            }
            if (removableStatsPoints[3] > 0) // Intellect
            {
                energyRemoveStatsButton.gameObject.SetActive(true);
            }
            else
            {
                energyRemoveStatsButton.gameObject.SetActive(false);
            }

            // Now we check if player use a stats points to display validation or cancel button
            for (int i = 0; i < removableStatsPoints.Length; i++)
            {
                if (removableStatsPoints[i] > 0)
                {
                    if (!statsCancel.gameObject.activeSelf)
                        statsCancel.gameObject.SetActive(true);
                    if (!statsValidation.gameObject.activeSelf)
                        statsValidation.gameObject.SetActive(true);

                    return;
                }
            }
        }       
    }

    // return a color linked to a projectile type to display for instance fire ability damage in red.
    Color SetAbilityDamageTextColor(ProjectileType type)
    {
        switch (type)
        {
            case ProjectileType.Electric:
                return Color.yellow;
            case ProjectileType.Fire:
                return Color.red;
            case ProjectileType.Frost:
                return Color.cyan;
            case ProjectileType.Normal:
                return Color.white;
            case ProjectileType.Poison:
                return Color.magenta;
            default:
                return Color.white;
        }
    }

    public void RefreshStatsDisplay()
    {
        if (!Player_Stats.instance)
            return;

        // Experience panel
        playerLevel.text = Player_Stats.instance.GetCurrentLevel().ToString();
        currentExp.text = Player_Stats.instance.GetCurrentExp().ToString();
        nextLevelExp.text = Player_Stats.instance.GetTotalLevelExp().ToString();

        // Health and armor panel
        maxHealthPoints.text = Player_Stats.instance.playerHealth.GetTotalHealthPoints().ToString();
        currentHealthPoints.text = Player_Stats.instance.playerHealth.GetCurrentHealthPoints().ToString();
        maxEnergyPoints.text = Player_Stats.instance.playerEnergy.GetTotalEnergyPoints().ToString();
        currentEnergyPoints.text = Player_Stats.instance.playerEnergy.GetCurrentEnergyPoints().ToString();
        armor.text = Player_Stats.instance.GetArmor().ToString();

        // Attack panel
        // Get acces to player's abilities, display abilities name and damage. Find way to get acces to projectile component to know what type is to show color text
        if (Player_Abilities.instance)
        {
            if (Player_Abilities.instance.GetPrimaryAbility() != null)
            {
                int minDamage = 0, maxDamage = 0;

                // A security if previous ability displayed was "Other" category
                primaryMinAttackDamage.enabled = true;
                primaryMaxAttackDamage.enabled = true;

                Ability_Config _primaryAbility = Player_Abilities.instance.GetPrimaryAbility();

                primaryAbilityText.text = _primaryAbility.abilityName;

                if (_primaryAbility.abilityType == AbilityType.Bow)
                {
                    Color damageTextsColor = Color.white;

                    if (_primaryAbility.abilityPrefab.GetComponent<Player_Projectile>())
                    {
                        damageTextsColor = SetAbilityDamageTextColor(_primaryAbility.abilityPrefab.GetComponent<Player_Projectile>().projectileType);
                    }
                    else
                    {
                        if (_primaryAbility.abilityPrefab.transform.childCount > 1)
                        {
                            if (_primaryAbility.abilityPrefab.transform.GetComponentInChildren<Player_Projectile>())
                            {
                                damageTextsColor = SetAbilityDamageTextColor(_primaryAbility.abilityPrefab.GetComponentInChildren<Player_Projectile>().projectileType);
                            }
                        }
                    }

                    primaryMinAttackDamage.color = damageTextsColor;
                    primaryMaxAttackDamage.color = damageTextsColor;

                    minDamage = Player_Stats.instance.GetCurrentRangedMinDamage() + _primaryAbility.abilityBonus;
                    maxDamage = Player_Stats.instance.GetCurrentRangedMaxDamage() + _primaryAbility.abilityBonus;

                    primaryMinAttackDamage.text = minDamage.ToString();
                    primaryMaxAttackDamage.text = maxDamage.ToString();
                }
                else if (_primaryAbility.abilityType == AbilityType.Punch)
                {
                    primaryMinAttackDamage.color = Color.white;
                    primaryMaxAttackDamage.color = Color.white;

                    minDamage = Player_Stats.instance.GetCurrentMinDamage() + _primaryAbility.abilityBonus;
                    maxDamage = Player_Stats.instance.GetCurrentMaxDamage() + _primaryAbility.abilityBonus;

                    primaryMinAttackDamage.text = minDamage.ToString();
                    primaryMaxAttackDamage.text = maxDamage.ToString();
                }
                else if (_primaryAbility.abilityType == AbilityType.Other)
                {
                    primaryMinAttackDamage.enabled = false;
                    primaryMaxAttackDamage.enabled = false;
                }
                else
                {
                    Debug.Log("Something goes wrong here. Ability must got an ability type set.");
                }
            }

            if (Player_Abilities.instance.GetSecondaryAbility() != null)
            {
                int minDamage = 0, maxDamage = 0;

                // A security if previous ability displayed was "Other" category
                secondaryMinAttackDamage.enabled = true;
                secondaryMaxAttackDamage.enabled = true;

                Ability_Config _secondaryAbility = Player_Abilities.instance.GetSecondaryAbility();

                secondaryAbilityText.text = _secondaryAbility.abilityName;

                if (_secondaryAbility.abilityType == AbilityType.Bow)
                {
                    Color damageTextsColor = Color.white;

                    if (_secondaryAbility.abilityPrefab.GetComponent<Player_Projectile>())
                    {
                        damageTextsColor = SetAbilityDamageTextColor(_secondaryAbility.abilityPrefab.GetComponent<Player_Projectile>().projectileType);
                    }
                    else
                    {
                        if (_secondaryAbility.abilityPrefab.transform.childCount > 1)
                        {
                            if (_secondaryAbility.abilityPrefab.transform.GetComponentInChildren<Player_Projectile>())
                            {
                                damageTextsColor = SetAbilityDamageTextColor(_secondaryAbility.abilityPrefab.GetComponentInChildren<Player_Projectile>().projectileType);
                            }
                        }
                    }

                    secondaryMinAttackDamage.color = damageTextsColor;
                    secondaryMaxAttackDamage.color = damageTextsColor;

                    minDamage = Player_Stats.instance.GetCurrentRangedMinDamage() + _secondaryAbility.abilityBonus;
                    maxDamage = Player_Stats.instance.GetCurrentRangedMaxDamage() + _secondaryAbility.abilityBonus;

                    secondaryMinAttackDamage.text = minDamage.ToString();
                    secondaryMaxAttackDamage.text = maxDamage.ToString();
                }
                else if (_secondaryAbility.abilityType == AbilityType.Punch)
                {
                    secondaryMinAttackDamage.color = Color.white;
                    secondaryMaxAttackDamage.color = Color.white;

                    minDamage = Player_Stats.instance.GetCurrentMinDamage() + _secondaryAbility.abilityBonus;
                    maxDamage = Player_Stats.instance.GetCurrentMaxDamage() + _secondaryAbility.abilityBonus;

                    secondaryMinAttackDamage.text = minDamage.ToString();
                    secondaryMaxAttackDamage.text = maxDamage.ToString();
                }
                else if (_secondaryAbility.abilityType == AbilityType.Other)
                {
                    secondaryMinAttackDamage.enabled = false;
                    secondaryMaxAttackDamage.enabled = false;
                }
                else
                {
                    Debug.Log("Something goes wrong here. Ability must got an ability type set.");
                }
            }
        }

        // Stats panel
        strengthStatsPoints.text = Player_Stats.instance.GetCurrentStatsByType(StatsType.STRENGTH).ToString();
        agilityStatsPoints.text = Player_Stats.instance.GetCurrentStatsByType(StatsType.AGILITY).ToString();
        vitalityStatsPoints.text = Player_Stats.instance.GetCurrentStatsByType(StatsType.VITALITY).ToString();
        intellectStatsPoints.text = Player_Stats.instance.GetCurrentStatsByType(StatsType.ENERGY).ToString();

        currentStatsPoints.text = Player_Stats.instance.GetCurrentStatsPoints().ToString();

        // Refresh stats buttons
        RefreshStatsButtons();
    }

    public void StatsValidation()
    {

        // Player has validate so no removable points
        for (int i = 0; i < removableStatsPoints.Length; i++)
        {
            removableStatsPoints[i] = 0;
        }
        // Hide validation&cancel buttons
        statsCancel.gameObject.SetActive(false);
        statsValidation.gameObject.SetActive(false);

        // track current stats
        Player_Stats.instance.TrackCurrentStats();
        Player_Stats.instance.RefreshPlayerStats();
    }

    public void StatsCancel()
    {
        for (int i = 0; i < removableStatsPoints.Length; i++)
        {
            if (removableStatsPoints[i] > 0)
            {
                Player_Stats.instance.AddCurrentStatsPoints(removableStatsPoints[i]);
                for (int j = 0; j < removableStatsPoints[i]; j++)
                {
                    Player_Stats.instance.DecrementStatsByType((StatsType)i); // While its enum, and we were strict on how we code, its ok to use index of an enum
                }
                removableStatsPoints[i] = 0;
            }
        }
        // Hide validation&cancel buttons
        statsValidation.gameObject.SetActive(false);
        statsCancel.gameObject.SetActive(false);
        Player_Stats.instance.RefreshPlayerStats();
    }

    // We can't use StatsType via OnClick from a UI button.
    // So we use int for choose stats : 0 = strength, 1 = agility, 2 = vitality, 3 = intellect
    public void AddStatsPoints(int statsType)
    {
        if (Player_Stats.instance.GetCurrentStatsPoints() > 0)
        {
            switch (statsType)
            {
                case 0:
                    Player_Stats.instance.IncrementStatsByType(StatsType.STRENGTH);
                    removableStatsPoints[0]++;
                    break;
                case 1:
                    Player_Stats.instance.IncrementStatsByType(StatsType.AGILITY);
                    removableStatsPoints[1]++;
                    break;
                case 2:
                    Player_Stats.instance.IncrementStatsByType(StatsType.VITALITY);
                    removableStatsPoints[2]++;
                    break;
                case 3:
                    Player_Stats.instance.IncrementStatsByType(StatsType.ENERGY);
                    removableStatsPoints[3]++;
                    break;
                default:
                    Debug.LogWarning("Don't know what stats to upgrade !");
                    return;
            }

            Player_Stats.instance.RemoveCurrentStatsPoints(1);
            Player_Stats.instance.RefreshPlayerStats();
        }
    }

    public void RemoveStatsPoints(int statsType)
    {
        switch(statsType)
        {
            case 0:
                if (removableStatsPoints[0] > 0)
                {
                    Player_Stats.instance.DecrementStatsByType(StatsType.STRENGTH);
                    removableStatsPoints[0]--;
                    Player_Stats.instance.AddCurrentStatsPoints(1);           
                }
                break;
            case 1:
                if (removableStatsPoints[1] > 0)
                {
                    Player_Stats.instance.DecrementStatsByType(StatsType.AGILITY);
                    removableStatsPoints[1]--;
                    Player_Stats.instance.AddCurrentStatsPoints(1);
                }
                break;
            case 2:
                if (removableStatsPoints[2] > 0)
                {
                    Player_Stats.instance.DecrementStatsByType(StatsType.VITALITY);
                    removableStatsPoints[2]--;
                    Player_Stats.instance.AddCurrentStatsPoints(1);
                }
                break;
            case 3:
                if (removableStatsPoints[3] > 0)
                {
                    Player_Stats.instance.DecrementStatsByType(StatsType.ENERGY);
                    removableStatsPoints[3]--;
                    Player_Stats.instance.AddCurrentStatsPoints(1);
                }
                break;
            default:
                Debug.LogWarning("Attention la valeur \"" + statsType + "\" n'est pas reconnu ");
                return;
        }

        Player_Stats.instance.RefreshPlayerStats();
    }
}
