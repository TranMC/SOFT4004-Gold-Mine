using UnityEngine;
using TMPro;
using System.Collections.Generic;

public class ShopManager : MonoBehaviour
{
    public static ShopManager Instance { get; private set; }

    [Header("References")]
    [SerializeField] private TextMeshProUGUI coinText;                    // Text hiển thị số coin (vùng vàng)
    [SerializeField] private TextMeshProUGUI descriptionText;             // Text mô tả ở DescriptionBoardBG (vùng đỏ)

    [Header("Inventory Keys (PlayerPrefs)")]
    private const string BOMB_KEY = "BombCount";
    private const string STRENGTH_KEY = "StrengthCount";
    private const string TIMEB00ST_KEY = "TimeBoostCount";

    private int playerCoin = 500;   // Bạn có thể thay đổi giá trị khởi điểm

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);   // Nếu muốn giữ coin qua scene

        LoadCoin();
        UpdateCoinUI();
    }

    // Hiển thị mô tả khi hover
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

    // Mua item (được gọi từ ShopItemUI)
    public void BuyItem(ShopItemUI itemUI, ShopItem itemData)
    {
        if (itemData == null) return;

        if (playerCoin < itemData.price)
        {
            Debug.Log("Không đủ coin!");
            return;
        }

        // Trừ coin
        playerCoin -= itemData.price;
        SaveCoin();
        UpdateCoinUI();

        // Thêm vào inventory
        AddToInventory(itemData.type);

        // Ẩn item sau khi mua
        if (itemUI != null)
            itemUI.DisableItem();

        Debug.Log($"Đã mua: {itemData.itemName} - Giá: {itemData.price}");
    }

    private void AddToInventory(ShopItemType type)
    {
        switch (type)
        {
            case ShopItemType.Bomb:
                int bomb = PlayerPrefs.GetInt(BOMB_KEY, 0) + 1;
                PlayerPrefs.SetInt(BOMB_KEY, bomb);
                break;

            case ShopItemType.Strength:
                int strength = PlayerPrefs.GetInt(STRENGTH_KEY, 0) + 1;
                PlayerPrefs.SetInt(STRENGTH_KEY, strength);
                break;

            case ShopItemType.TimeBoost:
                int time = PlayerPrefs.GetInt(TIMEB00ST_KEY, 0) + 1;
                PlayerPrefs.SetInt(TIMEB00ST_KEY, time);
                break;
        }

        PlayerPrefs.Save();
    }

    // Coin UI
    private void UpdateCoinUI()
    {
        if (coinText != null)
            coinText.text = playerCoin.ToString();
    }

    private void SaveCoin()
    {
        PlayerPrefs.SetInt("PlayerCoin", playerCoin);
    }

    private void LoadCoin()
    {
        playerCoin = PlayerPrefs.GetInt("PlayerCoin", 500);   // Giá trị mặc định 500 coin
    }

    // Nút Chơi Tiếp
    public void OnContinueButtonClicked()
    {
        // Load scene Level (bạn có thể thay bằng tên scene thực tế)
        UnityEngine.SceneManagement.SceneManager.LoadScene("Level");
    }
}