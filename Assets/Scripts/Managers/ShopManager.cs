using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Quan ly cua hang: danh sach item va xu ly mua.
/// </summary>
public class ShopManager : MonoBehaviour
{
    public static ShopManager Instance { get; private set; }

    [System.Serializable]
    public class ShopItemData
    {
        public string itemId;
        public int price;
    }

    [SerializeField] private List<ShopItemData> shopItems = new List<ShopItemData>();
    [SerializeField] private int playerMoney = 0;

    public IReadOnlyList<ShopItemData> ShopItems => shopItems;
    public int PlayerMoney => playerMoney;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    public bool BuyItem(string itemId)
    {
        ShopItemData item = shopItems.Find(x => x.itemId == itemId);
        if (item == null)
        {
            return false;
        }

        if (playerMoney < item.price)
        {
            return false;
        }

        playerMoney -= item.price;
        InventoryManager.Instance?.AddItem(item.itemId, 1);
        return true;
    }

    public void AddMoney(int amount)
    {
        playerMoney += Mathf.Max(0, amount);
    }
}
