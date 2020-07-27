/* UI_Player.cs
 * 
 * Utiliser pour gérer le reste des elements UI du joueur (Est en top hierarchie)
 * 
 * - Centralise les inputs pour afficher les différentes parties UI
 * - Permet d'acceder aux différents scripts gérant l'UI du joueur
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
    public UI_Player_Quest playerQuestUI;
    public UI_Player_Menu playerMenu; // To pause the game, save, load, quit...
    public UI_Player_Bank playerBankUI;
    public UI_Player_Pets playerPetsUI;
    public UI_GameOver gameOverUI;
    public UI_Teleporter teleporterUI;
    public UI_Map mapUI;
    public UI_Abilities playerAbilitiesUI;

    private void Start()
    {
        HideAllMenus();
    }

    private void OnLevelWasLoaded(int level)
    {
        HideAllMenus();
    }

    private void Update()
    {
        if (gameOverUI.gameObject.activeSelf)
        {
            return;
        }

        if (Cheats.instance && Cheats.instance.PlayerIsInCheatMode())
        {
            return;
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            TogglePlayerMenu();
        }

        if (playerMenu.gameObject.activeSelf)
        {
            return;
        }

        if (Input.GetKeyDown(KeyCode.I))
        {
            ToggleStatsMenu();
        }
        if (Input.GetKeyDown(KeyCode.B))
        {
            ToggleInventoryMenu();
        }
        if (Input.GetKeyDown(KeyCode.J))
        {
            ToggleQuestMenu();
        }
        if (Input.GetKeyDown(KeyCode.M))
        {
            ToggleMap();
        }
        if (Input.GetKeyDown(KeyCode.N))
        {
            TogglePlayerAbilitiesUI();
        }

        // Special condition for pets UI because we wants its unlock when player did a quest.
        if (Player_Pets.instance)
        {
            if (Player_Pets.instance.GetPetsUnlocked())
            {
                if (!playerPetsUI.uiButtonToDisplayPetsUI.activeSelf)
                    playerPetsUI.uiButtonToDisplayPetsUI.SetActive(true);

                if (Input.GetKeyDown(KeyCode.P))
                {
                    TogglePetsUI();
                }
            }
            else
            {
                if (playerPetsUI.uiButtonToDisplayPetsUI.activeSelf)
                    playerPetsUI.uiButtonToDisplayPetsUI.SetActive(false);
            }
        }

    }

    void HideAllMenus()
    {
        if (gameOverUI.gameObject.activeSelf)
        {
            gameOverUI.gameObject.SetActive(false);
        }
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
        if (playerMenu.gameObject.activeSelf)
        {
            playerMenu.gameObject.SetActive(false);
        }
        if (playerBankUI.gameObject.activeSelf)
        {
            playerBankUI.gameObject.SetActive(false);
        }
        if (playerPetsUI.gameObject.activeSelf)
        {
            playerPetsUI.gameObject.SetActive(false);
        }
        if (teleporterUI.gameObject.activeSelf)
        {
            teleporterUI.teleporterPanel.SetActive(false);
        }
        if (mapUI.gameObject.activeSelf)
        {
            mapUI.gameObject.SetActive(false);
        }
        if (playerAbilitiesUI.gameObject.activeSelf)
        {
            playerAbilitiesUI.gameObject.SetActive(false);
        }
    }

    void HideLeftMenus()
    {
        if (playerStatsUI.gameObject.activeSelf)
        {
            playerStatsUI.gameObject.SetActive(false);
        }
        if (playerQuestUI.gameObject.activeSelf)
        {
            playerQuestUI.gameObject.SetActive(false);
        }
        if (playerBankUI.gameObject.activeSelf)
        {
            playerBankUI.gameObject.SetActive(false);
        }
        if (mapUI.gameObject.activeSelf)
        {
            mapUI.gameObject.SetActive(false);
        }
    }

    void HideRightMenus()
    {
        if (playerInventoryUI.gameObject.activeSelf)
        {
            playerInventoryUI.gameObject.SetActive(false);
        }
        if (playerPetsUI.gameObject.activeSelf)
        {
            playerPetsUI.gameObject.SetActive(false);
        }
        if (teleporterUI.gameObject.activeSelf)
        {
            teleporterUI.teleporterPanel.SetActive(false);
        }
        if (mapUI.gameObject.activeSelf)
        {
            mapUI.gameObject.SetActive(false);
        }
        if (playerAbilitiesUI.gameObject.activeSelf)
        {
            playerAbilitiesUI.gameObject.SetActive(false);
        }
    }

    // Menu UI. On the center of the screen
    public void TogglePlayerMenu()
    {
        if (playerMenu)
        {
            if (!playerMenu.gameObject.activeSelf)
            {
                HideAllMenus();

                playerMenu.gameObject.SetActive(true);
            }
            else
            {
                playerMenu.gameObject.SetActive(false);
            }

            if (Sound_Manager.instance)
            {
                Sound_Manager.instance.PlaySound(Sound_Manager.instance.asset.toggleUI);
            }
        }
    }

    public void TogglePlayerAbilitiesUI()
    {
        if (playerAbilitiesUI)
        {
            if (!playerAbilitiesUI.gameObject.activeSelf)
            {
                if (playerMenu.gameObject.activeSelf)
                {
                    return;
                }

                playerAbilitiesUI.gameObject.SetActive(true);
            }
            else
            {
                playerAbilitiesUI.gameObject.SetActive(false);
            }

            if (Sound_Manager.instance)
            {
                Sound_Manager.instance.PlaySound(Sound_Manager.instance.asset.toggleUI);
            }
        }
    }

    public void ToggleMap()
    {
        if (mapUI)
        {
            if (!mapUI.gameObject.activeSelf)
            {
                if (playerMenu.gameObject.activeSelf)
                {
                    return;
                }

                mapUI.gameObject.SetActive(true);
            }
            else
            {
                mapUI.gameObject.SetActive(false);
            }

            if (Sound_Manager.instance)
            {
                Sound_Manager.instance.PlaySound(Sound_Manager.instance.asset.toggleUI);
            }
        }
    }

    public void ToggleTeleporterMenu(bool condition)
    {
        if (teleporterUI)
        {
            if (condition == true)
            {
                if (!teleporterUI.teleporterPanel.activeSelf)
                {
                    HideRightMenus();

                    teleporterUI.teleporterPanel.SetActive(true);

                    if (Sound_Manager.instance)
                    {
                        Sound_Manager.instance.PlaySound(Sound_Manager.instance.asset.toggleUI);
                    }
                }
            }
            else
            {
                if (teleporterUI.teleporterPanel.activeSelf)
                {
                    teleporterUI.teleporterPanel.SetActive(false);

                    // Do not play sound because when player change scene its cause issue.
                }
            }
        }
    }

    // Quest UI. On the left of the screen
    public void ToggleQuestMenu()
    {
        if (playerQuestUI)
        {
            if (!playerQuestUI.gameObject.activeSelf)
            {
                if (playerMenu.gameObject.activeSelf)
                {
                    return;
                }

                HideLeftMenus();

                playerQuestUI.gameObject.SetActive(true);
            }
            else
            {
                playerQuestUI.gameObject.SetActive(false);
            }

            if (Sound_Manager.instance)
            {
                Sound_Manager.instance.PlaySound(Sound_Manager.instance.asset.toggleUI);
            }
        }
    }

    // Inventory UI. On the right of the screen
    public void ToggleInventoryMenu()
    {
        if (playerInventoryUI)
        {            
            if (!playerInventoryUI.gameObject.activeSelf)
            {
                if (playerMenu.gameObject.activeSelf)
                {
                    return;
                }

                HideRightMenus();

                playerInventoryUI.gameObject.SetActive(true);
            }
            else
            {
                playerInventoryUI.gameObject.SetActive(false);
            }

            if (Sound_Manager.instance)
            {
                Sound_Manager.instance.PlaySound(Sound_Manager.instance.asset.toggleUI);
            }
        }
    }

    // Stats UI. On the left of the screen
    public void ToggleStatsMenu()
    {
        if (playerStatsUI) // To active/disable playerStatsUI
        {
            if (!playerStatsUI.gameObject.activeSelf)
            {
                if (playerMenu.gameObject.activeSelf)
                {
                    return;
                }

                HideLeftMenus();

                playerStatsUI.gameObject.SetActive(true);
                playerStatsUI.RefreshStatsDisplay();
            }
            else
            {
                playerStatsUI.gameObject.SetActive(false);
            }

            if (Sound_Manager.instance)
            {
                Sound_Manager.instance.PlaySound(Sound_Manager.instance.asset.toggleUI);
            }
        }
    }

    // Bank UI. On the left of the screen. We need a parameter on it because its not the same way its open/closed. (Interaction in game instead of Input pre def)
    public void ToggleBankUI(bool value)
    {
        if (playerBankUI)
        {
            if (value == true)
            {
                HideLeftMenus();

                if (!playerBankUI.gameObject.activeSelf)
                {
                    playerBankUI.gameObject.SetActive(true);

                    if (Sound_Manager.instance)
                    {
                        Sound_Manager.instance.PlaySound(Sound_Manager.instance.asset.toggleUI);
                    }
                }
            }
            else
            {
                if (playerBankUI.gameObject.activeSelf)
                {
                    playerBankUI.gameObject.SetActive(false);

                    if (Sound_Manager.instance)
                    {
                        Sound_Manager.instance.PlaySound(Sound_Manager.instance.asset.toggleUI);
                    }
                }
            }
        }
    }

    public void TogglePetsUI()
    {
        if (playerPetsUI)
        {
            if (!playerPetsUI.gameObject.activeSelf)
            {
                if (playerMenu.gameObject.activeSelf)
                {
                    return;
                }

                HideRightMenus();

                playerPetsUI.gameObject.SetActive(true);
            }
            else
            {
                playerPetsUI.gameObject.SetActive(false);
            }

            if (Sound_Manager.instance)
            {
                Sound_Manager.instance.PlaySound(Sound_Manager.instance.asset.toggleUI);
            }
        }
    }
}
