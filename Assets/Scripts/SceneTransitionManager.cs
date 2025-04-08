using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneTransitionManager : MonoBehaviour
{
    public enum TransitionAction
    {
        LoadScene,
        ExitGame
    }

    [Header("Fade Settings")]
    [SerializeField] private float defaultFadeDuration = 1f;
    [SerializeField] private float firstLaunchFadeDuration = 3f;
    [SerializeField] private string firstLaunchKey = "IsFirstLaunch";

    [Header("Transition Settings")]
    public AudioClip transitionSound;
    [SerializeField] private Canvas fadeCanvas;
    [SerializeField] private Image fadeImage;
    [SerializeField] private TransitionAction action;
    [SerializeField] private string sceneToLoad;

    private void Start()
    {
        if (fadeCanvas == null || fadeImage == null)
        {
            Debug.LogError("FadeCanvas или FadeImage не установлены!");
            return;
        }

        fadeCanvas.gameObject.SetActive(true);

        float currentFadeDuration = IsFirstLaunch() ? firstLaunchFadeDuration : defaultFadeDuration;
        if (IsFirstLaunch()) MarkAsLaunched();

        StartCoroutine(FadeIn(currentFadeDuration));
    }

    private bool IsFirstLaunch() => !PlayerPrefs.HasKey(firstLaunchKey);
    private void MarkAsLaunched() => PlayerPrefs.SetInt(firstLaunchKey, 1);

    public void OnButtonClick() => StartCoroutine(Transition());

    private System.Collections.IEnumerator Transition()
    {
        // Проигрываем звук через AudioManager с категорией MenuSFX
        AudioManager.Instance.PlaySFX(transitionSound, 1f, AudioCategory.MenuSFX);
        
        // Запускаем затенение одновременно со звуком
        yield return StartCoroutine(FadeOut());

        if (action == TransitionAction.LoadScene)
        {
            SceneManager.LoadScene(sceneToLoad);
        }
        else if (action == TransitionAction.ExitGame)
        {
            Application.Quit();
            #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
            #endif
        }
    }

    private System.Collections.IEnumerator FadeOut()
    {
        float elapsedTime = 0f;
        Color color = fadeImage.color;
        fadeCanvas.gameObject.SetActive(true);

        while (elapsedTime < defaultFadeDuration)
        {
            elapsedTime += Time.deltaTime;
            color.a = Mathf.Clamp01(elapsedTime / defaultFadeDuration);
            fadeImage.color = color;
            yield return null;
        }
        color.a = 1;
        fadeImage.color = color;
    }

    private System.Collections.IEnumerator FadeIn(float duration)
    {
        float elapsedTime = 0f;
        Color color = fadeImage.color;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            color.a = 1 - Mathf.Clamp01(elapsedTime / duration);
            fadeImage.color = color;
            yield return null;
        }
        fadeCanvas.gameObject.SetActive(false);
    }
}