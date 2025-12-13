using UnityEngine;
using TMPro;
using System.Collections.Generic;

public class HighScoreDisplay : MonoBehaviour
{
    // The parent object that holds all your score text rows (e.g., ScoreListContainer)
    [Header("UI Containers")]
    public Transform scoreEntryParent;

    // The GameObject Prefab that contains the Rank, Name, and Score text fields
    [Header("Prefabs")]
    public GameObject scoreEntryPrefab;

    // We call this function when the High Score Panel is opened
    public void RefreshDisplay()
    {
        // =================================================================
        // CRITICAL NULL CHECKS: These ensure wiring is correct in the Inspector.
        // =================================================================
        if (scoreEntryParent == null)
        {
            Debug.LogError("FATAL DISPLAY ERROR: 'Score Entry Parent' transform is NOT assigned in the Inspector on HighScoreDisplay!");
            return; // Stop the function instantly
        }

        if (scoreEntryPrefab == null)
        {
            Debug.LogError("FATAL DISPLAY ERROR: 'Score Entry Prefab' is NOT assigned in the Inspector on HighScoreDisplay!");
            return; // Stop the function instantly
        }

        // 1. Clear any existing entries
        foreach (Transform child in scoreEntryParent)
        {
            Destroy(child.gameObject);
        }

        // 2. Get the sorted top scores
        // NOTE: HighScoreEntry struct and HighScoreManager.GetTopScores() must be defined elsewhere
        List<HighScoreEntry> topScores = HighScoreManager.GetTopScores(10);
        int rank = 1;

        foreach (HighScoreEntry entry in topScores)
        {
            // Instantiates the score row prefab into the correct container
            GameObject entryObj = Instantiate(scoreEntryPrefab, scoreEntryParent);

            // =================================================================
            // ROBUST COMPONENT FINDING: Looks for children by their EXACT name
            // =================================================================
            TextMeshProUGUI rankText = entryObj.transform.Find("RankText").GetComponent<TextMeshProUGUI>();
            TextMeshProUGUI nameText = entryObj.transform.Find("NameText").GetComponent<TextMeshProUGUI>();
            TextMeshProUGUI scoreText = entryObj.transform.Find("ScoreText").GetComponent<TextMeshProUGUI>();

            // Check if all text components were successfully found
            if (rankText != null && nameText != null && scoreText != null)
            {
                rankText.text = $"{rank}.";
                nameText.text = entry.name;
                scoreText.text = entry.score.ToString();

                // Success Log: This WILL appear if the display drawing is successful.
                Debug.Log($"Displaying Score: Rank {rank} - {entry.name} ({entry.score})");
            }
            else
            {
                // Failure Log: This will appear if the prefab names or types are wrong.
                Debug.LogError("Display Failure: Could not find all required TextMeshPro components in the score prefab! Check component names (RankText, NameText, ScoreText) and type (TextMeshProUGUI).");
            }
            rank++;
        }
    }
}