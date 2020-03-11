/* UI_Player.cs
 * Utiliser pour gérer le reste des elements UI du joueur (Est en top hierarchie)
 * - Centralise les input pour afficher les différentes parties UI
 * 
 * 
 * */

using UnityEngine;
using UnityEngine.UI;

public class UI_Player : MonoBehaviour
{
    public static UI_Player uiPlayerInstance; // We singleton it to keep it between scene and acces it easily. It's the top hierarchy of Player UI

    private void Awake()
    {
        if (uiPlayerInstance == null)
        {
            uiPlayerInstance = this;
            DontDestroyOnLoad(gameObject); 
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    public UI_Player_Stats playerStatsUI;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F)) // To centralise in Player_Input.cs later
        {
            ToggleStatsMenu();
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
