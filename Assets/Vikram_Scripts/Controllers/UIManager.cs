using System;
using System.Collections;
using TMPro;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    [Header(" Scriptable Objects ")]
    public UIDataSO uiData;
    public BikeDataSO bikeData;

    [Header(" Fade Settings ")]
    public CanvasGroup FadeCanvas;
    public CanvasGroup RaceCompletedPanel;
    public TMP_Text fadeInText;
    
    public float fadeDuration = 0.5f;
    public float duration = 0.5f;
    private Coroutine fadeRoutine;
    private bool isFading = false;

    private void OnEnable()
    {
        uiData.FadeCanvasEvent += FadeCanvasGroup;
        bikeData.RaceCompletedEvent += RaceCompleted;
    }

    private void OnDisable()
    {
        uiData.FadeCanvasEvent -= FadeCanvasGroup;
        bikeData.RaceCompletedEvent -= RaceCompleted;
    }

    private void RaceCompleted()
    {
        if (!isFading)
        {
            StartCoroutine(SwitchPanels(RaceCompletedPanel));
        }
    }

    private void FadeCanvasGroup(int endAlpha)
    {
        if (FadeCanvas == null) return;

        if (fadeRoutine != null)
            StopCoroutine(fadeRoutine);

        fadeRoutine = StartCoroutine(FadeCanvasCoroutine(endAlpha));
    }

    private IEnumerator FadeCanvasCoroutine(int endAlpha)
    {
        Debug.Log("Fading Canvas to alpha: " + endAlpha);
        float startAlpha = FadeCanvas.alpha;
        float targetAlpha = Mathf.Clamp01(endAlpha);
        float elapsed = 0f;

        while (elapsed < duration)
        {
            FadeCanvas.alpha = Mathf.Lerp(startAlpha, targetAlpha, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }
        FadeCanvas.alpha = targetAlpha;
    }


    private IEnumerator SwitchPanels(CanvasGroup nextObject)
    {
        isFading = true;

        nextObject.gameObject.SetActive(true);
        var nextCanvas = nextObject.GetComponent<CanvasGroup>();
        yield return FadeCanvasPanel(nextCanvas, 0f, 1f, fadeDuration);

        isFading = false;
    }

    private IEnumerator FadeCanvasPanel(CanvasGroup group, float startAlpha, float endAlpha, float duration)
    {
        float elapsed = 0f;
        group.alpha = startAlpha;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            group.alpha = Mathf.Lerp(startAlpha, endAlpha, elapsed / duration);
            yield return null;
        }

        group.alpha = endAlpha;

    }
}
