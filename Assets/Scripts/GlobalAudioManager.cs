// GlobalAudioManager.cs
using UnityEngine;

public class GlobalAudioManager : MonoBehaviour
{
    public static GlobalAudioManager Instance { get; private set; }

    [Header("Timing Settings")]
    [Range(60, 240)] public int BPM = 120;
    [Tooltip("Pre-beat window in milliseconds")] 
    [SerializeField] private float preBeatWindow = 100f;
    [Tooltip("Post-beat window in milliseconds")]
    [SerializeField] private float postBeatWindow = 50f;

    private float beatInterval;
    private float nextBeatTime;
    private bool metronomeActive;

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
        
        CalculateBeatInterval();
    }

    private void Update()
    {
        if (metronomeActive && Time.time >= nextBeatTime)
        {
            nextBeatTime += beatInterval;
        }
    }

    public void CalculateBeatInterval() => beatInterval = 60f / BPM;

    public bool IsBeatInWindow(float customWindow = 0f)
    {
        float window = customWindow > 0 ? customWindow : (preBeatWindow + postBeatWindow) / 1000f;
        float currentTime = Time.time;
        return currentTime >= nextBeatTime - window/2 && currentTime <= nextBeatTime + window/2;
    }

    public void StartMetronome()
    {
        metronomeActive = true;
        nextBeatTime = Time.time + beatInterval;
    }

    public void StopMetronome()
    {
        metronomeActive = false;
    }
}