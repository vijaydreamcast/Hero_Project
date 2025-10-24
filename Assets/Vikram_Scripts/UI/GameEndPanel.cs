using System.Collections;
using UnityEngine;

public class GameEndPanel : MonoBehaviour
{
    [Header("Scriptable Objects")]
    public UIDataSO uiData;
    public InputDataSO inputData;

    [Header("UI Elements")]
    public CanvasGroup container;
    public CanvasGroup canvasGroup;
    public GameObject nextPanel;
    public float fadeDuration = 0.5f;

    // local variables
    public bool isFading = false;

    private void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        if (!canvasGroup)
        {
            canvasGroup = gameObject.AddComponent<CanvasGroup>();
        }
    }



    private void OnEnable()
    {
        container.alpha = 1;
        isFading = false;
        inputData.RightUIButtonClickedEvent += RightCliked;
    }

    private void OnDisable()
    {
       
        inputData.RightUIButtonClickedEvent -= RightCliked;
    }


    private void RightCliked(float val)
    {
        if (!isFading)
            StartCoroutine(SwitchPanels());
    }

    private IEnumerator SwitchPanels()
    {
        isFading = true;

        // Fade out current
        yield return FadeCanvas(canvasGroup, 1f, 0f, fadeDuration);
        gameObject.SetActive(false);

        // Fade in next
        if (nextPanel)
        {
            nextPanel.SetActive(true);
            var nextCanvas = nextPanel.GetComponent<CanvasGroup>();
            if (!nextCanvas) nextCanvas = nextPanel.AddComponent<CanvasGroup>();
            yield return FadeCanvas(nextCanvas, 0f, 1f, fadeDuration);
        }

        isFading = false;
    }

    private IEnumerator FadeCanvas(CanvasGroup group, float startAlpha, float endAlpha, float duration)
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
