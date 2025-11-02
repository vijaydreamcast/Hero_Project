using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

[CreateAssetMenu(fileName = "LeaderBoardDataSO", menuName = "ScriptableObjects/LeaderBoardDataSO")]
public class LeaderBoardDataSO : ScriptableObject
{

    public string leaderBoardKey = "LeaderBoardData";
    public List<LeaderBoardEntry> leaderBoardEntries;

    // file settings
    [Tooltip("Relative file name under StreamingAssets/Settings")]
    public string leaderBoardFileName = "leaderboard.json";


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

        SaveLeaderBoardData();
    }

    public void LoadLeaderBoardData()
    {
        // Ensure list exists
        if (leaderBoardEntries == null)
            leaderBoardEntries = new List<LeaderBoardEntry>();

        string filePath = GetLeaderBoardFilePath();

        try
        {
            // If file exists in StreamingAssets/Settings, load from file
            if (File.Exists(filePath))
            {
                string json = File.ReadAllText(filePath);
                if (!string.IsNullOrEmpty(json))
                {
                    var container = JsonUtility.FromJson<LeaderBoardData>(json);
                    leaderBoardEntries = container != null && container.leaderBoardEntries != null
                        ? container.leaderBoardEntries
                        : new List<LeaderBoardEntry>();
                }
                else
                {
                    leaderBoardEntries = new List<LeaderBoardEntry>();
                }

                return;
            }

            // Fallback: if PlayerPrefs has old data, migrate it to file (one-time)
            if (PlayerPrefs.HasKey(leaderBoardKey))
            {
                try
                {
                    string data = PlayerPrefs.GetString(leaderBoardKey);
                    var container = JsonUtility.FromJson<LeaderBoardData>(data);
                    leaderBoardEntries = container != null && container.leaderBoardEntries != null
                        ? container.leaderBoardEntries
                        : new List<LeaderBoardEntry>();

                    // Save migrated data to file
                    SaveLeaderBoardData();

                    // optional: remove PlayerPrefs key to avoid duplicate sources
                    PlayerPrefs.DeleteKey(leaderBoardKey);
                }
                catch (Exception ex)
                {
                    Debug.LogError($"[LeaderBoardDataSO] Failed to migrate PlayerPrefs data: {ex.Message}");
                    leaderBoardEntries = new List<LeaderBoardEntry>();
                }

                return;
            }

            // No data found — initialize empty list
            leaderBoardEntries = new List<LeaderBoardEntry>();
        }
        catch (Exception ex)
        {
            Debug.LogError($"[LeaderBoardDataSO] Error loading leaderboard file '{filePath}': {ex.Message}");
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
        data.leaderBoardEntries = leaderBoardEntries ?? new List<LeaderBoardEntry>();

        string json = JsonUtility.ToJson(data, true);

        string filePath = GetLeaderBoardFilePath();
        string dir = Path.GetDirectoryName(filePath);
        try
        {
            if (!Directory.Exists(dir))
                Directory.CreateDirectory(dir);

            File.WriteAllText(filePath, json);
            Debug.Log($"[LeaderBoardDataSO] Saved leaderboard to: {filePath}");
        }
        catch (Exception ex)
        {
            Debug.LogError($"[LeaderBoardDataSO] Failed to save leaderboard to '{filePath}': {ex.Message}");
        }
    }

    private string GetLeaderBoardFilePath()
    {
        // StreamingAssets/Settings/<leaderBoardFileName>
        string settingsFolder = Path.Combine(Application.streamingAssetsPath, "Settings");
        return Path.Combine(settingsFolder, leaderBoardFileName).Replace('\\', '/');
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