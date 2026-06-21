using UnityEngine;
using System.Collections;

public class WeaponController : MonoBehaviour
{
    [Header("Weapon Stats")]
    public int baseDamage = 25;
    public float fireRate = 0.12f;
    public float range = 150f;
    public float recoilAmount = 1.5f;
    public float recoilRecoverySpeed = 6f;
    public float spreadAngle = 1.5f;

    [Header("Magazine")]
    public int magazineSize = 30;
    public float reloadTime = 2f;
    private int _currentMag;
    private bool _isReloading;

    [Header("References")]
    public Camera playerCamera;
    public LayerMask hitLayers;
    public ParticleSystem muzzleFlash;
    public ParticleSystem hitEffect;
    public GameObject bulletHolePrefab;
    public Transform gunTransform;

    [Header("Audio")]
    public AudioSource audioSource;
    public AudioClip shootClip;
    public AudioClip reloadClip;
    public AudioClip emptyClip;

    private float _nextFireTime;
    private float _currentRecoil;
    private bool _canShoot = true;
    private AmmoManager _ammoManager;

    void Awake()
    {
        _ammoManager = GetComponentInParent<AmmoManager>() ?? FindObjectOfType<AmmoManager>();
        _currentMag = magazineSize;
    }

    void Update()
    {
        if (!_canShoot || !GameManager.Instance.IsGameRunning) return;
        if (Input.GetButton("Fire1") && Time.time >= _nextFireTime)
            TryShoot();
        if (Input.GetKeyDown(KeyCode.R) && !_isReloading)
            StartCoroutine(ReloadRoutine());
        RecoverRecoil();
    }

    void TryShoot()
    {
        if (_isReloading) return;
        if (_currentMag <= 0)
        {
            if (audioSource && emptyClip) audioSource.PlayOneShot(emptyClip, 0.5f);
            if (_ammoManager.Reserve > 0) StartCoroutine(ReloadRoutine());
            return;
        }

        _currentMag--;
        _nextFireTime = Time.time + fireRate;
        FireShot();
        UIManager.Instance?.UpdateAmmoUI(_currentMag, _ammoManager.Reserve);
    }

    void FireShot()
    {
        if (muzzleFlash) muzzleFlash.Play();
        if (audioSource && shootClip) audioSource.PlayOneShot(shootClip);
        ApplyRecoil();
        ScoreManager.Instance.RegisterShot();

        Vector3 spread = new Vector3(
            Random.Range(-spreadAngle, spreadAngle),
            Random.Range(-spreadAngle, spreadAngle), 0f) * 0.01f;
        Ray ray = new Ray(playerCamera.transform.position,
            playerCamera.transform.forward + playerCamera.transform.TransformDirection(spread));

        if (Physics.Raycast(ray, out RaycastHit hit, range, hitLayers))
        {
            BodyPart bp = hit.collider.GetComponent<BodyPart>();
            EnemyHealth eh = hit.collider.GetComponentInParent<EnemyHealth>() ?? bp?.GetComponentInParent<EnemyHealth>();

            if (eh != null && bp != null)
            {
                int dmg = Mathf.RoundToInt(baseDamage * bp.DamageMultiplier);
                bool isHead = bp.partType == BodyPart.PartType.Head;
                eh.TakeDamage(dmg, bp.partType);
                ScoreManager.Instance.RegisterHit(isHead);
                UIManager.Instance?.ShowHitMarker(isHead);
                SpawnHitEffect(hit.point, hit.normal, true);
            }
            else
            {
                SpawnHitEffect(hit.point, hit.normal, false);
                SpawnBulletHole(hit.point, hit.normal, hit.collider.transform);
            }
        }
    }

    void SpawnHitEffect(Vector3 pos, Vector3 normal, bool isFlesh)
    {
        if (hitEffect)
        {
            var ps = Instantiate(hitEffect, pos, Quaternion.LookRotation(normal));
            var main = ps.main;
            main.startColor = isFlesh ? Color.red : Color.gray;
            Destroy(ps.gameObject, 1.5f);
        }
    }

    void SpawnBulletHole(Vector3 pos, Vector3 normal, Transform parent)
    {
        if (bulletHolePrefab)
        {
            var hole = Instantiate(bulletHolePrefab, pos + normal * 0.01f,
                Quaternion.LookRotation(-normal), parent);
            Destroy(hole, 15f);
        }
    }

    IEnumerator ReloadRoutine()
    {
        if (_ammoManager.Reserve <= 0 || _currentMag == magazineSize) yield break;
        _isReloading = true;
        UIManager.Instance?.ShowReloadIndicator(true);
        if (audioSource && reloadClip) audioSource.PlayOneShot(reloadClip);
        yield return new WaitForSeconds(reloadTime);
        int needed = magazineSize - _currentMag;
        int taken = _ammoManager.ConsumeAmmo(needed);
        _currentMag += taken;
        _isReloading = false;
        UIManager.Instance?.ShowReloadIndicator(false);
        UIManager.Instance?.UpdateAmmoUI(_currentMag, _ammoManager.Reserve);
    }

    void ApplyRecoil() => _currentRecoil += recoilAmount;
    void RecoverRecoil() => _currentRecoil = Mathf.Lerp(_currentRecoil, 0f, recoilRecoverySpeed * Time.deltaTime);

    public void SetCanShoot(bool v) => _canShoot = v;
    public bool IsReloading => _isReloading;
}
