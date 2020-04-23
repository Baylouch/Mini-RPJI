using UnityEngine;
using UnityEngine.UI;

public class UI_Player_Profil : MonoBehaviour
{
    [SerializeField] Text currentLevel;

    [SerializeField] Button statsAvailable;

    [SerializeField] RectTransform healthLine; // We got rectTransform instead of image because its what we use to decrement health
    [SerializeField] RectTransform energyLine;
    [SerializeField] RectTransform expLine;

    [SerializeField] Text totalHealthpoints;
    [SerializeField] Text currentHealthpoints;
    [SerializeField] Text totalEnergypoints;
    [SerializeField] Text currentEnergypoints;
    [SerializeField] Text totalExppoints;
    [SerializeField] Text currentExppoints;

    [SerializeField] Image combatMode; // [0] = not in combat, [1] = in combat
    [SerializeField] Sprite noCombat; // when player isnt fighting
    [SerializeField] Sprite inCombat; // when player fighting

    Player_Combat player_combat; // To know when player is in combat or not.

    // Start is called before the first frame update
    void Start()
    {
        player_combat = FindObjectOfType<Player_Combat>();
    }

    // Update is called once per frame
    void Update()
    {
        // Level
        if (currentLevel.text != Player_Stats.instance.GetCurrentLevel().ToString())
        {
            currentLevel.text = Player_Stats.instance.GetCurrentLevel().ToString();
        }
        // Stats available button
        if (Player_Stats.instance.GetCurrentStatsPoints() > 0)
        {
            if (!statsAvailable.gameObject.activeSelf)
            {
                statsAvailable.gameObject.SetActive(true);
            }
        }
        else
        {
            if (statsAvailable.gameObject.activeSelf)
            {
                statsAvailable.gameObject.SetActive(false);
            }
        }
        // Healthbar
        if (totalHealthpoints.text != Player_Stats.instance.playerHealth.GetTotalHealthPoints().ToString())
        {
            totalHealthpoints.text = Player_Stats.instance.playerHealth.GetTotalHealthPoints().ToString();
        }
        if (currentHealthpoints.text != Player_Stats.instance.playerHealth.GetCurrentHealthPoints().ToString())
        {
            currentHealthpoints.text = Player_Stats.instance.playerHealth.GetCurrentHealthPoints().ToString();
            healthLine.sizeDelta = new Vector2((Player_Stats.instance.playerHealth.GetCurrentHealthPoints() * 100) / Player_Stats.instance.playerHealth.GetTotalHealthPoints(), healthLine.sizeDelta.y);
        }
        // Manabar
        if (totalEnergypoints.text != Player_Stats.instance.playerEnergy.GetTotalEnergyPoints().ToString())
        {
            totalEnergypoints.text = Player_Stats.instance.playerEnergy.GetTotalEnergyPoints().ToString();
        }
        if (currentEnergypoints.text != Player_Stats.instance.playerEnergy.GetCurrentEnergyPoints().ToString())
        {
            currentEnergypoints.text = Player_Stats.instance.playerEnergy.GetCurrentEnergyPoints().ToString();
            energyLine.sizeDelta = new Vector2((Player_Stats.instance.playerEnergy.GetCurrentEnergyPoints() * 100) / Player_Stats.instance.playerEnergy.GetTotalEnergyPoints(), energyLine.sizeDelta.y);
        }
        // Exp bar
        if (totalExppoints.text != Player_Stats.instance.GetTotalLevelExp().ToString())
        {
            totalExppoints.text = Player_Stats.instance.GetTotalLevelExp().ToString();
        }
        if (currentExppoints.text != Player_Stats.instance.GetCurrentExp().ToString())
        {
            currentExppoints.text = Player_Stats.instance.GetCurrentExp().ToString();
            if (Player_Stats.instance.GetCurrentExp() <= 0) // security if current xp is 0
            {
                expLine.sizeDelta = new Vector2(0f, expLine.sizeDelta.y);
            }
            else
            {
                expLine.sizeDelta = new Vector2((Player_Stats.instance.GetCurrentExp() * 100) / Player_Stats.instance.GetTotalLevelExp(), expLine.sizeDelta.y);
            }
        }
        // In combat icon
        if (player_combat.isInCombat && combatMode.sprite != inCombat)
        {
            combatMode.sprite = inCombat;
        }
        else if (!player_combat.isInCombat && combatMode.sprite != noCombat)
        {
            combatMode.sprite = noCombat;
        }
    }
}
