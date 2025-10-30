using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HUDDisplayPanel : MonoBehaviour
{
    [Header("Scriptable Objects")]
    public FeatureDataSO featureData;
    public SensorDataSO sensorData;
    public BikeDataSO bikeData;
    public UIDataSO uiData;
    public AudioSource beepSoundAS;

    [Header("Collision Images")]
    public CanvasGroup FrontCollisionImage;
    public CanvasGroup RearCollisionImage;
    public CanvasGroup LeftCollisionImage;
    public CanvasGroup RightCollisionImage;

    [Header("CloseVehicle Images")]

    public CanvasGroup FrontImage;
    public CanvasGroup RearImage;
    public CanvasGroup LeftImage;
    public CanvasGroup RightImage;



    // local variables
    private Coroutine displayRoutine;
    private Coroutine blinkIconRoutine;

    private void OnEnable()
    {
        sensorData.LeftBlindSpotTriggerEnterEvent += BlindSpotTriggerEnter;
        sensorData.RightBlindSpotTriggerEnterEvent += RightBlindSpotTriggerEnter;
        sensorData.FrontCollisionTriggerEnterEvent += FrontCollisionEnter;
        sensorData.RearCollisionTriggerEnterEvent += RearCollisionEnter;


        bikeData.CloseVehicleEvent += ShowCloseVehicleIcons;


        sensorData.LeftBlindSpotTriggerExitEvent += ClearMessage;
        sensorData.FrontCollisionTriggerExitEvent += ClearMessage;
        sensorData.RearCollisionTriggerExitEvent += ClearMessage;
        sensorData.RightBlindSpotTriggerExitEvent += ClearMessage;
        uiData.ClearAllTextEvent += ClearMessage;

    }

    private void OnDisable()
    {

        sensorData.LeftBlindSpotTriggerEnterEvent -= BlindSpotTriggerEnter;
        sensorData.FrontCollisionTriggerEnterEvent -= FrontCollisionEnter;
        sensorData.RearCollisionTriggerEnterEvent -= RearCollisionEnter;
        sensorData.RightBlindSpotTriggerEnterEvent -= RightBlindSpotTriggerEnter;


        bikeData.CloseVehicleEvent -= ShowCloseVehicleIcons;

        sensorData.LeftBlindSpotTriggerExitEvent -= ClearMessage;
        sensorData.FrontCollisionTriggerExitEvent -= ClearMessage;
        sensorData.RearCollisionTriggerExitEvent -= ClearMessage;
        sensorData.RightBlindSpotTriggerExitEvent -= ClearMessage;
        uiData.ClearAllTextEvent -= ClearMessage;

    }



    private void BlindSpotTriggerEnter()
    {
        beepSoundAS.Play();
        blinkIconRoutine = StartCoroutine(FadeCoroutine(LeftCollisionImage,true));
    }

    private void RightBlindSpotTriggerEnter()
    {
        beepSoundAS.Play();
        blinkIconRoutine = StartCoroutine(FadeCoroutine(RightCollisionImage,true));
    }

    private void FrontCollisionEnter()
    {
        beepSoundAS.Play();
        blinkIconRoutine = StartCoroutine(FadeCoroutine(FrontCollisionImage,true));
    }

    private void RearCollisionEnter()
    {
        beepSoundAS.Play();
        blinkIconRoutine = StartCoroutine(FadeCoroutine(RearCollisionImage,true));
    }


    private void ClearMessage()
    { 
        StopAllCoroutines();
        ResetIcons();
        beepSoundAS.Stop();        
    }


    private void ShowCloseVehicleIcons(CollisionType type)
    {
        if(type == CollisionType.Front)
        {
            StartCoroutine(ShowCloseVehicleIconsRoutine(FrontImage, 2f));
        }
        else if(type == CollisionType.Rear)
        {
           StartCoroutine(ShowCloseVehicleIconsRoutine(RearImage, 2f));
        }
        else if(type == CollisionType.Left)
        {
           StartCoroutine(ShowCloseVehicleIconsRoutine(LeftImage, 2f));
        }
        else if(type == CollisionType.Right)
        {
           StartCoroutine(ShowCloseVehicleIconsRoutine(RightImage, 2f));
        }
    }

    private IEnumerator ShowCloseVehicleIconsRoutine(CanvasGroup targetGroup, float displaySeconds)
    {
       
        yield return StartCoroutine(FadeCoroutine(targetGroup, true, 0.2f));

        // Wait the requested display time
        float elapsed = 0f;
        while (elapsed < displaySeconds)
        {
            elapsed += Time.deltaTime;
            yield return null;
        }

        // Fade out and clear reference
        yield return StartCoroutine(FadeCoroutine(targetGroup, false, 0.25f));
     
    }


    public IEnumerator FadeCoroutine(CanvasGroup targetGroup, bool canIShow,float fadeDuration = 0.5f)
    {
        float startAlpha = 0;
        float endAlpha = 1;
        if (!canIShow)
        {
            startAlpha = 1;
            endAlpha = 0;    
        }
      
        float time = 0f;

        while (time < fadeDuration)
        {
            time += Time.deltaTime;
            targetGroup.alpha = Mathf.Lerp(startAlpha, endAlpha, time / fadeDuration);
            yield return null;
        }
    }


    private void ResetIcons()
    {
        if (LeftCollisionImage.alpha > 0.5f)
        {
            StartCoroutine(FadeCoroutine(LeftCollisionImage, false));
        }

        if (FrontCollisionImage.alpha > 0.5f)
        {
            StartCoroutine(FadeCoroutine(FrontCollisionImage, false));
        }

        if (RearCollisionImage.alpha > 0.5f)
        {
            StartCoroutine(FadeCoroutine(RearCollisionImage, false));
        }

        if(RightCollisionImage.alpha > 0.5f)
        {
            StartCoroutine(FadeCoroutine(RightCollisionImage, false));
        }

        if(LeftImage.alpha > 0.5f)
        {
            StartCoroutine(FadeCoroutine(LeftImage, false));
        }

        if (FrontImage.alpha > 0.5f)
        {
            StartCoroutine(FadeCoroutine(FrontImage, false));
        }

        if (RearImage.alpha > 0.5f)
        {
            StartCoroutine(FadeCoroutine(RearImage, false));
        }
        if (RightImage.alpha > 0.5f)
        {
            StartCoroutine(FadeCoroutine(RightImage, false));
        }

    }

   
}
