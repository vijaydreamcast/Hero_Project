using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class LeaderBoardPanel : MonoBehaviour
{
    public LeaderBoardDataSO leaderBoardData;

    public List<TMP_Text> playerNameTexts;
    public List<TMP_Text> scoreTexts;


    private void OnEnable()
    {
        UpdateLeaderBoardUI();
    }

    private void UpdateLeaderBoardUI()
    {
        for (int i = 0; i < playerNameTexts.Count; i++)
        {
            if (i < leaderBoardData.leaderBoardEntries.Count)
            {
                playerNameTexts[i].text = leaderBoardData.leaderBoardEntries[i].playerName;
                scoreTexts[i].text = leaderBoardData.leaderBoardEntries[i].score.ToString();
            }
            else
            {
                playerNameTexts[i].text = "---";
                scoreTexts[i].text = "---";
            }
        }
    }

}
