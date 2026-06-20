using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("References")]
    public PlayerController playerController;
    public GunController gunController;

    public bool IsGameRunning { get; private set; }
    public bool IsPaused { get; private set; }

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
    }

    void Start() => UIManager.Instance?.ShowStartScreen();

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && IsGameRunning)
        {
            if (IsPaused) ResumeGame();
            else PauseGame();
        }
    }

    public void StartGame()
    {
        IsGameRunning = true;
        IsPaused = false;
        Time.timeScale = 1f;
        ScoreManager.Instance.ResetStats();
        TimerManager.Instance.StartTimer();
        playerController?.SetControlsEnabled(true);
        gunController?.SetCanShoot(true);
        UIManager.Instance?.ShowHUD();
    }

    public void EndGame()
    {
        IsGameRunning = false;
        playerController?.SetControlsEnabled(false);
        gunController?.SetCanShoot(false);
        ScoreManager.Instance.SaveHighScore();
        TimerManager.Instance.StopTimer();
        UIManager.Instance?.ShowGameOverScreen(
            ScoreManager.Instance.Score,
            ScoreManager.Instance.Hits,
            ScoreManager.Instance.Accuracy,
            ScoreManager.Instance.HighScore);
        AudioManager.Instance?.StopMusic();
    }

    public void PauseGame()
    {
        IsPaused = true;
        Time.timeScale = 0f;
        TimerManager.Instance.PauseTimer();
        playerController?.SetControlsEnabled(false);
        UIManager.Instance?.ShowPauseMenu();
    }

    public void ResumeGame()
    {
        IsPaused = false;
        Time.timeScale = 1f;
        TimerManager.Instance.ResumeTimer();
        playerController?.SetControlsEnabled(true);
        UIManager.Instance?.HidePauseMenu();
    }

    public void RestartGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void QuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
