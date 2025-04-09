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
        ConfigureAllButtons();
        InitializeButtonListeners();
        SetupPanels();
    }

    private void SetupPanels()
    {
        if (confirmationPanel != null) confirmationPanel.SetActive(false);
        if (nameInputPanel != null) nameInputPanel.SetActive(false);
    }

    private void ConfigureAllButtons()
    {
        ConfigureButtonHitbox(slot1DeleteButton);
        ConfigureButtonHitbox(slot2DeleteButton);
        ConfigureButtonHitbox(slot3DeleteButton);
        ConfigureButtonHitbox(confirmButton);
        ConfigureButtonHitbox(cancelButton);
        ConfigureButtonHitbox(nameConfirmButton);
    }

    private void ConfigureButtonHitbox(RectTransform buttonRect)
    {
        if (buttonRect == null) return;

        // Удаляем все ненужные компоненты Image
        Image[] images = buttonRect.GetComponents<Image>();
        for (int i = 1; i < images.Length; i++)
        {
            Destroy(images[i]);
        }

        // Оставляем/добавляем один Image
        Image image = buttonRect.GetComponent<Image>();
        if (image == null)
        {
            image = buttonRect.gameObject.AddComponent<Image>();
        }
        image.color = new Color(1, 1, 1, 0.01f); // Почти прозрачный
        image.raycastTarget = true;

        // Настраиваем Button
        Button button = buttonRect.GetComponent<Button>();
        if (button == null)
        {
            button = buttonRect.gameObject.AddComponent<Button>();
        }

        // Устанавливаем минимальные размеры
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
        UpdateSlotUI(1, slot1Text, slot1TimeText, slot1DeleteButton);
        UpdateSlotUI(2, slot2Text, slot2TimeText, slot2DeleteButton);
        UpdateSlotUI(3, slot3Text, slot3TimeText, slot3DeleteButton);
    }

    private void UpdateSlotUI(int slot, Text slotText, Text timeText, RectTransform deleteButton)
    {
        if (SaveManager.Instance == null) return;

        bool saveExists = SaveManager.Instance.SaveExists(slot);
        
        if (deleteButton != null)
            deleteButton.gameObject.SetActive(saveExists);

        if (slotText != null)
            slotText.text = saveExists ? 
                $"{SaveManager.Instance.LoadGame(slot)?.playerName ?? "Player"}" : 
                $"Empty slot {slot}";

        if (timeText != null)
            timeText.text = saveExists ? $"Saved: {SaveManager.Instance.LoadGame(slot)?.lastSaveTime:g}" : "";
    }

    private void ShowDeleteConfirmation(int slot)
    {
        selectedSlot = slot;
        
        string slotName = $"Empty slot {slot}";
        switch(slot)
        {
            case 1 when slot1Text != null: slotName = slot1Text.text; break;
            case 2 when slot2Text != null: slotName = slot2Text.text; break;
            case 3 when slot3Text != null: slotName = slot3Text.text; break;
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
        string playerName = nameInputField?.text?.Trim() ?? "Player";
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