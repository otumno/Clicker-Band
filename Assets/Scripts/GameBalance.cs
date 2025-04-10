using UnityEngine;

[CreateAssetMenu(fileName = "GameBalance", menuName = "AudioClicker/Balance Settings")]
public class GameBalance : ScriptableObject
{
    [Header("Base Scoring")]
    public int baseClickPoints = 10;
    public int basePerfectHitBonus = 50;
    public int comboMultiplier = 2;
    
    [Header("Level Progression")]
    public int[] levelUpRequirements = { 1000, 5000, 15000 };
    
    [Header("Instrument Upgrades")]
    public UpgradeData[] instrumentUpgrades;
    
    [Header("Metronome Settings")]
    public float defaultBPM = 120;
    public float defaultPreWindow = 100f;
    public float defaultPostWindow = 50f;

    [Header("Audio Settings")] 
    [Range(0f, 1f)] public float defaultMetronomeVolume = 0.7f;
    [Range(0f, 1f)] public float defaultSFXVolume = 0.8f;
}

[System.Serializable]
public class UpgradeData
{
    public string upgradeName;
    public int cost;
    public int unlockLevel;
    public float valueIncrease;
}