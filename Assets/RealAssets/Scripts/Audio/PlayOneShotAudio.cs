using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayOneShotAudio : MonoBehaviour
{
    private static readonly HashSet<PlayOneShotAudio> ActiveInstances = new HashSet<PlayOneShotAudio>();

    public AudioClip soundToPlay;
    public float volume = 1.0f;
    public bool playOnEnter = false, playOnExit = false, playAfterDelay = false;

    [Header("Looping Settings")]
    public bool loop = false;

    [Header("Hook Activation")]
    public bool playWhenTouchHook = true;
    public bool useCollisionEvents = false;
    public bool triggerOnce = true;

    public float playDelay = 0.25f;
    private AudioSource loopingAudioSource;
    private Coroutine delayedPlayCoroutine;
    private bool hasTriggered;

    private void Awake()
    {
        hasTriggered = false;
    }

    private void OnEnable()
    {
        ActiveInstances.Add(this);

        // In hook-trigger mode, audio should only start after touching the hook.
        if (playWhenTouchHook)
        {
            return;
        }

        if (playOnEnter)
        {
            PlayNow();
        }

        if (playAfterDelay)
        {
            delayedPlayCoroutine = StartCoroutine(PlayAfterDelayRoutine());
        }
    }

    private void OnDisable()
    {
        ActiveInstances.Remove(this);

        if (delayedPlayCoroutine != null)
        {
            StopCoroutine(delayedPlayCoroutine);
            delayedPlayCoroutine = null;
        }

        // Stop looping audio when exiting the state
        if (loopingAudioSource != null)
        {
            loopingAudioSource.Stop();
            Object.Destroy(loopingAudioSource.gameObject);
            loopingAudioSource = null;
        }

        if (playOnExit)
        {
            // Always play one-shot on exit, never loop.
            PlayOneShotFromManagerOrFallback(volume);
        }
    }

    public static void StopAllActive()
    {
        if (ActiveInstances.Count == 0)
        {
            return;
        }

        List<PlayOneShotAudio> snapshot = new List<PlayOneShotAudio>(ActiveInstances);
        for (int i = 0; i < snapshot.Count; i++)
        {
            if (snapshot[i] != null)
            {
                snapshot[i].StopNow();
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (useCollisionEvents)
        {
            return;
        }

        TryTriggerFromCollider(other);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!useCollisionEvents)
        {
            return;
        }

        TryTriggerFromCollider(collision.collider);
    }

    private IEnumerator PlayAfterDelayRoutine()
    {
        yield return new WaitForSeconds(Mathf.Max(0f, playDelay));
        PlayNow();
        delayedPlayCoroutine = null;
    }

    public void PlayNow()
    {
        if (soundToPlay == null)
        {
            return;
        }
        
        if (loop)
        {
            // Create a new GameObject with AudioSource for looping
            if (loopingAudioSource == null)
            {
                GameObject audioObject = new GameObject("LoopingAudio_" + soundToPlay.name);
                audioObject.transform.position = transform.position;
                loopingAudioSource = audioObject.AddComponent<AudioSource>();
                loopingAudioSource.clip = soundToPlay;
                loopingAudioSource.volume = Mathf.Clamp01(volume);
                loopingAudioSource.loop = true;
                loopingAudioSource.mute = AudioManager.Instance != null && AudioManager.Instance.IsMuted;
                loopingAudioSource.Play();
            }
        }
        else
        {
            PlayOneShotFromManagerOrFallback(volume);
        }
    }

    public void StopNow()
    {
        if (loopingAudioSource != null)
        {
            loopingAudioSource.Stop();
            Object.Destroy(loopingAudioSource.gameObject);
            loopingAudioSource = null;
        }
    }

    private void TryTriggerFromCollider(Collider2D other)
    {
        if (!playWhenTouchHook || other == null)
        {
            return;
        }

        if (triggerOnce && hasTriggered)
        {
            return;
        }

        // Hook child has HookTrigger and parent has HookController.
        bool touchedHook = other.GetComponent<HookTrigger>() != null || other.GetComponentInParent<HookController>() != null;
        if (!touchedHook)
        {
            return;
        }

        hasTriggered = true;
        PlayNow();
    }

    private void PlayOneShotFromManagerOrFallback(float volumeScale)
    {
        if (soundToPlay == null)
        {
            return;
        }

        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlaySfx(soundToPlay, volumeScale);
            return;
        }

        AudioSource.PlayClipAtPoint(soundToPlay, transform.position, Mathf.Clamp01(volumeScale));
    }
}
