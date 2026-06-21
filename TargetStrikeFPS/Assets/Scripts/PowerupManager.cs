using UnityEngine;
using System.Collections;

public class PowerupManager : MonoBehaviour
{
    public static PowerupManager Instance { get; private set; }

    public enum PowerupType { HealthPack, ArmorPack, AmmoPack, DamageBoost, SpeedBoost }

    [Header("Boosts")]
    public float boostDuration = 15f;

    private bool _damageBoostActive;
    private bool _speedBoostActive;
    private float _damageBoostTimer;
    private float _speedBoostTimer;

    public bool DamageBoostActive => _damageBoostActive;
    public bool SpeedBoostActive => _speedBoostActive;

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
    }

    void Update()
    {
        if (_damageBoostActive)
        {
            _damageBoostTimer -= Time.deltaTime;
            if (_damageBoostTimer <= 0f) { _damageBoostActive = false; UIManager.Instance?.HidePowerupIndicator("damage"); }
        }
        if (_speedBoostActive)
        {
            _speedBoostTimer -= Time.deltaTime;
            if (_speedBoostTimer <= 0f) { _speedBoostActive = false; UIManager.Instance?.HidePowerupIndicator("speed"); }
        }
    }

    public void ApplyPowerup(PowerupType type)
    {
        var hs = FindObjectOfType<HealthSystem>();
        var am = FindObjectOfType<AmmoManager>();

        switch (type)
        {
            case PowerupType.HealthPack:
                hs?.Heal(25);
                UIManager.Instance?.ShowPickupNotification("+25 Health", Color.green);
                break;
            case PowerupType.ArmorPack:
                hs?.AddArmor(25);
                UIManager.Instance?.ShowPickupNotification("+25 Armor", Color.cyan);
                break;
            case PowerupType.AmmoPack:
                am?.AddAmmo(60);
                UIManager.Instance?.ShowPickupNotification("+60 Ammo", Color.yellow);
                break;
            case PowerupType.DamageBoost:
                _damageBoostActive = true;
                _damageBoostTimer = boostDuration;
                UIManager.Instance?.ShowPickupNotification("Damage x2 15s", Color.red);
                break;
            case PowerupType.SpeedBoost:
                _speedBoostActive = true;
                _speedBoostTimer = boostDuration;
                UIManager.Instance?.ShowPickupNotification("Speed Boost 15s", Color.magenta);
                break;
        }
    }
}
