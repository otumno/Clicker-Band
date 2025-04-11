using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;

public class SceneTransitionManager : MonoBehaviour
{
    public enum TransitionAction { LoadScene, ExitGame }

    [Header("Transition Settings")]
    [SerializeField] private float fadeDuration = 1f;
    [SerializeField] private AudioClip transitionSound;
    [SerializeField] private Canvas fadeCanvas;
    [SerializeField] private Image fadeImage;
    [SerializeField] private TransitionAction action;
    [SerializeField] private string sceneToLoad;

    private void Start()
    {
        if (fadeCanvas == null || fadeImage == null) return;
        fadeCanvas.gameObject.SetActive(true);
        StartCoroutine(FadeIn());
    }

    public void OnButtonClick() => StartCoroutine(Transition());

    private IEnumerator Transition()
    {
        AudioManager.Instance?.PlaySFX(transitionSound, 1f, AudioCategory.MenuSFX);
        
        yield return StartCoroutine(FadeOut());

        // Добавляем остановку метронома только при переходе в меню
        if (action == TransitionAction.LoadScene && sceneToLoad == "Menu")
        {
            MetronomeManager.Instance?.StopMetronomeOnMenu();
        }

        ExecuteTransitionAction();
    }

    private void ExecuteTransitionAction()
    {
        switch (action)
        {
            case TransitionAction.LoadScene:
                SceneManager.LoadScene(sceneToLoad);
                break;
            
            case TransitionAction.ExitGame:
                Application.Quit();
                #if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
                #endif
                break;
        }
    }

    private IEnumerator FadeOut()
    {
        float elapsedTime = 0f;
        Color color = fadeImage.color;
        fadeCanvas.gameObject.SetActive(true);

        while (elapsedTime < fadeDuration)
        {
            color.a = Mathf.Clamp01(elapsedTime / fadeDuration);
            fadeImage.color = color;
            elapsedTime += Time.deltaTime;
            yield return null;
        }
    }

    private IEnumerator FadeIn()
    {
        float elapsedTime = 0f;
        Color color = fadeImage.color;

        while (elapsedTime < fadeDuration)
        {
            color.a = 1 - Mathf.Clamp01(elapsedTime / fadeDuration);
            fadeImage.color = color;
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        fadeCanvas.gameObject.SetActive(false);
    }
}