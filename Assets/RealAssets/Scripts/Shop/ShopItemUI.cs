using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class ShopItemUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [Header("Data")]
    public ShopItem shopItemData;

    [Header("UI References")]
    public TextMeshProUGUI priceText;
    public Button buyButton;

    private ShopManager shopManager;

    private void Awake()
    {
        shopManager = FindObjectOfType<ShopManager>();
    }

    private void Start()
    {
        if (priceText != null && shopItemData != null)
            priceText.text = shopItemData.price.ToString() + " Coin";

        if (buyButton != null)
            buyButton.onClick.AddListener(BuyItem);
    }

    // Hover chuột vào item
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (shopManager != null && shopItemData != null)
            shopManager.ShowDescription(shopItemData.description);
    }

    // Hover chuột ra
    public void OnPointerExit(PointerEventData eventData)
    {
        if (shopManager != null)
            shopManager.HideDescription();
    }

    private void BuyItem()
    {
        if (shopManager != null && shopItemData != null)
            shopManager.BuyItem(this, shopItemData);
    }

    // Gọi từ ShopManager để ẩn item sau khi mua thành công
    public void DisableItem()
    {
        gameObject.SetActive(false);
    }
}
