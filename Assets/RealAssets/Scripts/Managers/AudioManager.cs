using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Cross-scene audio controller.
/// Handle BGM per scene and one-shot SFX.
/// </summary>
public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    private const string MutePrefKey = "AudioManager_Muted";

    [Header("Music Clips")]
    [SerializeField] private AudioClip menuMusic;
    [SerializeField] private AudioClip levelMusic;
    [SerializeField] private AudioClip shopMusic;

    [Header("Sources")]
    [SerializeField] private AudioSource musicSource;
    [SerializeField] private AudioSource sfxSource;

    [Header("Defaults")]
    [SerializeField] private bool playMusicOnSceneLoaded = true;

    [Header("Music Volume")]
    [SerializeField, Range(0f, 1f)] private float volume = 1f;
    [SerializeField, Range(0f, 1f)] private float menuMusicVolume = 1f;
    [SerializeField, Range(0f, 1f)] private float levelMusicVolume = 1f;
    [SerializeField, Range(0f, 1f)] private float shopMusicVolume = 1f;

    public bool IsMuted { get; private set; }

    public event System.Action<bool> OnMuteStateChanged;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
        EnsureAudioSources();

        IsMuted = PlayerPrefs.GetInt(MutePrefKey, 0) == 1;
        ApplyMuteState();
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += HandleSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= HandleSceneLoaded;
    }

    private void Start()
    {
        if (playMusicOnSceneLoaded)
        {
            PlayMusicForScene(SceneManager.GetActiveScene().name);
        }
    }

    public void PlayMusic(AudioClip clip, bool loop = true, float volumeScale = 1f)
    {
        if (musicSource == null || clip == null)
        {
            return;
        }

        musicSource.loop = loop;
        musicSource.volume = Mathf.Clamp01(volume * Mathf.Clamp01(volumeScale));

        if (musicSource.clip == clip && musicSource.isPlaying)
        {
            return;
        }

        musicSource.clip = clip;
        musicSource.Play();
    }

    public void StopMusic()
    {
        if (musicSource == null)
        {
            return;
        }

        musicSource.Stop();
    }

    public void PlaySfx(AudioClip clip, float volumeScale = 1f)
    {
        if (IsMuted || sfxSource == null || clip == null)
        {
            return;
        }

        sfxSource.PlayOneShot(clip, Mathf.Clamp01(volumeScale));
    }

    public void PlayMusicForScene(string sceneName)
    {
        string lower = sceneName.ToLowerInvariant();

        if (lower.Contains("mainmenu") || lower.Contains("menu"))
        {
            PlayMusic(menuMusic, true, menuMusicVolume);
            return;
        }

        if (lower.Contains("shop"))
        {
            PlayMusic(shopMusic, true, shopMusicVolume);
            return;
        }

        if (lower.Contains("level") || lower.Contains("game"))
        {
            PlayMusic(levelMusic, true, levelMusicVolume);
        }
    }

    private void HandleSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (!playMusicOnSceneLoaded)
        {
            return;
        }

        PlayMusicForScene(scene.name);
    }

    private void EnsureAudioSources()
    {
        if (musicSource == null)
        {
            musicSource = gameObject.AddComponent<AudioSource>();
            musicSource.playOnAwake = false;
            musicSource.loop = true;
        }

        if (sfxSource == null)
        {
            sfxSource = gameObject.AddComponent<AudioSource>();
            sfxSource.playOnAwake = false;
            sfxSource.loop = false;
        }
    }

    public void ToggleMute()
    {
        SetMute(!IsMuted);
    }

    public void SetMute(bool mute)
    {
        if (IsMuted == mute)
        {
            return;
        }

        IsMuted = mute;
        ApplyMuteState();

        PlayerPrefs.SetInt(MutePrefKey, IsMuted ? 1 : 0);
        PlayerPrefs.Save();

        OnMuteStateChanged?.Invoke(IsMuted);
    }

    private void ApplyMuteState()
    {
        if (musicSource != null)
        {
            musicSource.mute = IsMuted;
        }

        if (sfxSource != null)
        {
            sfxSource.mute = IsMuted;
        }
    }
}
