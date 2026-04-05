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
    private SpriteRenderer[] spriteRenderers;

    private void Awake()
    {
        col = GetComponent<Collider2D>();
        spriteRenderers = GetComponentsInChildren<SpriteRenderer>(true);
    }

    /// <summary>
    /// Duoc goi boi HookController khi collision xay ra.
    /// </summary>
    public void AttachToHook(Transform hookTransform)
    {
        if (isHooked) return;

        isHooked = true;
        if (col != null)
        {
            col.enabled = false;
        }

        transform.SetParent(hookTransform);
        // Keep item slightly below hook head so player can still see it while retracting.
        transform.localPosition = new Vector3(0f, -0.2f, 0f);
        transform.localRotation = Quaternion.identity;

        BringToFrontWhileHooked();
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

    private void BringToFrontWhileHooked()
    {
        if (spriteRenderers == null || spriteRenderers.Length == 0)
        {
            return;
        }

        for (int i = 0; i < spriteRenderers.Length; i++)
        {
            if (spriteRenderers[i] == null)
            {
                continue;
            }

            spriteRenderers[i].sortingOrder += 20;
        }
    }
}