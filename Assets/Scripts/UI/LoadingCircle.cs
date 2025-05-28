using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;

public class LoadingCircle : MonoBehaviour
{
    [Header("UI Fill Image")]
    public Image fillImage;

    private Coroutine currentRoutine;

    /// <summary>
    /// Inicia la verificación de una condición mantenida durante un tiempo.
    /// </summary>
    /// <param name="condition">Función que devuelve un bool para verificar condición.</param>
    /// <param name="duration">Tiempo en segundos que la condición debe mantenerse.</param>
    /// <param name="onSuccess">Acción a ejecutar si se mantiene la condición todo el tiempo.</param>
    public void CheckConditionForTime(Func<bool> condition, float duration, Action onSuccess, Action onFail)
    {
        if (currentRoutine != null)
        {
            StopCoroutine(currentRoutine);
        }

        currentRoutine = StartCoroutine(ConditionCoroutine(condition, duration, onSuccess, onFail));
    }

    private IEnumerator ConditionCoroutine(Func<bool> condition, float duration, Action onSuccess, Action onFail)
    {
        float elapsed = 0f;
        fillImage.fillAmount = 0f;

        while (elapsed < duration)
        {
            if (!condition())
            {
                fillImage.fillAmount = 0f;
                fillImage.fillAmount = 0f;
                onFail?.Invoke();
                yield break;
            }

            elapsed += Time.deltaTime;
            fillImage.fillAmount = elapsed / duration;
            yield return null;
        }

        fillImage.fillAmount = 0f;
        onSuccess?.Invoke();
    }
}