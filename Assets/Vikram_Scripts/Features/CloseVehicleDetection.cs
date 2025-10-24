using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Splines;

public class CloseVehicleDetection : MonoBehaviour
{
    [Header(" Scriptable Objects")]
    public BikeDataSO bikeData;
    public SensorDataSO sensorData;
    public InputDataSO inputData;
    public UIDataSO uiData;


    [Header(" Audio Sources")]
    public AudioSource startAudioAs;
    public AudioSource endAudioAs;

    [Header(" Movement Objects")]
    public BSDCarMovement LeftLaneCarMovement;
    public BSDCarMovement RightLaneCarMovement;
    public BSDCarMovement HeroBikeMovement;


    [Header(" Game Objects")]
    public GameObject HeroBike;
    public GameObject BikeSteering;
    public GameObject BikeStartPoint;


    [Header(" Bike References ")]
    public SplineContainer BikeTrack;
    public SimpleBikeController bikeController;

    [Header("Animation Settings")]
    public float bikePositioningDuration = 1.5f;
    public float bikeMovementDuration = 1.5f;
    public float bikeTurnSpeed = 180f; // degrees per second

    [Header("Speed Setting")]
    public float bikeConstantSpeed = 1.2f;
    public float leftCarConstantSpeed = 1.2f;
    public float rightCarConstantSpeed = 1;

    [Header("Progress Settings")]
    public float leftCarFinalProgress;
    public float rightCarFinalProgress;
    public float bikeFinalProgress;



    // local variables
    public FeatureDisplayPanel featureDetectionPanel;
    public bool isBikeCloseVehicleZone = false;
    private bool isBikeCollided = false;
    private Coroutine bikeAnimRoutine;

    private void OnEnable()
    {
      
        sensorData.CloseVehicleTriggerEnterEvent += CloseVehicleTriggerEnter;
        sensorData.CloseVehicleTriggerExitEvent += CloseVehicleTriggerExit;

        bikeData.BikeCollidedEvent += BikeCollidedWithVehicle;

        uiData.GameEndEvent += Reset;

    }

    private void OnDisable()
    {
       
        sensorData.CloseVehicleTriggerEnterEvent -= CloseVehicleTriggerEnter;
        sensorData.CloseVehicleTriggerExitEvent -= CloseVehicleTriggerExit;

        bikeData.BikeCollidedEvent -= BikeCollidedWithVehicle;

        uiData.GameEndEvent -= Reset;

    }


    private void CloseVehicleTriggerEnter()
    {
        startAudioAs.Play();
        inputData.ActivateInput();
        isBikeCloseVehicleZone = true;
    }


    private void CloseVehicleTriggerExit()
    {

        isBikeCloseVehicleZone = false;

        if (!isBikeCollided)
        {
           
            uiData.CloseVehicleTestCompleted("Correct Action !!!");
            endAudioAs.Play();

            LeftLaneCarMovement.currentSpeed = 10;
            RightLaneCarMovement.currentSpeed = 10;
            bikeController.SetConstantSpeed(false);

            featureDetectionPanel.ShowFeatureResult(FeatureType.CloseVehicle, FeatureResult.Correct);
            inputData.DeactivateInput();
            bikeController.Reset();
        }

    }


    private void BikeCollidedWithVehicle(GameObject collidedObject)
    {
        if (isBikeCloseVehicleZone)
        {
 
            uiData.CloseVehicleTestCompleted("Wrong Action");
            endAudioAs.Play();

            LeftLaneCarMovement.currentSpeed = 10;
            RightLaneCarMovement.currentSpeed = 10;
            bikeController.SetConstantSpeed(false);
            isBikeCollided = true;

            featureDetectionPanel.ShowFeatureResult(FeatureType.CloseVehicle, FeatureResult.Wrong);
            inputData.DeactivateInput();
            bikeController.Reset();
        }
    }

    private void Reset()
    {
        RightLaneCarMovement.gameObject.SetActive(false);
        LeftLaneCarMovement.progress = 0;
        RightLaneCarMovement.progress = 0;
        LeftLaneCarMovement.currentSpeed = 0;
        RightLaneCarMovement.currentSpeed = 0;
        isBikeCollided = false;
        isBikeCloseVehicleZone = false;
    }
    private void StartCarsAndBikeAnimation()
    {
       
        RightLaneCarMovement.gameObject.SetActive(true);
        AnimateBike();
        AnimateCarsAlongSpline();
    }

    public void AnimateBike()
    {
        if (HeroBike == null || BikeTrack == null)
            return;

        if (bikeAnimRoutine != null)
            StopCoroutine(bikeAnimRoutine);

        bikeAnimRoutine = StartCoroutine(AnimateScooterRoutine());
    }

