using UnityEngine;

/// <summary>
/// Ap dung hieu ung power-up vao gameplay dua tren inventory.
/// </summary>
public class ItemEffectManager : MonoBehaviour
{
    public static ItemEffectManager Instance { get; private set; }

    [Header("Effect Values")]
    [SerializeField] private float extraTimeAmount = 10f;
    [SerializeField] private float scoreMultiplier = 2f;
    [SerializeField] private float pullSpeedBonus = 1.25f;

    public float CurrentScoreMultiplier { get; private set; } = 1f;
    public float CurrentPullSpeedMultiplier { get; private set; } = 1f;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    public void ApplyItemEffectsForLevelStart()
    {
        CurrentScoreMultiplier = 1f;
        CurrentPullSpeedMultiplier = 1f;

        if (InventoryManager.Instance == null)
        {
            return;
        }

        if (InventoryManager.Instance.UseItem("ExtraTime"))
        {
            TimerManager.Instance?.AddTime(extraTimeAmount);
        }

        if (InventoryManager.Instance.UseItem("DoubleScore"))
        {
            CurrentScoreMultiplier = scoreMultiplier;
        }

        if (InventoryManager.Instance.UseItem("PullSpeedUp"))
        {
            CurrentPullSpeedMultiplier = pullSpeedBonus;
        }
    }

    public int ApplyScoreModifier(int baseScore)
    {
        return Mathf.RoundToInt(baseScore * CurrentScoreMultiplier);
    }

    public void SpawnSpecialItemsIfNeeded()
    {
        // TODO: Spawn item dac biet theo ty le hoac theo level.
    }
}
