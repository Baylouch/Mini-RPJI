using UnityEngine;
using UnityEngine.UI;

public class UI_Player : MonoBehaviour
{
    public static UI_Player uiPlayerInstance;

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

    Player_Combat_Control player_combat; // To know when player is in combat or not.

    private void Awake()
    {
        if (uiPlayerInstance == null)
        {
            uiPlayerInstance = this;
            DontDestroyOnLoad(gameObject); // We make this because its safer for keep Player_Inventory between scenes (who is children of this)
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        player_combat = FindObjectOfType<Player_Combat_Control>();
    }

    // Update is called once per frame
    void Update()
    {
        // Level
        if (currentLevel.text != Player_Stats.stats_instance.getCurrentLevel().ToString())
        {
            currentLevel.text = Player_Stats.stats_instance.getCurrentLevel().ToString();
        }
        // Healthbar
        if (totalHealthpoints.text != Player_Stats.stats_instance.getTotalHealthPoints().ToString())
        {
            totalHealthpoints.text = Player_Stats.stats_instance.getTotalHealthPoints().ToString();
        }
        if (currentHealthpoints.text != Player_Stats.stats_instance.getCurrentHealthPoints().ToString())
        {
            currentHealthpoints.text = Player_Stats.stats_instance.getCurrentHealthPoints().ToString();
            healthLine.sizeDelta = new Vector2((Player_Stats.stats_instance.getCurrentHealthPoints() * 100) / Player_Stats.stats_instance.getTotalHealthPoints(), healthLine.sizeDelta.y);
        }
        // Manabar
        if (totalManapoints.text != Player_Stats.stats_instance.getTotalManaPoints().ToString())
        {
            totalManapoints.text = Player_Stats.stats_instance.getTotalManaPoints().ToString();
        }
        if (currentManapoints.text != Player_Stats.stats_instance.getCurrentManaPoints().ToString())
        {
            currentManapoints.text = Player_Stats.stats_instance.getCurrentManaPoints().ToString();
            manaLine.sizeDelta = new Vector2((Player_Stats.stats_instance.getCurrentManaPoints() * 100) / Player_Stats.stats_instance.getTotalManaPoints(), manaLine.sizeDelta.y);
        }
        // Exp bar
        if (totalExppoints.text != Player_Stats.stats_instance.getTotalLevelExp().ToString())
        {
            totalExppoints.text = Player_Stats.stats_instance.getTotalLevelExp().ToString();
        }
        if (currentExppoints.text != Player_Stats.stats_instance.getCurrentExp().ToString())
        {
            currentExppoints.text = Player_Stats.stats_instance.getCurrentExp().ToString();
            if (Player_Stats.stats_instance.getCurrentExp() <= 0) // security if current xp is 0
            {
                expLine.sizeDelta = new Vector2(0f, expLine.sizeDelta.y);
            }
            else
            {
                expLine.sizeDelta = new Vector2((Player_Stats.stats_instance.getCurrentExp() * 100) / Player_Stats.stats_instance.getTotalLevelExp(), expLine.sizeDelta.y);
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
