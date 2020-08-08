using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class UI_Teleporter : MonoBehaviour
{
    // A const to save and load data (representing all tps count)
    public const int teleporterNumber = 12;

    public GameObject teleporterPanel; // The UI of teleporters.

    public int currentTeleportail = -1; // To know the current teleportail using to not tp on it. Represent index of tpButtons.

    [SerializeField] Button[] tpButtons; // Contains all teleportation buttons.
                                         // /!\ Index are constant to match with build index :
                                         // [0] = Town (5),
                                         //
                                         // [1] = Forest part 2 (7),    [4] = Beach part 1 (16), [7]  = Volcano part 2 (23)
                                         // [2] = Forest part 3 (8),    [5] = Beach part 4 (19), [8]  = Volcano part 4 (25)
                                         // [3] = Forest part 5 (10),   [6] = Beach part 6 (21), [9]  = Volcano part 5 (26)
                                         //                                                      [10] = Volcano part 6 (27)
                                         //                                                      [11] = Volcano part 9 (29)

    private bool[] unlockedTps; // Represent all tp unlocked or not. Initialized by tpButtons.Length. Index of tpButtons and unlockedTps are linked.

    [SerializeField] GameObject ForestPanel;
    [SerializeField] GameObject BeachPanel;
    [SerializeField] GameObject VolcanoPanel;
    [SerializeField] GameObject OtherPanel;

    [SerializeField] GameObject teleportailEffect;

    // Start is called before the first frame update
    void Start()
    {
        unlockedTps = new bool[tpButtons.Length];

        for (int i = 0; i < unlockedTps.Length; i++)
        {
            if (unlockedTps[i] == false)
            {
                tpButtons[i].GetComponent<Image>().color = Color.grey;
                tpButtons[i].interactable = false;
            }
        }
    }

    void HideTpsZone()
    {
        ForestPanel.SetActive(false);
        BeachPanel.SetActive(false);
        VolcanoPanel.SetActive(false);
        OtherPanel.SetActive(false);
    }

    // zoneIndex : 0 is Forest, 1 is Beach, 2 is Volcano, 3 is Other.
    public void DisplayTpsZone(int zoneIndex)
    {
        switch (zoneIndex)
        {
            case 0:
                HideTpsZone();
                ForestPanel.SetActive(true);
                break;
            case 1:
                HideTpsZone();
                BeachPanel.SetActive(true);
                break;
            case 2:
                HideTpsZone();
                VolcanoPanel.SetActive(true);
                break;
            case 3:
                HideTpsZone();
                OtherPanel.SetActive(true);
                break;
            default:
                Debug.Log("Wrong index zone : \"" + zoneIndex + "\"");
                break;
        }
    }

    // Coroutine displaying effect on player and sound just before teleportation.
    // At the end execute Scene_Control.instance.SwitchGameLevel(tpIndex,true);
    IEnumerator SetTeleportation(int tpIndex)
    {
        // Freeze the player
        Player_Movement movement = FindObjectOfType<Player_Movement>();
        movement.canMove = false;

        // Close Teleporting menu
        teleporterPanel.SetActive(false);

        // Display Effect
        if (teleportailEffect)
        {
            GameObject effect = Instantiate(teleportailEffect, Player_Stats.instance.transform.position, teleportailEffect.transform.rotation);

            Destroy(effect, 1f);
        }

        // Play sound
        if (Sound_Manager.instance)
        {
            Sound_Manager.instance.PlaySound(Sound_Manager.instance.asset.teleportailSound);
        }

        yield return new WaitForSeconds(1f);


        if (Scenes_Control.instance)
        {
            Scenes_Control.instance.SwitchGameLevel(tpIndex, true);
        }
        else
        {
            Debug.Log("No Scene_Control instance found.");
        }

        movement.canMove = true;
    }

    // tpIndex refer to the build index of the scene we want to go
    public void UseTeleportation(int tpIndex)
    {
        if (currentTeleportail == tpIndex)
            return;

        StartCoroutine(SetTeleportation(tpIndex));
    }

    public bool GetUnlockedTp(int tpID)
    {
        if (unlockedTps[tpID])
        {
            return true;
        }

        return false;
    }

    public void SetUnlockedTp(int tpID, bool value)
    {
        if (unlockedTps.Length <= tpID)
        {
            Debug.Log("tpID out of range \"" + tpID + "\"");
            return;
        }

        unlockedTps[tpID] = value;

        if (value == true)
        {
            tpButtons[tpID].GetComponent<Image>().color = Color.white;
            tpButtons[tpID].interactable = true;
        }
        else
        {
            tpButtons[tpID].GetComponent<Image>().color = Color.grey;
            tpButtons[tpID].interactable = false;
        }
    }
}
