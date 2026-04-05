using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum GameState
{
    Menu,
    Playing,
    Pause,
    Win,
    Lose,
    Shop
}

/// <summary>
/// Trung tam dieu phoi luong game va trang thai toan cuc.
/// Cac manager khac nen thong qua GameManager de dong bo trang thai.
/// </summary>
public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Scene Names")]
    [SerializeField] private string mainMenuScene = "MainMenu";
    [SerializeField] private string levelScene = "Level";
    [SerializeField] private string shopScene = "Shop";

    public GameState CurrentState { get; private set; } = GameState.Menu;

    private int pendingLevelIndex = -1;

    public event Action<GameState> OnGameStateChanged;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
        ApplyFrameRateSettings();
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += HandleSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= HandleSceneLoaded;
    }

    private void OnApplicationFocus(bool hasFocus)
    {
        if (hasFocus)
        {
            ApplyFrameRateSettings();
        }
    }

    private void ApplyFrameRateSettings()
    {
#if UNITY_ANDROID || UNITY_IOS
        const int fallbackTargetFps = 60;
        const int highRefreshTargetFps = 120;

        int detectedRefreshRate = GetDisplayRefreshRate();
        int mobileTargetFps = detectedRefreshRate >= 110 ? highRefreshTargetFps : fallbackTargetFps;

        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = mobileTargetFps;
#endif
    }

        private static int GetDisplayRefreshRate()
        {
    #if UNITY_2022_2_OR_NEWER
        return Mathf.RoundToInt((float)Screen.currentResolution.refreshRateRatio.value);
    #else
        return Screen.currentResolution.refreshRate;
    #endif
        }

    private void Start()
    {
        SetState(GameState.Menu);
    }

    public void SetState(GameState newState)
    {
        if (CurrentState == newState)
        {
            return;
        }

        CurrentState = newState;

        switch (CurrentState)
        {
            case GameState.Pause:
                Time.timeScale = 0f;
                LevelManager.Instance?.PauseTimer();
                break;
            case GameState.Playing:
                Time.timeScale = 1f;
                LevelManager.Instance?.ResumeTimer();
                break;
            case GameState.Win:
            case GameState.Lose:
                Time.timeScale = 0f;
                LevelManager.Instance?.PauseTimer();
                break;
            default:
                Time.timeScale = 1f;
                break;
        }

        OnGameStateChanged?.Invoke(CurrentState);
    }

    public void GoToMainMenu()
    {
        SetState(GameState.Menu);
        SceneManager.LoadScene(mainMenuScene);
    }

    public void OnPlayButtonClicked()
    {
        LevelManager.Instance?.StartNewRun();
        InventoryManager.Instance?.StartNewRun();
        StartLevel(1);
    }

    public void StartLevel(int levelIndex)
    {
        pendingLevelIndex = levelIndex;

        if (SceneManager.GetActiveScene().name == levelScene)
        {
            SetState(GameState.Playing);
            InitializePendingLevel();
            return;
        }

        // Don't set state to Playing yet - wait for scene to load, then HandleSceneLoaded will set it
        SceneManager.LoadScene(levelScene);
    }

    public void PauseGame()
    {
        if (CurrentState == GameState.Playing)
        {
            SetState(GameState.Pause);
        }
    }

    public void ResumeGame()
    {
        if (CurrentState == GameState.Pause)
        {
            SetState(GameState.Playing);
        }
    }

    public void HandleWin()
    {
        if (CurrentState == GameState.Win)
        {
            return;
        }

        SetState(GameState.Win);
    }

    public void HandleLose()
    {
        if (CurrentState == GameState.Lose)
        {
            return;
        }

        SetState(GameState.Lose);
    }

    public void GoToShop()
    {
        SetState(GameState.Shop);
        SceneManager.LoadScene(shopScene);
    }

    public void GoToNextLevelFromShop()
    {
        if (LevelManager.Instance == null)
        {
            GoToMainMenu();
            return;
        }

        if (!LevelManager.Instance.HasNextLevel())
        {
            GoToMainMenu();
            return;
        }

        LevelManager.Instance.AdvanceLevel();
        StartLevel(LevelManager.Instance.CurrentLevel);
    }

    public void ReplayCurrentLevel()
    {
        if (LevelManager.Instance == null)
        {
            StartLevel(1);
            return;
        }

        StartLevel(LevelManager.Instance.CurrentLevel);
    }

    public void OnQuitButtonClicked()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    private void HandleSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name != levelScene)
        {
            return;
        }

        // Initialize level first, then set state to Playing to ensure Time.timeScale is properly set
        InitializePendingLevel();
        SetState(GameState.Playing);
    }

    private void InitializePendingLevel()
    {
        if (pendingLevelIndex <= 0)
        {
            return;
        }

        LevelManager.Instance?.SetCurrentLevel(pendingLevelIndex);
        LevelManager.Instance?.InitializeLevel(pendingLevelIndex);
        pendingLevelIndex = -1;
    }
}
