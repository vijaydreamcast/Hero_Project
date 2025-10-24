using System;
using System.Collections;
using UnityEngine;

public class TrafficJunction : MonoBehaviour
{

    [Header(" Traffic Lights")]
    public TrafficLight eastTrafficLight;
    public TrafficLight westTrafficLight;
    public TrafficLight northTrafficLight;
    public TrafficLight southTrafficLight;


    [Header(" Traffic Light Stoppers")]
    public GameObject eastLightStopper;
    public GameObject westLightStopper;
    public GameObject northLightStopper;
    public GameObject southLightStopper;


    [Header("Light Properties")]
    public float greenDuration = 15f;
    public float yellowDuration = 3f;

    [Header("Junction Settings")]
    public StartDirection startDirection = StartDirection.NorthSouth;

    // local variables
    private Coroutine stateChangeRoutine;


    private void OnEnable()
    {
        if (stateChangeRoutine == null)
        {
            stateChangeRoutine = StartCoroutine(WaitAndChangeStates());
        }
    }

    private void OnDisable()
    {
        if (stateChangeRoutine != null)
        {
            StopCoroutine(stateChangeRoutine);
            stateChangeRoutine = null;
        }
    }

    private IEnumerator WaitAndChangeStates()
    {
        // Decide where to start
        StartDirection initial = startDirection;

        if (initial == StartDirection.Random)
        {
            initial = (UnityEngine.Random.value > 0.5f) ? StartDirection.NorthSouth : StartDirection.EastWest;
        }

        // If starting from East-West, skip to that cycle first
        if (initial == StartDirection.EastWest)
        {
            yield return StartCoroutine(EastWestCycle());
        }
        else
        {
            yield return StartCoroutine(NorthSouthCycle());
        }

        // Then continue alternating forever
        while (true)
        {
            yield return StartCoroutine(EastWestCycle());
            yield return StartCoroutine(NorthSouthCycle());
        }
    }


    private IEnumerator NorthSouthCycle()
    {
        ResetStoppers();
        if (eastLightStopper != null) eastLightStopper.SetActive(true);
        if (westLightStopper != null) westLightStopper.SetActive(true);

        // Green
        if (northTrafficLight != null) northTrafficLight.SetLightState(LightState.Green, LightState.Red);
        if (southTrafficLight != null) southTrafficLight.SetLightState(LightState.Green, LightState.Red);
        if (eastTrafficLight != null) eastTrafficLight.SetLightState(LightState.Red, LightState.Red);
        if (westTrafficLight != null) westTrafficLight.SetLightState(LightState.Red, LightState.Red);
        yield return new WaitForSeconds(greenDuration);

        // Yellow
        if (northTrafficLight != null) northTrafficLight.SetLightState(LightState.Yellow, LightState.Red);
        if (southTrafficLight != null) southTrafficLight.SetLightState(LightState.Yellow, LightState.Red);
        yield return new WaitForSeconds(yellowDuration);
    }

    private IEnumerator EastWestCycle()
    {
        ResetStoppers();
        if (northLightStopper != null) northLightStopper.SetActive(true);
        if (southLightStopper != null) southLightStopper.SetActive(true);

        // Green
        if (northTrafficLight != null) northTrafficLight.SetLightState(LightState.Red, LightState.Red);
        if (southTrafficLight != null) southTrafficLight.SetLightState(LightState.Red, LightState.Red);
        if (eastTrafficLight != null) eastTrafficLight.SetLightState(LightState.Green, LightState.Red);
        if (westTrafficLight != null) westTrafficLight.SetLightState(LightState.Green, LightState.Red);
        yield return new WaitForSeconds(greenDuration);

        // Yellow
        if (eastTrafficLight != null) eastTrafficLight.SetLightState(LightState.Yellow, LightState.Red);
        if (westTrafficLight != null) westTrafficLight.SetLightState(LightState.Yellow, LightState.Red);
        yield return new WaitForSeconds(yellowDuration);
    }

    private void ResetStoppers()
    {
        if (eastLightStopper != null) eastLightStopper.SetActive(false);
        if (westLightStopper != null) westLightStopper.SetActive(false);
        if (northLightStopper != null) northLightStopper.SetActive(false);
        if (southLightStopper != null) southLightStopper.SetActive(false);
    }
}

[Serializable]
public enum StartDirection
{
    NorthSouth,
    EastWest,
    Random
}
