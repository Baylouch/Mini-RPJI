﻿/* UI_Player.cs
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

        if (Input.GetKeyDown(KeyCode.I)) // To centralise in Player_Input.cs later
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
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            TogglePlayerMenu();
        }

        // Special condition for pets UI because we wants its unlock when player did a quest.
        if (Player_Pets.instance && Player_Pets.instance.GetPetsUnlocked())
        {
            if (Input.GetKeyDown(KeyCode.P))
            {
                TogglePetsUI();
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

    // Quest UI. On the left of the screen
    public void ToggleQuestMenu()
    {
        if (playerQuestUI)
        {
            if (!playerQuestUI.gameObject.activeSelf)
            {
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
