using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System;
using System.Collections;
using System.Globalization;

public class SlotSelectionManager : MonoBehaviour
{
    [Header("Slot UI References")]
    public Text slot1NameText;
    public Text slot2NameText;
    public Text slot3NameText;
    public Text slot1TimeText;
    public Text slot2TimeText;
    public Text slot3TimeText;
    public Text slot1FameText;
    public Text slot2FameText;
    public Text slot3FameText;
    public RectTransform slot1DeleteButton;
    public RectTransform slot2DeleteButton;
    public RectTransform slot3DeleteButton;

    [Header("Confirmation Panel")]
    public GameObject confirmationPanel;
    public Text confirmationText;
    public RectTransform confirmButton;
    public RectTransform cancelButton;

    [Header("Name Input Panel")]
    public GameObject nameInputPanel;
    public InputField nameInputField;
    public RectTransform nameConfirmButton;

    private int selectedSlot;

    private void Awake()
    {
        // Установка английской культуры для корректного отображения месяцев
        CultureInfo.DefaultThreadCurrentCulture = new CultureInfo("en-US");
        CultureInfo.DefaultThreadCurrentUICulture = new CultureInfo("en-US");

        ConfigureAllButtons();
        InitializeButtonListeners();
        SetupPanels();
    }

    private string FormatCompactDateTime(DateTime date)
    {
        // Формат: "5 Jul 14:30" (день месяц время)
        return date.ToString("d MMM HH:mm", CultureInfo.CurrentCulture);
    }

    private void SetupPanels()
    {
        if (confirmationPanel != null) confirmationPanel.SetActive(false);
        if (nameInputPanel != null) nameInputPanel.SetActive(false);
    }

    private void ConfigureAllButtons()
    {
        ConfigureButton(slot1DeleteButton);
        ConfigureButton(slot2DeleteButton);
        ConfigureButton(slot3DeleteButton);
        ConfigureButton(confirmButton);
        ConfigureButton(cancelButton);
        ConfigureButton(nameConfirmButton);
    }

    private void ConfigureButton(RectTransform buttonRect)
    {
        if (buttonRect == null) return;

        // Настройка Image для кнопки
        Image image = buttonRect.GetComponent<Image>();
        if (image == null)
        {
            image = buttonRect.gameObject.AddComponent<Image>();
        }
        image.color = Color.white;
        image.raycastTarget = true;

        // Настройка Button компонента
        Button button = buttonRect.GetComponent<Button>();
        if (button == null)
        {
            button = buttonRect.gameObject.AddComponent<Button>();
        }

        // Минимальные размеры кнопки
        if (buttonRect.sizeDelta.x < 100f || buttonRect.sizeDelta.y < 50f)
        {
            buttonRect.sizeDelta = new Vector2(
                Mathf.Max(100f, buttonRect.sizeDelta.x),
                Mathf.Max(50f, buttonRect.sizeDelta.y)
            );
        }
    }

    private void InitializeButtonListeners()
    {
        AssignButtonListener(slot1DeleteButton, () => ShowDeleteConfirmation(1));
        AssignButtonListener(slot2DeleteButton, () => ShowDeleteConfirmation(2));
        AssignButtonListener(slot3DeleteButton, () => ShowDeleteConfirmation(3));
        AssignButtonListener(confirmButton, ConfirmDelete);
        AssignButtonListener(cancelButton, HideConfirmationPanel);
        AssignButtonListener(nameConfirmButton, ConfirmNameAndStartGame);
    }

    private void AssignButtonListener(RectTransform buttonRect, UnityEngine.Events.UnityAction action)
    {
        if (buttonRect == null) return;
        
        Button button = buttonRect.GetComponent<Button>();
        if (button != null)
        {
            button.onClick.RemoveAllListeners();
            button.onClick.AddListener(action);
        }
    }

    private IEnumerator Start()
    {
        while (SaveManager.Instance == null)
        {
            yield return null;
        }
        UpdateAllSlotsUI();
    }

    private void UpdateAllSlotsUI()
    {
        UpdateSlotUI(1, slot1NameText, slot1TimeText, slot1FameText, slot1DeleteButton);
        UpdateSlotUI(2, slot2NameText, slot2TimeText, slot2FameText, slot2DeleteButton);
        UpdateSlotUI(3, slot3NameText, slot3TimeText, slot3FameText, slot3DeleteButton);
    }

    private void UpdateSlotUI(int slot, Text nameText, Text timeText, Text fameText, RectTransform deleteButton)
    {
        if (SaveManager.Instance == null) return;

        bool saveExists = SaveManager.Instance.SaveExists(slot);
        
        if (deleteButton != null)
            deleteButton.gameObject.SetActive(saveExists);

        if (saveExists)
        {
            PlayerData data = SaveManager.Instance.LoadGame(slot);
            if (data != null)
            {
                if (nameText != null)
                    nameText.text = data.playerName; // Только имя

                if (timeText != null)
                    timeText.text = FormatCompactDateTime(data.lastSaveTime); // "5 Jul 14:30"

                if (fameText != null)
                    fameText.text = $"{data.score} Fame"; // "10 Fame"
            }
        }
        else
        {
            if (nameText != null)
                nameText.text = $"Empty slot {slot}";

            if (timeText != null)
                timeText.text = "";

            if (fameText != null)
                fameText.text = "";
        }
    }

    private void ShowDeleteConfirmation(int slot)
    {
        selectedSlot = slot;
        
        string slotName = $"Empty slot {slot}";
        switch(slot)
        {
            case 1 when slot1NameText != null: slotName = slot1NameText.text; break;
            case 2 when slot2NameText != null: slotName = slot2NameText.text; break;
            case 3 when slot3NameText != null: slotName = slot3NameText.text; break;
        }

        if (confirmationText != null)
            confirmationText.text = $"Delete {slotName}?";

        if (confirmationPanel != null)
        {
            confirmationPanel.SetActive(true);
            confirmationPanel.transform.SetAsLastSibling();
        }
    }

    private void HideConfirmationPanel()
    {
        if (confirmationPanel != null)
            confirmationPanel.SetActive(false);
    }

    private void ConfirmDelete()
    {
        if (SaveManager.Instance != null)
        {
            SaveManager.Instance.DeleteSave(selectedSlot);
            UpdateAllSlotsUI();
        }
        HideConfirmationPanel();
    }

    public void OnSlotSelected(int slot)
    {
        selectedSlot = slot;

        if (SaveManager.Instance == null) return;

        if (SaveManager.Instance.SaveExists(slot))
        {
            GameManager.Instance?.LoadGame(slot);
            SceneManager.LoadScene("GameScene");
        }
        else
        {
            if (nameInputPanel != null)
            {
                nameInputPanel.SetActive(true);
                nameInputPanel.transform.SetAsLastSibling();
                if (nameInputField != null)
                    nameInputField.text = "";
            }
        }
    }

    private void ConfirmNameAndStartGame()
    {
        string playerName = nameInputField?.text?.Trim();
        if (string.IsNullOrEmpty(playerName))
            playerName = "Player";

        if (GameManager.Instance != null)
        {
            GameManager.Instance.CreateNewGame(selectedSlot);
            GameManager.Instance.currentPlayerData.playerName = playerName;
            GameManager.Instance.SaveCurrentGame();
            SceneManager.LoadScene("GameScene");
        }
    }
}