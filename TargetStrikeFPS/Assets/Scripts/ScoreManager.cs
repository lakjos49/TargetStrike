using UnityEngine;
using UnityEngine.Events;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance { get; private set; }

    [Header("Penalty")]
    public int missPenalty = 2;

    public int Score { get; private set; }
    public int Hits { get; private set; }
    public int Shots { get; private set; }
    public float Accuracy => Shots > 0 ? (float)Hits / Shots * 100f : 0f;
    public int HighScore { get; private set; }

    public UnityEvent<int> OnScoreChanged = new UnityEvent<int>();
    public UnityEvent<float> OnAccuracyChanged = new UnityEvent<float>();

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        HighScore = PlayerPrefs.GetInt("HighScore", 0);
    }

    public void ResetStats()
    {
        Score = 0; Hits = 0; Shots = 0;
        OnScoreChanged.Invoke(Score);
        OnAccuracyChanged.Invoke(Accuracy);
    }

    public void RegisterHit(int points, Target.TargetType type)
    {
        Shots++;
        Hits++;
        Score += points;
        OnScoreChanged.Invoke(Score);
        OnAccuracyChanged.Invoke(Accuracy);
        UIManager.Instance?.UpdateScore(Score);
        UIManager.Instance?.UpdateAccuracy(Accuracy);
    }

    public void RegisterMiss()
    {
        Shots++;
        Score = Mathf.Max(0, Score - missPenalty);
        OnScoreChanged.Invoke(Score);
        OnAccuracyChanged.Invoke(Accuracy);
        UIManager.Instance?.UpdateScore(Score);
        UIManager.Instance?.UpdateAccuracy(Accuracy);
    }

    public void SaveHighScore()
    {
        if (Score > HighScore)
        {
            HighScore = Score;
            PlayerPrefs.SetInt("HighScore", HighScore);
            PlayerPrefs.Save();
        }
    }
}
