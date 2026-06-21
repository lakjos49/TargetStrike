using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [Header("Sources")]
    public AudioSource musicSource;
    public AudioSource sfxSource;
    public AudioSource footstepSource;

    [Header("Music")]
    public AudioClip combatMusic;
    public AudioClip waveCompleteClip;

    [Header("SFX")]
    public AudioClip headshotClip;
    public AudioClip uiClickClip;
    public AudioClip[] footstepClips;

    [Range(0f, 1f)] public float masterVolume = 1f;
    [Range(0f, 1f)] public float musicVolume   = 0.35f;
    [Range(0f, 1f)] public float sfxVolume     = 1f;

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    void Start() => PlayMusic(combatMusic);

    public void PlayMusic(AudioClip clip)
    {
        if (!musicSource || !clip) return;
        musicSource.clip = clip;
        musicSource.loop = true;
        musicSource.volume = musicVolume * masterVolume;
        musicSource.Play();
    }

    public void PlaySFX(AudioClip clip, float vol = 1f)
    {
        if (!sfxSource || !clip) return;
        sfxSource.PlayOneShot(clip, vol * sfxVolume * masterVolume);
    }

    public void PlayFootstep()
    {
        if (!footstepSource || footstepClips.Length == 0) return;
        footstepSource.PlayOneShot(footstepClips[Random.Range(0, footstepClips.Length)],
            0.4f * sfxVolume * masterVolume);
    }

    public void PlayHeadshot() => PlaySFX(headshotClip);
    public void PlayUIClick()  => PlaySFX(uiClickClip, 0.7f);
    public void StopMusic()    { if (musicSource) musicSource.Stop(); }

    public void SetMasterVolume(float v)
    {
        masterVolume = Mathf.Clamp01(v);
        if (musicSource) musicSource.volume = musicVolume * masterVolume;
    }
}
