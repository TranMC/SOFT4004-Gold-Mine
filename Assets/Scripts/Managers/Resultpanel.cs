using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Hien thi panel ket qua cuoi level (Pass/Fail).
/// Wire vao GameManager.OnGameStateChanged de tu dong hien thi.
/// </summary>
public class ResultPanel : MonoBehaviour
{
    [Header("Refs")]
    [SerializeField] private GameObject panel;
    [SerializeField] private TMP_Text messageText;
    [SerializeField] private Button backToMenuButton;

    [Header("Config")]
    [SerializeField] private float showDelay = 1f;         // delay truoc khi hien panel
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
        backToMenuButton.gameObject.SetActive(false); // pass thi tu dong chuyen scene, khong can nut

        StartCoroutine(AutoProceedAfterPass());
    }

    public void ShowFail()
    {
        panel.SetActive(true);
        messageText.text = failMessage;
        backToMenuButton.gameObject.SetActive(true); // fail thi hien nut ve menu
    }

    /// <summary>
    /// Pass: tu dong xu ly flow qua GameStateManager (Shop hoac MainMenu).
    /// </summary>
    private IEnumerator AutoProceedAfterPass()
    {
        yield return new WaitForSecondsRealtime(2f);

        if (GameStateManager.Instance == null)
        {
            GameManager.Instance?.GoToMainMenu();
            yield break;
        }

        var result = GameStateManager.Instance.ResolvePassFlow();

        if (result == LevelFlowResult.ContinueToShop)
        {
            GameStateManager.Instance.AdvanceLevel();
            GameManager.Instance?.GoToShop();
        }
        else
        {
            GameStateManager.Instance.StartNewRun();
            GameManager.Instance?.GoToMainMenu();
        }
    }

    private void OnBackToMenuClicked()
    {
        GameStateManager.Instance?.StartNewRun();
        GameManager.Instance?.GoToMainMenu();
    }
}