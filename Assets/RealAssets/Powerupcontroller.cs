using UnityEngine;

/// <summary>
/// Xu ly 3 power-up trong gameplay: Bomb, Strength, TimeBoost.
/// Goi cac method nay tu UI button hoac input.
/// Lay so luong tu InventoryManager, giam khi dung.
/// </summary>
public class PowerUpController : MonoBehaviour
{
    public static PowerUpController Instance { get; private set; }

    [Header("Config")]
    [SerializeField] private float strengthDuration = 10f;
    [SerializeField] private float timeBoostAmount = 10f;

    [Header("Refs")]
    [SerializeField] private HookController hookController;

    // Item ID phai khop voi key trong InventoryManager / ShopManager
    private const string ID_BOMB = "Bomb";
    private const string ID_STRENGTH = "Strength";
    private const string ID_TIMEBOOST = "TimeBoost";

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    /// <summary>
    /// Bom: xoa item dang duoc hook mang, hook tiep tuc retract ve.
    /// </summary>
    public void UseBomb()
    {
        if (!InventoryManager.Instance.UseItem(ID_BOMB)) return;
        if (hookController == null) return;

        // Lay item dang attach tu HookController
        ItemController attached = hookController.AttachedItem;
        if (attached != null)
        {
            attached.DropAndDestroy();
        }

        UIManager.Instance?.UpdateInventoryUI();
    }

    /// <summary>
    /// Strength: trong `strengthDuration` giay, bo qua weight khi tinh retract speed.
    /// </summary>
    public void UseStrength()
    {
        if (!InventoryManager.Instance.UseItem(ID_STRENGTH)) return;

        WeightCalculator.Instance?.ActivateStrength(strengthDuration);
        UIManager.Instance?.UpdateInventoryUI();
    }

    /// <summary>
    /// TimeBoost: cong them thoi gian vao timer.
    /// </summary>
    public void UseTimeBoost()
    {
        if (!InventoryManager.Instance.UseItem(ID_TIMEBOOST)) return;

        TimerManager.Instance?.AddTime(timeBoostAmount);
        UIManager.Instance?.UpdateInventoryUI();
    }
}