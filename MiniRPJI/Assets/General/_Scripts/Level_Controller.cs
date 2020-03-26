/* Level_Controller.cs
 * Permet le changement entre les scenes.
 * Est créé a partir du "splashscreen"
 * 
 * */

using UnityEngine;
using UnityEngine.SceneManagement;

public class Level_Controller : MonoBehaviour
{
    public static Level_Controller instance;

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

    public void ChangeLevel(int levelIndex)
    {
        if (levelIndex < SceneManager.sceneCountInBuildSettings)
        {
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
