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
    [SerializeField] private string levelSelectScene = "LevelSelect";
    [SerializeField] private string levelDemoScene = "LevelDemo";
    [SerializeField] private string shopScene = "Shop";

    [Header("Level Progress")]
    [SerializeField] private int currentLevelIndex = 1;

    public GameState CurrentState { get; private set; } = GameState.Menu;

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
                TimerManager.Instance?.PauseTimer();
                break;
            case GameState.Playing:
                Time.timeScale = 1f;
                TimerManager.Instance?.ResumeTimer();
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

    public void GoToLevelSelect()
    {
        SetState(GameState.Menu);
        SceneManager.LoadScene(levelSelectScene);
    }

    public void StartLevel(int levelIndex)
    {
        currentLevelIndex = levelIndex;
        SceneManager.LoadScene(levelDemoScene);

        // Goi trong scene gameplay sau khi cac manager da san sang.
        SetState(GameState.Playing);
        LevelManager.Instance?.InitializeLevel(levelIndex);
        ScoreManager.Instance?.ResetScore();
        TimerManager.Instance?.StartTimer();
        ItemEffectManager.Instance?.ApplyItemEffectsForLevelStart();
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
        currentLevelIndex++;
        StartLevel(currentLevelIndex);
    }

    public void ReplayCurrentLevel()
    {
        StartLevel(currentLevelIndex);
    }
}
