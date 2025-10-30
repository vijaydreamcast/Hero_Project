using System.Collections;
using UnityEngine;
using static UnityEngine.XR.Interaction.Toolkit.Inputs.Haptics.HapticsUtility;

public class BikeCollision : MonoBehaviour
{
    [Header(" Scriptable Objects")]
    public UIDataSO uiData;
    public BikeDataSO bikeData;
    public InputDataSO inputData;
    public GameDataSO gameData;

    [Header(" Other Objects")]
    public SimpleBikeController controller;
    public GameObject startTransform;
    public AudioSource collisionSoundAS;
    private bool canIUpdate = true;

    public void OnCollisionEnter(Collision collision)
    {

        if (collision.gameObject.layer == 6 || collision.gameObject.layer == 7 || collision.gameObject.layer == 8)
        {
           
            bikeData.BikeCollided(collision.gameObject);
           
        }


        if(collision.gameObject.layer == 6 && collision.gameObject.tag != "RightCar" && collision.gameObject.tag != "LeftCar")
        {
          
            if (canIUpdate)
            {
                Debug.Log("Collision Detected with Traffic Vehicle");
                bikeData.CloseVehicle(GetCollisionSide(collision));
                gameData.UpdateScore(-5);
                canIUpdate = false;
                StartCoroutine(WaitForUpdate());

            }
        }
    }


    private IEnumerator WaitForUpdate()
    {
        yield return new WaitForSeconds(2f);
        canIUpdate = true;
    }

    public void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "EndBlock")
        {

            //  bikeData.ResetSpeed();
            controller.ResetThrotlleAndSteer();
            inputData.DeactivateInput();

            transform.rotation = other.gameObject.transform.rotation;

            StartCoroutine(BikeStoppingRoutine(5f, 2f));

        }
    }

    private IEnumerator BikeStoppingRoutine(float forwardDistance = 5f, float duration = 2f)
    {


        while(bikeData.currentSpeed > 0.1f)
        {
            yield return null;
        }

        bikeData.RaceCompleted();
    }


    /// <summary>
    /// Determines which side (front, back, left, right) the collision occurred on relative to the bike.
    /// </summary>
    private CollisionType GetCollisionSide(Collision collision)
    {
        if (collision.contactCount == 0)
            return CollisionType.Front;

        // Use the first contact point
        Vector3 contactPoint = collision.GetContact(0).point;
        Vector3 direction = (contactPoint-transform.position).normalized;

        // Directions in local space
        Vector3 forward = transform.forward;
        Vector3 back = -transform.forward;
        Vector3 left = -transform.right;
        Vector3 right = transform.right;

        // Dot products
        float dotF = Vector3.Dot(direction, forward);
        float dotB = Vector3.Dot(direction, back);
        float dotL = Vector3.Dot(direction, left);
        float dotR = Vector3.Dot(direction, right);

        float maxDot = Mathf.Max(dotF, dotB, dotL, dotR);

        if (maxDot == dotF) return CollisionType.Front;
        if (maxDot == dotB) return CollisionType.Rear;
        if (maxDot == dotL) return CollisionType.Left;
        if (maxDot == dotR) return CollisionType.Right;

        return CollisionType.Front;
    }
}
