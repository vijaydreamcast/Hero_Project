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


    // local variables
    private Coroutine displayRoutine;
    private Coroutine blinkIconRoutine;

    private void OnEnable()
    {
        sensorData.LeftBlindSpotTriggerEnterEvent += BlindSpotTriggerEnter;
        sensorData.FrontCollisionTriggerEnterEvent += FrontCollisionEnter;
        sensorData.RearCollisionTriggerEnterEvent += RearCollisionEnter;


        sensorData.LeftBlindSpotTriggerExitEvent += ClearMessage;
        sensorData.FrontCollisionTriggerExitEvent += ClearMessage;
        sensorData.RearCollisionTriggerExitEvent += ClearMessage;

        uiData.ClearAllTextEvent += ClearMessage;

   

    }



    private void OnDisable()
    {

        sensorData.LeftBlindSpotTriggerEnterEvent -= BlindSpotTriggerEnter;
        sensorData.FrontCollisionTriggerEnterEvent -= FrontCollisionEnter;
        sensorData.RearCollisionTriggerEnterEvent -= RearCollisionEnter;


        sensorData.LeftBlindSpotTriggerExitEvent -= ClearMessage;
        sensorData.FrontCollisionTriggerExitEvent -= ClearMessage;
        sensorData.RearCollisionTriggerExitEvent -= ClearMessage;

        uiData.ClearAllTextEvent -= ClearMessage;



    }

    //private void ShowCollisionImage(CollisionType type)
    //{
      
    //    if (type == CollisionType.Front)
    //    {
    //        StartCoroutine(ShowAllCollisionImages(FrontCollisionImage.gameObject));
    //    }
    //    else if (type == CollisionType.Rear)
    //    {
    //        StartCoroutine(ShowAllCollisionImages(RearCollisionImage.gameObject));
    //    }
    //    if (type == CollisionType.Left)
    //    {
    //        StartCoroutine(ShowAllCollisionImages(LeftCollisionImage.gameObject));
    //    }
    //    if (type == CollisionType.Right)
    //    {
    //        StartCoroutine(ShowAllCollisionImages(RightCollisionImage.gameObject));
    //    }
       
    //}

    private void BlindSpotTriggerEnter()
    {
        beepSoundAS.Play();
        blinkIconRoutine = StartCoroutine(FadeCoroutine(LeftCollisionImage,true));
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
        Debug.Log(" Clearing Icons");
        StopAllCoroutines();
        ResetIcons();
        beepSoundAS.Stop();
        
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
        if (LeftCollisionImage.alpha == 1)
        {
            StartCoroutine(FadeCoroutine(LeftCollisionImage, false));
        }

        if (FrontCollisionImage.alpha == 1)
        {
            StartCoroutine(FadeCoroutine(FrontCollisionImage, false));
        }

        if (RearCollisionImage.alpha == 1)
        {
            StartCoroutine(FadeCoroutine(RearCollisionImage, false));
        }

    }

   
}
