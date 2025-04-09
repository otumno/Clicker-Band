using UnityEngine;
using System;
using System.IO;

public class SaveManager : MonoBehaviour
{
    public static SaveManager Instance { get; private set; }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void AutoCreate()
    {
        if (Instance == null)
        {
            GameObject obj = new GameObject("SaveManager");
            Instance = obj.AddComponent<SaveManager>();
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

    public void SaveGame(PlayerData data, int slot)
    {
        data.lastSaveTime = DateTime.Now;
        string json = JsonUtility.ToJson(data);
        File.WriteAllText(GetSavePath(slot), json);
        Debug.Log($"Game saved to slot {slot}");
    }

    public PlayerData LoadGame(int slot)
    {
        string path = GetSavePath(slot);
        if (File.Exists(path))
        {
            string json = File.ReadAllText(path);
            return JsonUtility.FromJson<PlayerData>(json);
        }
        return null;
    }

    public bool SaveExists(int slot)
    {
        return File.Exists(GetSavePath(slot));
    }

    public void DeleteSave(int slot)
    {
        string path = GetSavePath(slot);
        if (File.Exists(path))
        {
            File.Delete(path);
            Debug.Log($"Deleted save slot {slot}");
        }
    }

    private string GetSavePath(int slot)
    {
        return Path.Combine(Application.persistentDataPath, $"save_{slot}.json");
    }
}