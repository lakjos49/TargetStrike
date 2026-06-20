using UnityEngine;

public class GunController : MonoBehaviour
{
    [Header("Shooting")]
    public float fireRate = 0.15f;
    public float range = 200f;
    public Camera playerCamera;
    public LayerMask targetLayer;

    [Header("Effects")]
    public ParticleSystem muzzleFlash;
    public ParticleSystem hitEffect;
    public GameObject hitDecalPrefab;
    public AudioSource gunAudioSource;
    public AudioClip shootSound;
    public AudioClip missSound;

    [Header("Recoil")]
    public float recoilAmount = 0.5f;
    public float recoilRecoverySpeed = 8f;
    private float _currentRecoil;

    private float _nextFireTime;
    private bool _canShoot = true;

    void Update()
    {
        if (!GameManager.Instance.IsGameRunning || !_canShoot) return;

        if (Input.GetButton("Fire1") && Time.time >= _nextFireTime)
        {
            Shoot();
            _nextFireTime = Time.time + fireRate;
        }

        // Recoil recovery
        _currentRecoil = Mathf.Lerp(_currentRecoil, 0f, recoilRecoverySpeed * Time.deltaTime);
    }

    void Shoot()
    {
        if (muzzleFlash != null) muzzleFlash.Play();
        if (gunAudioSource != null && shootSound != null)
            gunAudioSource.PlayOneShot(shootSound);

        ApplyRecoil();

        Ray ray = playerCamera.ScreenPointToRay(new Vector3(Screen.width / 2f, Screen.height / 2f, 0f));

        if (Physics.Raycast(ray, out RaycastHit hit, range, targetLayer))
        {
            Target target = hit.collider.GetComponentInParent<Target>();
            if (target != null)
            {
                target.OnHit();
                SpawnHitEffect(hit.point, hit.normal);
                return;
            }
        }

        // Miss
        ScoreManager.Instance.RegisterMiss();
        if (gunAudioSource != null && missSound != null)
            gunAudioSource.PlayOneShot(missSound, 0.4f);

        if (Physics.Raycast(ray, out RaycastHit envHit, range))
            SpawnHitDecal(envHit.point, envHit.normal);
    }

    void SpawnHitEffect(Vector3 pos, Vector3 normal)
    {
        if (hitEffect != null)
        {
            ParticleSystem ps = Instantiate(hitEffect, pos, Quaternion.LookRotation(normal));
            Destroy(ps.gameObject, 2f);
        }
    }

    void SpawnHitDecal(Vector3 pos, Vector3 normal)
    {
        if (hitDecalPrefab != null)
        {
            GameObject decal = Instantiate(hitDecalPrefab, pos + normal * 0.01f, Quaternion.LookRotation(-normal));
            Destroy(decal, 10f);
        }
    }

    void ApplyRecoil()
    {
        _currentRecoil += recoilAmount;
        if (playerCamera != null)
        {
            PlayerController pc = playerCamera.GetComponentInParent<PlayerController>();
            // Visual recoil kick — handled via animation or direct transform tweak
        }
    }

    public void SetCanShoot(bool value) => _canShoot = value;
}
