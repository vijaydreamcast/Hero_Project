using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class StartPopUpScreen : MonoBehaviour
{
    [Header("Scriptable Objects")]
    public UIDataSO uiData;
    public InputDataSO inputData;
    public BikeDataSO bikeData;

    [Header(" strings ")]
    public CanvasGroup containerPanel;
    public TMP_Text displayText;
    public List<string> allStrings;

    private void OnEnable()
    {
       
        StartCoroutine(WaitAndStartGame());
    }


    private IEnumerator WaitAndStartGame()
    {
        foreach (var item in allStrings)
        {
            displayText.text = item;
            yield return new WaitForSeconds(1);
        }
        containerPanel.alpha = 0;
        inputData.ActivateInput();
        bikeData.StartBike();
        gameObject.SetActive(false);
    }
}
