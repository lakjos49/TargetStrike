using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

    [Header("HUD Elements")]
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI timerText;
    public TextMeshProUGUI accuracyText;
    public TextMeshProUGUI hitsText;
    public Image crosshair;
    public GameObject hudPanel;

    [Header("Screens")]
    public GameObject startScreen;
    public GameObject pauseMenu;
    public GameObject gameOverScreen;

    [Header("Game Over UI")]
    public TextMeshProUGUI goScoreText;
    public TextMeshProUGUI goHitsText;
    public TextMeshProUGUI goAccuracyText;
    public TextMeshProUGUI goHighScoreText;

    [Header("Timer Colors")]
    public Color timerNormalColor = Color.white;
    public Color timerUrgentColor = Color.red;
    public float urgentThreshold = 20f;

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
    }

    public void ShowStartScreen()
    {
        SetAllScreens(false);
        if (startScreen) startScreen.SetActive(true);
    }

    public void ShowHUD()
    {
        SetAllScreens(false);
        if (hudPanel) hudPanel.SetActive(true);
    }

    public void ShowPauseMenu()
    {
        if (pauseMenu) pauseMenu.SetActive(true);
    }

    public void HidePauseMenu()
    {
        if (pauseMenu) pauseMenu.SetActive(false);
    }

    public void ShowGameOverScreen(int score, int hits, float accuracy, int highScore)
    {
        SetAllScreens(false);
        if (gameOverScreen) gameOverScreen.SetActive(true);
        if (goScoreText) goScoreText.text = score.ToString("N0");
        if (goHitsText) goHitsText.text = hits.ToString();
        if (goAccuracyText) goAccuracyText.text = accuracy.ToString("F1") + "%";
        if (goHighScoreText) goHighScoreText.text = highScore.ToString("N0");
    }

    public void UpdateScore(int score)
    {
        if (scoreText) scoreText.text = score.ToString("N0");
    }

    public void UpdateTimer(float seconds)
    {
        int m = Mathf.FloorToInt(seconds / 60f);
        int s = Mathf.FloorToInt(seconds % 60f);
        if (timerText)
        {
            timerText.text = $"{m}:{s:00}";
            timerText.color = seconds <= urgentThreshold ? timerUrgentColor : timerNormalColor;
        }
    }

    public void UpdateAccuracy(float accuracy)
    {
        if (accuracyText) accuracyText.text = accuracy.ToString("F1") + "%";
    }

    public void UpdateHits(int hits)
    {
        if (hitsText) hitsText.text = hits.ToString();
    }

    void SetAllScreens(bool value)
    {
        if (startScreen) startScreen.SetActive(value);
        if (pauseMenu) pauseMenu.SetActive(value);
        if (gameOverScreen) gameOverScreen.SetActive(value);
        if (hudPanel) hudPanel.SetActive(value);
    }

    // Button callbacks
    public void OnStartButton() => GameManager.Instance.StartGame();
    public void OnResumeButton() => GameManager.Instance.ResumeGame();
    public void OnRestartButton() => GameManager.Instance.RestartGame();
    public void OnQuitButton() => GameManager.Instance.QuitGame();
}
