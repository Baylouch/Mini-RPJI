/* Scenes_Control.cs
 * Permet le changement entre les scenes.
 * Est créé a partir de la premiere scene du jeu
 * 
 * */

using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Scenes_Control : MonoBehaviour
{
    public static Scenes_Control instance;

    public const int totalGameLevels = 4; // All game levels (including Player_Level)

    public const int startGameLevelsBuildIndex = 4; // Because of scenes organisation in build settings, we got our first level at index 4 (previous are menus)

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

    // Method set on "readyButton" in transition scene to move on game levels.
    void SetPlayerAndLevelOne()
    {
        StartCoroutine(StartGameWhenLevelsLoaded());
    }
    
    // Method to call on start menu when player start the game
    public void LoadGameLevels()
    {
        StartCoroutine(LoadGameLevelsAsync());
    }

    // #1
    // Method to load transition scene then load all game levels 
    IEnumerator LoadGameLevelsAsync()
    {
        // First of all, load transition level single mode
        AsyncOperation asyncTransitionLoad = SceneManager.LoadSceneAsync("Level_Transition", LoadSceneMode.Single);

        while (!asyncTransitionLoad.isDone)
        {
            yield return null;
        }

        // Once we're in transition scene, we can search for loading bars and ready button.
        Loading_Bar currentLoadBar = GameObject.Find("LoadingBarCurrentLevel").GetComponent<Loading_Bar>();
        Loading_Bar totalLoadBar = GameObject.Find("LoadingBarTotal").GetComponent<Loading_Bar>();

        Button readyButton = GameObject.Find("ReadyButton").GetComponent<Button>();
        readyButton.gameObject.SetActive(false);

        int levelsLoad = 1; // To set total load bar as percentage with the total number of game levels

        // Before loading scene, we make sure we havnt already got a player in scenes.
        // (usefull when player want to load a game from a game level.
        if (Player_Stats.instance)
            Destroy(Player_Stats.instance.gameObject);
        if (UI_Player.instance)
            Destroy(UI_Player.instance.gameObject);

        // Then start to load levels
        // We know build index of each scenes. So game levels will start index 4
        // we need to load all levels so we'll do a loop to async load all level.
        for (int i = startGameLevelsBuildIndex; i < SceneManager.sceneCountInBuildSettings; i++)
        {
            AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(i, LoadSceneMode.Additive);

            while (!asyncLoad.isDone)
            {
                // Display current loading
                currentLoadBar.SetLoadingBar(asyncLoad.progress * 100);
                currentLoadBar.SetLoadingText(asyncLoad.progress * 100);

                yield return null;
            }

            // We need to disable the top hierarchy gameobject of each game level
            Scene loadedScene = SceneManager.GetSceneByBuildIndex(i);
            GameObject rootObject = loadedScene.GetRootGameObjects()[0];
            rootObject.SetActive(false);

            totalLoadBar.SetLoadingBar((levelsLoad / totalGameLevels) * 100);
            totalLoadBar.SetLoadingText((levelsLoad / totalGameLevels) * 100);
            levelsLoad++;

            currentLoadBar.SetLoadingBar(0);
            currentLoadBar.SetLoadingText(0);

        }

        Debug.Log("Levels loaded.");

        // Now we can display "readyButton" to let player continue to the game
        readyButton.onClick.AddListener(SetPlayerAndLevelOne);
        readyButton.gameObject.SetActive(true);

        currentLoadBar.gameObject.SetActive(false);
        totalLoadBar.gameObject.SetActive(false);
    }

    // #2
    // Method to unload transition scene and start the game when all levels are loaded
    IEnumerator StartGameWhenLevelsLoaded()
    {
        // Unload transition scene
        AsyncOperation unloadTransition = SceneManager.UnloadSceneAsync("Level_Transition");

        while (!unloadTransition.isDone)
        {
            yield return null;
        }

        // Set active player
        Scene playerLevel = SceneManager.GetSceneByName("Player_Level");
        GameObject[] roots = playerLevel.GetRootGameObjects();
        roots[0].SetActive(true);
        roots[1].SetActive(true);

        // Set active level 1
        Scene level_1 = SceneManager.GetSceneByName("Level_1");
        GameObject root = level_1.GetRootGameObjects()[0];
        root.SetActive(true);

        SceneManager.SetActiveScene(SceneManager.GetSceneByName("Level_1"));

        // Because we use this to switch into game levels
        if (Music_Manager.instance)
        {
            Music_Manager.instance.ToggleGameOrMenuMusics(true);
        }

    }

    // Method to switch player into another game level.
    // for exemple from level_1 to level_1_sublevel_1
    public void SwitchPlayerLevel(int levelToGoBuildIndex)
    {
        // Disable level from where you come
        Scene levelFrom = SceneManager.GetActiveScene();
        GameObject rootLevelFrom = levelFrom.GetRootGameObjects()[0]; // Because there is only one root gameobject per game level.
        rootLevelFrom.SetActive(false);

        // Enable level where you go
        Scene levelToGo = SceneManager.GetSceneByBuildIndex(levelToGoBuildIndex);
        GameObject rootLevelToGo = levelToGo.GetRootGameObjects()[0];
        rootLevelToGo.SetActive(true);

        SceneManager.SetActiveScene(SceneManager.GetSceneByBuildIndex(levelToGoBuildIndex));

        // Set player position
        FindObjectOfType<Player_Movement>().SetPlayerPosition(levelFrom.buildIndex);
    }

    // TODO find a way to move player to level 1 start position when needed. (In UI_GameOver for exemple.)
    public void SwitchToLevel1Start()
    {
        Scene levelFrom = SceneManager.GetActiveScene();
        GameObject rootLevelFrom = levelFrom.GetRootGameObjects()[0]; // Because there is only one root gameobject per game level.
        rootLevelFrom.SetActive(false);

        Scene startScene = SceneManager.GetSceneByName("Level_1");
        GameObject rootStartScene = startScene.GetRootGameObjects()[0];
        rootStartScene.SetActive(true);

        SceneManager.SetActiveScene(SceneManager.GetSceneByName("Level_1"));

    }

    // Used in Menu_Controller to simply change level
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
