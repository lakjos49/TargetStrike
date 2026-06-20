using UnityEngine;
using System.Collections;

public class Target : MonoBehaviour
{
    [Header("Settings")]
    public TargetType targetType = TargetType.Normal;
    public int scoreValue = 10;
    public float respawnDelay = 2f;

    [Header("Effects")]
    public ParticleSystem destructionEffect;
    public AudioClip hitSound;

    [Header("References")]
    public GameObject visualRoot;
    public FloatingScorePopup scorePopupPrefab;

    private bool _isActive = true;
    private AudioSource _audio;

    public enum TargetType { Normal, Moving, Bonus }

    void Awake()
    {
        _audio = GetComponent<AudioSource>();
        if (_audio == null) _audio = gameObject.AddComponent<AudioSource>();
    }

    public virtual void OnHit()
    {
        if (!_isActive) return;
        _isActive = false;

        ScoreManager.Instance.RegisterHit(scoreValue, targetType);

        if (hitSound != null) _audio.PlayOneShot(hitSound);
        if (destructionEffect != null) destructionEffect.Play();

        SpawnScorePopup();

        if (visualRoot != null) visualRoot.SetActive(false);

        StartCoroutine(RespawnRoutine());
    }

    void SpawnScorePopup()
    {
        if (scorePopupPrefab != null)
        {
            FloatingScorePopup popup = Instantiate(scorePopupPrefab,
                transform.position + Vector3.up * 0.5f, Quaternion.identity);
            popup.Initialize("+" + scoreValue);
        }
    }

    IEnumerator RespawnRoutine()
    {
        yield return new WaitForSeconds(respawnDelay);
        Respawn();
    }

    protected virtual void Respawn()
    {
        _isActive = true;
        if (visualRoot != null) visualRoot.SetActive(true);
    }

    public bool IsActive => _isActive;
}
