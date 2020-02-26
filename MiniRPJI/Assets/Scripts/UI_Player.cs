using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_Player : MonoBehaviour
{
    [SerializeField] Text currentLevel;

    [SerializeField] RectTransform healthLine; // We got rectTransform instead of image because its what we use to decrement health
    [SerializeField] RectTransform manaLine;
    [SerializeField] RectTransform expLine;

    [SerializeField] Text totalHealthpoints;
    [SerializeField] Text currentHealthpoints;
    [SerializeField] Text totalManapoints;
    [SerializeField] Text currentManapoints;
    [SerializeField] Text totalExppoints;
    [SerializeField] Text currentExppoints;

    [SerializeField] Image combatMode; // [0] = not in combat, [1] = in combat
    [SerializeField] Sprite noCombat; // when player isnt fighting
    [SerializeField] Sprite inCombat; // when player fighting

    // Because its child of player, we can get components by searching in parent
    Player_Stats playerStats; // To obtain life percentage for healthbar
    Player_Combat_Control player_combat; // To know when player is in combat or not.

    // Start is called before the first frame update
    void Start()
    {
        playerStats = GetComponentInParent<Player_Stats>();
        player_combat = GetComponentInParent<Player_Combat_Control>();
    }

    // Update is called once per frame
    void Update()
    {
        // Level
        if (currentLevel.text != playerStats.getCurrentLevel().ToString())
        {
            currentLevel.text = playerStats.getCurrentLevel().ToString();
        }
        // Healthbar
        if (totalHealthpoints.text != playerStats.getTotalHealthPoints().ToString())
        {
            totalHealthpoints.text = playerStats.getTotalHealthPoints().ToString();
        }
        if (currentHealthpoints.text != playerStats.getCurrentHealthPoints().ToString())
        {
            currentHealthpoints.text = playerStats.getCurrentHealthPoints().ToString();
            healthLine.sizeDelta = new Vector2((playerStats.getCurrentHealthPoints() * 100) / playerStats.getTotalHealthPoints(), healthLine.sizeDelta.y);
        }
        // Manabar
        if (totalManapoints.text != playerStats.getTotalManaPoints().ToString())
        {
            totalManapoints.text = playerStats.getTotalManaPoints().ToString();
        }
        if (currentManapoints.text != playerStats.getCurrentManaPoints().ToString())
        {
            currentManapoints.text = playerStats.getCurrentManaPoints().ToString();
            manaLine.sizeDelta = new Vector2((playerStats.getCurrentManaPoints() * 100) / playerStats.getTotalManaPoints(), manaLine.sizeDelta.y);
        }
        // Exp bar
        if (totalExppoints.text != playerStats.getTotalExp().ToString())
        {
            totalExppoints.text = playerStats.getTotalExp().ToString();
        }
        if (currentExppoints.text != playerStats.getCurrentExp().ToString())
        {
            currentExppoints.text = playerStats.getCurrentExp().ToString();
            if (playerStats.getCurrentExp() <= 0) // security if current xp is 0
            {
                expLine.sizeDelta = new Vector2(0f, expLine.sizeDelta.y);
            }
            else
            {
                expLine.sizeDelta = new Vector2((playerStats.getCurrentExp() * 100) / playerStats.getTotalExp(), expLine.sizeDelta.y);
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
