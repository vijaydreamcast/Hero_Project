using System;
using UnityEngine;

[CreateAssetMenu(fileName = "GameDataSO", menuName = "ScriptableObjects/GameDataSO")]
public class GameDataSO : ScriptableObject
{

    // Variables
    public int currentScore = 100;


    //Actions
    public Action RestartGameEvent;


    //Methods

    private void OnEnable()
    {
        currentScore = 100;
    }

    private void OnDisable()
    {
       
    }

    public void RestartGame()
    {
        RestartGameEvent?.Invoke();
    }
}


