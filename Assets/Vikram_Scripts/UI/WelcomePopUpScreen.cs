using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class WelocmePopUpScreenScreen : MonoBehaviour
{
    [Header("Scriptable Objects")]
    public UIDataSO uiData;
    public InputDataSO inputData;

    [Header("UI Elements")]
    public CanvasGroup canvasGroup;
    public GameObject nextPanel;
    public GameObject prevPanel;
    public Image NextBtnImage;
    public TMP_Text EnglishNameText;
    public TMP_Text ItalianNameText;


    [Header("Other Objects")]
    public AudioSource audioSource;
    public Color grayColor;
    public Color hightlightColor;


    [Header("Local Variables")]
    public float fadeDuration = 0.5f;
    public float waitTime;
    public bool isFading = false;
    public bool canITransistion = false;


    private void OnEnable()
    {
        canITransistion = false;
        NextBtnImage.color = grayColor;
        isFading = false;
        inputData.RightUIButtonClickedEvent += RightBrakeClicked;


        string playerName = uiData.PlayerInfo.playerName;
        EnglishNameText.text = $"Welcome <color=#FF0000>{playerName}</color>";
        ItalianNameText.text = $"Benvenuto <color=#FF0000>{playerName}</color>";
        StartCoroutine(WaitAndTransistion());
    }

    private void OnDisable()
    {
        inputData.RightUIButtonClickedEvent -= RightBrakeClicked;
 
    }


    private IEnumerator WaitAndTransistion()
    {
        yield return new WaitForSeconds(audioSource.clip.length);
        canITransistion = true;
        NextBtnImage.color = hightlightColor;
    }


    private void LeftBrakeClicked(float val)
    {
        if (!isFading && canITransistion) {
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
