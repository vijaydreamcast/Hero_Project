using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class GameRulesPopUpScreen : MonoBehaviour
{
    [Header("Scriptable Objects")]
    public UIDataSO uiData;
    public InputDataSO inputData;

    [Header("UI Elements")]
    public CanvasGroup canvasGroup;
    public GameObject nextPanel;
    public GameObject prevPanel;
    public Image NextBtnImage;
    public Image PreviousBtnImage;


    [Header("Other Objects")]
    public AudioSource audioSource;
    public Color grayColor;
    public Color hightlightColor;


    // local variables
    [Header("Local Variables")]
    public float fadeDuration = 0.5f;
    public bool isFading = false;
    public bool canITransistion = false;


    private void OnEnable()
    {
        canITransistion = false;
        isFading = false;
        NextBtnImage.color = grayColor;
        PreviousBtnImage.color = grayColor;

        inputData.RightUIButtonClickedEvent += RightBrakeClicked;
        inputData.LeftUIButtonClickedEvent += LeftBrakeClicked;
        StartCoroutine(WaitAndTransistion());
    }

    private void OnDisable()
    {
        inputData.RightUIButtonClickedEvent -= RightBrakeClicked;
        inputData.LeftUIButtonClickedEvent -= LeftBrakeClicked;
    }

    private IEnumerator WaitAndTransistion()
    {
        yield return new WaitForSeconds(audioSource.clip.length);
        canITransistion = true;
        NextBtnImage.color = hightlightColor;
        PreviousBtnImage.color = hightlightColor;
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
        if (nextObject)
        {
            nextObject.SetActive(true);
            var nextCanvas = nextObject.GetComponent<CanvasGroup>();
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
