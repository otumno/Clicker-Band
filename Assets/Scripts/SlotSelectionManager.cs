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
    public RectTransform confirmationPanel;
    public CanvasGroup confirmationCanvasGroup;
    public Text confirmationText;
    public Button confirmButton;
    public Button cancelButton;

    [Header("Name Input Panel")]
    public GameObject nameInputPanel;
    public InputField nameInputField;
    public Button nameConfirmButton;

    private int selectedSlot;

    private IEnumerator Start()
    {
        // Ждем инициализации SaveManager
        while (SaveManager.Instance == null)
        {
            yield return null;
        }

        InitializeUI();
    }

    private void InitializeUI()
    {
        if (!CheckUIReferences())
        {
            enabled = false;
            return;
        }

        SetupConfirmationPanel();
        SetupNameInputPanel();
        UpdateAllSlotsUI();
    }

    private bool CheckUIReferences()
    {
        bool isValid = true;

        if (slot1Text == null) { Debug.LogError("Slot1Text not assigned!"); isValid = false; }
        if (slot2Text == null) { Debug.LogError("Slot2Text not assigned!"); isValid = false; }
        if (slot3Text == null) { Debug.LogError("Slot3Text not assigned!"); isValid = false; }

        if (slot1TimeText == null) { Debug.LogError("Slot1TimeText not assigned!"); isValid = false; }
        if (slot2TimeText == null) { Debug.LogError("Slot2TimeText not assigned!"); isValid = false; }
        if (slot3TimeText == null) { Debug.LogError("Slot3TimeText not assigned!"); isValid = false; }

        if (slot1DeleteButton == null) { Debug.LogError("Slot1DeleteButton not assigned!"); isValid = false; }
        if (slot2DeleteButton == null) { Debug.LogError("Slot2DeleteButton not assigned!"); isValid = false; }
        if (slot3DeleteButton == null) { Debug.LogError("Slot3DeleteButton not assigned!"); isValid = false; }

        if (confirmationPanel == null) { Debug.LogError("ConfirmationPanel not assigned!"); isValid = false; }
        if (confirmationCanvasGroup == null) { Debug.LogError("ConfirmationCanvasGroup not assigned!"); isValid = false; }
        if (confirmationText == null) { Debug.LogError("ConfirmationText not assigned!"); isValid = false; }
        if (confirmButton == null) { Debug.LogError("ConfirmButton not assigned!"); isValid = false; }
        if (cancelButton == null) { Debug.LogError("CancelButton not assigned!"); isValid = false; }

        if (nameInputPanel == null) { Debug.LogError("NameInputPanel not assigned!"); isValid = false; }
        if (nameInputField == null) { Debug.LogError("NameInputField not assigned!"); isValid = false; }
        if (nameConfirmButton == null) { Debug.LogError("NameConfirmButton not assigned!"); isValid = false; }

        return isValid;
    }

    private void SetupConfirmationPanel()
    {
        if (confirmationCanvasGroup == null)
        {
            confirmationCanvasGroup = confirmationPanel.gameObject.AddComponent<CanvasGroup>();
        }

        confirmationCanvasGroup.alpha = 0;
        confirmationCanvasGroup.blocksRaycasts = false;

        confirmButton.onClick.RemoveAllListeners();
        cancelButton.onClick.RemoveAllListeners();

        confirmButton.onClick.AddListener(ConfirmDelete);
        cancelButton.onClick.AddListener(() => 
        {
            confirmationCanvasGroup.alpha = 0;
            confirmationCanvasGroup.blocksRaycasts = false;
        });
    }

    private void SetupNameInputPanel()
    {
        nameInputPanel.SetActive(false);
        nameConfirmButton.onClick.RemoveAllListeners();
        nameConfirmButton.onClick.AddListener(ConfirmNameAndStartGame);
    }

    private void UpdateAllSlotsUI()
    {
        UpdateSlotUI(1, slot1Text, slot1TimeText, slot1DeleteButton);
        UpdateSlotUI(2, slot2Text, slot2TimeText, slot2DeleteButton);
        UpdateSlotUI(3, slot3Text, slot3TimeText, slot3DeleteButton);
    }

    private void UpdateSlotUI(int slot, Text slotText, Text timeText, Button deleteButton)
    {
        if (slotText == null || timeText == null || deleteButton == null)
        {
            Debug.LogError($"UI elements for slot {slot} are not assigned!");
            return;
        }

        if (SaveManager.Instance == null)
        {
            Debug.LogError("SaveManager is not initialized!");
            return;
        }

        bool saveExists = SaveManager.Instance.SaveExists(slot);

        if (saveExists)
        {
            PlayerData data = SaveManager.Instance.LoadGame(slot);
            if (data != null)
            {
                slotText.text = $"{data.playerName} (Slot {slot})";
                timeText.text = $"Saved: {data.lastSaveTime:g}";
            }
            deleteButton.gameObject.SetActive(true);
        }
        else
        {
            slotText.text = $"Slot {slot} (Empty)";
            timeText.text = "";
            deleteButton.gameObject.SetActive(false);
        }
    }

    public void ShowDeleteConfirmation(int slot)
    {
        selectedSlot = slot;
        
        string slotName = "";
        switch(slot)
        {
            case 1: slotName = slot1Text.text; break;
            case 2: slotName = slot2Text.text; break;
            case 3: slotName = slot3Text.text; break;
        }

        confirmationText.text = $"Delete {slotName}?";
        confirmationCanvasGroup.alpha = 1;
        confirmationCanvasGroup.blocksRaycasts = true;
    }

    private void ConfirmDelete()
    {
        if (SaveManager.Instance == null)
        {
            Debug.LogError("SaveManager is missing!");
            return;
        }

        SaveManager.Instance.DeleteSave(selectedSlot);
        UpdateAllSlotsUI();
        confirmationCanvasGroup.alpha = 0;
        confirmationCanvasGroup.blocksRaycasts = false;
    }

    public void OnSlotSelected(int slot)
    {
        selectedSlot = slot;

        if (SaveManager.Instance == null)
        {
            Debug.LogError("SaveManager is not initialized!");
            return;
        }

        if (SaveManager.Instance.SaveExists(slot))
        {
            GameManager.Instance.LoadGame(slot);
            SceneManager.LoadScene("GameScene");
        }
        else
        {
            nameInputPanel.SetActive(true);
            nameInputPanel.transform.SetAsLastSibling();
        }
    }

    private void ConfirmNameAndStartGame()
    {
        string playerName = nameInputField.text.Trim();
        if (string.IsNullOrEmpty(playerName))
        {
            playerName = "Player";
        }

        GameManager.Instance.CreateNewGame(selectedSlot);
        GameManager.Instance.currentPlayerData.playerName = playerName;
        GameManager.Instance.SaveCurrentGame();
        SceneManager.LoadScene("GameScene");
    }
}