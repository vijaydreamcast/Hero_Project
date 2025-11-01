using System.Collections;
using System.Drawing;
using TMPro;
using UnityEngine;

public class GameEndPanel : MonoBehaviour
{
    [Header("Scriptable Objects")]
    public UIDataSO uiData;
    public InputDataSO inputData;
    public GameDataSO gameData;

    [Header("Audio Objects")]
    public AudioSource gameEndAS;
    public AudioClip MilanEndClip;
    public AudioClip DelhiEndClip;
    public AudioClip SaoPauloEndClip;
    public AudioClip ManilaEndClip;



    private void OnEnable()
    {
       if(uiData.PlayerInfo.selectedCity == City.Milan)
        {
            gameEndAS.clip = MilanEndClip;
        }
       else if (uiData.PlayerInfo.selectedCity == City.Delhi)
        {
            gameEndAS.clip = DelhiEndClip;
        }
       else if (uiData.PlayerInfo.selectedCity == City.SaoPaulo)
        {
            gameEndAS.clip = SaoPauloEndClip;
        }
       else if (uiData.PlayerInfo.selectedCity == City.Manila)
        {
            gameEndAS.clip = ManilaEndClip;
        }
        gameEndAS.Play();
        

    }

  

}
