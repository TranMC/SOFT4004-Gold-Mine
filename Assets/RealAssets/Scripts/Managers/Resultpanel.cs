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

    [Header("Config")]
    [SerializeField] private float showDelay = 1f;         // delay truoc khi hien panel
    [SerializeField] private float autoProceedDelay = 2f;   // thoi gian hien ket qua truoc khi auto chuyen scene
    [SerializeField] private string passMessage = "Bạn đã lên cấp độ tiếp!";
    [SerializeField] private string failMessage = "Bạn chưa hoàn thành nhiệm vụ!";

    private void Awake()
    {
        panel.SetActive(false);
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
        {
            StopAllPlayOneShotAudio();
            StartCoroutine(ShowWithDelay(true));
        }
        else if (state == GameState.Lose)
            StartCoroutine(ShowWithDelay(false));
    }

    private static void StopAllPlayOneShotAudio()
    {
        MonoBehaviour[] allBehaviours = FindObjectsByType<MonoBehaviour>(FindObjectsInactive.Include, FindObjectsSortMode.None);
        for (int i = 0; i < allBehaviours.Length; i++)
        {
            MonoBehaviour behaviour = allBehaviours[i];
            if (behaviour == null)
            {
                continue;
            }

            System.Type type = behaviour.GetType();
            if (type.Name != "PlayOneShotAudio")
            {
                continue;
            }

            System.Reflection.MethodInfo stopMethod = type.GetMethod("StopNow", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public);
            stopMethod?.Invoke(behaviour, null);
        }
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

        StartCoroutine(AutoProceedAfterPass());
    }

    public void ShowFail()
    {
        panel.SetActive(true);
        messageText.text = failMessage;

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