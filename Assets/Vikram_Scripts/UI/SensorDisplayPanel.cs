using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SensorDisplayPanel : MonoBehaviour
{
    [Header("Scriptable Objects")]
    public FeatureDataSO featureData;
    public SensorDataSO sensorData;
    public BikeDataSO bikeData;
    public UIDataSO uiData;

    [Header("UI Elements")]
    public TMP_Text headingText;
    public TMP_Text alertText;
    public TMP_Text resultText;
    public TMP_Text safetyText;

    [Header("Feature Icons")]
    public Image BlindSpotIcon;
    public Image FrontCollisionIcon;
    public Image RearCollisionIcon;
    public Image CloseVehicleIcon;



    // local variables
    private Coroutine displayRoutine;
    private Coroutine BlinkIconRoutine;

    private void OnEnable()
    {
        sensorData.LeftBlindSpotTriggerEnterEvent += BlindSpotTriggerEnter;
        sensorData.FrontCollisionTriggerEnterEvent += FrontCollisionEnter;
        sensorData.RearCollisionTriggerEnterEvent += RearCollisionEnter;
        sensorData.CloseVehicleTriggerEnterEvent += CloseVehicleEnter;


        uiData.ClearAllTextEvent += ClearMessage;


        bikeData.RaceCompletedEvent += ShowWinningMessage;
    }



    private void OnDisable()
    {

        sensorData.LeftBlindSpotTriggerEnterEvent -= BlindSpotTriggerEnter;
        sensorData.FrontCollisionTriggerEnterEvent -= FrontCollisionEnter;
        sensorData.RearCollisionTriggerEnterEvent -= RearCollisionEnter;
        sensorData.CloseVehicleTriggerEnterEvent -= CloseVehicleEnter;


        uiData.ClearAllTextEvent -= ClearMessage;

        bikeData.RaceCompletedEvent -= ShowWinningMessage;
    }

    private void BlindSpotTriggerEnter()
    {
        headingText.text = "BSD - BLIND SPOT \n" + "DETECTION";
        BlinkIconRoutine = StartCoroutine(BlinkImageCoroutine(BlindSpotIcon));
    }

    private void FrontCollisionEnter()
    {
        headingText.text = "FCW - FORWARD \n"+"COLLISION WARNING";
        BlinkIconRoutine = StartCoroutine(BlinkImageCoroutine(FrontCollisionIcon));
    }

    private void RearCollisionEnter()
    {
        headingText.text = "RCW - REAR \n"+"COLLISION WARNING";
        BlinkIconRoutine = StartCoroutine(BlinkImageCoroutine(RearCollisionIcon));
    }

    private void CloseVehicleEnter()
    {
        headingText.text = "CVW - CLOSE \n"+ "VEHICLE WARNING";
        BlinkIconRoutine = StartCoroutine(BlinkImageCoroutine(CloseVehicleIcon));
    }

    private void ShowResult(string message)
    {
        resultText.text = message;
    }

    private void ShowWinningMessage()
    {
        resultText.text = " You have completed the race";
    }


    private void ClearMessage()
    {
        alertText.text = "";
        resultText.text = "";

        if (BlinkIconRoutine != null)
        {
            StopCoroutine(BlinkIconRoutine);
        }
    }



    public IEnumerator BlinkImageCoroutine(Image icon, float blinkInterval = 0.1f)
    {
        if (icon == null)
            yield break;

        while (true)
        {
            icon.enabled = !icon.enabled;
            yield return new WaitForSeconds(blinkInterval);
        }
    }
}
