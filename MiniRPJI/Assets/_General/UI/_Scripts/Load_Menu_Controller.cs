﻿/* Load_Controller.cs
 * Est utilisé dans Level_Load afin de charger les sauvegardes disponibles.
 * Les boutons affiche si une sauvegarde est disponible ou non, et affiche les elements
 * de la sauvegarde (level, or...)
 * 
 * */
using UnityEngine;
using UnityEngine.UI;

public class Load_Menu_Controller : MonoBehaviour
{
    [SerializeField] Load_Button[] loadButtons;

    [SerializeField] GameObject confirmationUI;
    [SerializeField] Text confirmationText;
    [SerializeField] Button loadSlotButton;
    [SerializeField] Button deleteSlotButton;
    [SerializeField] Button cancelButton;

    [SerializeField] GameObject playerDataToSet;

    // Start is called before the first frame update
    void Start()
    {
        HideConfirmationWindow();


        if (Game_Data_Control.data_instance)
        {
            // all is ok
            // Setup buttons
            for (int i = 0; i < loadButtons.Length; i++)
            {
                if (Game_Data_Control.data_instance.GetLoadData(i) != null)
                {
                    int x = i; // We need to do like this to pass a loop parameter to a button listener. Because of an issue with Unity.
                    PlayerData currentData = Game_Data_Control.data_instance.GetLoadData(i);
                    loadButtons[i].SetDataButton(currentData.playerStats.level, currentData.playerInventory.gold);
                    loadButtons[i].GetComponent<Button>().onClick.AddListener(() => DisplayConfirmationWindow(x));
                }
                else
                {
                    loadButtons[i].SetNoDataButton();
                }
            }
        }
        else
        {
            Debug.Log("No Game Data Control instance found.");
        }
    }

    void InstantiatePlayerDataSetter(int saveIndex)
    {
        GameObject _playerData = Instantiate(playerDataToSet);
        _playerData.GetComponent<Player_Data_Setter>().dataToSet = saveIndex;
    }

    public void BackToMenu()
    {
        if (Scenes_Control.instance)
        {
            Scenes_Control.instance.ChangeLevel("Start_Menu");
        }
        else
        {
            Debug.Log("No level controller !");
        }
    }

    public void DisplayConfirmationWindow(int saveIndex)
    {
        // Just a security
        HideConfirmationWindow();

        confirmationText.text = "Modification on slot " + (saveIndex + 1).ToString();
        confirmationUI.SetActive(true);

        if (Scenes_Control.instance)
        {
            loadSlotButton.onClick.AddListener(() => InstantiatePlayerDataSetter(saveIndex));
            loadSlotButton.onClick.AddListener(() => Scenes_Control.instance.LoadTownLevel());
        }
        else
        {
            Debug.Log("No Level Controller !");
        }

        deleteSlotButton.onClick.AddListener(() => Game_Data_Control.data_instance.RemoveData(saveIndex));
        deleteSlotButton.onClick.AddListener(() => loadButtons[saveIndex].SetNoDataButton());
        deleteSlotButton.onClick.AddListener(() => loadButtons[saveIndex].GetComponent<Button>().onClick.RemoveAllListeners());
        deleteSlotButton.onClick.AddListener(HideConfirmationWindow);
    }

    public void HideConfirmationWindow()
    {
        confirmationUI.SetActive(false);

        loadSlotButton.onClick.RemoveAllListeners();
        deleteSlotButton.onClick.RemoveAllListeners();
    }
}
