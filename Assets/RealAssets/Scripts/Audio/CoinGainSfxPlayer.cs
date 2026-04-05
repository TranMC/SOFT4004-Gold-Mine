using UnityEngine;

/// <summary>
/// Play SFX whenever run coins increase.
/// Attach to a persistent object (for example AudioManager).
/// </summary>
public class CoinGainSfxPlayer : MonoBehaviour
{
    [SerializeField] private AudioClip coinGainClip;
    [SerializeField, Range(0f, 1f)] private float volume = 1f;

    private int lastKnownCoins;
    private bool isSubscribed;

    private void OnEnable()
    {
        TrySubscribe();
    }

    private void OnDisable()
    {
        if (isSubscribed && InventoryManager.Instance != null)
        {
            InventoryManager.Instance.OnRunCoinsChanged -= HandleRunCoinsChanged;
            isSubscribed = false;
        }
    }

    private void Update()
    {
        if (!isSubscribed)
        {
            TrySubscribe();
        }
    }

    private void TrySubscribe()
    {
        if (isSubscribed || InventoryManager.Instance == null)
        {
            return;
        }

        lastKnownCoins = InventoryManager.Instance.RunCoins;
        InventoryManager.Instance.OnRunCoinsChanged += HandleRunCoinsChanged;
        isSubscribed = true;
    }

    private void HandleRunCoinsChanged(int newValue)
    {
        if (newValue > lastKnownCoins)
        {
            PlayCoinGainSfx();
        }

        lastKnownCoins = newValue;
    }

    private void PlayCoinGainSfx()
    {
        if (coinGainClip == null)
        {
            return;
        }

        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlaySfx(coinGainClip, volume);
            return;
        }

        AudioSource.PlayClipAtPoint(coinGainClip, transform.position, Mathf.Clamp01(volume));
    }
}
