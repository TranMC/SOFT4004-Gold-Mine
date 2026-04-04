using UnityEngine;

[CreateAssetMenu(fileName = "NewShopItem", menuName = "Shop/Shop Item", order = 1)]
public class ShopItem : ScriptableObject
{
    [Header("Thông tin cơ bản")]
    public string itemName;           // Tên món đồ (Bomb, Strength, Time Boost)
    public Sprite icon;               // Hình ảnh item (kéo sprite từ Project vào)
    public int price;                 // Giá tiền

    [Header("Công dụng & Hiển thị")]
    [TextArea(3, 5)]
    public string description;        // Mô tả khi hover (ví dụ: "Nổ bỏ vật phẩm đang kéo về")

    [Header("Loại item")]
    public ShopItemType type;             // Sẽ tạo enum bên dưới
}
