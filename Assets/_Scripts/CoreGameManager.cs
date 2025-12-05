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
    public GameObject pausePanel; // <--- NEW: Drag PausePanel here

    [Header("Audio Settings")]
    public AudioSource sfxSource;
    public AudioClip clearSound;
    public AudioClip rotateSound;

    [Header("Game State")]
    public bool isGameOver = false;
    public bool isPaused = false; // <--- NEW: State Tracker
    public int score = 0;
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
        // Ensure time is running when scene starts
        Time.timeScale = 1f;
    }

    // --- PAUSE FUNCTIONALITY ---
    public void TogglePause()
    {
        // Don't pause if the game is already over
        if (isGameOver) return;

        isPaused = !isPaused;

        if (isPaused)
        {
            // FREEZE TIME
            Time.timeScale = 0f;
            if (pausePanel) pausePanel.SetActive(true);
        }
        else
        {
            // UNFREEZE TIME
            Time.timeScale = 1f;
            if (pausePanel) pausePanel.SetActive(false);
        }
    }
    // ---------------------------

    public void PlayRotate()
    {
        // Only play sound if NOT paused
        if (!isPaused && sfxSource != null && rotateSound != null)
            sfxSource.PlayOneShot(rotateSound);
    }

    public void PlayLineClear()
    {
        if (sfxSource != null && clearSound != null)
            sfxSource.PlayOneShot(clearSound);
    }

    public void GameOver()
    {
        if (isGameOver) return;
        isGameOver = true;
        if (gameOverPanel) gameOverPanel.SetActive(true);
    }

    public void ReturnToMenu()
    {
        Time.timeScale = 1f; // Always reset time before leaving!
        SceneManager.LoadScene("MainMenu");
    }

    public void RegisterLinesCleared(int count)
    {
        if (count == 0) return;
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