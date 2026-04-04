using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Persistent player progress via PlayerPrefs.
/// Save coins, power-up inventory and best score per level.
/// </summary>
public class PlayerProgressManager : MonoBehaviour
{
    [Serializable]
    private class InventoryEntry
    {
        public string itemId;
        public int quantity;
    }

    [Serializable]
    private class InventoryData
    {
        public List<InventoryEntry> entries = new List<InventoryEntry>();
    }

    public static PlayerProgressManager Instance { get; private set; }

    private const string CoinsKey = "PPM_Coins";
    private const string InventoryKey = "PPM_Inventory";
    private const string BestScorePrefix = "PPM_BestScore_Level_";

    private readonly Dictionary<string, int> powerUpInventory = new Dictionary<string, int>();

    public int Coins { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        LoadAll();
    }

    public void AddCoins(int amount)
    {
        if (amount <= 0)
        {
            return;
        }

        Coins += amount;
    }

    public bool TrySpendCoins(int amount)
    {
        if (amount <= 0)
        {
            return false;
        }

        if (Coins < amount)
        {
            return false;
        }

        Coins -= amount;
        return true;
    }

    public void SetCoins(int amount)
    {
        Coins = Mathf.Max(0, amount);
    }

    public int GetPowerUpCount(string itemId)
    {
        if (string.IsNullOrEmpty(itemId))
        {
            return 0;
        }

        if (powerUpInventory.TryGetValue(itemId, out int qty))
        {
            return qty;
        }

        return 0;
    }

    public void AddPowerUp(string itemId, int amount = 1)
    {
        if (string.IsNullOrEmpty(itemId) || amount <= 0)
        {
            return;
        }

        if (!powerUpInventory.ContainsKey(itemId))
        {
            powerUpInventory[itemId] = 0;
        }

        powerUpInventory[itemId] += amount;
    }

    public bool TryUsePowerUp(string itemId)
    {
        if (GetPowerUpCount(itemId) <= 0)
        {
            return false;
        }

        powerUpInventory[itemId]--;
        return true;
    }

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

    public void SaveAll()
    {
        SaveCoins();
        SaveInventory();
        PlayerPrefs.Save();
    }

    public void LoadAll()
    {
        LoadCoins();
        LoadInventory();
    }

    public void ResetProgress()
    {
        Coins = 0;
        powerUpInventory.Clear();

        PlayerPrefs.DeleteKey(CoinsKey);
        PlayerPrefs.DeleteKey(InventoryKey);
        PlayerPrefs.DeleteKey(GetBestScoreKey(1));
        PlayerPrefs.DeleteKey(GetBestScoreKey(2));
        PlayerPrefs.DeleteKey(GetBestScoreKey(3));
        PlayerPrefs.Save();
    }

    private void SaveCoins()
    {
        PlayerPrefs.SetInt(CoinsKey, Coins);
    }

    private void LoadCoins()
    {
        Coins = Mathf.Max(0, PlayerPrefs.GetInt(CoinsKey, 0));
    }

    private void SaveInventory()
    {
        InventoryData data = new InventoryData();

        foreach (KeyValuePair<string, int> pair in powerUpInventory)
        {
            if (string.IsNullOrEmpty(pair.Key) || pair.Value <= 0)
            {
                continue;
            }

            data.entries.Add(new InventoryEntry
            {
                itemId = pair.Key,
                quantity = pair.Value
            });
        }

        string json = JsonUtility.ToJson(data);
        PlayerPrefs.SetString(InventoryKey, json);
    }

    private void LoadInventory()
    {
        powerUpInventory.Clear();

        string json = PlayerPrefs.GetString(InventoryKey, string.Empty);
        if (string.IsNullOrEmpty(json))
        {
            return;
        }

        InventoryData data = JsonUtility.FromJson<InventoryData>(json);
        if (data == null || data.entries == null)
        {
            return;
        }

        for (int i = 0; i < data.entries.Count; i++)
        {
            InventoryEntry entry = data.entries[i];
            if (entry == null || string.IsNullOrEmpty(entry.itemId) || entry.quantity <= 0)
            {
                continue;
            }

            powerUpInventory[entry.itemId] = entry.quantity;
        }
    }

    private static string GetBestScoreKey(int level)
    {
        return BestScorePrefix + level;
    }
}
