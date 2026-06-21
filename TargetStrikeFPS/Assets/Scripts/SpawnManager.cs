using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    public static SpawnManager Instance { get; private set; }

    public enum EnemyType { Soldier, Elite, Heavy }

    [Header("Prefabs")]
    public GameObject soldierPrefab;
    public GameObject elitePrefab;
    public GameObject heavyPrefab;

    [Header("Spawn Points")]
    public Transform[] spawnPoints;

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
    }

    public GameObject SpawnEnemy(EnemyType type, float speedMult = 1f, float accMult = 1f)
    {
        if (spawnPoints == null || spawnPoints.Length == 0)
        {
            Debug.LogWarning("No spawn points assigned to SpawnManager");
            return null;
        }

        Transform sp = spawnPoints[Random.Range(0, spawnPoints.Length)];
        GameObject prefab = type switch
        {
            EnemyType.Soldier => soldierPrefab,
            EnemyType.Elite   => elitePrefab,
            EnemyType.Heavy   => heavyPrefab,
            _                 => soldierPrefab
        };

        if (prefab == null) return null;

        var go = Instantiate(prefab, sp.position, sp.rotation);
        var ai = go.GetComponent<EnemyAI>();
        if (ai != null)
        {
            ai.GetComponent<UnityEngine.AI.NavMeshAgent>().speed *= speedMult;
            ai.accuracy = accMult;
        }
        return go;
    }
}
