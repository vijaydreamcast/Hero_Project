using UnityEngine;

public class FollowBike : MonoBehaviour
{
    public GameObject Bike;



    private void LateUpdate()
    {
        transform.position = Bike.transform.position;
        transform.rotation = Bike.transform.rotation;
    }
}
