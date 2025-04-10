using UnityEngine;

[System.Serializable]
public class MetronomePattern
{
    public bool enabled = true;
    public AudioClip sound;
    [Range(0.8f, 1.2f)] public float pitchVariation = 1f;
    [Range(0f, 1f)] public float volume = 1f;
    [Tooltip("Start time in seconds")] 
    public float startTime;
}

[System.Serializable]
public class InstrumentBeat
{
    public AudioClip correctSound;
    public AudioClip missSound;
    [Range(0f, 1f)] public float volume = 1f;
}