using UnityEngine;
using System.Collections;

public class BonusTarget : Target
{
    [Header("Bonus Settings")]
    public float minSpawnDelay = 5f;
    public float maxSpawnDelay = 12f;
    public float activeTime = 3f;
    public Color normalColor = Color.yellow;
    public Color urgentColor = Color.red;

    private Renderer _renderer;
    private float _timeRemaining;

    void Start()
    {
        _renderer = GetComponentInChildren<Renderer>();
        scoreValue = 50;
        targetType = TargetType.Bonus;
        if (visualRoot != null) visualRoot.SetActive(false);
        StartCoroutine(BonusSpawnLoop());
    }

    IEnumerator BonusSpawnLoop()
    {
        while (true)
        {
            yield return new WaitForSeconds(Random.Range(minSpawnDelay, maxSpawnDelay));
            if (!GameManager.Instance.IsGameRunning) continue;
            yield return StartCoroutine(AppearAndTimeout());
        }
    }

    IEnumerator AppearAndTimeout()
    {
        if (visualRoot != null) visualRoot.SetActive(true);
        _timeRemaining = activeTime;

        while (_timeRemaining > 0f)
        {
            _timeRemaining -= Time.deltaTime;
            float t = _timeRemaining / activeTime;
            if (_renderer != null)
                _renderer.material.color = Color.Lerp(urgentColor, normalColor, t);
            yield return null;
        }

        if (IsActive && visualRoot != null)
            visualRoot.SetActive(false);
    }

    public override void OnHit()
    {
        StopAllCoroutines();
        base.OnHit();
        StartCoroutine(BonusSpawnLoop());
    }
}
