using UnityEngine;
using UnityEngine.SceneManagement;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [Header("BGM Clips")]
    [SerializeField] private AudioClip titleBgm;
    [SerializeField] private AudioClip level1Bgm;
    [SerializeField] private AudioClip endBgm;

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
        if (bgmSource == null) bgmSource = gameObject.AddComponent<AudioSource>();
        bgmSource.loop = true;
        bgmSource.playOnAwake = false;
        bgmSource.volume = bgmVolume;

        SceneManager.sceneLoaded -= OnSceneLoaded;
        SceneManager.sceneLoaded += OnSceneLoaded;

        OnSceneLoaded(SceneManager.GetActiveScene(), LoadSceneMode.Single);
    }

    private void OnDestroy()
    {
        if (Instance == this)
            SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        switch (scene.name)
        {
            case "TitleScreen":
                PlayBgm(titleBgm);
                break;

            case "Level1":
                PlayBgm(level1Bgm);
                break;

            case "EndScreen":
            case "EndScene":
                PlayBgm(endBgm != null ? endBgm : titleBgm);
                break;
        }
    }

    private void PlayBgm(AudioClip clip)
    {
        if (clip == null) return;

        bgmSource.Stop();
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