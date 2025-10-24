using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class FeaturePopUpPanel : MonoBehaviour
{
    [Header(" Scriptable Objects")]
    public UIDataSO uiData;

    [Header(" UI Elements")]
    public CanvasGroup canvasGroup;
    public TMP_Text displayText;
    public TMP_Text actionText;
    public Image timerImage;
    public float duration = 3;

    private void OnEnable()
    {
        uiData.ShowZoneEnterPopUpEvent += ShowZonePopUp;
        uiData.TakeActionEvent += ShowActionPopup;
    }

    private void OnDisable()
    {
        uiData.ShowZoneEnterPopUpEvent -= ShowZonePopUp;
        uiData.TakeActionEvent -= ShowActionPopup;
    }



    private void ShowZonePopUp(FeatureType type)
    {
        if(type == FeatureType.BlindSpot)
        {
            displayText.text = "Showcasing: Blind Spot Detection(BSD)";
        }
        else if (type == FeatureType.FrontVehicle)
        {
            displayText.text =  "Showcasing: Forward Collision Warning(FCW)";
        }
        else if (type == FeatureType.RearVehicle)
        {
            displayText.text = "Showcasing: Rear Collision Warning (RCW)";
        }

        StartCoroutine(PopUp());
    }
    private void ShowActionPopup()
    {
        actionText.text = "Take Action!";
        canvasGroup.alpha = 1;
        timerImage.fillAmount = 1;
        StartCoroutine(StartTimer());
    }

    private IEnumerator PopUp()
    {
        canvasGroup.alpha = 1;
        yield return new WaitForSeconds(3);
        canvasGroup.alpha = 0;
        displayText.text = "";
    }

    /// <summary>
    /// Coroutine to decrease the timer fill from 1 → 0 over 'duration' seconds.
    /// </summary>
    private IEnumerator StartTimer()
    {
        float elapTime = 0f;
        timerImage.fillAmount = 1f;

        while (elapTime < duration)
        {
            elapTime += Time.deltaTime;
            float progress = elapTime / duration;

            // Fill goes from 1 → 0 as time progresses
            timerImage.fillAmount = Mathf.Lerp(1f, 0f, progress);

            yield return null; // Frame-independent and smoother than WaitForEndOfFrame
        }

        // Ensure it ends exactly at zero
        timerImage.fillAmount = 0f;
        canvasGroup.alpha = 0;
        actionText.text = "";
    }
}
