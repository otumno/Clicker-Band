using UnityEngine;

public class ScoreSystem : MonoBehaviour
{
    public static ScoreSystem Instance { get; private set; }

    [SerializeField] private GameBalance balance;
    
    public int CurrentScore { get; private set; }
    public int CurrentCombo { get; private set; }
    public int MaxCombo { get; private set; }
    public int CurrentLevel { get; private set; }

    public delegate void ScoreUpdated(int newScore);
    public event ScoreUpdated OnScoreUpdated;

    public delegate void ComboUpdated(int newCombo);
    public event ComboUpdated OnComboUpdated;

    public delegate void LevelUp(int newLevel);
    public event LevelUp OnLevelUp;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void RegisterHit(bool isPerfect)
    {
        if (isPerfect)
        {
            int points = balance.baseClickPoints + 
                        (balance.basePerfectHitBonus * (CurrentCombo + 1)) * balance.comboMultiplier;
            
            CurrentScore += points;
            CurrentCombo++;
            
            if (CurrentCombo > MaxCombo)
                MaxCombo = CurrentCombo;
            
            OnScoreUpdated?.Invoke(CurrentScore);
            OnComboUpdated?.Invoke(CurrentCombo);
            
            CheckLevelUp();
        }
        else
        {
            CurrentCombo = 0;
            OnComboUpdated?.Invoke(CurrentCombo);
        }
    }

    private void CheckLevelUp()
    {
        if (CurrentLevel < balance.levelUpRequirements.Length && 
            CurrentScore >= balance.levelUpRequirements[CurrentLevel])
        {
            CurrentLevel++;
            OnLevelUp?.Invoke(CurrentLevel);
        }
    }

    public void ResetScore()
    {
        CurrentScore = 0;
        CurrentCombo = 0;
        CurrentLevel = 0;
        
        OnScoreUpdated?.Invoke(CurrentScore);
        OnComboUpdated?.Invoke(CurrentCombo);
    }
}