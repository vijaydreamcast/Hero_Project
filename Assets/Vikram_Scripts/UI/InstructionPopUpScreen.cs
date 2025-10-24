using System.Collections;
using UnityEngine;
using UnityEngine.Audio;

public class InstructionPopUpScreen : MonoBehaviour
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

    // local variables
    public bool isFading = false;
    public bool canITransistion = false;



    private void OnEnable()
    {
        canITransistion = false;
        isFading = false;
        inputData.RightUIButtonClickedEvent += RightBrakeClicked;
        inputData.LeftUIButtonClickedEvent += LeftBrakeClicked;
        StartCoroutine(WaitAndTransistion());
    }

    private void OnDisable()
    {
        inputData.RightUIButtonClickedEvent -= RightBrakeClicked;
        inputData.LeftUIButtonClickedEvent -= LeftBrakeClicked;
        canITransistion = false;
        isFading = false;
    }

    private IEnumerator WaitAndTransistion()
    {
        yield return new WaitForSeconds(audioSource.clip.length);
        canITransistion = true;
    }



    private void LeftBrakeClicked(float val)
    {
        if (!isFading && canITransistion)
        {
            StartCoroutine(SwitchPanels(prevPanel));
        }
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
  
        nextObject.SetActive(true);

        var nextCanvas = nextObject.GetComponent<CanvasGroup>();
        yield return FadeCanvas(nextCanvas, 0f, 1f, fadeDuration);
        

        isFading = false;

        gameObject.SetActive(false);
    }

    private IEnumerator FadeCanvas(CanvasGroup group, float startAlpha, float endAlpha, float duration)
    {
        float elapsed = 0f;
        group.alpha = startAlpha;

        while (elapsed < duration)
        {
          
            group.alpha = Mathf.Lerp(startAlpha, endAlpha, elapsed / duration);
            yield return new WaitForEndOfFrame();
            elapsed += Time.deltaTime;
        }

        group.alpha = endAlpha;

    }
}
