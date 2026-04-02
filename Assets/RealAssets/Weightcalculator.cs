using UnityEngine;

/// <summary>
/// Tinh toc do keo (retract) dua tren trong luong item dang duoc hook mang.
/// Cong thuc: speed = baseSpeed / (1 + weight * weightFactor)
/// Neu Strength dang active -> bo qua weight, dung baseSpeed.
/// </summary>
public class WeightCalculator : MonoBehaviour
{
    public static WeightCalculator Instance { get; private set; }

    [Header("Speed Config")]
    [SerializeField] private float baseRetractSpeed = 15f;
    [SerializeField] private float weightFactor = 0.15f;

    // Strength power-up
    private bool strengthActive = false;
    private float strengthTimer = 0f;

    public bool StrengthActive => strengthActive;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    private void Update()
    {
        if (strengthActive)
        {
            strengthTimer -= Time.deltaTime;
            if (strengthTimer <= 0f)
            {
                strengthActive = false;
            }
        }
    }

    /// <summary>
    /// Tra ve toc do retract thuc te dua vao weight cua item (neu co).
    /// </summary>
    public float GetRetractSpeed(ItemController item)
    {
        if (strengthActive || item == null)
            return baseRetractSpeed;

        return baseRetractSpeed / (1f + item.weight * weightFactor);
    }

    /// <summary>
    /// Kich hoat Strength trong duration giay.
    /// </summary>
    public void ActivateStrength(float duration)
    {
        strengthActive = true;
        strengthTimer = duration;
    }
}