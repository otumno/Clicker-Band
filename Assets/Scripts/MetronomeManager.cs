using UnityEngine;
using System.Collections;

public class MetronomeManager : MonoBehaviour
{
    [SerializeField] private MetronomePattern[] customPattern;
    [SerializeField] private float startDelay = 1f;
    
    private int currentPatternIndex;
    private bool isActive;
    private Coroutine metronomeRoutine;

    private void Start()
    {
        InitializeMetronome();
    }

    private void InitializeMetronome()
    {
        if (customPattern == null || customPattern.Length == 0)
        {
            Debug.LogError($"{name}: Metronome pattern not configured!", this);
            return;
        }

        if (GlobalAudioManager.Instance == null)
        {
            Debug.LogError($"{name}: GlobalAudioManager not found!", this);
            return;
        }

        GlobalAudioManager.Instance.StartMetronome();
        StartMetronome();
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

        Debug.Log($"Playing beat {currentPatternIndex} with clip: {beat.sound.name}");
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
    }
}