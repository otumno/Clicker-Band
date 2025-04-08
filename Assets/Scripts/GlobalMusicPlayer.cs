using UnityEngine;

public class GlobalMusicPlayer : MonoBehaviour
{
    [Header("Music Settings")]
    [SerializeField] private AudioClip musicClip;
    [Range(0.1f, 1f)] public float volume = 0.7f;
    [SerializeField] private float fadeInTime = 1f;

    public static GlobalMusicPlayer Instance { get; private set; }

    private void Awake()
    {
        // Реализация Singleton
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        // Инициализация музыки
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayGlobalMusic(musicClip, volume, fadeInTime > 0);
        }
    }

    private void OnDestroy()
    {
        // Гасим музыку только при полном уничтожении
        if (Instance == this && AudioManager.Instance != null)
        {
            AudioManager.Instance.SetGlobalMusicVolume(0f, 1f);
        }
    }
}