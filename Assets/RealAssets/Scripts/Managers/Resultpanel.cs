using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Hien thi panel ket qua cuoi level va dieu huong theo flow game.
/// </summary>
public class ResultPanel : MonoBehaviour
{
    [Header("Refs")]
    [SerializeField] private GameObject panel;
    [SerializeField] private TMP_Text messageText;
    [SerializeField] private Button backToMenuButton;

    [Header("Config")]
    [SerializeField] private float showDelay = 1f;         // delay truoc khi hien panel
    [SerializeField] private float autoProceedDelay = 2f;   // thoi gian hien ket qua truoc khi auto chuyen scene
    [SerializeField] private string passMessage = "Bạn đã lên cấp độ tiếp!";
    [SerializeField] private string failMessage = "Bạn chưa hoàn thành nhiệm vụ!";

    private void Awake()
    {
        panel.SetActive(false);
        backToMenuButton.gameObject.SetActive(false);
        backToMenuButton.onClick.AddListener(OnBackToMenuClicked);
    }

    private void OnEnable()
    {
        if (GameManager.Instance != null)
            GameManager.Instance.OnGameStateChanged += HandleStateChanged;
    }

    private void OnDisable()
    {
        if (GameManager.Instance != null)
            GameManager.Instance.OnGameStateChanged -= HandleStateChanged;
    }

    private void HandleStateChanged(GameState state)
    {
        if (state == GameState.Win)
            StartCoroutine(ShowWithDelay(true));
        else if (state == GameState.Lose)
            StartCoroutine(ShowWithDelay(false));
    }

    private IEnumerator ShowWithDelay(bool isPass)
    {
        yield return new WaitForSecondsRealtime(showDelay); // dung RealTime vi timeScale co the = 0

        if (isPass) ShowPass();
        else ShowFail();
    }

    public void ShowPass()
    {
        panel.SetActive(true);
        messageText.text = passMessage;
        backToMenuButton.gameObject.SetActive(false);

        StartCoroutine(AutoProceedAfterPass());
    }

    public void ShowFail()
    {
        panel.SetActive(true);
        messageText.text = failMessage;
        backToMenuButton.gameObject.SetActive(false);

        StartCoroutine(AutoProceedAfterFail());
    }

    private IEnumerator AutoProceedAfterPass()
    {
        yield return new WaitForSecondsRealtime(autoProceedDelay);

        if (LevelManager.Instance == null)
        {
            GameManager.Instance?.GoToMainMenu();
            yield break;
        }

        PlayerProgressManager.Instance?.SaveBestScore(
            LevelManager.Instance.CurrentLevel,
            LevelManager.Instance.CurrentScore
        );

        if (LevelManager.Instance.HasNextLevel())
        {
            // Don't call AdvanceLevel() here - GoToNextLevelFromShop() will handle it
            GameManager.Instance?.GoToShop();
        }
        else
        {
            LevelManager.Instance.StartNewRun();
            InventoryManager.Instance?.StartNewRun();
            GameManager.Instance?.GoToMainMenu();
        }
    }

    private void OnBackToMenuClicked()
    {
        LevelManager.Instance?.StartNewRun();
        InventoryManager.Instance?.StartNewRun();
        GameManager.Instance?.GoToMainMenu();
    }

    private IEnumerator AutoProceedAfterFail()
    {
        yield return new WaitForSecondsRealtime(autoProceedDelay);

        LevelManager.Instance?.StartNewRun();
        InventoryManager.Instance?.StartNewRun();
        GameManager.Instance?.GoToMainMenu();
    }
}