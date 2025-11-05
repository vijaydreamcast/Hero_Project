using UnityEngine;

public class TestManager : MonoBehaviour
{
    public SocketDataSO socketData;


    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.M))
        {
            socketData.ReSpawn();
        }
    }
}
