using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class ArcadeStartController : MonoBehaviour
{
    // Existing fields for starting the game
    [Header("Arcade Name Input")]
    public TMP_InputField usernameInput;
    private const int ArcadeSceneIndex = 1;

    // CRITICAL: We only need the GameObject reference here.
    [Header("High Score Display References")]
    public GameObject highScorePanel;
    // We intentionally removed the 'public HighScoreDisplay highScoreDisplay;' line.

    void Start()
    {
        // 1. Check for the flag set by the CoreGameManager upon returning from a game.
        CheckForPostGameScoreView();
    }

    // This function handles the automatic opening of the high score panel after a game
    private void CheckForPostGameScoreView()
    {
        // Check PlayerPrefs for the flag set by CoreGameManager (1 = show scores)
        if (PlayerPrefs.GetInt("ShowHighScoresNext", 0) == 1)
        {
            // Ensure the panel GameObject slot is assigned in the Inspector
            if (highScorePanel != null)
            {
                highScorePanel.SetActive(true);

                // CRITICAL FIX: Get the HighScoreDisplay component instance directly from the panel GameObject
                HighScoreDisplay displayScript = highScorePanel.GetComponent<HighScoreDisplay>();

                // Refresh the display using the found component
                if (displayScript != null)
                {
                    displayScript.RefreshDisplay();
                    Debug.Log("High Score Panel opened and display refreshed successfully.");
                }
                else
                {
                    // This error means the HighScoreDisplay.cs script is missing from the HighScorePanel GameObject
                    Debug.LogError("FATAL ERROR: HighScorePanel is missing the HighScoreDisplay script component!");
                }
            }

            // Clear the flag so the panel doesn't open on subsequent normal menu loads
            PlayerPrefs.SetInt("ShowHighScoresNext", 0);
            PlayerPrefs.Save();
        }
    }

    // Existing StartArcadeGame function
    public void StartArcadeGame()
    {
        string username = usernameInput.text;

        if (string.IsNullOrWhiteSpace(username))
        {
            username = "Player";
        }

        // 1. CRITICAL ADDITION: Clear the flag before starting a new game!
        PlayerPrefs.SetInt("ShowHighScoresNext", 0);
        PlayerPrefs.Save();

        // 2. Save the identity
        if (PlayerIdentity.Instance != null)
        {
            PlayerIdentity.Instance.SetIdentity(username, "Arcade Solo");
        }

        // 3. Load the Arcade Scene
        SceneManager.LoadScene(ArcadeSceneIndex);
    }
}