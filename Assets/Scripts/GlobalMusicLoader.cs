using UnityEngine;

public class GlobalMusicLoader : MonoBehaviour
{
    [SerializeField] private GameObject globalMusicPrefab;

    private void Start()
    {
        // Проверяем, существует ли уже экземпляр
        if (FindObjectOfType<GlobalMusicPlayer>() == null)
        {
            // Создаем новый экземпляр из префаба
            Instantiate(globalMusicPrefab);
        }
    }
}