using UnityEngine;

public class TrafficLight : MonoBehaviour
{

    [Header(" STates ")]
    public TrafficLightType lightType;
    public LightState frontLightState;
    public LightState backLightState;
    public MeshRenderer meshRenderer;


    [Header("Light Materials ")]
    public Material redMat;
    public Material yellowMat;
    public Material greenMat;

    [Header("Walking Materials ")]
    public Material redMatW;
    public Material greenMatW;



    public void SetLightState(LightState frontLightState,LightState backLightState)
    {
        if(meshRenderer == null)
        {
            meshRenderer = GetComponent<MeshRenderer>();
        }

        this.frontLightState = frontLightState;
        this.backLightState = backLightState;

        SetFrontLightImages();
        SetBackLightImages();

    }

    private void SetFrontLightImages()
    {
        Material[] mats = meshRenderer.materials;

        if (frontLightState == LightState.Green)
        {
            mats[1] = greenMat;
            mats[2] = redMatW;
        }
        else if (frontLightState == LightState.Yellow)
        {
            mats[1] = yellowMat;
            mats[2] = redMatW;
        }
        else if (frontLightState == LightState.Red)
        {
            mats[1] = redMat;
            mats[2] = greenMat;
        }

        meshRenderer.materials = mats;
    }

    private void SetBackLightImages()
    {
       
    }

    private void ResetLightImages()
    {
       
    }
}
