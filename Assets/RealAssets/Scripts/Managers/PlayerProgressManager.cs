using System;
using UnityEngine;

/// <summary>
/// Persistent player progress via PlayerPrefs.
/// ONLY save best score per level.
/// </summary>
public class PlayerProgressManager : MonoBehaviour
{
    public static PlayerProgressManager Instance { get; private set; }

    private const string BestScorePrefix = "PPM_BestScore_Level_";

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

    // ========================
    // BEST SCORE
    // ========================

    public void SaveBestScore(int level, int score)
    {
        if (level < 1)
        {
            return;
        }

        int safeScore = Mathf.Max(0, score);
        int currentBest = GetBestScore(level);

        if (safeScore > currentBest)
        {
            PlayerPrefs.SetInt(GetBestScoreKey(level), safeScore);
            PlayerPrefs.Save();
        }
    }

    public int GetBestScore(int level)
    {
        if (level < 1)
        {
            return 0;
        }

        return PlayerPrefs.GetInt(GetBestScoreKey(level), 0);
    }

    // ========================
    // RESET
    // ========================

    public void ResetBestScores()
    {
        // nếu có nhiều level thì loop cho sạch
        for (int i = 1; i <= 10; i++)
        {
            PlayerPrefs.DeleteKey(GetBestScoreKey(i));
        }

        PlayerPrefs.Save();
    }

    private static string GetBestScoreKey(int level)
    {
        return BestScorePrefix + level;
    }
}