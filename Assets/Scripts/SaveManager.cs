using System;
using System.IO;
using UnityEngine;

public class SaveManager : MonoBehaviour
{
    public static SaveManager Instance;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void SaveGame(PlayerData data, int slot)
    {
        data.lastSaveTime = DateTime.Now;
        string json = JsonUtility.ToJson(data);
        File.WriteAllText(GetSavePath(slot), json);
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
            Debug.Log($"Save in slot {slot} deleted");
        }
    }

    public DateTime GetSaveTime(int slot)
    {
        if (SaveExists(slot))
        {
            return LoadGame(slot).lastSaveTime;
        }
        return DateTime.MinValue;
    }

    private string GetSavePath(int slot)
    {
        return Path.Combine(Application.persistentDataPath, $"save_{slot}.json");
    }
}