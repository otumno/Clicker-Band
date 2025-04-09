using UnityEngine;
using UnityEngine.UI;
using System;
using System.Globalization;
using System.Collections; // Добавлено для IEnumerator

public class GameInitializer : MonoBehaviour
{
    [Header("UI Elements")]
    public Text fameText;
    public Text playerNameText;
    public Text saveTimeText;

    [Header("Settings")]
    public float initializationDelay = 0.5f;

    private void Start()
    {
        CultureInfo.DefaultThreadCurrentCulture = new CultureInfo("en-US");
        CultureInfo.DefaultThreadCurrentUICulture = new CultureInfo("en-US");
        StartCoroutine(InitializeWithDelay());
    }

    private IEnumerator InitializeWithDelay()
    {
        float elapsedTime = 0f;
        while (GameManager.Instance == null && elapsedTime < initializationDelay)
        {
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        if (GameManager.Instance == null || GameManager.Instance.currentPlayerData == null)
        {
            Debug.LogWarning("Creating default player data");
            CreateDefaultData();
        }

        UpdateUI();
    }

    private void CreateDefaultData()
    {
        if (GameManager.Instance == null) return;

        GameManager.Instance.CreateNewGame(1);
        GameManager.Instance.currentPlayerData.playerName = "Player";
        GameManager.Instance.currentPlayerData.score = 0;
        GameManager.Instance.currentPlayerData.lastSaveTime = DateTime.Now;
    }

    private void UpdateUI()
    {
        if (GameManager.Instance?.currentPlayerData == null) return;

        if (fameText != null)
            fameText.text = $"{GameManager.Instance.currentPlayerData.score} Fame";
        
        if (playerNameText != null)
            playerNameText.text = GameManager.Instance.currentPlayerData.playerName;
        
        if (saveTimeText != null)
            saveTimeText.text = FormatCompactDateTime(GameManager.Instance.currentPlayerData.lastSaveTime);
    }

    private string FormatCompactDateTime(DateTime date)
    {
        return date.ToString("d MMM HH:mm", CultureInfo.CurrentCulture);
    }

    public void OnScoreButtonClicked()
    {
        if (GameManager.Instance?.currentPlayerData == null) return;

        GameManager.Instance.currentPlayerData.score++;
        UpdateUI();
        GameManager.Instance.SaveCurrentGame();
    }
}