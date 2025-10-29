using UnityEngine;

public class FPSLimiter : MonoBehaviour
{
    public int targetFPS = 72; // Set your desired target FPS here

    void Awake()
    {
        QualitySettings.vSyncCount = 0; // Disable VSync to allow targetFrameRate to work
        Application.targetFrameRate = targetFPS;
    }
}