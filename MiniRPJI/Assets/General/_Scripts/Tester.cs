using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Tester : MonoBehaviour
{

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.M))
        {
            Scenes_Control.instance.LoadGameLevels();
        }
    }

    //IEnumerator LoadAsyncLevel(int sceneIndex)
    //{
    //    AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneIndex, LoadSceneMode.Additive);
    //    asyncLoad.allowSceneActivation = false;
        
    //    while (!asyncLoad.isDone)
    //    {
    //        if (asyncLoad.progress >= 0.9f)
    //        {
    //            if (Input.GetKeyDown(KeyCode.M))
    //            {
    //                asyncLoad.allowSceneActivation = true;
    //            }
    //        }
            
    //        yield return null;
    //    }
    //}

    //IEnumerator UnLoadAsyncLevel(int sceneIndex)
    //{
    //    AsyncOperation asyncUnload = SceneManager.UnloadSceneAsync(sceneIndex);

    //    while (!asyncUnload.isDone)
    //    {
    //        yield return null;
    //    }
    //}

}
