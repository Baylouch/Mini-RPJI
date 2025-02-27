﻿/* Scenes_Control.cs
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
    public static Scenes_Control instance;

    public const int finalLevelBuildIndex = 30;

    public const int creditSceneBuildIndex = 33;

    public const int levelTransitionBuildIndex = 3;
    public const int PlayerAndSettingsBuildIndex = 4; // Because of scenes organisation in build settings, we got our first level at index 4 (previous are menus)
    public const int TownLevelBuildIndex = 5;

    // Limit of loaded scenes
    public const int maxScenesLoaded = 5;
    public Scene[] gameScenesLoaded;
    private int gameScenesLoadedIndex = 0;

    // Credit scene UI transition stuff
    [SerializeField] GameObject sceneTransitionCanvas;

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

    private void Start()
    {
        gameScenesLoaded = new Scene[maxScenesLoaded];
    }

    // This method is used everytime a game level is load to recycle scenes and avoid performances issues by limiting the maximum loaded game scenes.
    void CheckScenesLoaded(Scene newScene)
    {
        // Debug.Log("Scene cycle in progress...");

        if (gameScenesLoadedIndex >= maxScenesLoaded)
        {
            gameScenesLoadedIndex = 0;
        }

        if (gameScenesLoaded[gameScenesLoadedIndex].IsValid())
        {
            SceneManager.UnloadSceneAsync(gameScenesLoaded[gameScenesLoadedIndex]);
        }

        gameScenesLoaded[gameScenesLoadedIndex] = newScene;

        gameScenesLoadedIndex++;
    }

    // A way we can teleport player to the town level (when he's game over for instance).
    public void SwitchPlayerToTheTown()
    {
        Scene levelFrom = SceneManager.GetActiveScene();
        GameObject rootLevelFrom = levelFrom.GetRootGameObjects()[0]; // Because there is only one root gameobject per game level.
        rootLevelFrom.SetActive(false);

        Scene townScene = SceneManager.GetSceneByBuildIndex(TownLevelBuildIndex);
        GameObject rootTownScene = townScene.GetRootGameObjects()[0];
        rootTownScene.SetActive(true);

        SceneManager.SetActiveScene(townScene);
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

    public void LoadTownLevel()
    {
        StartCoroutine(LoadTownLevelFirstTime());
    }

    public void LaunchGame()
    {
        StartCoroutine(StartTheGame());
    }

    // Method to load the "Town Level" (the level when player enter into the game)
    // This method is use only when player load a game or when he starts the game from the Start Menu.
    IEnumerator LoadTownLevelFirstTime()
    {
        // First we must load the transition level where the player will wait for town level and settings to load
        AsyncOperation asyncTransitionLoad = SceneManager.LoadSceneAsync(levelTransitionBuildIndex, LoadSceneMode.Single);

        while (!asyncTransitionLoad.isDone)
        {
            yield return null;
        }

        TransitionInformationSetter transitionInfoSetter = FindObjectOfType<TransitionInformationSetter>();
        if (transitionInfoSetter)
        {
            transitionInfoSetter.DisplayRandomInfoConfig();
        }

        // Once we're in transition scene, we can search for loading bar and ready button.
        Loading_Bar loadingBar = GameObject.Find("LoadingBarCurrentLevel").GetComponent<Loading_Bar>();
        Button readyButton = GameObject.Find("ReadyButton").GetComponent<Button>();
        readyButton.gameObject.SetActive(false); // Set it unactive to not show before level is loaded.

        // Before loading Player and Settings scene, we make sure we havnt already got a player in scenes.
        // (usefull when player want to load a game from a game level). so useless now ?
        if (Player_Stats.instance)
            Destroy(Player_Stats.instance.gameObject);
        if (UI_Player.instance)
            Destroy(UI_Player.instance.gameObject);

        // Now we're ready to load levels we need.
        // So first we'll load the scene containing Player and Settings (etc...)
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

            yield return new WaitForEndOfFrame();
        }

        // We need to disable the top hierarchy gameobject of town level until player click on ready button.
        Scene townScene = SceneManager.GetSceneByBuildIndex(TownLevelBuildIndex);
        GameObject rootObject = townScene.GetRootGameObjects()[0];
        rootObject.SetActive(false);

        SceneManager.SetActiveScene(townScene);

        // Debug.Log("Player Settings and Town level are loaded.");

        // Now we can display "readyButton" to let player continue to the game
        readyButton.onClick.AddListener(LaunchGame);
        readyButton.gameObject.SetActive(true);
        loadingBar.gameObject.SetActive(false);
    }

    // Method to start the game when Player settings and town are loaded
    IEnumerator StartTheGame()
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
            StartCoroutine(LoadNewGameLevel(levelToGoBuildIndex, usedTeleportail));
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

            // If player used usedTeleportail to get into the scene, we must research the one in the new scene to teleport player on it.
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

        // Clean projectiles
        Player_Projectile[] p_proj = FindObjectsOfType<Player_Projectile>();
        for (int i = 0; i < p_proj.Length; i++)
        {
            Destroy(p_proj[i].gameObject);
        }
        Enemy_Projectile[] e_proj = FindObjectsOfType<Enemy_Projectile>();
        for (int i = 0; i < e_proj.Length; i++)
        {
            Destroy(e_proj[i].gameObject);
        }
    }

    // To load a new game level. Will put the player into transition scene while waiting.
    IEnumerator LoadNewGameLevel(int levelToLoadBuildIndex, bool usedTeleportail)
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

        TransitionInformationSetter transitionInfoSetter = FindObjectOfType<TransitionInformationSetter>();
        if (transitionInfoSetter)
        {
            transitionInfoSetter.DisplayRandomInfoConfig();
        }

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

            yield return new WaitForEndOfFrame();
        }

        // Disable level to go
        Scene levelToGo = SceneManager.GetSceneByBuildIndex(levelToLoadBuildIndex);
        GameObject rootLevelToGo = levelToGo.GetRootGameObjects()[0];
        rootLevelToGo.SetActive(false);

        // Set scenes cycles
        CheckScenesLoaded(levelToGo);

        // Check for unload ressources to optimize memory
        AsyncOperation unloadingUselessRessources = Resources.UnloadUnusedAssets();

        while (!unloadingUselessRessources.isDone)
        {
            yield return null;
        }

        // Set the ready button to switch to the new game level
        readyButton.onClick.AddListener(() => NewGameLevelReady(levelToLoadBuildIndex, levelFrom.buildIndex, usedTeleportail));
        readyButton.gameObject.SetActive(true);
        loadingBar.gameObject.SetActive(false);
    }

    public void NewGameLevelReady(int newGameLevelBuildIndex, int levelFromBuildIndex, bool usedTeleportail)
    {
        StartCoroutine(SetNewGameLevel(newGameLevelBuildIndex, levelFromBuildIndex, usedTeleportail));
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

        // Set player succes
        if (Player_Success.instance)
        {
            if (!Player_Success.instance.successDatabase.GetSuccessByID(11).isDone ||
                !Player_Success.instance.successDatabase.GetSuccessByID(12).isDone ||
                !Player_Success.instance.successDatabase.GetSuccessByID(13).isDone)
                    Player_Success.instance.SetExploredZone(newGameLevelBuildIndex);
        }
    }

    // To transition to credit scene. Used in SendPlayerToCreditScene.cs who's attach on the last boss of the game and will be activated via AI_Health when he dies.
    public void StartTransitionToCreditScene()
    {
        StartCoroutine(TransitionToCreditScene());
    }

    IEnumerator TransitionToCreditScene()
    {
        // First, check if credit level isn't loaded. If not, load it. Else just return.
        Scene creditLevel = SceneManager.GetSceneByBuildIndex(creditSceneBuildIndex);

        if (!creditLevel.isLoaded)
        {
            // Start by making player not able to open the menus
            if (UI_Player.instance)
            {
                UI_Player.instance.playerCanInteract = false;
                UI_Player.instance.HideAllMenus();
            }

            // Instantiate transition image
            GameObject transitionCanvasInstance = Instantiate(sceneTransitionCanvas);

            // Slow time
            Time.timeScale = 0.5f;

            yield return new WaitForSeconds(3f);

            Player_Combat player_combat = FindObjectOfType<Player_Combat>();
            if (player_combat)
            {
                player_combat.playerCanCombat = false;
            }

            Player_Movement player_movement = FindObjectOfType<Player_Movement>();
            if (player_movement)
            {
                player_movement.canMove = false;
            }

            // Now disable the previous level
            Scene levelFrom = SceneManager.GetActiveScene();
            GameObject rootLevelFrom = levelFrom.GetRootGameObjects()[0]; // Because there is only one root gameobject per game level.
            rootLevelFrom.SetActive(false);

            // Load the credit level
            AsyncOperation levelAsyncLoad = SceneManager.LoadSceneAsync(creditSceneBuildIndex, LoadSceneMode.Additive);

            // Wait until loading is done
            while (!levelAsyncLoad.isDone)
            {
                yield return null;
            }

            // Set the credit scene as the active one.
            Scene loadedCreditScene = SceneManager.GetSceneByBuildIndex(creditSceneBuildIndex); // We must get a new reference to the scene once its loaded.

            SceneManager.SetActiveScene(loadedCreditScene);

            // Reset timescale
            Time.timeScale = 1f;

            // Place player
            if (player_movement)
            {
                player_movement.SetPlayerPosition(5);
            }

            // Set pet's player position
            if (FindObjectOfType<PetMovement>())
            {
                FindObjectOfType<PetMovement>().transform.position = FindObjectOfType<Player_Movement>().transform.position;
            }

            // TODO Play the success music

            // We know there is a Animator attach to the children image of scene transition canvas
            transitionCanvasInstance.GetComponentInChildren<Animator>().SetTrigger("FadeOut");

            // TODO Add a button to skip the credit scene.

            yield return new WaitForSeconds(.5f);

            if (player_movement)
            {
                player_movement.SetPlayerVelocity(new Vector2(1f, 0f) * (Player_Stats.instance.GetSpeed() - 1f));
            }

            yield return new WaitForSeconds(2f);

            Destroy(transitionCanvasInstance.gameObject);
        }
        else
        {
            yield return null;
        }
    }

    // Method used on the continue button from the credit level.
    public void SwitchPlayerToTownFromCredits()
    {
        StartCoroutine(SetPlayerToTownFromCredits());
    }

    IEnumerator SetPlayerToTownFromCredits()
    {
        // Instantiate transition image
        GameObject transitionCanvasInstance = Instantiate(sceneTransitionCanvas);

        // Slow time
        Time.timeScale = 0.5f;

        yield return new WaitForSeconds(3f);

        if (GameObject.Find("Sounds"))
        {
            for (int i = 0; i < GameObject.Find("Sounds").transform.childCount; i++)
            {
                Destroy(GameObject.Find("Sounds").transform.GetChild(i).gameObject);
            }
        }

        Scene levelFrom = SceneManager.GetActiveScene();
        GameObject rootLevelFrom = levelFrom.GetRootGameObjects()[0]; // Because there is only one root gameobject per game level.
        rootLevelFrom.SetActive(false);

        Scene townScene = SceneManager.GetSceneByBuildIndex(TownLevelBuildIndex);
        GameObject rootTownScene = townScene.GetRootGameObjects()[0];
        rootTownScene.SetActive(true);

        SceneManager.SetActiveScene(townScene);

        Player_Combat player_combat = FindObjectOfType<Player_Combat>();
        if (player_combat)
        {
            player_combat.playerCanCombat = true;
        }

        Player_Movement player_movement = FindObjectOfType<Player_Movement>();
        if (player_movement)
        {
            player_movement.SetPlayerPosition(0);
            player_movement.canMove = true;
        }

        if (UI_Player.instance)
        {
            UI_Player.instance.playerCanInteract = true;
        }

        // Set pet's player position
        if (FindObjectOfType<PetMovement>())
        {
            FindObjectOfType<PetMovement>().transform.position = FindObjectOfType<Player_Movement>().transform.position;
        }

        transitionCanvasInstance.GetComponentInChildren<Animator>().SetTrigger("FadeOut");

        Time.timeScale = 1f;

        yield return new WaitForSeconds(3f);

        Destroy(transitionCanvasInstance.gameObject);
    }
}
