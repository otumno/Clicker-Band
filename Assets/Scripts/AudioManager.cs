// AudioManager.cs
using UnityEngine;
using UnityEngine.Audio;
using System.Collections;
using System.Collections.Generic;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [Header("Audio Sources")]
    [SerializeField] private AudioSource globalMusicSource;
    [SerializeField] private AudioSource localMusicSource;

    [Header("Mixer Groups")]
    [SerializeField] private AudioMixer mainMixer;
    [SerializeField] private AudioMixerGroup masterGroup;
    [SerializeField] private AudioMixerGroup musicGroup;
    [SerializeField] private AudioMixerGroup globalMusicGroup;
    [SerializeField] private AudioMixerGroup localMusicGroup;
    [SerializeField] private AudioMixerGroup sfxGroup;
    [SerializeField] private AudioMixerGroup menuSfxGroup;
    [SerializeField] private AudioMixerGroup gameSfxGroup;
    [SerializeField] private AudioMixerGroup metronomeGroup;

    private float defaultGlobalVolume;
    private Coroutine currentGlobalFade;
    private readonly List<AudioSource> audioPool = new();

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        defaultGlobalVolume = globalMusicSource.volume;
        localMusicSource.volume = 0f;
        localMusicSource.playOnAwake = false;
        
        globalMusicSource.outputAudioMixerGroup = globalMusicGroup;
        localMusicSource.outputAudioMixerGroup = localMusicGroup;
    }

    #region Public Methods
    public void UpdateAllVolumes(AudioSettings settings)
    {
        SetMixerVolume("MasterVol", settings.masterVolume);
        SetMixerVolume("MusicVol", settings.musicVolume);
        SetMixerVolume("SFXVol", settings.sfxVolume);
        SetMixerVolume("MenuSFXVol", settings.menuSFXVolume);
        SetMixerVolume("GameSFXVol", settings.gameSFXVolume);
        SetMixerVolume("MetronomeVol", settings.metronomeVolume);
    }

    public void PlayGlobalMusic(AudioClip clip, float volume = 1f, bool fadeIn = false)
    {
        if (clip == null) return;

        globalMusicSource.clip = clip;
        globalMusicSource.volume = fadeIn ? 0f : volume;
        globalMusicSource.loop = true;
        globalMusicSource.Play();

        if (fadeIn) StartCoroutine(FadeGlobalMusic(volume, 1f));
    }

    public void PlayLocalMusic(AudioClip clip, float volume = 1f, bool fadeIn = false)
    {
        if (clip == null) return;

        localMusicSource.clip = clip;
        localMusicSource.volume = fadeIn ? 0f : volume;
        localMusicSource.loop = true;
        localMusicSource.Play();

        if (fadeIn) StartCoroutine(FadeLocalMusic(volume, 1f));
    }

    public void StopLocalMusic(bool fadeOut = false)
    {
        if (fadeOut)
        {
            StartCoroutine(FadeLocalMusic(0f, 1f, true));
        }
        else
        {
            localMusicSource.Stop();
        }
    }

    public void SetGlobalMusicVolume(float volume, float fadeTime = 0f)
    {
        if (fadeTime > 0f)
        {
            if (currentGlobalFade != null) StopCoroutine(currentGlobalFade);
            currentGlobalFade = StartCoroutine(FadeGlobalMusic(volume, fadeTime));
        }
        else
        {
            globalMusicSource.volume = volume;
        }
    }

    public void RestoreGlobalMusic(float fadeTime = 0f)
    {
        SetGlobalMusicVolume(defaultGlobalVolume, fadeTime);
    }

    public void PlaySFX(AudioClip clip, float volume = 1f, AudioCategory category = AudioCategory.SFX)
    {
        if (clip == null) return;

        AudioSource source = GetAvailableAudioSource();
        source.clip = clip;
        source.volume = volume;
        source.outputAudioMixerGroup = GetMixerGroup(category);
        source.Play();
        StartCoroutine(ReturnToPoolAfterPlay(source, clip.length));
    }

    public void PlayMetronomeSound(AudioClip clip, float volume = 1f, float pitch = 1f)
{
    if (clip == null)
    {
        Debug.LogWarning("Metronome clip is null!");
        return;
    }

    if (metronomeGroup == null)
    {
        Debug.LogError("Metronome mixer group not assigned!");
        return;
    }

    AudioSource source = GetAvailableAudioSource();
    source.pitch = Mathf.Clamp(pitch, 0.5f, 2f);
    source.volume = Mathf.Clamp(volume, 0f, 1f);
    source.outputAudioMixerGroup = metronomeGroup;
    source.PlayOneShot(clip);
    StartCoroutine(ReturnToPoolAfterPlay(source, clip.length));
    
    Debug.Log($"Playing metronome sound: {clip.name}");
}

    public void PlayInstrumentSound(AudioClip clip, float volume = 1f, float pitch = 1f)
    {
        if (clip == null) return;

        AudioSource source = GetAvailableAudioSource();
        source.pitch = pitch;
        source.volume = volume;
        source.outputAudioMixerGroup = gameSfxGroup;
        source.PlayOneShot(clip);
        StartCoroutine(ReturnToPoolAfterPlay(source, clip.length));
    }
    #endregion

    #region Private Methods
    private IEnumerator FadeGlobalMusic(float targetVolume, float fadeTime)
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

        if (stopAfterFade) localMusicSource.Stop();
    }

    private void SetMixerVolume(string parameter, float volume)
    {
        if (mainMixer == null) return;
        float dB = volume > 0.0001f ? 20f * Mathf.Log10(volume) : -80f;
        mainMixer.SetFloat(parameter, dB);
    }

    private AudioSource GetAvailableAudioSource()
    {
        AudioSource source = audioPool.Find(a => !a.isPlaying);
        if (source == null)
        {
            source = gameObject.AddComponent<AudioSource>();
            audioPool.Add(source);
        }
        return source;
    }

    private IEnumerator ReturnToPoolAfterPlay(AudioSource source, float delay)
    {
        yield return new WaitForSeconds(delay);
        source.Stop();
        source.clip = null;
    }

    private AudioMixerGroup GetMixerGroup(AudioCategory category)
    {
        return category switch
        {
            AudioCategory.GlobalMusic => globalMusicGroup,
            AudioCategory.LocalMusic => localMusicGroup,
            AudioCategory.SFX => sfxGroup,
            AudioCategory.MenuSFX => menuSfxGroup,
            AudioCategory.GameSFX => gameSfxGroup,
            AudioCategory.Metronome => metronomeGroup,
            _ => masterGroup
        };
    }
    #endregion
}