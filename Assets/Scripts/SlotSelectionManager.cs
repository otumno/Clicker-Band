using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

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
    public GameObject confirmationPanel;
    public Image confirmationBackground;
    public Text confirmationText;
    public Button confirmButton;
    public Button cancelButton;

    [Header("Name Input Panel")]
    public GameObject nameInputPanel;
    public Image nameInputBackground;
    public InputField nameInputField;
    public Button nameConfirmButton;

    private int selectedSlot;

    private void Awake()
    {
        // Инициализация обработчиков кнопок
        InitializeButtonListeners();
        
        // Настройка панелей
        SetupPanels();
    }

    private void InitializeButtonListeners()
    {
        // Кнопки удаления
        if (slot1DeleteButton != null) slot1DeleteButton.onClick.AddListener(() => ShowDeleteConfirmation(1));
        if (slot2DeleteButton != null) slot2DeleteButton.onClick.AddListener(() => ShowDeleteConfirmation(2));
        if (slot3DeleteButton != null) slot3DeleteButton.onClick.AddListener(() => ShowDeleteConfirmation(3));

        // Кнопки подтверждения
        if (confirmButton != null) confirmButton.onClick.AddListener(ConfirmDelete);
        if (cancelButton != null) cancelButton.onClick.AddListener(HideConfirmationPanel);
        if (nameConfirmButton != null) nameConfirmButton.onClick.AddListener(ConfirmNameAndStartGame);
    }

    private void SetupPanels()
    {
        // Настройка панели подтверждения
        if (confirmationBackground != null)
        {
            confirmationBackground.raycastTarget = true;
            confirmationBackground.color = new Color(0, 0, 0, 0.7f);
        }

        // Настройка панели ввода имени
        if (nameInputBackground != null)
        {
            nameInputBackground.raycastTarget = true;
            nameInputBackground.color = new Color(0, 0, 0, 0.7f);
        }

        // Скрываем панели при старте
        if (confirmationPanel != null) confirmationPanel.SetActive(false);
        if (nameInputPanel != null) nameInputPanel.SetActive(false);
    }

    private IEnumerator Start()
    {
        // Ждем инициализации SaveManager
        while (SaveManager.Instance == null)
        {
            yield return null;
        }

        // Обновляем UI слотов
        UpdateAllSlotsUI();
    }

    private void UpdateAllSlotsUI()
    {
        UpdateSlotUI(1, slot1Text, slot1TimeText, slot1DeleteButton);
        UpdateSlotUI(2, slot2Text, slot2TimeText, slot2DeleteButton);
        UpdateSlotUI(3, slot3Text, slot3TimeText, slot3DeleteButton);
    }

    private void UpdateSlotUI(int slot, Text slotText, Text timeText, Button deleteButton)
    {
        if (SaveManager.Instance == null || slotText == null || timeText == null || deleteButton == null)
            return;

        bool saveExists = SaveManager.Instance.SaveExists(slot);
        
        if (deleteButton != null)
            deleteButton.gameObject.SetActive(saveExists);

        if (saveExists)
        {
            PlayerData data = SaveManager.Instance.LoadGame(slot);
            if (data != null)
            {
                if (slotText != null) slotText.text = $"{data.playerName} (Slot {slot})";
                if (timeText != null) timeText.text = $"Saved: {data.lastSaveTime:g}";
            }
        }
        else
        {
            if (slotText != null) slotText.text = $"Slot {slot} (Empty)";
            if (timeText != null) timeText.text = string.Empty;
        }
    }

    private void ShowDeleteConfirmation(int slot)
    {
        selectedSlot = slot;
        
        string slotName = slot switch
        {
            1 => slot1Text != null ? slot1Text.text : $"Slot {slot}",
            2 => slot2Text != null ? slot2Text.text : $"Slot {slot}",
            3 => slot3Text != null ? slot3Text.text : $"Slot {slot}",
            _ => $"Slot {slot}"
        };

        if (confirmationText != null)
            confirmationText.text = $"Delete {slotName}?";

        if (confirmationPanel != null)
        {
            confirmationPanel.SetActive(true);
            confirmationPanel.transform.SetAsLastSibling();
            LayoutRebuilder.ForceRebuildLayoutImmediate(confirmationPanel.GetComponent<RectTransform>());
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
            if (GameManager.Instance != null)
            {
                GameManager.Instance.LoadGame(slot);
                SceneManager.LoadScene("GameScene");
            }
        }
        else
        {
            if (nameInputPanel != null)
            {
                nameInputPanel.SetActive(true);
                nameInputPanel.transform.SetAsLastSibling();
                LayoutRebuilder.ForceRebuildLayoutImmediate(nameInputPanel.GetComponent<RectTransform>());
                
                if (nameInputField != null)
                    nameInputField.text = string.Empty;
            }
        }
    }

    private void ConfirmNameAndStartGame()
    {
        string playerName = "Player";
        if (nameInputField != null && !string.IsNullOrWhiteSpace(nameInputField.text))
            playerName = nameInputField.text.Trim();

        if (GameManager.Instance != null)
        {
            GameManager.Instance.CreateNewGame(selectedSlot);
            GameManager.Instance.currentPlayerData.playerName = playerName;
            GameManager.Instance.SaveCurrentGame();
            SceneManager.LoadScene("GameScene");
        }
    }
}