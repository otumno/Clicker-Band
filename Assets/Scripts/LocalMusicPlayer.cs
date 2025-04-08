using UnityEngine;
using System.Collections;

public class LocalMusicPlayer : MonoBehaviour
{
    [Header("Music Settings")]
    [SerializeField] private AudioClip levelMusic;
    [Range(0.1f, 1f)] public float volume = 1f;
    [SerializeField] private float fadeInTime = 1f;
    
    [Header("Global Music Control")]
    [SerializeField] private bool controlGlobalMusic = true;
    [SerializeField] private float globalFadeOutTime = 0.5f;
    [SerializeField] private float globalVolumeDuringLocalMusic = 0.1f;

    private void Start()
    {
        StartCoroutine(StartMusicSequence());
    }

    private IEnumerator StartMusicSequence()
    {
        if (controlGlobalMusic && AudioManager.Instance != null)
        {
            AudioManager.Instance.SetGlobalMusicVolume(globalVolumeDuringLocalMusic, globalFadeOutTime);
            yield return new WaitForSeconds(globalFadeOutTime);
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