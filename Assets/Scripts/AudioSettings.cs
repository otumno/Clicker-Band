// AudioSettings.cs
using UnityEngine;

[System.Serializable]
public class AudioSettings
{
    [Range(0f, 1f)] public float masterVolume = 1.0f;
    [Range(0f, 1f)] public float musicVolume = 1.0f;
    [Range(0f, 1f)] public float sfxVolume = 1.0f;
    [Range(0f, 1f)] public float menuSFXVolume = 1.0f;
    [Range(0f, 1f)] public float gameSFXVolume = 1.0f;
    [Range(0f, 1f)] public float metronomeVolume = 1.0f;
}