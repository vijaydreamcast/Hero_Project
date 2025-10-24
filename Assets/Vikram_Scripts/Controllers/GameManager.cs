using CurvedUI;
using System.Collections;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [Header("Scriptable Objects")]
    public GameDataSO gameData;
    public UIDataSO uiData;
    public InputDataSO inputData;

    [Header("Game Elements")]
    public GameObject HeroBike;
    public GameObject BikeStartTransform;
    public GameObject CurvedCanvas;

    private void OnEnable()
    {
        gameData.RestartGameEvent += Restart;
    }

    private void OnDisable()
    {
        gameData.RestartGameEvent -= Restart;
    }


    private IEnumerator Start()
    {
        yield return new WaitForSeconds(1.0f);
        CurvedCanvas.GetComponent<CurvedUIRaycaster>().enabled = false;
    }

    private void Restart()
    {
        StartCoroutine(WaitAndRespawn(1.0f));
    }


    private IEnumerator WaitAndRespawn(float waitTime)
    {
        uiData.FadeCanvas(1);
        yield return new WaitForSeconds(waitTime);
        HeroBike.transform.position = BikeStartTransform.transform.position;
        HeroBike.transform.rotation = BikeStartTransform.transform.rotation;
        uiData.FadeCanvas(0);

    }
}
