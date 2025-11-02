using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AdvisoryScreen : MonoBehaviour
{
    [Header("Scriptable Objects")]
    public UIDataSO uiData;
    public InputDataSO inputData;
    public GameDataSO gameData;

    private void OnEnable()
    {        
      //  inputData.RightUIButtonClickedEvent += RightClicked;
    }

    private void OnDisable()
    {    
      //  inputData.RightUIButtonClickedEvent -= RightClicked;
    }


    private void RightClicked(float val)
    {
        int sceneNumber = (int)uiData.PlayerInfo.selectedCity + 1;
        gameData.isGameCompleted = false;
        SceneManager.LoadSceneAsync(sceneNumber);
    }

}
