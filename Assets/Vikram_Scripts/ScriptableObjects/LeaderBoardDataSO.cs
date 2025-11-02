using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "LeaderBoardDataSO", menuName = "ScriptableObjects/LeaderBoardDataSO")]
public class LeaderBoardDataSO : ScriptableObject
{

     public string leaderBoardKey = "LeaderBoardData";
     public List<LeaderBoardEntry> leaderBoardEntries;



    private void OnEnable()
    {
        LoadLeaderBoardData();
    }


    public void AddEntry(LeaderBoardEntry leaderBoardEntry)
    {
        // Remove any existing entry with the same player name and score (to avoid duplicates)
        leaderBoardEntries.RemoveAll(e => e.playerName == leaderBoardEntry.playerName && e.score == leaderBoardEntry.score);

        // Add the new entry
        leaderBoardEntries.Add(leaderBoardEntry);

        // Sort descending by score, then by most recent (last added wins if scores are equal)
        // Since we add new entries at the end, we reverse the list after sorting by score
        leaderBoardEntries.Sort((a, b) =>
        {
            int scoreCompare = b.score.CompareTo(a.score);
            if (scoreCompare != 0)
                return scoreCompare;
            // If scores are equal, keep the most recent (the one later in the list)
            // So, do nothing here; stable sort will keep the last one
            return 0;
        });

        // Remove older duplicates with the same score (keep the most recent)
        for (int i = leaderBoardEntries.Count - 2; i >= 0; i--)
        {
            if (leaderBoardEntries[i].score == leaderBoardEntries[i + 1].score &&
                leaderBoardEntries[i].playerName == leaderBoardEntries[i + 1].playerName)
            {
                leaderBoardEntries.RemoveAt(i);
            }
        }

        // Keep only top 10
        if (leaderBoardEntries.Count > 10)
        {
            leaderBoardEntries.RemoveRange(10, leaderBoardEntries.Count - 10);
        }
    }

    public void LoadLeaderBoardData()
    {
        if (PlayerPrefs.HasKey(leaderBoardKey))
        {
          
            string data = PlayerPrefs.GetString(leaderBoardKey);
            leaderBoardEntries = JsonUtility.FromJson<LeaderBoardData>(data).leaderBoardEntries;
    

        }
        else
        {
            leaderBoardEntries = new List<LeaderBoardEntry>();
        }
    }


    public List<LeaderBoardEntry> GetBoardEntries()
    {
        return leaderBoardEntries;
    }


    public void SaveLeaderBoardData()
    {
        LeaderBoardData data = new LeaderBoardData();
        data.leaderBoardEntries = leaderBoardEntries;

        PlayerPrefs.SetString(leaderBoardKey,JsonUtility.ToJson(data));
    }

}

[Serializable]
public class LeaderBoardEntry
{
    public string playerName;
    public int score;
}

[Serializable]
public class LeaderBoardData
{
    public List<LeaderBoardEntry> leaderBoardEntries;
}
