using UnityEngine;
using UnityEngine.Events;

public class HealthSystem : MonoBehaviour
{
    [Header("Stats")]
    public int maxHealth = 100;
    public int maxArmor = 50;

    public int CurrentHealth { get; private set; }
    public int CurrentArmor { get; private set; }

    public UnityEvent<int, int> OnHealthChanged = new UnityEvent<int, int>();
    public UnityEvent OnDeath = new UnityEvent();

    void Awake()
    {
        CurrentHealth = maxHealth;
        CurrentArmor = maxArmor;
    }

    public void TakeDamage(int rawDamage, string bodyPart = "body")
    {
        if (CurrentHealth <= 0) return;

        int damage = rawDamage;
        if (CurrentArmor > 0)
        {
            int armorAbsorb = Mathf.Min(CurrentArmor, Mathf.CeilToInt(damage * 0.5f));
            CurrentArmor = Mathf.Max(0, CurrentArmor - armorAbsorb);
            damage -= armorAbsorb;
        }

        CurrentHealth = Mathf.Max(0, CurrentHealth - damage);
        OnHealthChanged.Invoke(CurrentHealth, CurrentArmor);
        UIManager.Instance?.UpdateHealthUI(CurrentHealth, maxHealth, CurrentArmor, maxArmor);
        UIManager.Instance?.ShowDamageIndicator();

        if (CurrentHealth <= 0)
        {
            OnDeath.Invoke();
            GameManager.Instance.EndGame();
        }
    }

    public void Heal(int amount)
    {
        CurrentHealth = Mathf.Min(maxHealth, CurrentHealth + amount);
        UIManager.Instance?.UpdateHealthUI(CurrentHealth, maxHealth, CurrentArmor, maxArmor);
    }

    public void AddArmor(int amount)
    {
        CurrentArmor = Mathf.Min(maxArmor, CurrentArmor + amount);
        UIManager.Instance?.UpdateHealthUI(CurrentHealth, maxHealth, CurrentArmor, maxArmor);
    }

    public void ResetStats()
    {
        CurrentHealth = maxHealth;
        CurrentArmor = maxArmor;
        UIManager.Instance?.UpdateHealthUI(CurrentHealth, maxHealth, CurrentArmor, maxArmor);
    }
}
