using UnityEngine;
using UnityEngine.UI;

public class Options_Menu_Controller : MonoBehaviour
{
    [SerializeField] Slider musicVolumeSlider;
    [SerializeField] Toggle toggleMusic;

    [SerializeField] Slider soundVolumeSlider;
    [SerializeField] Toggle toggleSound;

    [SerializeField] Toggle toggleFullScreen;

    [SerializeField] GameObject CommandsPanel;
    [SerializeField] GameObject ZQSDShortcutsPanel;
    [SerializeField] GameObject ARROWSShortcutsPanel;

    private void Start()
    {
        if (CommandsPanel.activeSelf)
            CommandsPanel.SetActive(false);

        SetOptionsElements();
    }

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

        if (toggleFullScreen)
        {
            if (Screen.fullScreen)
            {
                toggleFullScreen.isOn = true;
            }
            else
            {
                toggleFullScreen.isOn = false;
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

    public void DisplayCommands(bool value)
    {
        if (value == true)
        {
            if (Player_Shortcuts.GetShortCuts() == 0)
            {
                ZQSDShortcutsPanel.SetActive(true);
                ARROWSShortcutsPanel.SetActive(false);
            }
            else
            {
                ZQSDShortcutsPanel.SetActive(false);
                ARROWSShortcutsPanel.SetActive(true);
            }
        }

        CommandsPanel.SetActive(value);
    }

    public void SwitchShortCuts()
    {
        if (Player_Shortcuts.GetShortCuts() == 0)
        {
            Player_Shortcuts.SetShortCuts(1);
        }
        else
        {
            Player_Shortcuts.SetShortCuts(0);
        }

        DisplayCommands(true);
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

    public void ToggleFullScreen()
    {
        if (!Screen.fullScreen)
        {
            toggleFullScreen.isOn = true;
            Screen.fullScreen = true;
        }
        else
        {
            toggleFullScreen.isOn = false;
            Screen.fullScreen = false;
        }
    }
}
