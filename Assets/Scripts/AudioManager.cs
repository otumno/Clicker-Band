using UnityEngine;
using UnityEngine.Audio;
using System.Collections;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [Header("Audio Sources")]
    [SerializeField] private AudioSource globalMusicSource;
    [SerializeField] private AudioSource localMusicSource;

    [Header("Mixer Groups")]
    [SerializeField] private AudioMixer mainMixer;
    [SerializeField] private AudioMixerGroup musicMixerGroup;
    [SerializeField] private AudioMixerGroup sfxMixerGroup;
    [SerializeField] private AudioMixerGroup menuSfxMixerGroup;

    private float defaultGlobalVolume;
    private Coroutine currentGlobalFade;
    private Coroutine currentLocalFade;
    private bool isLocalMusicPlaying = false;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        defaultGlobalVolume = globalMusicSource.volume;
        localMusicSource.volume = 0f;
        localMusicSource.playOnAwake = false;
        
        // Назначаем группы по умолчанию
        globalMusicSource.outputAudioMixerGroup = musicMixerGroup;
        localMusicSource.outputAudioMixerGroup = musicMixerGroup;
    }

    #region Volume Control
    public void UpdateAllVolumes(AudioSettings settings)
    {
        SetMixerVolume("MasterVol", settings.masterVolume);
        SetMixerVolume("MusicVol", settings.musicVolume);
        SetMixerVolume("SFXVol", settings.sfxVolume);
        SetMixerVolume("MenuSFXVol", settings.menuSFXVolume);
        SetMixerVolume("GameSFXVol", settings.gameSFXVolume);
        SetMixerVolume("MetronomeVol", settings.metronomeVolume);
    }

    public void SetMixerVolume(string parameter, float volume)
    {
        float dB = volume > 0.0001f ? 20f * Mathf.Log10(volume) : -80f;
        mainMixer.SetFloat(parameter, dB);
    }
    #endregion

    #region SFX Methods
    public void PlaySFX(AudioClip clip, float volume = 1f, AudioCategory category = AudioCategory.SFX)
    {
        if (clip == null) return;

        GameObject tempGO = new GameObject("TempAudio_SFX");
        AudioSource audioSource = tempGO.AddComponent<AudioSource>();
        
        // Настройка AudioSource
        audioSource.clip = clip;
        audioSource.volume = volume;
        audioSource.outputAudioMixerGroup = GetMixerGroup(category);
        audioSource.playOnAwake = false;
        audioSource.loop = false;
        
        audioSource.Play();
        Destroy(tempGO, clip.length);
    }

    private AudioMixerGroup GetMixerGroup(AudioCategory category)
    {
        return category switch
        {
            AudioCategory.MenuSFX => menuSfxMixerGroup,
            AudioCategory.SFX => sfxMixerGroup,
            _ => null
        };
    }
    #endregion

    #region Music Control
    // ... (остальные методы управления музыкой без изменений)
    #endregion
}

public enum AudioCategory
{
    SFX,
    MenuSFX,
    GameSFX
}