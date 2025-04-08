using UnityEngine;
using System.Collections;

public class LevelMusic : MonoBehaviour
{
    [Header("Music Settings")]
    [SerializeField] private AudioClip levelMusic;
    [Range(0.1f, 1f)] public float volume = 1f;
    [SerializeField] private float fadeInTime = 1f;
    
    [Header("Global Music Control")]
    [SerializeField] private bool controlGlobalMusic = true;
    [SerializeField] private float globalFadeOutTime = 0.5f;

    private IEnumerator Start()
    {
        if (controlGlobalMusic && AudioManager.Instance != null)
        {
            yield return AudioManager.Instance.FadeGlobalMusic(0.1f, globalFadeOutTime);
        }

        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayLocalMusic(levelMusic, volume, fadeInTime > 0);
        }
    }

    private void OnDestroy()
    {
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.StopLocalMusic(true);
            
            if (controlGlobalMusic)
            {
                AudioManager.Instance.RestoreGlobalMusic(1f);
            }
        }
    }
}