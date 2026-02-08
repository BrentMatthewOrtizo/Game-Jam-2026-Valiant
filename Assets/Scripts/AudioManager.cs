using UnityEngine;
using UnityEngine.SceneManagement;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [Header("BGM Clips")]
    [SerializeField] private AudioClip titleBgm;
    [SerializeField] private AudioClip level1Bgm;

    [Header("SFX Clips")]
    [SerializeField] private AudioClip sfxDash;
    [SerializeField] private AudioClip sfxJump;
    [SerializeField] private AudioClip sfxHitHurt;
    [SerializeField] private AudioClip sfxPickupCoin;

    [Header("Volumes")]
    [Range(0f, 1f)][SerializeField] private float bgmVolume = 0.6f;
    [Range(0f, 1f)][SerializeField] private float sfxVolume = 0.9f;

    private AudioSource bgmSource;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        bgmSource = GetComponent<AudioSource>();
        bgmSource.loop = true;
        bgmSource.playOnAwake = false;
        bgmSource.volume = bgmVolume;

        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDestroy()
    {
        if (Instance == this)
            SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "TitleScreen")
        {
            PlayBgm(titleBgm);
        }
        else if (scene.name == "Level1")
        {
            PlayBgm(level1Bgm);
        }
    }

    private void PlayBgm(AudioClip clip)
    {
        if (clip == null) return;
        if (bgmSource.clip == clip && bgmSource.isPlaying) return;

        bgmSource.clip = clip;
        bgmSource.volume = bgmVolume;
        bgmSource.Play();
    }
    
    public void PlayDash() => PlaySfx(sfxDash);
    public void PlayJump() => PlaySfx(sfxJump);
    public void PlayHitHurt() => PlaySfx(sfxHitHurt);
    public void PlayPickupCoin() => PlaySfx(sfxPickupCoin);

    private void PlaySfx(AudioClip clip)
    {
        if (clip == null) return;
        AudioSource.PlayClipAtPoint(clip, Vector3.zero, sfxVolume);
    }
}