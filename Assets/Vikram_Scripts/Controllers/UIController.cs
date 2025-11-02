using CurvedUI;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UIController : MonoBehaviour
{
    [Header("Scriptable Objects")]
    public GameDataSO gameData;
    public UIDataSO uiData;
    public SocketDataSO socketData;
    public LeaderBoardDataSO leaderBoardData;

    [Header("Key Codes")]
    public KeyCode leaderBoardKey = KeyCode.L;
    public KeyCode videoPanelKey = KeyCode.V;
  

    public GameObject leaderBoardPanel;
    public GameObject videoPanel;
    public TMP_Text serverIPText1;
    public TMP_Text serverIPText2;


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
        socketData.SetServerIpEvent += UpdateServerIP;
    }

    private void OnDisable()
    {
        socketData.SetServerIpEvent -= UpdateServerIP;
    }

    private void UpdateServerIP(string ip)
    {
        serverIPText1.text = $"Server IP: {ip}";
        serverIPText2.text = $"Server IP: {ip}";
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


        LeaderBoardEntry leaderBoardEntry = new LeaderBoardEntry
        {
            playerName = uiData.PlayerInfo.playerName,
            score = gameData.currentScore
        };

        leaderBoardData.AddEntry(leaderBoardEntry);

        gameData.isGameCompleted = false; // Reset for next session
    }

    private void ResetPanels()
    {
        EnglishAdvisoryPanel.SetActive(false);
        ItalianAdvisoryPanel.SetActive(false);
        EnglishScorePanel.SetActive(false);
        ItalianScorePanel.SetActive(false);
    }


    private void Update()
    {
        if (Input.GetKeyDown(leaderBoardKey))
        {
            leaderBoardPanel.SetActive(true);
            videoPanel.SetActive(false);
        }
        if (Input.GetKeyDown(videoPanelKey))
        {
             videoPanel.SetActive(true);
              leaderBoardPanel.SetActive(false);
        }
    
    }

    public void ShowBtnClicked()
    {
        if(serverIPText1.gameObject.activeInHierarchy)
        {
            serverIPText1.gameObject.SetActive(false);
           
        }
        else
        {
            serverIPText1.gameObject.SetActive(true);            
        }
    }
}

