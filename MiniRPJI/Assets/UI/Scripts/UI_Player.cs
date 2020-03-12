/* UI_Player.cs
 * Utiliser pour gérer le reste des elements UI du joueur (Est en top hierarchie)
 * - Centralise les input pour afficher les différentes parties UI
 * 
 * 
 * */

using UnityEngine;

public class UI_Player : MonoBehaviour
{
    public static UI_Player instance; // We singleton it to keep it between scene and acces it easily. It's the top hierarchy of Player UI

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject); 
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    public UI_Player_Stats playerStatsUI;
    public UI_Player_Inventory playerInventoryUI;

    private void Start()
    {
        if(playerInventoryUI.gameObject.activeSelf)
        {
            playerInventoryUI.gameObject.SetActive(false);
        }
        if (playerStatsUI.gameObject.activeSelf)
        {
            playerStatsUI.gameObject.SetActive(false);
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
    }

    public void ToggleInventoryMenu()
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
                // playerStatsUI Must have UI_Player_Stats component !
                playerStatsUI.RefreshStatsDisplay();
            }
        }
    }
}
