/* UI_Player_Menu.cs
 * Utilisé pour accéder au menu permettant de mettre le jeu en PAUSE, de SAUVEGARDER, CHARGER, acceder aux OPTIONS et QUITTER.
 * OnEnable = pause, OnDisable = stop la pause
 * 
 * */

using UnityEngine;
using UnityEngine.UI;

public class UI_Player_Menu : MonoBehaviour
{
    [SerializeField] GameObject SaveSlotsUI;
    [SerializeField] Load_Button[] saveSlots;

    [SerializeField] GameObject confirmationUI;
    [SerializeField] Button validationButton;
    [SerializeField] Button cancelButton;
    [SerializeField] Text confirmationText;

    [SerializeField] GameObject playerDataToSet;

    private void Start()
    {
        HideConfirmationWindow();
        HideAndResetSaveSlots();
    }

    private void OnEnable()
    {
        Time.timeScale = 0;
    }

    private void OnDisable()
    {
        HideConfirmationWindow();
        HideAndResetSaveSlots();

        Time.timeScale = 1;
    }

    void InstantiatePlayerDataSetter(int saveIndex)
    {
        GameObject _playerData = Instantiate(playerDataToSet);
        _playerData.GetComponent<PlayerDataSetter>().dataToSet = saveIndex;
    }

    public void ContinueGame()
    {
        gameObject.SetActive(false);
    }

    public void SaveGame()
    {
        SaveSlotsUI.SetActive(true);

        for (int i = 0; i < saveSlots.Length; i++)
        {
            if (GameDataControl.dataControl_instance)
            {
                int x = i;
                // Change button style if there is a save.
                // and add a listener on each for save the game. Add a confirmation UI if there is already a save on the slot.
                // "Etes-vous sûr de vouloir écraser les données ?" Oui / Non.
                if (GameDataControl.dataControl_instance.GetLoadData(i) != null)
                {                    
                    PlayerData currentData = GameDataControl.dataControl_instance.GetLoadData(i);
                    saveSlots[i].SetDataButton(currentData.playerStats.level, currentData.playerInventory.gold);
                    saveSlots[i].GetComponent<Button>().onClick.AddListener(() => DisplaySaveConfirmationWindow(x));                  
                }
                else
                {
                    saveSlots[i].SetNoDataButton();
                    saveSlots[i].GetComponent<Button>().onClick.AddListener(() => GameDataControl.dataControl_instance.SavePlayerData(x));
                    saveSlots[i].GetComponent<Button>().onClick.AddListener(HideAndResetSaveSlots);
                }
            }
        }
    }

    public void LoadGame()
    {
        SaveSlotsUI.SetActive(true);

        for (int i = 0; i < saveSlots.Length; i++)
        {
            if (GameDataControl.dataControl_instance)
            {
                int x = i;

                if (GameDataControl.dataControl_instance.GetLoadData(i) != null)
                {
                    PlayerData currentData = GameDataControl.dataControl_instance.GetLoadData(i);
                    saveSlots[i].SetDataButton(currentData.playerStats.level, currentData.playerInventory.gold);
                    saveSlots[i].GetComponent<Button>().onClick.AddListener(() => DisplayLoadConfirmationWindow(x));
                }
                else
                {
                    saveSlots[i].SetNoDataButton();
                }
            }
        }
    }

    // TODO Make public void OptionsGame()

    public void QuitGame()
    {
        // Security if player was on Save / Load buttons
        HideAndResetSaveSlots();
        HideConfirmationWindow();

        confirmationUI.SetActive(true);

        confirmationText.text = "Quitter le jeu ?";

        if (Level_Controller.instance)
        {
            validationButton.onClick.AddListener(() => Level_Controller.instance.ChangeLevel("Level_Menu"));
           
        }
    }

    public void DisplaySaveConfirmationWindow(int saveIndex)
    {
        // Just a security
        HideConfirmationWindow();

        confirmationUI.SetActive(true);

        confirmationText.text = "Ecraser les données ?";
        validationButton.onClick.AddListener(() => GameDataControl.dataControl_instance.SavePlayerData(saveIndex));
        validationButton.onClick.AddListener(HideAndResetSaveSlots);
        validationButton.onClick.AddListener(HideConfirmationWindow);
    }

    public void DisplayLoadConfirmationWindow(int saveIndex)
    {
        // Just a security
        HideConfirmationWindow();

        confirmationUI.SetActive(true);

        confirmationText.text = "Charger les données ?";

        if (Level_Controller.instance)
        {
            validationButton.onClick.AddListener(() => InstantiatePlayerDataSetter(saveIndex));
            validationButton.onClick.AddListener(() => Level_Controller.instance.ChangeLevel("Level_1"));
        }
        else
        {
            Debug.Log("No Level Controller !");
        }

        validationButton.onClick.AddListener(HideAndResetSaveSlots);
        validationButton.onClick.AddListener(HideConfirmationWindow);
    }

    public void HideAndResetSaveSlots()
    {
        SaveSlotsUI.SetActive(false);

        for (int i = 0; i < saveSlots.Length; i++)
        {
            saveSlots[i].GetComponent<Button>().onClick.RemoveAllListeners();
        }
    }

    public void HideConfirmationWindow()
    {
        confirmationUI.SetActive(false);

        confirmationText.text = "";

        validationButton.onClick.RemoveAllListeners();
    }
}
