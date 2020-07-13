using UnityEngine;

public class Sound_Manager : MonoBehaviour
{
    public static Sound_Manager instance;
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public const float volumeMax = .5f; // TODO move into options ?

    // Player prefs consts
    public const string volumeKey = "SOUNDVOLUME";
    public const string activeKey = "SOUNDACTIVATE"; // 0 = Off   1 = ON

    public Sounds_Asset asset;

    float volume;
    bool playSound = true;

    private void Start()
    {
        // Check if playerprefs keys exist or create it.
        if (PlayerPrefs.HasKey(volumeKey))
        {
            volume = PlayerPrefs.GetFloat(volumeKey);
        }
        else
        {
            PlayerPrefs.SetFloat(volumeKey, volumeMax);
            volume = PlayerPrefs.GetFloat(volumeKey);
        }

        if (PlayerPrefs.HasKey(activeKey))
        {
            if (PlayerPrefs.GetInt(activeKey) == 1)
            {
                playSound = true;
            }
            else
            {
                playSound = false;
                return;
            }
        }
        else
        {
            PlayerPrefs.SetInt(activeKey, 1);
            playSound = true;
        }
    }

    public void PlaySound(AudioClip sound, Transform soundPosition = null)
    {
        if (!playSound)
            return;

        GameObject newSound = new GameObject(sound.name);

        if (GameObject.Find("Sounds"))
        {
            newSound.transform.parent = GameObject.Find("Sounds").transform;

        }

        if (soundPosition)
            newSound.transform.position = soundPosition.position;

        AudioSource newAudio = newSound.AddComponent<AudioSource>();

        newAudio.clip = sound;
        newAudio.volume = volume;

        newAudio.Play();
        Destroy(newSound, newAudio.clip.length);
    }

    // Methods use in options to set volume
    public void SetVolume(float volume)
    {
        this.volume = volume;
    }

    public void ToggleSound(bool value)
    {
        playSound = value;
    }
}
