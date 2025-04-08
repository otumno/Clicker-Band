using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System;

public class SlotSelectionManager : MonoBehaviour
{
    [Header("Slot UI References")]
    public Text slot1Text;
    public Text slot2Text;
    public Text slot3Text;
    public Text slot1TimeText;
    public Text slot2TimeText;
    public Text slot3TimeText;
    public Button slot1DeleteButton;
    public Button slot2DeleteButton;
    public Button slot3DeleteButton;

    [Header("Confirmation Panel")]
    public RectTransform confirmationPanel;
    public CanvasGroup confirmationCanvasGroup;
    public Text confirmationText;
    public Button confirmButton;
    public Button cancelButton;
    public RectTransform confirmButtonRect;
    public RectTransform cancelButtonRect;

    [Header("Settings")]
    public bool debugMode = true;
    public float buttonSpacing = 200f;

    private int selectedSlot;
    private bool isInitialized = false;

    private void Start()
    {
        InitializeUI();
    }

    private void InitializeUI()
    {
        if (debugMode) Debug.Log("Initializing SlotSelectionManager UI");

        if (!CheckUIReferences())
        {
            enabled = false;
            return;
        }

        SetupButtonsText();
        SetupButtonListeners();
        SetupConfirmationPanel();
        UpdateAllSlotsUI();

        isInitialized = true;
        if (debugMode) Debug.Log("UI initialization complete");
    }

    private bool CheckUIReferences()
    {
        bool isValid = true;

        // Проверка основных элементов слотов
        if (slot1Text == null) { Debug.LogError("Slot1Text not assigned!"); isValid = false; }
        if (slot2Text == null) { Debug.LogError("Slot2Text not assigned!"); isValid = false; }
        if (slot3Text == null) { Debug.LogError("Slot3Text not assigned!"); isValid = false; }

        if (slot1TimeText == null) { Debug.LogError("Slot1TimeText not assigned!"); isValid = false; }
        if (slot2TimeText == null) { Debug.LogError("Slot2TimeText not assigned!"); isValid = false; }
        if (slot3TimeText == null) { Debug.LogError("Slot3TimeText not assigned!"); isValid = false; }

        if (slot1DeleteButton == null) { Debug.LogError("Slot1DeleteButton not assigned!"); isValid = false; }
        if (slot2DeleteButton == null) { Debug.LogError("Slot2DeleteButton not assigned!"); isValid = false; }
        if (slot3DeleteButton == null) { Debug.LogError("Slot3DeleteButton not assigned!"); isValid = false; }

        // Проверка элементов панели подтверждения
        if (confirmationPanel == null) { Debug.LogError("ConfirmationPanel not assigned!"); isValid = false; }
        if (confirmButton == null) { Debug.LogError("ConfirmButton not assigned!"); isValid = false; }
        if (cancelButton == null) { Debug.LogError("CancelButton not assigned!"); isValid = false; }
        if (confirmationText == null) { Debug.LogError("ConfirmationText not assigned!"); isValid = false; }
        if (confirmButtonRect == null) { Debug.LogError("ConfirmButtonRect not assigned!"); isValid = false; }
        if (cancelButtonRect == null) { Debug.LogError("CancelButtonRect not assigned!"); isValid = false; }

        return isValid;
    }

    private void SetupConfirmationPanel()
    {
        // Добавляем CanvasGroup если отсутствует
        if (confirmationCanvasGroup == null)
        {
            confirmationCanvasGroup = confirmationPanel.gameObject.AddComponent<CanvasGroup>();
        }

        // Настройка позиционирования
        confirmationPanel.anchorMin = new Vector2(0.5f, 0.5f);
        confirmationPanel.anchorMax = new Vector2(0.5f, 0.5f);
        confirmationPanel.pivot = new Vector2(0.5f, 0.5f);
        confirmationPanel.anchoredPosition = Vector2.zero;

        // Расположение кнопок с заданным интервалом
        confirmButtonRect.anchoredPosition = new Vector2(-buttonSpacing/2, -50f);
        cancelButtonRect.anchoredPosition = new Vector2(buttonSpacing/2, -50f);

        // Отключаем панель
        confirmationCanvasGroup.alpha = 0f;
        confirmationCanvasGroup.blocksRaycasts = false;
        confirmationCanvasGroup.interactable = false;

        // Настройка порядка отрисовки
        confirmationPanel.transform.SetAsLastSibling();

        if (debugMode) Debug.Log("Confirmation panel setup complete");
    }

