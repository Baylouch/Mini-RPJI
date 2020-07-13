/* Scenes_Control.cs
 * 
 * Permet le changement entre les scenes.
 * Est créé à partir de la première scene du jeu
 * 
 * */

using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Scenes_Control : MonoBehaviour
{
    // TODO Think about split Player and Settings scene into Player Settings (contain Player and Player_UI) and Game settings (contain pathfinding etc)

    public static Scenes_Control instance;

    //public const int totalGameLevels = 4; // All game levels (including Player_Level)

    public const int levelTransitionBuildIndex = 3;
    public const int PlayerAndSettingsBuildIndex = 4; // Because of scenes organisation in build settings, we got our first level at index 4 (previous are menus)
    public const int TownLevelBuildIndex = 5;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
            Destroy(gameObject);

    }

    public void LoadTownLevel()
    {
        StartCoroutine(LoadTownLevelCoroutine());
    }

    public void LaunchGame()
    {
        StartCoroutine(LaunchGameCoroutine());
    }

    // Method to load the "Town Level" (the level when player enter into the game)
    // This method is use only when player load a game or when he starts the game from the Start Menu.
    IEnumerator LoadTownLevelCoroutine()
    {
        // First we must load the transition level where the player will wait for town level and settings to load
        AsyncOperation asyncTransitionLoad = SceneManager.LoadSceneAsync(levelTransitionBuildIndex, LoadSceneMode.Single);

        while (!asyncTransitionLoad.isDone)
        {
            yield return null;
        }

        // Once we're in transition scene, we can search for loading bar and ready button.
        Loading_Bar loadingBar = GameObject.Find("LoadingBarCurrentLevel").GetComponent<Loading_Bar>();
        Button readyButton = GameObject.Find("ReadyButton").GetComponent<Button>();
        readyButton.gameObject.SetActive(false); // Set it unactive to not show before level is loaded.

        // Before loading Player and Settings scene, we make sure we havnt already got a player in scenes.
        // (usefull when player want to load a game from a game level).
        if (Player_Stats.instance)
            Destroy(Player_Stats.instance.gameObject);
        if (UI_Player.instance)
            Destroy(UI_Player.instance.gameObject);

        // Now we're ready to load levels we need.
        // So first we'll load the scene containing Player and Settings (Pathfinding etc...)
        AsyncOperation playerAndSettingsAsyncLoad = SceneManager.LoadSceneAsync(PlayerAndSettingsBuildIndex, LoadSceneMode.Additive);

        while (!playerAndSettingsAsyncLoad.isDone)
        {
            yield return null;
        }

        // Once Player And Settings scene is loaded, we can load the Town level.
        AsyncOperation townAsyncLoad = SceneManager.LoadSceneAsync(TownLevelBuildIndex, LoadSceneMode.Additive);

        while (!townAsyncLoad.isDone)
        {
            // Display current loading
            loadingBar.SetLoadingBar(townAsyncLoad.progress * 100);
            loadingBar.SetLoadingText(townAsyncLoad.progress * 100);

            yield return null;
        }

        // We need to disable the top hierarchy gameobject of town level until player click on ready button.
        Scene townScene = SceneManager.GetSceneByBuildIndex(TownLevelBuildIndex);
        GameObject rootObject = townScene.GetRootGameObjects()[0];
        rootObject.SetActive(false);

        Debug.Log("Player Settings and Town level are loaded.");

        // Now we can display "readyButton" to let player continue to the game
        readyButton.onClick.AddListener(LaunchGame);
        readyButton.gameObject.SetActive(true);
        loadingBar.gameObject.SetActive(false);
    }

    // Method to launch the game when Player settings and town are loaded
    IEnumerator LaunchGameCoroutine()
    {
        // Unload transition scene
        AsyncOperation unloadTransition = SceneManager.UnloadSceneAsync(levelTransitionBuildIndex);

        while (!unloadTransition.isDone)
        {
            yield return null;
        }

        // Set active player
        Scene playerAndSettingsScene = SceneManager.GetSceneByBuildIndex(PlayerAndSettingsBuildIndex);
        GameObject[] roots = playerAndSettingsScene.GetRootGameObjects();
        roots[0].SetActive(true);
        roots[1].SetActive(true);

        // Set active Town level
        Scene townScene = SceneManager.GetSceneByBuildIndex(TownLevelBuildIndex);
        GameObject root = townScene.GetRootGameObjects()[0];
        root.SetActive(true);

        SceneManager.SetActiveScene(SceneManager.GetSceneByBuildIndex(TownLevelBuildIndex));

        // Because we use this to switch into game levels
        if (Music_Manager.instance)
        {
            Music_Manager.instance.ToggleGameOrMenuMusics(true);
        }
    }

    // An other SwitchPlayerLevel(int) method but now it must test :
    // If the level is already loaded just switch on it (disable previous and enable the new level)
    // Else if the level isn't loaded yet, then load it before switch player on it.
    public void SwitchGameLevel(int levelToGoBuildIndex, bool usedTeleportail = false) // bool here to know if player used teleportail to change game level
    {
        if (levelToGoBuildIndex > SceneManager.sceneCountInBuildSettings)
        {
            Debug.Log("Level build index is out of range.");
            return;
        }

        // Check if level to go isn't loaded. If not, load it.
        Scene levelToGo = SceneManager.GetSceneByBuildIndex(levelToGoBuildIndex);
        if (!levelToGo.isLoaded)
        {
            StartCoroutine(LoadGameLevel(levelToGoBuildIndex, usedTeleportail));
            return;
        }
        else
        {
            // Disable level from where you come
            Scene levelFrom = SceneManager.GetActiveScene();
            GameObject rootLevelFrom = levelFrom.GetRootGameObjects()[0]; // Because there is only one root gameobject per game level.
            rootLevelFrom.SetActive(false);
            

            // Active level to go gameobjects
            GameObject rootLevelToGo = levelToGo.GetRootGameObjects()[0];
            rootLevelToGo.SetActive(true);

            // Set the new scene as the active one.
            SceneManager.SetActiveScene(SceneManager.GetSceneByBuildIndex(levelToGoBuildIndex));

            // If player used usedTeleportail to get into the scene, we must research the one is in the new scene the zone to tp player on it
            if (usedTeleportail)
            {
                FindObjectOfType<Player_Movement>().transform.position = FindObjectOfType<Teleportail>().transform.position;
            }
            else
            {
                // Set player position with teleportation index
                FindObjectOfType<Player_Movement>().SetPlayerPosition(levelFrom.buildIndex);
            }

            // Set pet's player position relative to player
            if (FindObjectOfType<PetMovement>())
            {
                FindObjectOfType<PetMovement>().transform.position = FindObjectOfType<Player_Movement>().transform.position;
            }
        }
    }

    // To load a new game level. Will put the player into transition scene while waiting.
    IEnumerator LoadGameLevel(int levelToLoadBuildIndex, bool usedTeleportail)
    {
        // Load Transition scene while player is waiting.
        AsyncOperation asyncTransitionLoad = SceneManager.LoadSceneAsync(levelTransitionBuildIndex, LoadSceneMode.Additive);

        while (!asyncTransitionLoad.isDone)
        {
            yield return null;
        }

        // Disable player for now
        Player_Stats.instance.gameObject.SetActive(false);
        UI_Player.instance.gameObject.SetActive(false);

        // Once we're in transition scene, we can search for loading bar and ready button.
        Loading_Bar loadingBar = GameObject.Find("LoadingBarCurrentLevel").GetComponent<Loading_Bar>();
        Button readyButton = GameObject.Find("ReadyButton").GetComponent<Button>();
        readyButton.gameObject.SetActive(false); // Set it unactive to not show before level is loaded.

        // Disable the game level where player came from
        Scene levelFrom = SceneManager.GetActiveScene();
        GameObject rootLevelFrom = levelFrom.GetRootGameObjects()[0]; // Because there is only one root gameobject per game level.
        rootLevelFrom.SetActive(false);

        // Set transition scene as active one
        SceneManager.SetActiveScene(SceneManager.GetSceneByBuildIndex(levelTransitionBuildIndex));

        // ************* AD HERE *********************

        // Load the new game level
        AsyncOperation levelAsyncLoad = SceneManager.LoadSceneAsync(levelToLoadBuildIndex, LoadSceneMode.Additive);

        // Wait until loading is done
        while (!levelAsyncLoad.isDone)
        {
            // Display current loading
            loadingBar.SetLoadingBar(levelAsyncLoad.progress * 100);
            loadingBar.SetLoadingText(levelAsyncLoad.progress * 100);

            yield return null;
        }

        // Disable level to go
        Scene levelToGo = SceneManager.GetSceneByBuildIndex(levelToLoadBuildIndex);
        GameObject rootLevelToGo = levelToGo.GetRootGameObjects()[0];
        rootLevelToGo.SetActive(false);

        // Set the ready button to switch to the new game level
        readyButton.onClick.AddListener(() => NewGameLevelReady(levelToLoadBuildIndex, levelFrom.buildIndex, usedTeleportail));
        readyButton.gameObject.SetActive(true);
        loadingBar.gameObject.SetActive(false);
    }

    IEnumerator SetNewGameLevel(int newGameLevelBuildIndex, int levelFromBuildIndex, bool usedTeleportail)
    {
        // Unload transition scene
        AsyncOperation unloadTransition = SceneManager.UnloadSceneAsync(levelTransitionBuildIndex);

        while (!unloadTransition.isDone)
        {
            yield return null;
        }

        // Active new game level
        Scene newGameLevel = SceneManager.GetSceneByBuildIndex(newGameLevelBuildIndex);
        GameObject rootLevelToGo = newGameLevel.GetRootGameObjects()[0];
        rootLevelToGo.SetActive(true);

        // Enable player 
        Player_Stats.instance.gameObject.SetActive(true);
        UI_Player.instance.gameObject.SetActive(true);

        // Set the new scene as the active one.
        SceneManager.SetActiveScene(SceneManager.GetSceneByBuildIndex(newGameLevelBuildIndex));

        // If player used usedTeleportail to get into the scene, we must research the one is in the new scene the zone to tp player on it
        if (usedTeleportail)
        {
            FindObjectOfType<Player_Movement>().transform.position = FindObjectOfType<Teleportail>().transform.position;
        }
        else
        {
            // Set player position with teleportation index
            FindObjectOfType<Player_Movement>().SetPlayerPosition(levelFromBuildIndex);
        }

        // Set pet's player position
        if (FindObjectOfType<PetMovement>())
        {
            FindObjectOfType<PetMovement>().transform.position = FindObjectOfType<Player_Movement>().transform.position;
        }

        // Active ennemies
        if (FindObjectOfType<Player_Activator>())
        {
            FindObjectOfType<Player_Activator>().CheckForEnnemiesActivationOnNewLevel();
        }
    }

    public void NewGameLevelReady(int newGameLevelBuildIndex, int levelFromBuildIndex, bool usedTeleportail)
    {
        StartCoroutine(SetNewGameLevel(newGameLevelBuildIndex, levelFromBuildIndex, usedTeleportail));
    }

    // TODO find a way to move player to level 1 start position when needed. (In UI_GameOver for exemple.)
    public void SwitchToLevel1Start()
    {
        Scene levelFrom = SceneManager.GetActiveScene();
        GameObject rootLevelFrom = levelFrom.GetRootGameObjects()[0]; // Because there is only one root gameobject per game level.
        rootLevelFrom.SetActive(false);

        Scene startScene = SceneManager.GetSceneByBuildIndex(TownLevelBuildIndex);
        GameObject rootStartScene = startScene.GetRootGameObjects()[0];
        rootStartScene.SetActive(true);

        SceneManager.SetActiveScene(startScene);

    }

    // Used in Menu_Controller to simply change level by index
    public void ChangeLevel(int levelIndex)
    {
        if (levelIndex < SceneManager.sceneCountInBuildSettings)
        {
            if (Music_Manager.instance)
                Music_Manager.instance.ToggleGameOrMenuMusics(false); // Because we use this only in menus or for back to menus from games level

            SceneManager.LoadScene(levelIndex);
        }
        else
        {
            Debug.Log("Index level is out of range. Watch Build settings.");
        }
    }

    // Used in Menu_Controller to simply change level by name
    public void ChangeLevel(string levelName)
    {
        if (SceneManager.GetSceneByName(levelName) != null)
        {
            if (Music_Manager.instance)
                Music_Manager.instance.ToggleGameOrMenuMusics(false);

            SceneManager.LoadScene(levelName);
        }
        else
        {
            Debug.Log("Unknown level name. Watch Build settings.");
        }
    }

    public int GetCurrentSceneBuildIndex()
    {        
        Scene currentScene = SceneManager.GetActiveScene();
        return currentScene.buildIndex;
    }
}
