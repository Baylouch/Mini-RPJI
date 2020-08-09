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

    // Menus prefabs to instantiate
    [SerializeField] GameObject playerStatsUIGO;
    [SerializeField] GameObject playerInventoryUIGO;
    [SerializeField] GameObject playerQuestUIGO;
    [SerializeField] GameObject playerMenuGO;
    [SerializeField] GameObject playerBankUIGO;
    [SerializeField] GameObject playerPetsUIGO;
    [SerializeField] GameObject gameOverUIGO;
    // [SerializeField] GameObject teleporterUIGO;
    [SerializeField] GameObject mapUIGO;
    [SerializeField] GameObject playerAbilitiesUIGO;
    [SerializeField] GameObject successUIGO;

    // All these menus are spawnable. Spawn them is a nice performance optimization.
    [HideInInspector] public UI_Player_Stats playerStatsUI;
    [HideInInspector] public UI_Player_Inventory playerInventoryUI;
    [HideInInspector] public UI_Player_Quest playerQuestUI;
    [HideInInspector] public UI_Player_Menu playerMenu; // To pause the game, save, load, quit...
    [HideInInspector] public UI_Player_Bank playerBankUI;
    [HideInInspector] public UI_Player_Pets playerPetsUI;
    [HideInInspector] public UI_GameOver gameOverUI;
    [HideInInspector] public UI_Map mapUI;
    [HideInInspector] public UI_Abilities playerAbilitiesUI;
    [HideInInspector] public UI_Player_Success successUI;

    public UI_Teleporter teleporterUI; // TODO Make it acting as other UI elements (instantiate/Destroy) for simplicity i let as it is because i must change the behaviour of multiple things to make its working again

    [SerializeField] GameObject uiButtonToDisplayPetsUI;

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
        if (gameOverUI != null)
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

        if (playerMenu != null)
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
        if (Input.GetKeyDown(KeyCode.L))
        {
            ToggleSuccessUI();
        }

        // Special condition for pets UI because we wants its unlock when player did a quest.
        if (Player_Pets.instance)
        {
            if (Player_Pets.instance.GetPetsUnlocked())
            {
                if (!uiButtonToDisplayPetsUI.activeSelf)
                    uiButtonToDisplayPetsUI.SetActive(true);

                if (Input.GetKeyDown(KeyCode.P))
                {
                    TogglePetsUI();
                }
            }
            else
            {
                if (uiButtonToDisplayPetsUI.activeSelf)
                    uiButtonToDisplayPetsUI.SetActive(false);
            }
        }

    }

    void HideAllMenus()
    {
        if (gameOverUI != null)
        {
            Destroy(gameOverUI.gameObject);
            gameOverUI = null;
        }
        if (playerInventoryUI != null)
        {
            Destroy(playerInventoryUI.gameObject);
            playerInventoryUI = null;
        }
        if (playerStatsUI != null)
        {
            Destroy(playerStatsUI.gameObject);
            playerStatsUI = null;
        }
        if (playerQuestUI != null)
        {
            Destroy(playerQuestUI.gameObject);
            playerQuestUI = null;
        }
        if (playerMenu != null)
        {
            Destroy(playerMenu.gameObject);
            playerMenu = null;
        }
        if (playerBankUI != null)
        {
            Destroy(playerBankUI.gameObject);
            playerBankUI = null;
        }
        if (playerPetsUI != null)
        {
            Destroy(playerPetsUI.gameObject);
            playerPetsUI = null;
        }
        if (mapUI != null)
        {
            Destroy(mapUI.gameObject);
            mapUI = null;
        }
        if (playerAbilitiesUI != null)
        {
            Destroy(playerAbilitiesUI.gameObject);
            playerAbilitiesUI = null;
        }
        if (successUI != null)
        {
            Destroy(successUI.gameObject);
            successUI = null;
        }

        if (teleporterUI.gameObject.activeSelf)
        {
            teleporterUI.teleporterPanel.SetActive(false);
        }
    }

    void HideLeftMenus()
    {
        if (playerStatsUI != null)
        {
            Destroy(playerStatsUI.gameObject);
            playerStatsUI = null;
        }
        if (playerQuestUI != null)
        {
            Destroy(playerQuestUI.gameObject);
            playerQuestUI = null;
        }
        if (playerBankUI != null)
        {
            Destroy(playerBankUI.gameObject);
            playerBankUI = null;
        }
        if (mapUI != null)
        {
            Destroy(mapUI.gameObject);
            mapUI = null;
        }
        if (successUI != null)
        {
            Destroy(successUI.gameObject);
            successUI = null;
        }

        if (teleporterUI.gameObject.activeSelf)
        {
            teleporterUI.teleporterPanel.SetActive(false);
        }
    }

    void HideRightMenus()
    {
        if (playerInventoryUI != null)
        {
            Destroy(playerInventoryUI.gameObject);
            playerInventoryUI = null;
        }
        if (playerPetsUI != null)
        {
            Destroy(playerPetsUI.gameObject);
            playerPetsUI = null;
        }
        if (mapUI != null)
        {
            Destroy(mapUI.gameObject);
            mapUI = null;
        }
        if (playerAbilitiesUI != null)
        {
            Destroy(playerAbilitiesUI.gameObject);
            playerAbilitiesUI = null;
        }
        if (successUI != null)
        {
            Destroy(successUI.gameObject);
            successUI = null;
        }
    }

    // Menu UI. On the center of the screen
    public void TogglePlayerMenu()
    {
        if (playerMenu == null)
        {
            HideAllMenus();

            GameObject newPlayerMenu = Instantiate(playerMenuGO, transform);
            playerMenu = newPlayerMenu.GetComponent<UI_Player_Menu>();
        }
        else
        {
            Destroy(playerMenu.gameObject);
            playerMenu = null;
        }

        if (Sound_Manager.instance)
        {
            Sound_Manager.instance.PlaySound(Sound_Manager.instance.asset.toggleUI);
        }
    }

    public void ToggleSuccessUI()
    {
        if (successUI == null)
        {
            HideAllMenus();

            GameObject newPlayerSucces = Instantiate(successUIGO, transform);
            successUI = newPlayerSucces.GetComponent<UI_Player_Success>();
        }
        else
        {
            Destroy(successUI.gameObject);
            successUI = null;
        }

        if (Sound_Manager.instance)
        {
            Sound_Manager.instance.PlaySound(Sound_Manager.instance.asset.toggleUI);
        }
    }

    public void TogglePlayerAbilitiesUI()
    {
        if (playerAbilitiesUI == null)
        {
            HideRightMenus();

            GameObject newPlayerAbilities = Instantiate(playerAbilitiesUIGO, transform);
            playerAbilitiesUI = newPlayerAbilities.GetComponent<UI_Abilities>();
        }
        else
        {
            Destroy(playerAbilitiesUI.gameObject);
            playerAbilitiesUI = null;
        }

        if (Sound_Manager.instance)
        {
            Sound_Manager.instance.PlaySound(Sound_Manager.instance.asset.toggleUI);
        }
    }

    public void ToggleMap()
    {
        if (mapUI == null)
        {
            HideAllMenus();

            GameObject newMap = Instantiate(mapUIGO, transform);
            mapUI = newMap.GetComponent<UI_Map>();
        }
        else
        {
            Destroy(mapUI.gameObject);
            mapUI = null;
        }

        if (Sound_Manager.instance)
        {
            Sound_Manager.instance.PlaySound(Sound_Manager.instance.asset.toggleUI);
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
        if (playerQuestUI == null)
        {
            HideLeftMenus();

            GameObject newPlayerQuest = Instantiate(playerQuestUIGO, transform);
            playerQuestUI = newPlayerQuest.GetComponent<UI_Player_Quest>();
        }
        else
        {
            Destroy(playerQuestUI.gameObject);
            playerQuestUI = null;
        }

        if (Sound_Manager.instance)
        {
            Sound_Manager.instance.PlaySound(Sound_Manager.instance.asset.toggleUI);
        }
    }

    // Inventory UI. On the right of the screen
    public void ToggleInventoryMenu()
    {
        if (playerInventoryUI == null)
        {
            HideRightMenus();

            GameObject newPlayerInventory = Instantiate(playerInventoryUIGO, transform);
            playerInventoryUI = newPlayerInventory.GetComponent<UI_Player_Inventory>();
        }
        else
        {
            Destroy(playerInventoryUI.gameObject);
            playerInventoryUI = null;
        }

        if (Sound_Manager.instance)
        {
            Sound_Manager.instance.PlaySound(Sound_Manager.instance.asset.toggleUI);
        }
    }

    // Stats UI. On the left of the screen
    public void ToggleStatsMenu()
    {
        if (playerStatsUI == null)
        {
            HideRightMenus();

            GameObject newPlayerStats = Instantiate(playerStatsUIGO, transform);
            playerStatsUI = newPlayerStats.GetComponent<UI_Player_Stats>();
        }
        else
        {
            Destroy(playerStatsUI.gameObject);
            playerStatsUI = null;
        }

        if (Sound_Manager.instance)
        {
            Sound_Manager.instance.PlaySound(Sound_Manager.instance.asset.toggleUI);
        }
    }

    // Bank UI. On the left of the screen. We need a parameter on it because its not the same way its open/closed. (Interaction in game instead of Input pre def)
    public void ToggleBankUI(bool value)
    {
        if (value == true)
        {
            if (playerBankUI == null)
            {
                HideLeftMenus();

                GameObject newPlayerBank = Instantiate(playerBankUIGO, transform);
                playerBankUI = newPlayerBank.GetComponent<UI_Player_Bank>();
            }
        }
        else
        {
            if (playerBankUI)
            {
                Destroy(playerBankUI.gameObject);
                playerBankUI = null;

                if (Sound_Manager.instance)
                {
                    Sound_Manager.instance.PlaySound(Sound_Manager.instance.asset.toggleUI);
                }
            }
        }
    }

    public void TogglePetsUI()
    {
        if (playerPetsUI == null)
        {
            HideRightMenus();

            GameObject newPlayerPets = Instantiate(playerPetsUIGO, transform);
            playerPetsUI = newPlayerPets.GetComponent<UI_Player_Pets>();
        }
        else
        {
            Destroy(playerPetsUI.gameObject);
            playerPetsUI = null;
        }

        if (Sound_Manager.instance)
        {
            Sound_Manager.instance.PlaySound(Sound_Manager.instance.asset.toggleUI);
        }
    }

    public void ToggleGameOverUI(bool condition)
    {
        if (condition == true)
        {
            if (gameOverUI == null)
            {
                HideRightMenus();

                GameObject newGameOver = Instantiate(gameOverUIGO, transform);
                gameOverUI = newGameOver.GetComponent<UI_GameOver>();
            }
        }
        else
        {
            if (gameOverUI != null)
            {
                Destroy(gameOverUI.gameObject);
                gameOverUI = null;
            }
        }
    }
}
