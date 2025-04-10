// InstrumentController.cs
using UnityEngine;

public class InstrumentController : MonoBehaviour
{
    [SerializeField] private InstrumentBeat[] beatSounds;
    [SerializeField] private float hitWindow = 0.1f;
    
    private int currentBeatIndex;

    private void OnValidate()
    {
        if (beatSounds == null || beatSounds.Length == 0)
        {
            Debug.LogWarning($"{name}: Instrument beats not configured!", this);
        }
    }

    public void OnInstrumentClicked()
    {
        if (!CheckDependencies()) return;

        bool isPerfectHit = GlobalAudioManager.Instance.IsBeatInWindow(hitWindow);
        PlaySound(isPerfectHit);
        ScoreSystem.Instance?.RegisterHit(isPerfectHit);
    }

    private bool CheckDependencies()
    {
        if (GlobalAudioManager.Instance == null)
        {
            Debug.LogError($"{name}: GlobalAudioManager not found!", this);
            return false;
        }
        
        if (beatSounds == null || beatSounds.Length == 0)
        {
            Debug.LogError($"{name}: No beat sounds configured!", this);
            return false;
        }
        
        return true;
    }

    private void PlaySound(bool isPerfectHit)
    {
        currentBeatIndex %= beatSounds.Length;
        var beat = beatSounds[currentBeatIndex];
        
        if (beat == null) return;

        AudioClip clipToPlay = isPerfectHit ? beat.correctSound : beat.missSound;
        if (clipToPlay == null) return;

        AudioManager.Instance?.PlayInstrumentSound(
            clipToPlay, 
            beat.volume,
            isPerfectHit ? 1f : 0.8f
        );

        currentBeatIndex = (currentBeatIndex + 1) % beatSounds.Length;
    }
}