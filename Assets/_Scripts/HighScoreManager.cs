using UnityEngine;
using System.Collections.Generic;
using System.Linq; // Required for sorting and list manipulation

public class HighScoreManager : MonoBehaviour
{
    // The key used to store the entire JSON string in PlayerPrefs
    private const string HighScoresKey = "Top5HighScores";

    // The main container for all score entries
    private HighScoreData highScores = new HighScoreData();

    void Awake()
    {
        // Load existing scores immediately when the game starts
        LoadHighScores();
    }

    private void LoadHighScores()
    {
        if (PlayerPrefs.HasKey(HighScoresKey))
        {
            // Retrieve the JSON string
            string json = PlayerPrefs.GetString(HighScoresKey);

            // Convert the JSON string back into our HighScoreData object
            highScores = JsonUtility.FromJson<HighScoreData>(json);
        }
    }

    // --- PUBLIC STATIC METHODS ---

    // Called by the CoreGameManager when a game ends
    public static void CheckForNewHighScore(int newScore)
    {
        // Find the active instance of the manager in the scene
        var manager = FindObjectOfType<HighScoreManager>();
        if (manager == null) return;

        // 1. Create the new score entry using the current PlayerIdentity data
        HighScoreEntry newEntry = new HighScoreEntry
        {
            score = newScore,
            // Safely get name and team, defaulting if PlayerIdentity is somehow null
            name = PlayerIdentity.Instance?.PlayerUsername ?? "Anon",
            team = PlayerIdentity.Instance?.TeamRepresenting ?? "N/A"
        };

        // 2. Add the entry
        manager.highScores.scores.Add(newEntry);

        // 3. Sort the list by score (descending)
        manager.highScores.scores = manager.highScores.scores.OrderByDescending(e => e.score).ToList();

        // 4. Clean up the list to only keep the top 10 (good practice to limit growth)
        if (manager.highScores.scores.Count > 10)
        {
            manager.highScores.scores.RemoveRange(10, manager.highScores.scores.Count - 10);
        }

        // 5. Serialize and Save to PlayerPrefs
        string json = JsonUtility.ToJson(manager.highScores);
        PlayerPrefs.SetString(HighScoresKey, json);
        PlayerPrefs.Save();

        Debug.Log($"Score of {newScore} saved. Total high scores: {manager.highScores.scores.Count}");
    }

    // Called by your High Scores UI to retrieve the display data
    public static List<HighScoreEntry> GetTopScores(int count = 5)
    {
        var manager = FindObjectOfType<HighScoreManager>();
        if (manager == null) return new List<HighScoreEntry>();

        // Ensure the list is sorted before taking the top entries
        manager.highScores.scores = manager.highScores.scores.OrderByDescending(e => e.score).ToList();

        // Return the requested number of top scores (default is 5)
        return manager.highScores.scores.Take(count).ToList();
    }

    public void ClearDisplayAndReset()
    {
    
        if (PlayerPrefs.HasKey(HighScoresKey))
        {
            PlayerPrefs.DeleteKey(HighScoresKey);
            PlayerPrefs.Save();
            Debug.Log($"High Scores deleted using key: {HighScoresKey}");
        }
        else
        {
            // Add logging to show if the key was missing
            Debug.LogWarning($"Could not delete High Scores. Key '{HighScoresKey}' not found in PlayerPrefs.");
        }

        highScores.scores.Clear();

        
        HighScoreDisplay display = FindObjectOfType<HighScoreDisplay>();
        if (display != null)
        {
            display.RefreshDisplay();
        }
    }
}