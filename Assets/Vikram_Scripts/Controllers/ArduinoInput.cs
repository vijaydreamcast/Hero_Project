using UnityEngine;

public class ArduinoInput : MonoBehaviour
{
    public InputDataSO inputData;
    public bool isLeftBrakeCLicked = false;
    public bool isRightBrakeClicked = false;

    private void Update()
    {
        if(inputData.leftbrake > 0.5f && !isLeftBrakeCLicked)
        {
            inputData.LeftUIClicked();
            isLeftBrakeCLicked = true;
            
        }

        if (inputData.rightbrake > 0.5f && !isRightBrakeClicked)
        {
            inputData.RightUIClicked();
            isRightBrakeClicked = true;
        }


        if(inputData.leftbrake <0.1f && isLeftBrakeCLicked)
        {
            isLeftBrakeCLicked = false;
        }

        if (inputData.rightbrake < 0.1f && isRightBrakeClicked)
        {
            isRightBrakeClicked = false;
        }
    }
}
