using UnityEngine;

/// <summary>
/// Quan ly diem hien tai va thong bao khi dat target score.
/// </summary>
public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance { get; private set; }

    [SerializeField] private int currentScore;

    public int CurrentScore => currentScore;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    public void AddScore(int amount)
    {
        currentScore += Mathf.Max(0, amount);
        UIManager.Instance?.UpdateScoreUI(currentScore);

        if (LevelManager.Instance != null && currentScore >= LevelManager.Instance.TargetScore)
        {
            GameManager.Instance?.HandleWin();
        }
    }

    public void ResetScore()
    {
        currentScore = 0;
        UIManager.Instance?.UpdateScoreUI(currentScore);
    }
}
