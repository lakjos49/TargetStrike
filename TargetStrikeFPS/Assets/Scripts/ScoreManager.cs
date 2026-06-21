using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance { get; private set; }

    public int Score { get; private set; }
    public int Kills { get; private set; }
    public int Headshots { get; private set; }
    public int TotalShots { get; private set; }
    public int TotalHits { get; private set; }
    public float Accuracy => TotalShots > 0 ? (float)TotalHits / TotalShots * 100f : 0f;
    public int HighScore { get; private set; }

    private int _killStreak;
    private float _streakResetTimer;
    private const float STREAK_TIMEOUT = 6f;

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        HighScore = PlayerPrefs.GetInt("IS_HighScore", 0);
    }

    void Update()
    {
        if (_killStreak > 0)
        {
            _streakResetTimer -= Time.deltaTime;
            if (_streakResetTimer <= 0f) _killStreak = 0;
        }
    }

    public void RegisterShot() => TotalShots++;

    public void RegisterHit(bool isHeadshot)
    {
        TotalHits++;
        if (isHeadshot)
        {
            Headshots++;
            AddScore(50);
            UIManager.Instance?.ShowHeadshotNotification();
        }
    }

    public void RegisterKill(int basePoints, bool isHeavy = false)
    {
        Kills++;
        _killStreak++;
        _streakResetTimer = STREAK_TIMEOUT;
        AddScore(basePoints);
        int streakBonus = _killStreak switch { >= 10 => 500, >= 5 => 250, >= 3 => 100, _ => 0 };
        if (streakBonus > 0)
        {
            AddScore(streakBonus);
            UIManager.Instance?.ShowKillStreakNotification(_killStreak, streakBonus);
        }
        UIManager.Instance?.UpdateScoreUI(Score, Kills, Headshots);
    }

    void AddScore(int pts)
    {
        Score += pts;
        UIManager.Instance?.UpdateScoreUI(Score, Kills, Headshots);
    }

    public void SaveHighScore()
    {
        if (Score > HighScore)
        {
            HighScore = Score;
            PlayerPrefs.SetInt("IS_HighScore", HighScore);
            PlayerPrefs.Save();
        }
    }

    public void Reset()
    {
        Score = 0; Kills = 0; Headshots = 0; TotalShots = 0; TotalHits = 0; _killStreak = 0;
    }
}
