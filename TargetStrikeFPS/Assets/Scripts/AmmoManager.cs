using UnityEngine;

public class AmmoManager : MonoBehaviour
{
    [Header("Ammo")]
    public int startingReserve = 120;
    public int maxReserve = 240;

    public int Reserve { get; private set; }

    void Awake() => Reserve = startingReserve;

    public int ConsumeAmmo(int amount)
    {
        int taken = Mathf.Min(amount, Reserve);
        Reserve -= taken;
        return taken;
    }

    public void AddAmmo(int amount)
    {
        Reserve = Mathf.Min(maxReserve, Reserve + amount);
        UIManager.Instance?.UpdateAmmoUI(-1, Reserve);
    }

    public void ResetAmmo()
    {
        Reserve = startingReserve;
    }
}
