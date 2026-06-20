using UnityEngine;
using UnityEngine.Events;
using System.Collections;

public class TimerManager : MonoBehaviour
{
    public static TimerManager Instance { get; private set; }

    [Header("Settings")]
    public float gameDuration = 120f;

    public float TimeRemaining { get; private set; }
    public bool IsRunning { get; private set; }

    public UnityEvent<float> OnTimerUpdate = new UnityEvent<float>();
    public UnityEvent OnTimerExpired = new UnityEvent();

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
    }

    public void StartTimer()
    {
        TimeRemaining = gameDuration;
        IsRunning = true;
        StartCoroutine(TimerRoutine());
    }

    public void PauseTimer() => IsRunning = false;
    public void ResumeTimer() => IsRunning = true;

    public void StopTimer()
    {
        IsRunning = false;
        StopAllCoroutines();
    }

    IEnumerator TimerRoutine()
    {
        while (TimeRemaining > 0f)
        {
            yield return null;
            if (!IsRunning) continue;
            TimeRemaining -= Time.deltaTime;
            TimeRemaining = Mathf.Max(0f, TimeRemaining);
            OnTimerUpdate.Invoke(TimeRemaining);
            UIManager.Instance?.UpdateTimer(TimeRemaining);
        }
        IsRunning = false;
        OnTimerExpired.Invoke();
        GameManager.Instance.EndGame();
    }

    public void ResetTimer()
    {
        StopTimer();
        TimeRemaining = gameDuration;
        UIManager.Instance?.UpdateTimer(TimeRemaining);
    }
}
