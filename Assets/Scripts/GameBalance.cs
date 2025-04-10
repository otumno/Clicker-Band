using UnityEngine;

[CreateAssetMenu(fileName = "GameBalance", menuName = "AudioClicker/Balance Settings")]
public class GameBalance : ScriptableObject
{
    [Header("Base Scoring")]
    public int baseClickPoints = 10;
    public int basePerfectHitBonus = 50;
    [Tooltip("Multiplier applied to perfect hit bonus based on combo")]
    public int comboMultiplier = 2;
    
    [Header("Level Progression")]
    public int[] levelUpRequirements = { 1000, 5000, 15000 };
    
    [Header("Instrument Upgrades")]
    public UpgradeData[] instrumentUpgrades;
    
    [Header("Metronome Settings")]
    [Range(60, 240)] public float defaultBPM = 120;
    [Tooltip("Pre-beat window in milliseconds")]
    [Range(0, 500)] public float defaultPreWindow = 100f;
    [Tooltip("Post-beat window in milliseconds")]
    [Range(0, 500)] public float defaultPostWindow = 50f;

    [Header("Audio Settings")] 
    [Range(0f, 1f)] public float defaultMetronomeVolume = 0.7f;
    [Range(0f, 1f)] public float defaultSFXVolume = 0.8f;

    private void OnValidate()
    {
        defaultBPM = Mathf.Clamp(defaultBPM, 60, 240);
        defaultPreWindow = Mathf.Clamp(defaultPreWindow, 0, 500);
        defaultPostWindow = Mathf.Clamp(defaultPostWindow, 0, 500);
    }
}

// Добавьте этот класс в тот же файл (или вынесите в отдельный файл UpgradeData.cs)
[System.Serializable]
public class UpgradeData
{
    public string upgradeName;
    public int cost;
    public int unlockLevel;
    public float valueIncrease;
}