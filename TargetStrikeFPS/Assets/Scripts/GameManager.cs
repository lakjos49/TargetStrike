using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("References")]
    public PlayerController playerController;
    public WeaponController weaponController;
    public WaveManager waveManager;

    public bool IsGameRunning { get; private set; }
    public bool IsPaused { get; private set; }
    private float _startTime;
    public float TimeSurvived => IsGameRunning ? Time.time - _startTime : _endTime - _startTime;
    private float _endTime;

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
    }

    void Start() => UIManager.Instance?.ShowStartScreen();

    void Update()
    {
        if (!IsGameRunning) return;
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (IsPaused) ResumeGame(); else PauseGame();
        }
    }

    public void StartGame()
    {
        IsGameRunning = true;
        IsPaused = false;
        _startTime = Time.time;
        Time.timeScale = 1f;
        ScoreManager.Instance.Reset();
        FindObjectOfType<HealthSystem>()?.ResetStats();
        playerController?.SetControlsEnabled(true);
        weaponController?.SetCanShoot(true);
        waveManager?.StartWaves();
        UIManager.Instance?.ShowHUD();
    }

    public void EndGame()
    {
        IsGameRunning = false;
        _endTime = Time.time;
        playerController?.SetControlsEnabled(false);
        weaponController?.SetCanShoot(false);
        ScoreManager.Instance.SaveHighScore();
        UIManager.Instance?.ShowGameOver(
            ScoreManager.Instance.Score,
            ScoreManager.Instance.Kills,
            ScoreManager.Instance.Headshots,
            ScoreManager.Instance.Accuracy,
            WaveManager.Instance?.CurrentWave ?? 1,
            Mathf.FloorToInt(TimeSurvived),
            ScoreManager.Instance.HighScore);
    }

    public void PauseGame()
    {
        IsPaused = true;
        Time.timeScale = 0f;
        playerController?.SetControlsEnabled(false);
        UIManager.Instance?.ShowPauseMenu();
    }

    public void ResumeGame()
    {
        IsPaused = false;
        Time.timeScale = 1f;
        playerController?.SetControlsEnabled(true);
        UIManager.Instance?.HidePauseMenu();
    }

    public void RestartGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void QuitToMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(0);
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
