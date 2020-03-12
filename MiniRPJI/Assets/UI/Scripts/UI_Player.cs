/* UI_Player.cs
 * Utiliser pour gérer le reste des elements UI du joueur (Est en top hierarchie)
 * - Centralise les input pour afficher les différentes parties UI
 * - Permet d'acceder aux différents scripts gérant l'UI du joueur
 * 
 * */

using UnityEngine;

public class UI_Player : MonoBehaviour
{
    public static UI_Player ui_instance; // We singleton it to keep it between scene and acces it easily. It's the top hierarchy of Player UI

    private void Awake()
    {
        if (ui_instance == null)
        {
            ui_instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public UI_Player_Stats playerStatsUI;
    public UI_Player_Inventory playerInventoryUI;
    public UI_Player_Quest playerQuestUI;

    private void Start()
    {
        if (playerInventoryUI.gameObject.activeSelf)
        {
            playerInventoryUI.gameObject.SetActive(false);
        }
        if (playerStatsUI.gameObject.activeSelf)
        {
            playerStatsUI.gameObject.SetActive(false);
        }
        if (playerQuestUI.gameObject.activeSelf)
        {
            playerQuestUI.gameObject.SetActive(false);
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F)) // To centralise in Player_Input.cs later
        {
            ToggleStatsMenu();
        }
        // Toggle inventoryUI
        if (Input.GetKeyDown(KeyCode.B))
        {
            ToggleInventoryMenu();
        }
        if (Input.GetKeyDown(KeyCode.J))
        {
            ToggleQuestMenu();
        }
    }

    public void ToggleQuestMenu()
    {
        if (playerQuestUI)
        {
            if (!playerQuestUI.gameObject.activeSelf)
            {
                playerQuestUI.gameObject.SetActive(true);
            }
            else
            {
                playerQuestUI.gameObject.SetActive(false);
            }
        }
    }

    public void ToggleInventoryMenu()
    {
        if (playerInventoryUI)
        {
            if (!playerInventoryUI.gameObject.activeSelf)
            {
                playerInventoryUI.gameObject.SetActive(true);
            }
            else
            {
                playerInventoryUI.gameObject.SetActive(false);
            }
        }
    }

    public void ToggleStatsMenu()
    {
        if (playerStatsUI) // To active/disable playerStatsUI
        {
            if (playerStatsUI.gameObject.activeSelf)
            {
                playerStatsUI.gameObject.SetActive(false);
            }
            else
            {
                playerStatsUI.gameObject.SetActive(true);
                playerStatsUI.RefreshStatsDisplay();
            }
        }
    }
}
