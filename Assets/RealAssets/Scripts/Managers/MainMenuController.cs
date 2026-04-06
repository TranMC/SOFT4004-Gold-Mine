using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;

public class MainMenuController : MonoBehaviour
{
    // [Header("Scene Settings")]
    // [SerializeField] private string gameplaySceneName = "GameScene";

    // [Header("UI Panels")]
    // [SerializeField] private GameObject settingsPanel;

    private const string LastUnlockedLevelKey = "LastUnlockedLevel";
    private const string HasSaveKey = "HasSaveData";

    private void Start()
    {
        // // Ẩn panel cài đặt lúc mở menu (nếu có gán)
        // if (settingsPanel != null)
        // {
        //     settingsPanel.SetActive(false);
        // }
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
    // public void OnSettingsButton()
    // {
    //     if (settingsPanel != null)
    //     {
    //         settingsPanel.SetActive(true);
    //     }
    //     else
    //     {
    //         Debug.LogWarning("Chưa gán Settings Panel trong MainMenuController.");
    //     }
    // }

    // // Nút trong panel cài đặt để đóng lại
    // public void OnCloseSettingsButton()
    // {
    //     if (settingsPanel != null)
    //     {
    //         settingsPanel.SetActive(false);
    //     }
    // }

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
    [SerializeField] private Button soundToggleButton;

    private bool isSubscribedToMuteEvent;
    private Coroutine waitForAudioManagerCoroutine;

    private void OnEnable()
    {
        TryBindAudioManager();

        if (!isSubscribedToMuteEvent)
        {
            waitForAudioManagerCoroutine = StartCoroutine(WaitAndBindAudioManager());
        }
    }

    private void OnDisable()
    {
        if (waitForAudioManagerCoroutine != null)
        {
            StopCoroutine(waitForAudioManagerCoroutine);
            waitForAudioManagerCoroutine = null;
        }

        if (isSubscribedToMuteEvent && AudioManager.Instance != null)
        {
            AudioManager.Instance.OnMuteStateChanged -= HandleMuteStateChanged;
            isSubscribedToMuteEvent = false;
        }
    }

    public void OnSoundToggleClicked()
    {
        if (!isSubscribedToMuteEvent)
        {
            TryBindAudioManager();
        }

        AudioManager.Instance?.ToggleMute();
    }

    private void HandleMuteStateChanged(bool isMuted)
    {
        Image targetImage = ResolveSoundTargetImage();
        if (targetImage != null)
        {
            targetImage.sprite = isMuted ? soundOffSprite : soundOnSprite;
        }
    }

    private void TryBindAudioManager()
    {
        if (isSubscribedToMuteEvent || AudioManager.Instance == null)
        {
            return;
        }

        AudioManager.Instance.OnMuteStateChanged += HandleMuteStateChanged;
        isSubscribedToMuteEvent = true;
        HandleMuteStateChanged(AudioManager.Instance.IsMuted);
    }

    private IEnumerator WaitAndBindAudioManager()
    {
        while (!isSubscribedToMuteEvent)
        {
            TryBindAudioManager();
            if (isSubscribedToMuteEvent)
            {
                break;
            }

            yield return null;
        }

        waitForAudioManagerCoroutine = null;
    }

    private Image ResolveSoundTargetImage()
    {
        if (soundIcon != null)
        {
            return soundIcon;
        }

        if (soundToggleButton == null)
        {
            return null;
        }

        if (soundToggleButton.targetGraphic is Image targetGraphicImage)
        {
            return targetGraphicImage;
        }

        return soundToggleButton.image;
    }
}