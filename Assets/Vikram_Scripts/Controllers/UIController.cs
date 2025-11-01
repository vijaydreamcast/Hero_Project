using CurvedUI;
using System.Collections;
using TMPro;
using UnityEngine;

public class UIController : MonoBehaviour
{
    public GameDataSO gameData;


    [Header("English UI Elements")]
    public GameObject EnglishAdvisoryPanel;
    public GameObject EnglishScorePanel;
    public TMP_Text englishScoreText;

    [Header("Italian UI Elements")]
    public GameObject ItalianAdvisoryPanel;
    public GameObject ItalianScorePanel;
    public TMP_Text italianScoreText;

    [Header("Canvas Elements")]
    public GameObject EnglishUI;
    public GameObject ItalianUI;


    private void OnEnable()
    {
        ResetPanels();
        if (gameData.isGameCompleted)
        {
            ShowScorePanel();
        }
        else
        {
            ShowAdvisoryPanel();
        }
    }

    private IEnumerator Start()
    {
        yield return new WaitForSeconds(0.1f); // Small delay to ensure all systems are initialized
        EnglishUI.GetComponent<CurvedUIRaycaster>().enabled = false;
        ItalianUI.GetComponent<CurvedUIRaycaster>().enabled = false;
    }


    private void ShowAdvisoryPanel()
    {
  
        EnglishAdvisoryPanel.SetActive(true);
        ItalianAdvisoryPanel.SetActive(true);

    }

    private void ShowScorePanel()
    {
        EnglishScorePanel.SetActive(true);
        ItalianScorePanel.SetActive(true);

        string redScore = $"<color=#FF0000>{gameData.currentScore}</color>";
        englishScoreText.text = $"Your Score : {redScore}";
        italianScoreText.text = $"Il tuo punteggio : {redScore}";
    }

    private void ResetPanels()
    {
        EnglishAdvisoryPanel.SetActive(false);
        ItalianAdvisoryPanel.SetActive(false);
        EnglishScorePanel.SetActive(false);
        ItalianScorePanel.SetActive(false);
    }
}
