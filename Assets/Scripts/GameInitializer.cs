using UnityEngine;
using UnityEngine.UI;

public class GameInitializer : MonoBehaviour
{
    [Header("UI Elements")]
    public Text scoreText;
    public Text playerNameText;
    public Text saveTimeText;

    private void Start()
    {
        // Инициализация игровых данных
        if (GameManager.Instance != null && GameManager.Instance.currentPlayerData != null)
        {
            UpdateUI();
        }
        else
        {
            Debug.LogError("No player data loaded or GameManager is not initialized!");
        }
    }

    private void UpdateUI()
    {
        PlayerData data = GameManager.Instance.currentPlayerData;

        // Обновление текстовых полей с данными игрока
        scoreText.text = $"Score: {data.score}";
        playerNameText.text = $"Player: {data.playerName}";
        saveTimeText.text = $"Last Save: {data.lastSaveTime:g}";
    }

    public void OnScoreButtonClicked()
    {
        // Увеличение счета и обновление UI
        GameManager.Instance.currentPlayerData.score++;
        UpdateUI();

        // Сохранение текущей игры
        GameManager.Instance.SaveCurrentGame();
    }
}
