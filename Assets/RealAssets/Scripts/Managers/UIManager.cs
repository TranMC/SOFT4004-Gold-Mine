using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Quan ly UI gameplay: diem, thoi gian, panel pause/win/lose.
/// </summary>
public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

    [Header("HUD")]
    [SerializeField] private TMP_Text scoreText;
    [SerializeField] private TMP_Text timerText;
    [SerializeField] private TMP_Text targetText;
    [SerializeField] private TMP_Text capText;

    [Header("Panels")]
    [SerializeField] private GameObject pausePanel;
    [SerializeField] private GameObject winPanel;
    [SerializeField] private GameObject losePanel;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    private void OnEnable()
    {
        if (GameManager.Instance != null)
            GameManager.Instance.OnGameStateChanged += HandleGameStateChanged;

        // Value-only defaults to avoid duplicating static labels in the canvas.
        UpdateScoreUI(0);
        UpdateTargetUI(0);
        UpdateTimeUI(0f);
        UpdateCapUI(1);
    }

    private void OnDisable()
    {
        if (GameManager.Instance != null)
            GameManager.Instance.OnGameStateChanged -= HandleGameStateChanged;
    }

    public void UpdateScoreUI(int score)
    {
        if (scoreText != null)
            scoreText.text = $"${score}";
    }

    public void UpdateTimeUI(float timeRemaining)
    {
        if (timerText != null)
            timerText.text = Mathf.CeilToInt(timeRemaining).ToString();
    }

    public void UpdateTargetUI(int target)
    {
        if (targetText != null)
            targetText.text = $"${target}";
    }

    public void UpdateCapUI(int level)
    {
        if (capText != null)
            capText.text = level.ToString();
    }

    // Giữ lại để PowerUpController vẫn gọi được, không báo lỗi
    public void UpdateInventoryUI() { }

    private void HandleGameStateChanged(GameState state)
    {
        SetPanelActive(pausePanel, state == GameState.Pause);
        SetPanelActive(winPanel, state == GameState.Win);
        SetPanelActive(losePanel, state == GameState.Lose);
    }

    private static void SetPanelActive(GameObject panel, bool active)
    {
        if (panel != null)
            panel.SetActive(active);
    }
}