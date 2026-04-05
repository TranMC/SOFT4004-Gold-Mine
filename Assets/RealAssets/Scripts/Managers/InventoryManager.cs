using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Quan ly du lieu theo run: coin va inventory item.
/// Ton tai xuyen scene.
/// </summary>
public class InventoryManager : MonoBehaviour
{
    public static InventoryManager Instance { get; private set; }

    private readonly Dictionary<ShopItemType, int> itemCounts = new Dictionary<ShopItemType, int>();

    [SerializeField] private int runCoins = 0;

    public int RunCoins => runCoins;

    public event System.Action<int> OnRunCoinsChanged;
    public event System.Action OnInventoryChanged;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
        runCoins = Mathf.Max(0, runCoins);
    }

    public void StartNewRun()
    {
        runCoins = 0;
        itemCounts.Clear();
        OnRunCoinsChanged?.Invoke(runCoins);
        OnInventoryChanged?.Invoke();
    }

    public void AddCoins(int amount)
    {
        if (amount <= 0)
        {
            return;
        }

        runCoins += amount;
        OnRunCoinsChanged?.Invoke(runCoins);
    }

    public bool TrySpendCoins(int amount)
    {
        if (amount <= 0)
        {
            return false;
        }

        if (runCoins < amount)
        {
            return false;
        }

        runCoins -= amount;
        OnRunCoinsChanged?.Invoke(runCoins);
        return true;
    }

    public void AddItem(ShopItemType itemType, int amount = 1)
    {
        if (amount <= 0)
        {
            return;
        }

        if (!itemCounts.ContainsKey(itemType))
        {
            itemCounts[itemType] = 0;
        }

        itemCounts[itemType] += amount;
        OnInventoryChanged?.Invoke();
    }

    public bool HasItem(ShopItemType itemType)
    {
        return itemCounts.ContainsKey(itemType) && itemCounts[itemType] > 0;
    }

    public bool UseItem(ShopItemType itemType)
    {
        if (!HasItem(itemType))
        {
            return false;
        }

        itemCounts[itemType]--;
        OnInventoryChanged?.Invoke();
        return true;
    }

    public int GetCount(ShopItemType itemType)
    {
        if (itemCounts.TryGetValue(itemType, out int count))
            return count;
        return 0;
    }

    // Legacy string API cho script cu
    public void AddItem(string itemId, int amount = 1)
    {
        if (TryParseItemType(itemId, out ShopItemType parsed))
        {
            AddItem(parsed, amount);
        }
    }

    public bool HasItem(string itemId)
    {
        return TryParseItemType(itemId, out ShopItemType parsed) && HasItem(parsed);
    }

    public bool UseItem(string itemId)
    {
        return TryParseItemType(itemId, out ShopItemType parsed) && UseItem(parsed);
    }

    public int GetCount(string itemId)
    {
        return TryParseItemType(itemId, out ShopItemType parsed) ? GetCount(parsed) : 0;
    }

    private static bool TryParseItemType(string itemId, out ShopItemType parsed)
    {
        parsed = default;
        if (string.IsNullOrEmpty(itemId))
        {
            return false;
        }

        if (System.Enum.TryParse(itemId, true, out parsed))
        {
            return true;
        }

        if (itemId.Equals("TimeBoost", System.StringComparison.OrdinalIgnoreCase))
        {
            parsed = ShopItemType.TimeBoost;
            return true;
        }

        return false;
    }
}