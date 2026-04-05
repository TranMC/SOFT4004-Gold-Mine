using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Quan ly level xuyen scene: current level, target, timer va spawn prefab level.
/// Cung quan ly buff co hieu luc cho level tiep theo.
/// </summary>
public class LevelManager : MonoBehaviour
{
    [System.Serializable]
    private class LevelDefinition
    {
        public int levelIndex = 1;
        public int targetCoins = 500;
        public float levelDuration = 60f;
        public GameObject levelPrefab;
    }

    public static LevelManager Instance { get; private set; }

    [Header("Level Config")]
    [SerializeField] private List<LevelDefinition> levels = new List<LevelDefinition>();
    [SerializeField] private Transform levelRoot;

    [Header("Runtime State")]
    [SerializeField] private int currentLevel = 1;

    private const int MinLevel = 1;
    private const int MaxLevel = 3;

    private GameObject currentLevelInstance;

    private bool hasQueuedStrengthBoost;
    private int queuedExtraTimeSeconds;
    private int currentScore;
    private float remainingTime;
    private bool isTimerRunning;

    public int CurrentLevel => currentLevel;
    public int CurrentScore => currentScore;
    public float RemainingTime => remainingTime;
    public int TargetScore { get; private set; } = 500;
    public float CurrentLevelDuration { get; private set; } = 60f;
    public bool StrengthBoostActiveThisLevel { get; private set; }

    public event System.Action<int> OnLevelChanged;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
        currentLevel = Mathf.Clamp(currentLevel, MinLevel, MaxLevel);
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += HandleSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= HandleSceneLoaded;
    }

    private void Update()
    {
        if (!isTimerRunning)
        {
            return;
        }

        remainingTime -= Time.deltaTime;
        remainingTime = Mathf.Max(remainingTime, 0f);
        UIManager.Instance?.UpdateTimeUI(remainingTime);

        if (remainingTime <= 0f)
        {
            isTimerRunning = false;
            // Check win/lose condition only when time runs out
            if (currentScore >= TargetScore)
            {
                GameManager.Instance?.HandleWin();
            }
            else
            {
                GameManager.Instance?.HandleLose();
            }
        }
    }

    public void InitializeLevel(int levelIndex)
    {
        SetCurrentLevel(levelIndex);
        ApplyLevelData(currentLevel);
        EnsureLevelRoot();

        ResetLevel();
        SpawnLevelPrefab();

        StrengthBoostActiveThisLevel = hasQueuedStrengthBoost;

        if (StrengthBoostActiveThisLevel)
        {
            // 1 level only: set duration cao de giu active tron level.
            WeightCalculator.Instance?.ActivateStrength(CurrentLevelDuration + 1f);
        }

        // Keep total run coins across levels.
        currentScore = InventoryManager.Instance != null ? InventoryManager.Instance.RunCoins : 0;
        UIManager.Instance?.UpdateScoreUI(currentScore);

        remainingTime = CurrentLevelDuration + queuedExtraTimeSeconds;
        isTimerRunning = true;
        UIManager.Instance?.UpdateTimeUI(remainingTime);

        hasQueuedStrengthBoost = false;
        queuedExtraTimeSeconds = 0;

        UIManager.Instance?.UpdateTargetUI(TargetScore);
        UIManager.Instance?.UpdateCapUI(currentLevel);
    }

    public void SetCurrentLevel(int level)
    {
        int clamped = Mathf.Clamp(level, MinLevel, MaxLevel);
        if (currentLevel == clamped)
        {
            return;
        }

        currentLevel = clamped;
        OnLevelChanged?.Invoke(currentLevel);
    }

    public bool HasNextLevel()
    {
        return currentLevel < MaxLevel;
    }

    public void AdvanceLevel()
    {
        if (!HasNextLevel())
        {
            return;
        }

        currentLevel++;
        OnLevelChanged?.Invoke(currentLevel);
    }

    public void StartNewRun()
    {
        currentLevel = MinLevel;
        hasQueuedStrengthBoost = false;
        queuedExtraTimeSeconds = 0;
        StrengthBoostActiveThisLevel = false;
        OnLevelChanged?.Invoke(currentLevel);
    }

    public void QueueStrengthBoostForNextLevel()
    {
        hasQueuedStrengthBoost = true;
    }

    public void QueueExtraTimeForNextLevel(int seconds)
    {
        queuedExtraTimeSeconds += Mathf.Max(0, seconds);
    }

    public int ApplyScoreModifiers(int baseScore)
    {
        return Mathf.Max(0, baseScore);
    }

    public void AddCollectedItemValue(int baseValue)
    {
        if (GameManager.Instance != null && GameManager.Instance.CurrentState != GameState.Playing)
        {
            return;
        }

        int finalScore = ApplyScoreModifiers(Mathf.Max(0, baseValue));

        currentScore += finalScore;
        InventoryManager.Instance?.AddCoins(finalScore);
        UIManager.Instance?.UpdateScoreUI(currentScore);
        
        // Don't check win condition here - wait until timer hits 0
    }

    public void AddTime(float amount)
    {
        remainingTime += Mathf.Max(0f, amount);
        UIManager.Instance?.UpdateTimeUI(remainingTime);
    }

    public void PauseTimer()
    {
        isTimerRunning = false;
    }

    public void ResumeTimer()
    {
        if (GameManager.Instance != null && GameManager.Instance.CurrentState == GameState.Playing)
        {
            isTimerRunning = true;
        }
    }

    public void ResetLevel()
    {
        if (currentLevelInstance != null)
        {
            Destroy(currentLevelInstance);
            currentLevelInstance = null;
        }
    }

    private void SpawnLevelPrefab()
    {
        LevelDefinition definition = FindDefinition(currentLevel);
        if (definition == null || definition.levelPrefab == null)
        {
            return;
        }

        if (levelRoot == null)
        {
            EnsureLevelRoot();
        }

        currentLevelInstance = Instantiate(definition.levelPrefab, levelRoot);
        currentLevelInstance.name = definition.levelPrefab.name;
    }

    private void ApplyLevelData(int levelIndex)
    {
        LevelDefinition definition = FindDefinition(levelIndex);

        if (definition == null)
        {
            TargetScore = 500;
            CurrentLevelDuration = 60f;
            return;
        }

        TargetScore = Mathf.Max(0, definition.targetCoins);
        CurrentLevelDuration = Mathf.Max(1f, definition.levelDuration);
    }

    private LevelDefinition FindDefinition(int levelIndex)
    {
        for (int i = 0; i < levels.Count; i++)
        {
            if (levels[i] != null && levels[i].levelIndex == levelIndex)
            {
                return levels[i];
            }
        }

        return null;
    }

    private void EnsureLevelRoot()
    {
        if (levelRoot != null)
        {
            return;
        }

        GameObject rootObj = GameObject.Find("LevelRoot");
        if (rootObj == null)
        {
            rootObj = new GameObject("LevelRoot");
            SceneManager.MoveGameObjectToScene(rootObj, SceneManager.GetActiveScene());
        }

        levelRoot = rootObj.transform;
    }

    private void HandleSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (!scene.name.ToLowerInvariant().Contains("level"))
        {
            return;
        }

        levelRoot = null;
        EnsureLevelRoot();
    }
}
