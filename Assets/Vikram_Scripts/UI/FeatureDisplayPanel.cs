using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class FeatureDisplayPanel : MonoBehaviour
{

    [Header(" Scriptable Objects")]
    public UIDataSO uiData;
    public FeatureDataSO featureData;
    public InputDataSO inputData;

    [Header(" UI Elements")]
    public CanvasGroup container;
    public CanvasGroup group;
    public GameObject CorrectActionImage;
    public GameObject WrongActionImage;
    public TMP_Text headingText;
    public Image featureIcon;
    public TMP_Text correctDesciptionText;
    public TMP_Text wrongDescriptionText;

    // local variables
    private Coroutine displayRoutine;
    private bool isFeatureShown = false;


    private void OnEnable()
    {
    
        inputData.RightUIButtonClickedEvent += TurnOffPanel;
    }

    private void OnDisable()
    {

        inputData.RightUIButtonClickedEvent -= TurnOffPanel;
    }


    private void TurnOffPanel(float val)
    {
        if (group.alpha == 1)
        {
            uiData.ClearText();
            inputData.ActivateInput();
            group.alpha = 0f;
            container.alpha = 0;
        }

    }

    public void ShowFeatureResult(FeatureType featureType,FeatureResult result)
    {
        if(featureType == FeatureType.BlindSpot)
        {
            headingText.text = featureData.BlindSpotData.featureName;
            featureIcon.sprite = featureData.BlindSpotData.featureIcon;
            correctDesciptionText.text = featureData.BlindSpotData.correctAction;
            wrongDescriptionText.text = featureData.BlindSpotData.wrongAction;
        }
        else if (featureType == FeatureType.FrontVehicle)
        {
            headingText.text = featureData.FrontCollisionData.featureName;
            featureIcon.sprite = featureData.FrontCollisionData.featureIcon;
            correctDesciptionText.text = featureData.FrontCollisionData.correctAction;
            wrongDescriptionText.text = featureData.FrontCollisionData.wrongAction;
        }
        else if (featureType == FeatureType.RearVehicle)
        {
            headingText.text = featureData.RearCollisionData.featureName;
            featureIcon.sprite = featureData.RearCollisionData.featureIcon;
            correctDesciptionText.text = featureData.RearCollisionData.correctAction;
            wrongDescriptionText.text = featureData.RearCollisionData.wrongAction;
        }
        else if (featureType == FeatureType.CloseVehicle)
        {
            headingText.text = featureData.CloseVehicleData.featureName;
            featureIcon.sprite = featureData.CloseVehicleData.featureIcon;
            correctDesciptionText.text = featureData.CloseVehicleData.correctAction;
            wrongDescriptionText.text = featureData.CloseVehicleData.wrongAction;
        }

        if(result == FeatureResult.Correct)
        {
            CorrectActionImage.SetActive(true);
            WrongActionImage.SetActive(false);
        }
        else
        {
            CorrectActionImage.SetActive(false);
            WrongActionImage.SetActive(true);
        }

      
         StartCoroutine(FadeCanvasGroupCoroutine());
    }
    public IEnumerator FadeCanvasGroupCoroutine()
    {
        if (group == null)
            yield break;

        // Fade in
        float duration = 1.5f;
        float elapsed = 0f;
        group.alpha = 0f;
        container.alpha = 1;
        yield return new WaitForSeconds(1); // slight delay

        while (elapsed < duration)
        {
            group.alpha = Mathf.Lerp(0f, 1f, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }
        group.alpha = 1f;

    }
}
