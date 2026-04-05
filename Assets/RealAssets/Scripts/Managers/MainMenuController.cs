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
        PlayerPrefs.SetInt(LastUnlockedLevelKey, 1);
        PlayerPrefs.SetInt(HasSaveKey, 1);
        PlayerPrefs.Save();

        GameManager.Instance?.OnPlayButtonClicked();
    }

    // Nút "Tiếp tục"
    public void OnContinueButton()
    {
        if (PlayerPrefs.GetInt(HasSaveKey, 0) == 1)
        {
            GameManager.Instance?.StartLevel(LevelManager.Instance != null ? LevelManager.Instance.CurrentLevel : 1);
        }
        else
        {
            Debug.Log("Chưa có dữ liệu lưu để tiếp tục.");
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
        GameManager.Instance?.OnQuitButtonClicked();
    }

    // Nút "Âm thanh"
    [Header("Sound Toggle")]
    [SerializeField] private Image soundIcon;
    [SerializeField] private Sprite soundOnSprite;
    [SerializeField] private Sprite soundOffSprite;

    private void OnEnable()
    {
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.OnMuteStateChanged += HandleMuteStateChanged;
            HandleMuteStateChanged(AudioManager.Instance.IsMuted);
        }
    }

    private void OnDisable()
    {
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.OnMuteStateChanged -= HandleMuteStateChanged;
        }
    }

    public void OnSoundToggleClicked()
    {
        AudioManager.Instance?.ToggleMute();
    }

    private void HandleMuteStateChanged(bool isMuted)
    {
        if (soundIcon != null)
        {
            soundIcon.sprite = isMuted ? soundOffSprite : soundOnSprite;
        }
    }
}