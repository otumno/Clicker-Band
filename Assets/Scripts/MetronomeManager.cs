using UnityEngine;
using System.Collections; // Добавлено для IEnumerator
using UnityEngine.SceneManagement; // Добавьте эту строку

public class MetronomeManager : MonoBehaviour
{
    public static MetronomeManager Instance { get; private set; }

    [SerializeField] private MetronomePattern[] customPattern;
    [SerializeField] private float startDelay = 0.1f;
    
    private int currentPatternIndex;
    private bool isActive;
    private Coroutine metronomeRoutine;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded; // Подписываемся на событие загрузки сцены
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded; // Отписываемся от события
        StopMetronome(); // Останавливаем метроном
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        StopMetronome(); // Останавливаем метроном при загрузке новой сцены
    }

    public void StartMetronome()
    {
        if (isActive || customPattern == null || customPattern.Length == 0) return;
        
        isActive = true;
        currentPatternIndex = 0;
        metronomeRoutine = StartCoroutine(MetronomeCoroutine());
    }

    private IEnumerator MetronomeCoroutine()
    {
        yield return new WaitForSeconds(startDelay);
        
        while (isActive && GlobalAudioManager.Instance != null)
        {
            if (currentPatternIndex < customPattern.Length)
            {
                PlayCurrentBeat();
                currentPatternIndex = (currentPatternIndex + 1) % customPattern.Length;
            }
            yield return new WaitForSeconds(60f / GlobalAudioManager.Instance.BPM);
        }
    }

    private void PlayCurrentBeat()
    {
        if (currentPatternIndex >= customPattern.Length) return;
        
        var beat = customPattern[currentPatternIndex];
        if (beat == null || !beat.enabled || beat.sound == null) return;

        AudioManager.Instance?.PlayMetronomeSound(
            beat.sound,
            beat.volume,
            beat.pitchVariation
        );
    }

    public void StopMetronome()
    {
        if (!isActive) return;
        
        isActive = false;
        if (metronomeRoutine != null)
        {
            StopCoroutine(metronomeRoutine);
        }
        GlobalAudioManager.Instance.StopMetronome();
        GlobalAudioManager.Instance.ResetFirstClick(); // Сброс первого клика
    }

    private void OnDestroy()
    {
        StopMetronome(); // Останавливаем метроном при уничтожении объекта
    }

    public void StopMetronomeOnMenu()
    {
        StopMetronome(); // Используем новый метод для остановки метронома
    }
}
