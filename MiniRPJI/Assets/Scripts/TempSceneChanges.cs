using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TempSceneChanges : MonoBehaviour
{
    public void ChangeScene(int sceneIndex)
    {
        SceneManager.LoadScene(sceneIndex); 
    }

    public void Save()
    {
        GameControl.control.SavePlayerData();
    }

    public void Load()
    {
        GameControl.control.LoadPlayerData();
    }
}
