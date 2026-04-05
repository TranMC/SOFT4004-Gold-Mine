using UnityEngine;

public enum ItemType
{
    GoldSmall,
    GoldLarge,
    StoneSmall,
    StoneLarge,
    Diamond
}

/// <summary>
/// Gan vao moi prefab item trong scene (vang, da, kim cuong).
/// Xu ly: attach vao hook, collect ve LevelManager, wire weight cho HookController.
/// </summary>
public class ItemController : MonoBehaviour
{
    [Header("Item Config")]
    public ItemType itemType;
    public int value = 100;
    public float weight = 1f;

    [HideInInspector] public bool isHooked = false;

    private Collider2D col;

    private void Awake()
    {
        col = GetComponent<Collider2D>();
    }

    /// <summary>
    /// Duoc goi boi HookController khi collision xay ra.
    /// </summary>
    public void AttachToHook(Transform hookTransform)
    {
        if (isHooked) return;

        isHooked = true;
        col.enabled = false;

        transform.SetParent(hookTransform);
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.identity;
    }

    /// <summary>
    /// Duoc goi boi HookController khi keo ve den minLength.
    /// Cong diem va coin theo logic cua LevelManager, sau do destroy item.
    /// </summary>
    public void Collect()
    {
        LevelManager.Instance?.AddCollectedItemValue(value);

        Destroy(gameObject);
    }

    /// <summary>
    /// Duoc goi boi PowerUpController khi dung Bomb.
    /// Tha item ra khoi hook va destroy.
    /// </summary>
    public void DropAndDestroy()
    {
        transform.SetParent(null);
        Destroy(gameObject);
    }
}