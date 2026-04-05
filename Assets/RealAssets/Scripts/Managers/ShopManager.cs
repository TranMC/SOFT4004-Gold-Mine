using UnityEngine;
using TMPro;
using System.Collections.Generic;

public class ShopManager : MonoBehaviour
{
    public static ShopManager Instance { get; private set; }

    [Header("References")]
    [SerializeField] private TextMeshProUGUI coinText;
    [SerializeField] private TextMeshProUGUI descriptionText;

    [Header("Dynamic Shop")]
    [SerializeField] private ShopItemUI itemPrefab;
    [SerializeField] private Transform itemContainer;
    [SerializeField] private List<ShopItem> itemCatalog = new List<ShopItem>();
    [SerializeField] private int itemCountPerVisit = 3;

    private readonly List<ShopItemUI> generatedItems = new List<ShopItemUI>();

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    private void Start()
    {
        RefreshShopItems();
        UpdateCoinUI();

        if (InventoryManager.Instance != null)
        {
            InventoryManager.Instance.OnRunCoinsChanged += UpdateCoinUI;
        }
    }

    private void OnDestroy()
    {
        if (InventoryManager.Instance != null)
        {
            InventoryManager.Instance.OnRunCoinsChanged -= UpdateCoinUI;
        }
    }

    // ========================
    // DESCRIPTION UI
    // ========================

    public void ShowDescription(string desc)
    {
        if (descriptionText != null)
            descriptionText.text = desc;
    }

    public void HideDescription()
    {
        if (descriptionText != null)
            descriptionText.text = "";
    }

    // ========================
    // BUY ITEM
    // ========================

    public void BuyItem(ShopItemUI itemUI, ShopItem itemData)
    {
        if (itemData == null || InventoryManager.Instance == null)
        {
            return;
        }

        if (!InventoryManager.Instance.TrySpendCoins(itemData.price))
        {
            Debug.Log("Không đủ coin!");
            return;
        }

        InventoryManager.Instance.AddItem(itemData.type, 1);

        // Item mua trong shop ap dung cho level tiep theo.
        if (LevelManager.Instance != null)
        {
            switch (itemData.type)
            {
                case ShopItemType.Strength:
                    LevelManager.Instance.QueueStrengthBoostForNextLevel();
                    break;
                case ShopItemType.TimeBoost:
                    LevelManager.Instance.QueueExtraTimeForNextLevel(10);
                    break;
                case ShopItemType.Bomb:
                    break;
            }
        }

        if (itemUI != null)
        {
            itemUI.DisableItem();
        }

        Debug.Log($"Đã mua: {itemData.itemName}");
    }

    // ========================
    // COIN UI
    // ========================

    private void UpdateCoinUI(int coin)
    {
        if (coinText != null)
            coinText.text = coin.ToString();
    }

    private void UpdateCoinUI()
    {
        if (coinText != null && InventoryManager.Instance != null)
            coinText.text = InventoryManager.Instance.RunCoins.ToString();
    }

    // ========================
    // BUTTON
    // ========================

    public void OnContinueButtonClicked()
    {
        GameManager.Instance?.GoToNextLevelFromShop();
    }

    public void RefreshShopItems()
    {
        if (itemPrefab == null || itemContainer == null || itemCatalog.Count == 0)
        {
            return;
        }

        for (int i = 0; i < generatedItems.Count; i++)
        {
            if (generatedItems[i] != null)
            {
                Destroy(generatedItems[i].gameObject);
            }
        }

        generatedItems.Clear();

        List<ShopItem> pool = new List<ShopItem>(itemCatalog);
        int spawnCount = Mathf.Clamp(itemCountPerVisit, 1, pool.Count);

        for (int i = 0; i < spawnCount; i++)
        {
            int randomIndex = Random.Range(0, pool.Count);
            ShopItem selected = pool[randomIndex];
            pool.RemoveAt(randomIndex);

            ShopItemUI ui = Instantiate(itemPrefab, itemContainer);
            ui.shopItemData = selected;
            generatedItems.Add(ui);
        }
    }
}