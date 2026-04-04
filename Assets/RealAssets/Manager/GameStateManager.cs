using System;
using UnityEngine;

public enum LevelFlowResult
{
    ContinueToShop,
    ReturnToMainMenu
}

/// <summary>
/// Cross-scene state for run flow.
/// Keep current level, run coins and pass/fail decisions.
/// </summary>
public class GameStateManager : MonoBehaviour
{
    public static GameStateManager Instance { get; private set; }

    [Header("Run State")]
    [SerializeField] private int currentLevel = 1;
    [SerializeField] private int totalCoins = 0;

    public int CurrentLevel => currentLevel;
    public int TotalCoins => totalCoins;

    public event Action<int> OnLevelChanged;
    public event Action<int> OnRunCoinsChanged;

    private const int MinLevel = 1;
    private const int MaxLevel = 3;

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
        totalCoins = Mathf.Max(0, totalCoins);
    }

    public void StartNewRun()
    {
        currentLevel = MinLevel;
        totalCoins = 0;

        OnLevelChanged?.Invoke(currentLevel);
        OnRunCoinsChanged?.Invoke(totalCoins);
    }

    public void SetLevel(int level)
    {
        int clamped = Mathf.Clamp(level, MinLevel, MaxLevel);
        if (currentLevel == clamped)
        {
            return;
        }

        currentLevel = clamped;
        OnLevelChanged?.Invoke(currentLevel);
    }

    public void AddRunCoins(int amount)
    {
        if (amount <= 0)
        {
            return;
        }

        totalCoins += amount;
        OnRunCoinsChanged?.Invoke(totalCoins);
    }

    public void ResetRunCoins()
    {
        totalCoins = 0;
        OnRunCoinsChanged?.Invoke(totalCoins);
    }

    public LevelFlowResult ResolvePassFlow()
    {
        if (currentLevel < MaxLevel)
        {
            return LevelFlowResult.ContinueToShop;
        }

        return LevelFlowResult.ReturnToMainMenu;
    }

    public LevelFlowResult ResolveFailFlow()
    {
        return LevelFlowResult.ReturnToMainMenu;
    }

    public void AdvanceLevel()
    {
        if (currentLevel < MaxLevel)
        {
            currentLevel++;
            OnLevelChanged?.Invoke(currentLevel);
        }
    }
}
