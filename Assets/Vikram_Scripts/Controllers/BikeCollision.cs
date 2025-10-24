using UnityEngine;

public class BikeCollision : MonoBehaviour
{
    [Header(" Scriptable Objects")]
    public UIDataSO uiData;
    public BikeDataSO bikeData;
    public InputDataSO inputData;

    [Header(" Other Objects")]
    public GameObject startTransform;
    public AudioSource collisionSoundAS;

    public void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.layer == 6 || collision.gameObject.layer == 7 || collision.gameObject.layer == 8)
        {
           
            bikeData.BikeCollided(collision.gameObject);

            // Determine collision side
            CollisionType type = GetCollisionSide(collision);

            bikeData.BikeCollidedDirection(type);
            collisionSoundAS.Play();
           
        }
    }

    public void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "EndBlock")
        {
            //transform.position = startTransform.transform.position;
            //transform.rotation = startTransform.transform.rotation;
            bikeData.ResetSpeed();
            inputData.DeactivateInput();
            bikeData.RaceCompleted();
        }
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
