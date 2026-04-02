using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Quan ly item nguoi choi so huu de dung cho gameplay.
/// </summary>
public class InventoryManager : MonoBehaviour
{
    public static InventoryManager Instance { get; private set; }

    private readonly Dictionary<string, int> itemCounts = new Dictionary<string, int>();

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

    public void AddItem(string itemId, int amount = 1)
    {
        if (string.IsNullOrEmpty(itemId) || amount <= 0)
        {
            return;
        }

        if (!itemCounts.ContainsKey(itemId))
        {
            itemCounts[itemId] = 0;
        }

        itemCounts[itemId] += amount;
    }

    public bool HasItem(string itemId)
    {
        return itemCounts.ContainsKey(itemId) && itemCounts[itemId] > 0;
    }

    public bool UseItem(string itemId)
    {
        if (!HasItem(itemId))
        {
            return false;
        }

        itemCounts[itemId]--;
        return true;
    }

    public int GetCount(string itemId)
    {
        if (itemCounts.TryGetValue(itemId, out int count))
            return count;
        return 0;
    }
}