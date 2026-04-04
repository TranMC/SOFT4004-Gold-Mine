using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuController : MonoBehaviour
{
    [Header("Scene Settings")]
    [SerializeField] private string gameplaySceneName = "GameScene";

    [Header("UI Panels")]
    [SerializeField] private GameObject settingsPanel;

    private const string LastUnlockedLevelKey = "LastUnlockedLevel";
    private const string HasSaveKey = "HasSaveData";

    private void Start()
    {
        // Ẩn panel cài đặt lúc mở menu (nếu có gán)
        if (settingsPanel != null)
        {
            settingsPanel.SetActive(false);
        }
    }

    // Nút "Bắt đầu"
    public void OnStartButton()
    {
        // New game: reset data cơ bản (tùy game bạn có thể thêm key khác)
        PlayerPrefs.SetInt(LastUnlockedLevelKey, 1);
        PlayerPrefs.SetInt(HasSaveKey, 1);
        PlayerPrefs.Save();

        SceneManager.LoadScene(gameplaySceneName);
    }

    // Nút "Tiếp tục"
    public void OnContinueButton()
    {
        if (PlayerPrefs.GetInt(HasSaveKey, 0) == 1)
        {
            // Có save thì vào game
            SceneManager.LoadScene(gameplaySceneName);
        }
        else
        {
            Debug.Log("Chưa có dữ liệu lưu để tiếp tục.");
            // Có thể hiện popup "Chưa có dữ liệu"
        }
    }

    // Nút "Cài đặt"
    public void OnSettingsButton()
    {
        if (settingsPanel != null)
        {
            settingsPanel.SetActive(true);
        }
        else
        {
            Debug.LogWarning("Chưa gán Settings Panel trong MainMenuController.");
        }
    }

    // Nút trong panel cài đặt để đóng lại
    public void OnCloseSettingsButton()
    {
        if (settingsPanel != null)
        {
            settingsPanel.SetActive(false);
        }
    }

    // Nút "Thoát"
    public void OnQuitButton()
    {
        Debug.Log("Thoát game");

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    // Nút "Âm thanh"
    [Header("Sound Toggle")]
    [SerializeField] private Image soundIcon;
    [SerializeField] private Sprite soundOnSprite;
    [SerializeField] private Sprite soundOffSprite;
    private bool isSoundOn = true;

    public void OnSoundToggleClicked()
    {
        isSoundOn = !isSoundOn;
        if (soundIcon != null)
            soundIcon.sprite = isSoundOn ? soundOnSprite : soundOffSprite;
        Debug.Log("Am thanh: " + (isSoundOn ? "BAT" : "TAT"));
    }
}