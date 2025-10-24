using System.Collections;
using UnityEngine;

public class WelocmePopUpScreenScreen : MonoBehaviour
{
    [Header("Scriptable Objects")]
    public UIDataSO uiData;
    public InputDataSO inputData;

    [Header("UI Elements")]
    public AudioSource audioSource;
    public CanvasGroup canvasGroup;
    public GameObject nextPanel;
    public GameObject prevPanel;
    public float fadeDuration = 0.5f;
    public float waitTime;

    // local variables
    public bool isFading = false;
    public bool canITransistion = false;


    private void OnEnable()
    {
        canITransistion = false;
        isFading = false;
        inputData.RightUIButtonClickedEvent += RightBrakeClicked;
     
        StartCoroutine(WaitAndTransistion());
    }

    private void OnDisable()
    {
        inputData.RightUIButtonClickedEvent -= RightBrakeClicked;
        canITransistion = false;
        isFading = false;
    }


    private IEnumerator WaitAndTransistion()
    {
        yield return new WaitForSeconds(audioSource.clip.length);
        canITransistion = true;
    }

    private void RightBrakeClicked(float val)
    {
        if (!isFading && canITransistion)
            StartCoroutine(SwitchPanels(nextPanel));
    }

    private IEnumerator SwitchPanels(GameObject nextObject)
    {
        isFading = true;

        // Fade out current
        yield return FadeCanvas(canvasGroup, 1f, 0f, fadeDuration);
       

        // Fade in next
        if (nextObject)
        {
            nextObject.SetActive(true);
            var nextCanvas = nextObject.GetComponent<CanvasGroup>();
            if (!nextCanvas) nextCanvas = nextObject.AddComponent<CanvasGroup>();
            yield return FadeCanvas(nextCanvas, 0f, 1f, fadeDuration);
        }

      
        isFading = false;

        gameObject.SetActive(false);
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
