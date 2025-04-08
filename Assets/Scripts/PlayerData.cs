using System;

[Serializable]
public class PlayerData
{
    public int score;
    public string playerName;
    public DateTime lastSaveTime;
    
    // Добавьте другие необходимые данные игры
    public PlayerData()
    {
        score = 0;
        playerName = "Player";
        lastSaveTime = DateTime.Now;
    }
}