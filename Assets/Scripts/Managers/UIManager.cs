using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Quan ly UI gameplay: diem, thoi gian, panel pause/win/lose.
/// </summary>
public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

    [Header("HUD")]
    [SerializeField] private Text scoreText;
    [SerializeField] private Text timerText;

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
        {
            GameManager.Instance.OnGameStateChanged += HandleGameStateChanged;
        }
    }

    private void OnDisable()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnGameStateChanged -= HandleGameStateChanged;
        }
    }

    public void UpdateScoreUI(int score)
    {
        if (scoreText != null)
        {
            scoreText.text = $"Score: {score}";
        }
    }

    public void UpdateTimeUI(float timeRemaining)
    {
        if (timerText != null)
        {
            timerText.text = $"Time: {Mathf.CeilToInt(timeRemaining)}";
        }
    }

    private void HandleGameStateChanged(GameState state)
    {
        SetPanelActive(pausePanel, state == GameState.Pause);
        SetPanelActive(winPanel, state == GameState.Win);
        SetPanelActive(losePanel, state == GameState.Lose);
    }

    private static void SetPanelActive(GameObject panel, bool active)
    {
        if (panel != null)
        {
            panel.SetActive(active);
        }
    }
}
