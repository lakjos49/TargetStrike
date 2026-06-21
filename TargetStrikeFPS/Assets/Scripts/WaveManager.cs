using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class WaveManager : MonoBehaviour
{
    public static WaveManager Instance { get; private set; }

    [Header("Waves")]
    public int[] enemiesPerWave = { 5, 10, 15, 20, 25 };
    public float timeBetweenWaves = 5f;
    public float spawnInterval = 1.5f;

    [Header("Enemy Prefabs")]
    public GameObject soldierPrefab;
    public GameObject elitePrefab;
    public GameObject heavyPrefab;

    [Header("Spawn Points")]
    public Transform[] spawnPoints;

    public int CurrentWave { get; private set; } = 0;
    public int EnemiesAlive { get; private set; }
    private int _enemiesSpawnedThisWave;
    private bool _waveInProgress;

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
    }

    public void StartWaves() => StartCoroutine(WaveLoop());

    IEnumerator WaveLoop()
    {
        while (true)
        {
            CurrentWave++;
            UIManager.Instance?.UpdateWaveUI(CurrentWave);
            yield return StartCoroutine(SpawnWave(CurrentWave));
            while (EnemiesAlive > 0) yield return null;
            UIManager.Instance?.ShowWaveComplete(CurrentWave);
            yield return new WaitForSeconds(timeBetweenWaves);
        }
    }

    IEnumerator SpawnWave(int wave)
    {
        int waveIdx = Mathf.Min(wave - 1, enemiesPerWave.Length - 1);
        int count = enemiesPerWave[waveIdx];
        EnemiesAlive = 0;
        _enemiesSpawnedThisWave = 0;
        _waveInProgress = true;

        for (int i = 0; i < count; i++)
        {
            if (!GameManager.Instance.IsGameRunning) yield break;
            SpawnEnemy(wave);
            yield return new WaitForSeconds(spawnInterval);
        }
        _waveInProgress = false;
    }

    void SpawnEnemy(int wave)
    {
        if (spawnPoints.Length == 0) return;
        Transform sp = spawnPoints[Random.Range(0, spawnPoints.Length)];
        GameObject prefab = ChoosePrefab(wave);
        if (!prefab) return;
        var enemy = Instantiate(prefab, sp.position, sp.rotation);
        EnemiesAlive++;
        _enemiesSpawnedThisWave++;

        var ai = enemy.GetComponent<EnemyAI>();
        if (ai != null)
        {
            var agent = enemy.GetComponent<UnityEngine.AI.NavMeshAgent>();
            if (agent) agent.speed *= 1f + wave * 0.05f;
        }
    }

    GameObject ChoosePrefab(int wave)
    {
        float r = Random.value;
        if (wave >= 3 && r < 0.15f && heavyPrefab)  return heavyPrefab;
        if (wave >= 2 && r < 0.4f  && elitePrefab)  return elitePrefab;
        return soldierPrefab;
    }

    public void EnemyKilled()
    {
        EnemiesAlive = Mathf.Max(0, EnemiesAlive - 1);
    }
}