    private void SetupButtonsText()
    {
        SetButtonText(slot1DeleteButton, "DELETE");
        SetButtonText(slot2DeleteButton, "DELETE");
        SetButtonText(slot3DeleteButton, "DELETE");
        
        SetButtonText(confirmButton, "YES");
        SetButtonText(cancelButton, "NO");
    }

    private void SetButtonText(Button button, string text)
    {
        if (button == null) return;

        Text btnText = button.GetComponentInChildren<Text>(true);
        if (btnText != null)
        {
            btnText.text = text;
        }
        else if (debugMode)
        {
            Debug.LogWarning($"No Text component found on button: {button.name}");
        }
    }

    private void SetupButtonListeners()
    {
        // Очищаем старые обработчики
        slot1DeleteButton.onClick.RemoveAllListeners();
        slot2DeleteButton.onClick.RemoveAllListeners();
        slot3DeleteButton.onClick.RemoveAllListeners();
        confirmButton.onClick.RemoveAllListeners();
        cancelButton.onClick.RemoveAllListeners();

        // Назначаем новые обработчики
        slot1DeleteButton.onClick.AddListener(() => ShowDeleteConfirmation(1));
        slot2DeleteButton.onClick.AddListener(() => ShowDeleteConfirmation(2));
        slot3DeleteButton.onClick.AddListener(() => ShowDeleteConfirmation(3));
        
        confirmButton.onClick.AddListener(ConfirmDelete);
        cancelButton.onClick.AddListener(CancelDelete);

        if (debugMode) Debug.Log("Button listeners setup complete");
    }

    private void UpdateAllSlotsUI()
    {
        UpdateSlotUI(1, slot1Text, slot1TimeText, slot1DeleteButton);
        UpdateSlotUI(2, slot2Text, slot2TimeText, slot2DeleteButton);
        UpdateSlotUI(3, slot3Text, slot3TimeText, slot3DeleteButton);
    }

    private void UpdateSlotUI(int slot, Text slotText, Text timeText, Button deleteButton)
    {
        if (slotText == null || timeText == null || deleteButton == null) return;

        bool saveExists = SaveManager.Instance.SaveExists(slot);
        slotText.text = saveExists ? $"Slot {slot} (Used)" : $"Slot {slot} (Empty)";

        if (saveExists)
        {
            DateTime saveTime = SaveManager.Instance.GetSaveTime(slot);
            timeText.text = $"Last save: {saveTime:g}";
            deleteButton.gameObject.SetActive(true);
        }
        else
        {
            timeText.text = "";
            deleteButton.gameObject.SetActive(false);
        }
    }

    private void ShowDeleteConfirmation(int slot)
    {
        if (!isInitialized) return;

        selectedSlot = slot;
        confirmationText.text = $"Are you sure you want to delete save in Slot {slot}?\nAll progress will be lost!";
        
        // Активируем панель
        confirmationPanel.transform.SetAsLastSibling();
        confirmationCanvasGroup.alpha = 1f;
        confirmationCanvasGroup.blocksRaycasts = true;
        confirmationCanvasGroup.interactable = true;

        if (debugMode) Debug.Log($"Showing confirmation for slot {slot}");
    }

    private void HideConfirmationPanel()
    {
        confirmationCanvasGroup.alpha = 0f;
        confirmationCanvasGroup.blocksRaycasts = false;
        confirmationCanvasGroup.interactable = false;

        if (debugMode) Debug.Log("Confirmation panel hidden");
    }

    private void ConfirmDelete()
    {
        if (!isInitialized) return;

        SaveManager.Instance.DeleteSave(selectedSlot);
        UpdateAllSlotsUI();
        HideConfirmationPanel();

        if (debugMode) Debug.Log($"Confirmed deletion of slot {selectedSlot}");
    }

    private void CancelDelete()
    {
        if (!isInitialized) return;

        HideConfirmationPanel();
        if (debugMode) Debug.Log("Deletion cancelled");
    }

    public void OnSlotSelected(int slot)
    {
        if (!isInitialized) return;

        selectedSlot = slot;
        
        if (SaveManager.Instance.SaveExists(slot))
        {
            GameManager.Instance.LoadGame(slot);
            SceneManager.LoadScene("GameScene");
        }
        else
        {
            GameManager.Instance.CreateNewGame(slot);
            SceneManager.LoadScene("GameScene");
        }
    }
}