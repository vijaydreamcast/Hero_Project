using CurvedUI;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Video;

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
  
    public VideoPlayer videoPlayer1;
    public VideoPlayer videoPlayer2;
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


    private IEnumerator Start()
    {
        yield return new WaitForSeconds(0.1f); // Small delay to ensure all systems are initialized
        EnglishUI.GetComponent<CurvedUIRaycaster>().enabled = false;
        ItalianUI.GetComponent<CurvedUIRaycaster>().enabled = false;


        yield return new WaitForSeconds(1);
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

        StartCoroutine(LoadAndPlayFirstVideoInStreamingAssets());
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

        leaderBoardPanel.SetActive(true);
        videoPanel.SetActive(false);
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
            StartCoroutine(LoadAndPlayFirstVideoInStreamingAssets());
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

    // Call with: StartCoroutine(LoadAndPlayFirstVideoInStreamingAssets());
    private IEnumerator LoadAndPlayFirstVideoInStreamingAssets(bool loop = true, float timeout = 10f)
    {
        if (videoPlayer1 == null)
        {
            Debug.LogWarning("[UIController] VideoPlayer is not assigned.");
            yield break;
        }

        string videosDir = System.IO.Path.Combine(Application.streamingAssetsPath, "Videos");
        if (!System.IO.Directory.Exists(videosDir))
        {
            Debug.LogError($"[UIController] Videos folder not found: {videosDir}");
            yield break;
        }

        string[] allowedExt = new[] { ".mp4", ".mov", ".mkv", ".webm", ".ogv", ".ogg" };
        string foundPath = null;

        // Get files in deterministic order (alphabetical)
        var files = System.IO.Directory.GetFiles(videosDir);
        System.Array.Sort(files);

        foreach (var file in files)
        {
            string ext = System.IO.Path.GetExtension(file);
            if (string.IsNullOrEmpty(ext)) continue;
            if (!System.Array.Exists(allowedExt, e => e.Equals(ext, System.StringComparison.OrdinalIgnoreCase)))
                continue;

            foundPath = file;
            break;
        }

        if (string.IsNullOrEmpty(foundPath))
        {
            Debug.LogError($"[UIController] No supported video file found in {videosDir}");
            yield break;
        }

        string url;
        try
        {
            url = new System.Uri(foundPath).AbsoluteUri; // yields file:///... on desktop
        }
        catch
        {
            url = "file:///" + foundPath.Replace("\\", "/");
        }

        if (videoPlayer1.isPlaying) videoPlayer1.Stop();

        videoPlayer1.source = VideoSource.Url;
        videoPlayer1.url = url;
        videoPlayer1.isLooping = loop;

        videoPlayer2.source = VideoSource.Url;
        videoPlayer2.url = url;
        videoPlayer2.isLooping = loop;

        videoPlayer1.Prepare();
        videoPlayer2.Prepare();
        float timer = 0f;
        while (!videoPlayer1.isPrepared && timer < timeout)
        {
            timer += Time.deltaTime;
            yield return null;
        }

        if (!videoPlayer1.isPrepared)
            Debug.LogWarning("[UIController] VideoPlayer did not prepare within timeout; attempting Play anyway.");

        videoPlayer1.Play();
        videoPlayer2.Play();
    }
}

[System.Serializable]
public class VideoSetting
{
    public string url;
    public bool loop = true;
    // add other fields if needed (startTime, subtitles, etc.)
}

