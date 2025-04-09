using System;

[Serializable]
public class PlayerData
{
    public string playerName = "Player";
    public int score;
    public DateTime lastSaveTime;
    
    public PlayerData()
    {
        lastSaveTime = DateTime.Now;
    }
}