    private IEnumerator AnimateScooterRoutine()
    {
        Vector3 startPos = HeroBike.transform.position;
        Quaternion startRot = HeroBike.transform.rotation;
        Vector3 endPos = BikeStartPoint.transform.position;
        Quaternion endRot = BikeStartPoint.transform.rotation;

        Debug.Log("Bike speed is " + (endPos - startPos).magnitude / bikePositioningDuration);

        // Direction from scooter to animation point
        Vector3 toTarget = (endPos - startPos).normalized;
        Quaternion pathDirectionRot = Quaternion.LookRotation(toTarget, Vector3.up);


        // Store initial steering local rotation
        Quaternion initialSteerLocalRot = Quaternion.identity;
        if (BikeSteering != null)
            initialSteerLocalRot = BikeSteering.transform.localRotation;

        float elapsed = 0f;

        while (elapsed < bikePositioningDuration)
        {
            float t = elapsed / bikePositioningDuration;
            HeroBike.transform.position = Vector3.Lerp(startPos, endPos, t);

            if (t < 0.5f)
            {
                // First half: Lerp from startRot to path direction
                float tRot = t / 0.5f;
                HeroBike.transform.rotation = Quaternion.Slerp(startRot, pathDirectionRot, tRot);

                // Turn the steering visually along Y axis for display
                if (BikeSteering != null)
                {
                    // Calculate the angle between the scooter's forward and the path direction
                    float targetSteerAngle = Quaternion.Angle(startRot, pathDirectionRot);
                    // Lerp the steering Y angle
                    float steerY = Mathf.Lerp(0f, targetSteerAngle, tRot);
                    BikeSteering.transform.localRotation = initialSteerLocalRot * Quaternion.Euler(0f, steerY, 0f);
                }
            }
            else
            {
                // Second half: Lerp from path direction to endRot
                float tRot = (t - 0.5f) / 0.5f;
                HeroBike.transform.rotation = Quaternion.Slerp(pathDirectionRot, endRot, tRot);
                // Reset steering to initial rotation
                if (BikeSteering != null)
                    BikeSteering.transform.localRotation = initialSteerLocalRot;
            }

            elapsed += Time.deltaTime;
            yield return null;
        }

        // Ensure final position and rotation
        HeroBike.transform.position = endPos;
        HeroBike.transform.rotation = endRot;

        sensorData.ActivateCloseVehicleSensor();

        HeroBikeMovement.splineContainer = BikeTrack;
        HeroBikeMovement.progress = 0.001f;
        HeroBikeMovement.enabled = true;


        float totalTime = bikeMovementDuration;
        elapsed = 0f;

        float bikeStart = HeroBikeMovement.progress;
        float bikeEnd = bikeFinalProgress;

        while (elapsed < totalTime)
        {
            float t = elapsed / totalTime;

            // Linearly interpolate progress from initial to 1.0
            HeroBikeMovement.progress = Mathf.Lerp(bikeStart, bikeEnd, t);

            elapsed += Time.deltaTime;
            yield return null;
        }

        HeroBikeMovement.progress = bikeEnd;
        HeroBikeMovement.enabled = false;

        bikeController.SetConstantSpeed(true);
        bikeController.MaintainConstantSpeed(bikeConstantSpeed);

        bikeAnimRoutine = null;

    }
    public void AnimateCarsAlongSpline()
    {
        if (LeftLaneCarMovement == null || RightLaneCarMovement == null)
            return;

        StartCoroutine(AnimateCarsAlongSplineRoutine());
    }

    private IEnumerator AnimateCarsAlongSplineRoutine()
    {
        float totalTime = bikePositioningDuration + bikeMovementDuration;
        float elapsed = 0f;

        float leftStart = LeftLaneCarMovement.progress;
        float leftEnd = leftCarFinalProgress;
        float rightStart = RightLaneCarMovement.progress;
        float rightEnd = rightCarFinalProgress;


        // Optionally, enable movement if needed
        LeftLaneCarMovement.SetMovement(true);
        RightLaneCarMovement.SetMovement(true);

        while (elapsed < totalTime)
        {
            float t = elapsed / totalTime;

            // Linearly interpolate progress from initial to 1.0
            LeftLaneCarMovement.progress = Mathf.Lerp(leftStart, leftEnd, t);
            RightLaneCarMovement.progress = Mathf.Lerp(rightStart, rightEnd, t);

            elapsed += Time.deltaTime;
            yield return null;
        }

        // Ensure final progress is set
        LeftLaneCarMovement.progress = leftEnd;
        RightLaneCarMovement.progress = rightEnd;


        // Now apply a constant speed for both
        LeftLaneCarMovement.currentSpeed = leftCarConstantSpeed;
        RightLaneCarMovement.currentSpeed = rightCarConstantSpeed;
    }
}
