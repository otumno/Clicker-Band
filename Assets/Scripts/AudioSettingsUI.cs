using UnityEngine;
using UnityEngine.UI;

public class AudioSettingsUI : MonoBehaviour
{
    [SerializeField] private AudioSettings audioSettings;

    [Header("Sliders")]
    [SerializeField] private Slider masterSlider;
    [SerializeField] private Slider musicSlider;
    [SerializeField] private Slider sfxSlider;
    [SerializeField] private Slider menuSfxSlider;
    [SerializeField] private Slider gameSfxSlider;
    [SerializeField] private Slider metronomeSlider;

    // Ключи для PlayerPrefs
    private const string MASTER_KEY = "MasterVolume";
    private const string MUSIC_KEY = "MusicVolume";
    private const string SFX_KEY = "SFXVolume";
    private const string MENU_SFX_KEY = "MenuSFXVolume";
    private const string GAME_SFX_KEY = "GameSFXVolume";
    private const string METRONOME_KEY = "MetronomeVolume";

    private void Start()
    {
        // Загрузка сохранённых значений или значений по умолчанию из AudioSettings
        LoadSavedVolumes();

        // Подписка на события изменения слайдеров
        SubscribeSliders();

        // Первоначальное обновление громкости
        AudioManager.Instance?.UpdateAllVolumes(audioSettings);
    }

    private void LoadSavedVolumes()
    {
        masterSlider.value = PlayerPrefs.GetFloat(MASTER_KEY, audioSettings.masterVolume);
        musicSlider.value = PlayerPrefs.GetFloat(MUSIC_KEY, audioSettings.musicVolume);
        sfxSlider.value = PlayerPrefs.GetFloat(SFX_KEY, audioSettings.sfxVolume);
        menuSfxSlider.value = PlayerPrefs.GetFloat(MENU_SFX_KEY, audioSettings.menuSFXVolume);
        gameSfxSlider.value = PlayerPrefs.GetFloat(GAME_SFX_KEY, audioSettings.gameSFXVolume);
        metronomeSlider.value = PlayerPrefs.GetFloat(METRONOME_KEY, audioSettings.metronomeVolume);
    }

    private void SubscribeSliders()
    {
        masterSlider.onValueChanged.AddListener(UpdateMasterVolume);
        musicSlider.onValueChanged.AddListener(UpdateMusicVolume);
        sfxSlider.onValueChanged.AddListener(UpdateSFXVolume);
        menuSfxSlider.onValueChanged.AddListener(UpdateMenuSFXVolume);
        gameSfxSlider.onValueChanged.AddListener(UpdateGameSFXVolume);
        metronomeSlider.onValueChanged.AddListener(UpdateMetronomeVolume);
    }

    private void UpdateMasterVolume(float value)
    {
        audioSettings.masterVolume = value;
        PlayerPrefs.SetFloat(MASTER_KEY, value);
        AudioManager.Instance?.UpdateAllVolumes(audioSettings);
    }

    private void UpdateMusicVolume(float value)
    {
        audioSettings.musicVolume = value;
        PlayerPrefs.SetFloat(MUSIC_KEY, value);
        AudioManager.Instance?.UpdateAllVolumes(audioSettings);
    }

    private void UpdateSFXVolume(float value)
    {
        audioSettings.sfxVolume = value;
        PlayerPrefs.SetFloat(SFX_KEY, value);
        AudioManager.Instance?.UpdateAllVolumes(audioSettings);
    }

    private void UpdateMenuSFXVolume(float value)
    {
        audioSettings.menuSFXVolume = value;
        PlayerPrefs.SetFloat(MENU_SFX_KEY, value);
        AudioManager.Instance?.UpdateAllVolumes(audioSettings);
    }

    private void UpdateGameSFXVolume(float value)
    {
        audioSettings.gameSFXVolume = value;
        PlayerPrefs.SetFloat(GAME_SFX_KEY, value);
        AudioManager.Instance?.UpdateAllVolumes(audioSettings);
    }

    private void UpdateMetronomeVolume(float value)
    {
        audioSettings.metronomeVolume = value;
        PlayerPrefs.SetFloat(METRONOME_KEY, value);
        AudioManager.Instance?.UpdateAllVolumes(audioSettings);
    }

    // Опционально: сохранение при выходе из игры
    private void OnApplicationQuit()
    {
        PlayerPrefs.Save();
    }
}