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

    [SerializeField] GameObject optionsPanel;
    [SerializeField] GameObject commandsPanel;
    [SerializeField] Slider musicVolumeSlider;
    [SerializeField] Toggle toggleMusic;
    [SerializeField] Slider soundVolumeSlider;
    [SerializeField] Toggle toggleSound;
    [SerializeField] Toggle togglePopUp;

    [SerializeField] GameObject playerDataToSet;

    private void Start()
    {
        HideConfirmationWindow();
        HideAndResetSaveSlots();
        HideCommands();
        HideOptionsGame();

        Time.timeScale = 0;
    }

    private void OnDestroy()
    {
        Time.timeScale = 1;
    }

    void InstantiatePlayerDataSetter(int saveIndex)
    {
        GameObject _playerData = Instantiate(playerDataToSet);
        _playerData.GetComponent<Player_Data_Setter>().dataToSet = saveIndex;
    }

    public void ContinueGame()
    {
        UI_Player.instance.TogglePlayerMenu();
    }

    public void SaveGame()
    {
        SaveSlotsUI.SetActive(true);

        for (int i = 0; i < saveSlots.Length; i++)
        {
            if (Game_Data_Control.data_instance)
            {
                int x = i;
                // Change button style if there is a save.
                // and add a listener on each for save the game. Add a confirmation UI if there is already a save on the slot.
                // "Etes-vous sûr de vouloir écraser les données ?" Oui / Non.
                if (Game_Data_Control.data_instance.GetLoadData(i) != null)
                {                    
                    PlayerData currentData = Game_Data_Control.data_instance.GetLoadData(i);
                    saveSlots[i].SetDataButton(currentData.playerStats.level, currentData.playerInventory.gold);
                    saveSlots[i].GetComponent<Button>().onClick.AddListener(() => DisplaySaveConfirmationWindow(x));                  
                }
                else
                {
                    saveSlots[i].SetNoDataButton();
                    saveSlots[i].GetComponent<Button>().onClick.AddListener(() => Game_Data_Control.data_instance.SavePlayerData(x));
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
            if (Game_Data_Control.data_instance)
            {
                int x = i;

                if (Game_Data_Control.data_instance.GetLoadData(i) != null)
                {
                    PlayerData currentData = Game_Data_Control.data_instance.GetLoadData(i);
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

    public void QuitGame()
    {
        // Security if player was on Save / Load buttons
        HideAndResetSaveSlots();
        HideConfirmationWindow();

        confirmationUI.SetActive(true);

        confirmationText.text = "Quitter le jeu ?";

        if (Scenes_Control.instance)
        {
            validationButton.onClick.AddListener(() => Scenes_Control.instance.ChangeLevel("Start_Menu"));
           
        }
    }

    public void DisplaySaveConfirmationWindow(int saveIndex)
    {
        // Just a security
        HideConfirmationWindow();

        confirmationUI.SetActive(true);

        confirmationText.text = "Ecraser les données ?";
        validationButton.onClick.AddListener(() => Game_Data_Control.data_instance.SavePlayerData(saveIndex));
        validationButton.onClick.AddListener(HideAndResetSaveSlots);
        validationButton.onClick.AddListener(HideConfirmationWindow);
    }

    public void DisplayLoadConfirmationWindow(int saveIndex)
    {
        // Just a security
        HideConfirmationWindow();

        confirmationUI.SetActive(true);

        confirmationText.text = "Charger les données ?";

        if (Scenes_Control.instance)
        {
            validationButton.onClick.AddListener(() => Destroy(Player_Stats.instance.gameObject)); // We must destroy player gameobject to avoid issue with data setter
            validationButton.onClick.AddListener(() => Scenes_Control.instance.LoadTownLevel());
            validationButton.onClick.AddListener(() => InstantiatePlayerDataSetter(saveIndex));
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

    // Options methods
    #region Options
    public void OptionsGame()
    {
        SetOptionsElements();
        optionsPanel.SetActive(true);
    }

    public void HideOptionsGame()
    {
        optionsPanel.SetActive(false);
    }

    public void DisplayCommands()
    {
        commandsPanel.SetActive(true);
    }

    public void HideCommands()
    {
        commandsPanel.SetActive(false);
    }


    // TODO See if there is a way to use Options_Menu_Controller.cs here too (because its the same code here)
    void SetOptionsElements()
    {
        // Musique options
        if (musicVolumeSlider)
        {
            if (PlayerPrefs.HasKey(Music_Manager.volumeKey))
            {
                musicVolumeSlider.value = PlayerPrefs.GetFloat(Music_Manager.volumeKey);
            }
        }

        if (toggleMusic)
        {
            if (PlayerPrefs.HasKey(Music_Manager.activeKey))
            {
                if (PlayerPrefs.GetInt(Music_Manager.activeKey) == 1)
                {
                    toggleMusic.isOn = true;
                }
                else
                {
                    toggleMusic.isOn = false;
                }
            }
        }

        // Sound options
        if (soundVolumeSlider)
        {
            if (PlayerPrefs.HasKey(Sound_Manager.volumeKey))
            {
                soundVolumeSlider.value = PlayerPrefs.GetFloat(Sound_Manager.volumeKey);
            }
        }

        if (toggleSound)
        {
            if (PlayerPrefs.HasKey(Sound_Manager.activeKey))
            {
                if (PlayerPrefs.GetInt(Sound_Manager.activeKey) == 1)
                {
                    toggleSound.isOn = true;
                }
                else
                {
                    toggleSound.isOn = false;
                }
            }
        }

        // PopUp options
        if (togglePopUp)
        {
            if (PlayerPrefs.HasKey(UI_SuccessDisplayer.activePopUpKey))
            {
                if (PlayerPrefs.GetInt(UI_SuccessDisplayer.activePopUpKey) == 1)
                {
                    togglePopUp.isOn = true;
                }
                else
                {
                    togglePopUp.isOn = false;
                }
            }
        }
    }

    public void SetMusicVolume(float value)
    {
        if (Music_Manager.instance)
        {
            Music_Manager.instance.SetVolume(value);
            PlayerPrefs.SetFloat(Music_Manager.volumeKey, value);
        }
    }

    public void SetSoundVolume(float value)
    {
        if (Sound_Manager.instance)
        {
            Sound_Manager.instance.SetVolume(value);
            PlayerPrefs.SetFloat(Sound_Manager.volumeKey, value);
        }
    }

    public void ActiveMusic(bool value)
    {
        if (Music_Manager.instance)
        {
            // 0 = unactive  1 = active
            if (value == true)
            {
                PlayerPrefs.SetInt(Music_Manager.activeKey, 1);
            }
            else
            {
                PlayerPrefs.SetInt(Music_Manager.activeKey, 0);
            }

            Music_Manager.instance.enabled = value;
        }
    }

    public void ActiveSound(bool value)
    {
        if (Sound_Manager.instance)
        {
            if (value == true)
            {
                PlayerPrefs.SetInt(Sound_Manager.activeKey, 1);
            }
            else
            {
                PlayerPrefs.SetInt(Sound_Manager.activeKey, 0);
            }

            Sound_Manager.instance.ToggleSound(value);
        }
    }

    public void ActivePopUp(bool value)
    {
        if (UI_SuccessDisplayer.instance)
        {
            if (value == true)
            {
                PlayerPrefs.SetInt(UI_SuccessDisplayer.activePopUpKey, 1);
            }
            else
            {
                PlayerPrefs.SetInt(UI_SuccessDisplayer.activePopUpKey, 0);
            }

            UI_SuccessDisplayer.instance.TogglePopUp(value);
        }
    }

    #endregion
}
