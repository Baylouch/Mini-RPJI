using UnityEngine;

// This enum define the npc voice actor.
public enum NPC_VOICE { Karen, Meghan, Alex, Ian, Sean, None };
// This one is for the type of interaction.
public enum NPC_Interaction { Greetings, Farewell, Completion };

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

    AudioSource lastAudio = null;
    float lastTimePlayed = 0;

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

    public void PlayNPCSound(NPC_VOICE _voice, NPC_Interaction _interaction)
    {
        AudioClip soundToPlay = null;

        switch (_voice)
        {
            case NPC_VOICE.Karen:
                switch (_interaction)
                {
                    case NPC_Interaction.Greetings:
                        soundToPlay = asset.karenGreetings[Random.Range(0, asset.karenGreetings.Length)];

                        if (soundToPlay)
                            PlaySound(soundToPlay);

                        break;
                    case NPC_Interaction.Farewell:
                        soundToPlay = asset.karenFarewell[Random.Range(0, asset.karenFarewell.Length)];

                        if (soundToPlay)
                            PlaySound(soundToPlay);

                        break;
                    case NPC_Interaction.Completion:
                        soundToPlay = asset.karenCompletion[Random.Range(0, asset.karenCompletion.Length)];

                        if (soundToPlay)
                            PlaySound(soundToPlay);

                        break;
                }
                break;
            case NPC_VOICE.Meghan:
                switch (_interaction)
                {
                    case NPC_Interaction.Greetings:
                        soundToPlay = asset.meghanGreetings[Random.Range(0, asset.meghanGreetings.Length)];

                        if (soundToPlay)
                            PlaySound(soundToPlay);

                        break;
                    case NPC_Interaction.Farewell:
                        soundToPlay = asset.meghanFarewell[Random.Range(0, asset.meghanFarewell.Length)];

                        if (soundToPlay)
                            PlaySound(soundToPlay);

                        break;
                    case NPC_Interaction.Completion:
                        soundToPlay = asset.meghanCompletion[Random.Range(0, asset.meghanCompletion.Length)];

                        if (soundToPlay)
                            PlaySound(soundToPlay);

                        break;
                }
                break;
            case NPC_VOICE.Alex:
                switch (_interaction)
                {
                    case NPC_Interaction.Greetings:
                        soundToPlay = asset.alexGreetings[Random.Range(0, asset.alexGreetings.Length)];

                        if (soundToPlay)
                            PlaySound(soundToPlay);

                        break;
                    case NPC_Interaction.Farewell:
                        soundToPlay = asset.alexFarewell[Random.Range(0, asset.alexFarewell.Length)];

                        if (soundToPlay)
                            PlaySound(soundToPlay);

                        break;
                    case NPC_Interaction.Completion:
                        soundToPlay = asset.alexCompletion[Random.Range(0, asset.alexCompletion.Length)];

                        if (soundToPlay)
                            PlaySound(soundToPlay);

                        break;
                }
                break;
            case NPC_VOICE.Ian:
                switch (_interaction)
                {
                    case NPC_Interaction.Greetings:
                        soundToPlay = asset.ianGreetings[Random.Range(0, asset.ianGreetings.Length)];

                        if (soundToPlay)
                            PlaySound(soundToPlay);

                        break;
                    case NPC_Interaction.Farewell:
                        soundToPlay = asset.ianFarewell[Random.Range(0, asset.ianFarewell.Length)];

                        if (soundToPlay)
                            PlaySound(soundToPlay);

                        break;
                    case NPC_Interaction.Completion:
                        soundToPlay = asset.ianCompletion[Random.Range(0, asset.ianCompletion.Length)];

                        if (soundToPlay)
                            PlaySound(soundToPlay);

                        break;
                }
                break;
            case NPC_VOICE.Sean:
                switch (_interaction)
                {
                    case NPC_Interaction.Greetings:
                        soundToPlay = asset.seanGreetings[Random.Range(0, asset.seanGreetings.Length)];

                        if (soundToPlay)
                            PlaySound(soundToPlay);

                        break;
                    case NPC_Interaction.Farewell:
                        soundToPlay = asset.seanFarewell[Random.Range(0, asset.seanFarewell.Length)];

                        if (soundToPlay)
                            PlaySound(soundToPlay);

                        break;
                    case NPC_Interaction.Completion:
                        soundToPlay = asset.seanCompletion[Random.Range(0, asset.seanCompletion.Length)];

                        if (soundToPlay)
                            PlaySound(soundToPlay);

                        break;
                }
                break;
            default:
                break;
        }
    }

    public void PlaySound(AudioClip sound, Transform soundPosition = null)
    {
        if (lastAudio && lastAudio.clip == sound && Time.time - 1f < lastTimePlayed)
            return;

        if (!playSound)
            return;

        GameObject newSound = new GameObject(sound.name);

        if (GameObject.Find("Sounds"))
        {
            newSound.transform.parent = GameObject.Find("Sounds").transform;
        }
        else
        {
            GameObject newSoundHierarchy = new GameObject("Sounds");

            newSound.transform.parent = newSoundHierarchy.transform;
        }

        if (soundPosition)
            newSound.transform.position = soundPosition.position;

        lastAudio = newSound.AddComponent<AudioSource>();

        lastAudio.clip = sound;
        lastAudio.volume = volume;

        lastAudio.Play();
        lastTimePlayed = Time.time;
        Destroy(newSound, lastAudio.clip.length);
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
