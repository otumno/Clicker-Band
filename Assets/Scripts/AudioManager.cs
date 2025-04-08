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

    #region Global Music Control
    public void PlayGlobalMusic(AudioClip clip, float volume = 1f, bool fadeIn = false)
    {
        if (clip == null) return;

        globalMusicSource.clip = clip;
        globalMusicSource.volume = fadeIn ? 0f : volume;
        globalMusicSource.loop = true;
        globalMusicSource.Play();

        if (fadeIn)
        {
            SetGlobalMusicVolume(volume, 1f);
        }
    }

    public void SetGlobalMusicVolume(float volume, float fadeTime = 0f)
    {
        if (fadeTime > 0f)
        {
            if (currentGlobalFade != null)
            {
                StopCoroutine(currentGlobalFade);
            }
            currentGlobalFade = StartCoroutine(FadeGlobalMusic(volume, fadeTime));
        }
        else
        {
            globalMusicSource.volume = volume;
        }
    }

    public IEnumerator FadeGlobalMusic(float targetVolume, float fadeTime)
    {
        float startVolume = globalMusicSource.volume;
        float elapsedTime = 0f;

        while (elapsedTime < fadeTime)
        {
            globalMusicSource.volume = Mathf.Lerp(startVolume, targetVolume, elapsedTime / fadeTime);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        globalMusicSource.volume = targetVolume;
        currentGlobalFade = null;
    }

    public void RestoreGlobalMusic(float fadeTime = 0f)
    {
        SetGlobalMusicVolume(defaultGlobalVolume, fadeTime);
    }
    #endregion

    #region Local Music Control
    public void PlayLocalMusic(AudioClip clip, float volume = 1f, bool fadeIn = false)
    {
        if (clip == null) return;

        if (currentLocalFade != null)
        {
            StopCoroutine(currentLocalFade);
        }

        localMusicSource.clip = clip;
        localMusicSource.volume = fadeIn ? 0f : volume;
        localMusicSource.loop = true;
        localMusicSource.Play();

        if (fadeIn)
        {
            currentLocalFade = StartCoroutine(FadeLocalMusic(volume, 1f));
        }
    }

    public void StopLocalMusic(bool fadeOut = false)
    {
        if (fadeOut)
        {
            if (currentLocalFade != null)
            {
                StopCoroutine(currentLocalFade);
            }
            currentLocalFade = StartCoroutine(FadeLocalMusic(0f, 1f, true));
        }
        else
        {
            localMusicSource.Stop();
        }
    }

    private IEnumerator FadeLocalMusic(float targetVolume, float fadeTime, bool stopAfterFade = false)
    {
        float startVolume = localMusicSource.volume;
        float elapsedTime = 0f;

        while (elapsedTime < fadeTime)
        {
            localMusicSource.volume = Mathf.Lerp(startVolume, targetVolume, elapsedTime / fadeTime);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        localMusicSource.volume = targetVolume;

        if (stopAfterFade && targetVolume <= 0f)
        {
            localMusicSource.Stop();
        }

        currentLocalFade = null;
    }
    #endregion

    #region SFX Methods
    public void PlaySFX(AudioClip clip, float volume = 1f, AudioCategory category = AudioCategory.SFX)
    {
        if (clip == null) return;

        GameObject tempGO = new GameObject("TempAudio_SFX");
        AudioSource audioSource = tempGO.AddComponent<AudioSource>();
        
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
}

public enum AudioCategory
{
    SFX,
    MenuSFX,
    GameSFX
}