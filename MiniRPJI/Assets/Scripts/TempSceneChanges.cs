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
        GameDataControl.dataControl_instance.SavePlayerData();
    }

    public void Load()
    {
        GameDataControl.dataControl_instance.LoadPlayerData();
    }
}
