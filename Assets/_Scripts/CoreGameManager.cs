using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class CoreGameManager : MonoBehaviour
{
    public static CoreGameManager instance;

    [Header("UI References")]
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI levelText;
    public GameObject gameOverPanel;
    public GameObject pausePanel;

    [Header("Audio Settings")]
    public AudioSource sfxSource;
    public AudioClip clearSound;
    public AudioClip rotateSound;

    [Header("Game State")]
    public bool isGameOver = false;
    public bool isPaused = false;
    public int score = 0; // The final score used for saving!
    public int level = 1;
    public int totalLinesCleared = 0;
    public float currentSpeed = 1.0f;

    void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        UpdateUI();
        Time.timeScale = 1f;
    }

    // --- PAUSE FUNCTIONALITY ---
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            TogglePause();
        }
    }

    public void TogglePause()
    {
        if (isGameOver) return;

        isPaused = !isPaused;

        if (isPaused)
        {
            Time.timeScale = 0f;
            if (pausePanel) pausePanel.SetActive(true);
        }
        else
        {
            Time.timeScale = 1f;
            if (pausePanel) pausePanel.SetActive(false);
        }
    }
    // ---------------------------

    public void PlayRotate()
    {
        if (!isPaused && sfxSource != null && rotateSound != null)
            sfxSource.PlayOneShot(rotateSound);
    }

    public void PlayLineClear()
    {
        if (sfxSource != null && clearSound != null)
            sfxSource.PlayOneShot(clearSound);
    }

    // --- GAME OVER LOGIC WITH HIGH SCORE SAVE ---
    public void GameOver()
    {
        if (isGameOver) return;

        isGameOver = true;

        // 1. CRITICAL: Save the final score before showing the panel
        // This requires the HighScoreManager script to be in the scene.
        HighScoreManager.CheckForNewHighScore(score);

        // 2. Show the Game Over panel
        if (gameOverPanel) gameOverPanel.SetActive(true);

        // Ensure game time is completely stopped
        Time.timeScale = 0f;
    }

    public void ReturnToMenu()
    {
        // 1. CRITICAL: Set the flag in PlayerPrefs to tell the Main Menu to open the score panel.
        PlayerPrefs.SetInt("ShowHighScoresNext", 1);
        PlayerPrefs.Save();

        // 2. Load the menu scene
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu");
    }
    // ------------------------------------------

    public void RegisterLinesCleared(int count)
    {
        if (count == 0 || isPaused || isGameOver) return;

        PlayLineClear();

        int points = 0;
        switch (count)
        {
            case 1: points = 40; break;
            case 2: points = 100; break;
            case 3: points = 300; break;
            case 4: points = 1200; break;
        }

        score += points * level;
        totalLinesCleared += count;

        if (totalLinesCleared >= level * 10) LevelUp();
        UpdateUI();
    }

    void LevelUp()
    {
        level++;
        currentSpeed = Mathf.Max(0.05f, currentSpeed - 0.1f);
    }

    void UpdateUI()
    {
        if (scoreText) scoreText.text = "SCORE: " + score;
        if (levelText) levelText.text = "LEVEL " + level;
    }
}