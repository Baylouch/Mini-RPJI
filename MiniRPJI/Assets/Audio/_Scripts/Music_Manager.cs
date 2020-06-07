using System.Collections;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class Music_Manager : MonoBehaviour
{
    public static Music_Manager instance;
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
    public const string volumeKey = "MUSICVOLUME";
    public const string activeKey = "MUSICACTIVATE"; // 0 = Off   1 = ON

    public bool isInGameLevel = false; // To switch when we enter in gameLevels to play gameMusics

    [SerializeField] AudioClip[] menuMusics;
    [SerializeField] AudioClip[] gameMusics;

    private AudioSource audioSource;
    int musicIndex = 0; // For change music

    bool stoppingMusic = false;
    float stoppingTimer;

    // Start is called before the first frame update
    void Start()
    {
        audioSource = GetComponent<AudioSource>();

        // Check if playerprefs keys exist or create it.
        if (PlayerPrefs.HasKey(volumeKey))
        {
            audioSource.volume = PlayerPrefs.GetFloat(volumeKey);
        }
        else
        {
            PlayerPrefs.SetFloat(volumeKey, volumeMax / 2);
            audioSource.volume = PlayerPrefs.GetFloat(volumeKey);
        }

        if (PlayerPrefs.HasKey(activeKey))
        {
            if (PlayerPrefs.GetInt(activeKey) == 1)
            {
                this.enabled = true; 
            }
            else
            {
                this.enabled = false;
                return;
            }
        }
        else
        {
            PlayerPrefs.SetInt(activeKey, 1);
        }

        PlayRandomMusic();
    }

    private void OnDisable()
    {
        if (audioSource == null)
            return;

        if (audioSource.isPlaying)
            audioSource.Stop();

        audioSource.clip = null;
        stoppingMusic = false;
    }

    private void OnEnable()
    {
        // Security for game start
        if (audioSource == null)
            return;

        if (PlayerPrefs.HasKey(activeKey))
            if (PlayerPrefs.GetInt(activeKey) == 0)
                return;

         PlayRandomMusic();
    }

    // Update is called once per frame
    void Update()
    {
        if (PlayerPrefs.HasKey(activeKey))
            if (PlayerPrefs.GetInt(activeKey) == 0)
                return;

        if (audioSource.clip)
        {
            if (audioSource.isPlaying)
            {
                if (Time.time + 10 >= stoppingTimer && !stoppingMusic)
                {
                    stoppingMusic = true;
                    StartCoroutine(ChangeMusic());
                }
            }
        }       
    }

    // Method used in Start, OnEnable and ToggleGameOrMenuMusics to play randomly a sound.
    void PlayRandomMusic()
    {
        if (PlayerPrefs.HasKey(activeKey))
            if (PlayerPrefs.GetInt(activeKey) == 0)
                return;

        if (!isInGameLevel) // It should be always in there at start because its created in Start_Menu
        {
            musicIndex = Random.Range(0, menuMusics.Length); // we deal with INT so param2 is EXCLUSIVE (Random.Range(0, 2) = 0 || 1)
            audioSource.clip = menuMusics[musicIndex];
        }
        else
        {
            musicIndex = Random.Range(0, gameMusics.Length);
            audioSource.clip = gameMusics[musicIndex];
        }

        stoppingTimer = Time.time + audioSource.clip.length;
        musicIndex++;
        audioSource.Play();
    }

    // Coroutines to end and start music with volume smoothing
    IEnumerator ChangeMusic()
    {
        // Beggin by stop the music
        while (audioSource.volume > 0)
        {
            audioSource.volume -= .03f;
            yield return new WaitForSeconds(.4f);
        }

        audioSource.Stop();
        audioSource.clip = null;
        stoppingMusic = false;

        yield return new WaitForSeconds(1f);

        // then start a new music

        if (!isInGameLevel)
        {
            if (musicIndex >= menuMusics.Length) // If we played all musics reset musicIndex
                musicIndex = 0;

            audioSource.clip = menuMusics[musicIndex];
        }
        else
        {
            if (musicIndex >= gameMusics.Length) // If we played all musics reset musicIndex
                musicIndex = 0;

            audioSource.clip = gameMusics[musicIndex];
        }

        stoppingTimer = Time.time + audioSource.clip.length;
        musicIndex++;
        audioSource.Play();

        float volumeInPlayerPrefs = 0f;

        if (PlayerPrefs.HasKey(volumeKey))
        {
            volumeInPlayerPrefs = PlayerPrefs.GetFloat(volumeKey);

            while (audioSource.volume < volumeInPlayerPrefs)
            {
                audioSource.volume += .03f;
                if (audioSource.volume > volumeInPlayerPrefs)
                    audioSource.volume = volumeInPlayerPrefs;

                yield return new WaitForSeconds(.4f);
            }
        }
        else
        {
            while (audioSource.volume < volumeMax)
            {
                audioSource.volume += .03f;
                if (audioSource.volume > volumeMax)
                    audioSource.volume = volumeMax;

                yield return new WaitForSeconds(.4f);
            }
        }
    }

    // TODO Delete StopMusic & StartMusic ? (Because ChangeMusic is a combinaison of them)
    IEnumerator StopMusic()
    {
        while (audioSource.volume > 0)
        {
            audioSource.volume -= .03f;
            yield return new WaitForSeconds(.4f);
        }

        audioSource.Stop();
        audioSource.clip = null;
        stoppingMusic = false;

        yield return new WaitForSeconds(1f);

        StartCoroutine(StartMusic());
    }

    IEnumerator StartMusic()
    {
        if (!isInGameLevel) // It should be always in there at start because its created in Start_Menu
        {
            musicIndex = Random.Range(0, menuMusics.Length); // we deal with INT so param2 is EXCLUSIVE (Random.Range(0, 2) = 0 || 1)
            audioSource.clip = menuMusics[musicIndex];
        }
        else
        {
            musicIndex = Random.Range(0, gameMusics.Length);
            audioSource.clip = gameMusics[musicIndex];
        }

        stoppingTimer = Time.time + audioSource.clip.length;
        musicIndex++;
        audioSource.Play();

        while (audioSource.volume < volumeMax)
        {
            audioSource.volume += .03f;
            if (audioSource.volume > volumeMax)
                audioSource.volume = volumeMax;

            yield return new WaitForSeconds(.4f);
        }
    }

    // Method used into Scenes_Control to switch between menu or game musics.
    // Set parameter true when you enter in game levels, set false to back from game to menu
    public void ToggleGameOrMenuMusics(bool menuToGame)
    {
        if (isInGameLevel != menuToGame)
        {
            isInGameLevel = menuToGame;
            audioSource.Stop();

            PlayRandomMusic();
        }
    }

    // Methods use in options to set volume
    public void SetVolume(float volume)
    {
        audioSource.volume = volume;
    }
}
