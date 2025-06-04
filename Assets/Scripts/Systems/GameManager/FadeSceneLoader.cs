using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System;
using System.Collections;

public class FadeSceneLoader : MonoBehaviour
{
    public Image fadeImage;
    public float fadeDuration = 1f;

    private Action onSceneLoadedCallback;

    void Start()
    {
        StartCoroutine(FadeIn());
        SceneManager.sceneLoaded += OnSceneLoaded; // Se registra para ejecutar el callback después del cambio de escena
    }

    void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    public void LoadScene(string sceneName, Action onSceneLoaded = null)
    {
        onSceneLoadedCallback = onSceneLoaded;
        StartCoroutine(FadeOutAndLoad(sceneName));
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        StartCoroutine(FadeInAfterSceneLoad());
        onSceneLoadedCallback?.Invoke();
        onSceneLoadedCallback = null;
    }

    IEnumerator FadeInAfterSceneLoad()
    {
        yield return null;
        yield return FadeIn();
    }

    IEnumerator FadeIn()
    {
        float t = fadeDuration;
        Color c = fadeImage.color;
        fadeImage.gameObject.SetActive(true);
        while (t > 0)
        {
            t -= Time.deltaTime;
            c.a = Mathf.Clamp01(t / fadeDuration);
            fadeImage.color = c;
            yield return null;
        }
        fadeImage.gameObject.SetActive(false);
    }

    IEnumerator FadeOutAndLoad(string sceneName)
    {
        fadeImage.gameObject.SetActive(true);
        float t = 0f;
        Color c = fadeImage.color;
        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            c.a = Mathf.Clamp01(t / fadeDuration);
            fadeImage.color = c;
            yield return null;
        }

        SceneManager.LoadScene(sceneName);
    }
}
