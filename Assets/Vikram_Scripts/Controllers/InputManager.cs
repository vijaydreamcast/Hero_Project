using UnityEngine;

public class InputManager : MonoBehaviour
{
    [Header(" Scriptable Objects ")]
    public InputDataSO inputData;


    public KeyCode leftBrakeKeyCode = KeyCode.L;
    public KeyCode rightBrakeKeyCode = KeyCode.R;


    // local variables
    float steerInput;
    float throttleInput;

    private void Update()
    {

        if (inputData.isInputActivated)
        {
            steerInput = Input.GetAxis("Horizontal");
            throttleInput = Input.GetAxis("Vertical");

            inputData.ApplyThrottle(throttleInput);


            inputData.ApplySteer(steerInput);

            if (Input.GetKey(leftBrakeKeyCode))
            {
                inputData.ApplyLeftBrake(1);

            }
            else
            {
                inputData.ApplyLeftBrake(0);
            }

            if (Input.GetKey(rightBrakeKeyCode))
            {
                inputData.ApplyRightBrake(1);

            }
            else
            {
                inputData.ApplyRightBrake(0);
            }
        }

        if (Input.GetKeyDown(leftBrakeKeyCode))
        {
            inputData.LeftUIClicked();

        }
        if (Input.GetKeyDown(rightBrakeKeyCode))
        {
            inputData.RightUIClicked();

        }

    }

}
