using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public PlayerData currentPlayerData;
    public int currentSlot;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void AutoCreate()
    {
        if (Instance == null)
        {
            GameObject obj = new GameObject("GameManager");
            Instance = obj.AddComponent<GameManager>();
            DontDestroyOnLoad(obj);
        }
    }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
        }
    }

    public void CreateNewGame(int slot)
    {
        currentPlayerData = new PlayerData();
        currentSlot = slot;
        SaveManager.Instance.SaveGame(currentPlayerData, slot);
    }

    public void LoadGame(int slot)
    {
        currentPlayerData = SaveManager.Instance.LoadGame(slot);
        currentSlot = slot;
    }

    public void SaveCurrentGame()
    {
        if (currentPlayerData != null)
        {
            SaveManager.Instance.SaveGame(currentPlayerData, currentSlot);
        }
    }
}