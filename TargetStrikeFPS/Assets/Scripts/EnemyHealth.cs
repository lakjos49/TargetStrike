using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class EnemyHealth : MonoBehaviour
{
    [Header("Stats")]
    public int maxHealth = 100;
    public EnemyType enemyType = EnemyType.Soldier;

    public enum EnemyType { Soldier, Elite, Heavy }

    public int CurrentHealth { get; private set; }
    public bool IsDead { get; private set; }

    [Header("UI")]
    public Slider healthBar;
    public GameObject healthBarRoot;

    [Header("Drops")]
    public GameObject[] possibleDrops;
    [Range(0f, 1f)] public float dropChance = 0.25f;

    public UnityEvent OnDeath = new UnityEvent();

    void Awake() => CurrentHealth = maxHealth;

    public void TakeDamage(int damage, BodyPart.PartType part)
    {
        if (IsDead) return;
        CurrentHealth = Mathf.Max(0, CurrentHealth - damage);

        if (healthBar) healthBar.value = (float)CurrentHealth / maxHealth;
        UIManager.Instance?.ShowFloatingDamage(transform.position + Vector3.up * 2f, damage, part == BodyPart.PartType.Head);

        if (CurrentHealth <= 0) Die();
    }

    void Die()
    {
        IsDead = true;
        if (healthBarRoot) healthBarRoot.SetActive(false);

        int pts = enemyType switch { EnemyType.Elite => 250, EnemyType.Heavy => 500, _ => 100 };
        ScoreManager.Instance.RegisterKill(pts, enemyType == EnemyType.Heavy);

        if (possibleDrops.Length > 0 && Random.value <= dropChance)
        {
            var drop = possibleDrops[Random.Range(0, possibleDrops.Length)];
            if (drop) Instantiate(drop, transform.position, Quaternion.identity);
        }

        OnDeath.Invoke();
        WaveManager.Instance?.EnemyKilled();
        GetComponent<EnemyAI>()?.OnDeath();

        Destroy(gameObject, 0.5f);
    }
}
