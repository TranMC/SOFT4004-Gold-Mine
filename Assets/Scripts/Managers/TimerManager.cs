using UnityEngine;

/// <summary>
/// Quan ly thoi gian cua man choi va bao het gio.
/// </summary>
public class TimerManager : MonoBehaviour
{
    public static TimerManager Instance { get; private set; }

    [SerializeField] private float levelDuration = 60f;

    private float remainingTime;
    private bool isRunning;

    public float RemainingTime => remainingTime;

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
        if (!isRunning)
        {
            return;
        }

        remainingTime -= Time.deltaTime;
        remainingTime = Mathf.Max(remainingTime, 0f);
        UIManager.Instance?.UpdateTimeUI(remainingTime);

        if (remainingTime <= 0f)
        {
            isRunning = false;
            GameManager.Instance?.HandleLose();
        }
    }

    public void StartTimer()
    {
        remainingTime = levelDuration;
        isRunning = true;
        UIManager.Instance?.UpdateTimeUI(remainingTime);
    }

    public void PauseTimer()
    {
        isRunning = false;
    }

    public void ResumeTimer()
    {
        if (GameManager.Instance != null && GameManager.Instance.CurrentState == GameState.Playing)
        {
            isRunning = true;
        }
    }

    public void AddTime(float amount)
    {
        remainingTime += Mathf.Max(0f, amount);
        UIManager.Instance?.UpdateTimeUI(remainingTime);
    }
}
