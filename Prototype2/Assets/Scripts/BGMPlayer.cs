using UnityEngine;
using System.Collections;

[RequireComponent(typeof(AudioSource))]
public class BGMPlayer : MonoBehaviour
{
    [Header("BGM Tracks")]
    public AudioClip[] bgmClips;
    public bool loopPlaylist = true;

    [Header("Wind Sound")]
    public AudioClip windClip;
    [Range(0f, 1f)] public float windVolume = 0.35f;
    [Range(0f, 5f)] public float windFadeTime = 2f;

    private AudioSource bgmSource;
    private AudioSource windSource;

    void Awake()
    {
        // Main BGM
        bgmSource = GetComponent<AudioSource>();
        bgmSource.loop = false;
        bgmSource.playOnAwake = false;
        bgmSource.volume = 0f;

        // Wind
        windSource = gameObject.AddComponent<AudioSource>();
        windSource.clip = windClip;
        windSource.loop = true;
        windSource.playOnAwake = false;
        windSource.volume = 0f; // start silent; we'll fade in later
    }

    void Start()
    {
        // Delay all audio until the countdown has finished (timeScale > 0)
        StartCoroutine(BeginAfterUnpause());
    }

    IEnumerator BeginAfterUnpause()
    {
        // Wait for the countdown to finish
        yield return new WaitUntil(() => Time.timeScale > 0f);

        if (windClip) StartCoroutine(FadeIn(windSource, windVolume, windFadeTime));
        StartCoroutine(PlayRandomLoop());
    }

    IEnumerator PlayRandomLoop()
    {
        if (bgmClips == null || bgmClips.Length == 0)
        {
            Debug.LogWarning("BGMPlayer: No BGM clips assigned!");
            yield break;
        }

        while (true)
        {
            AudioClip clip = bgmClips[Random.Range(0, bgmClips.Length)];
            bgmSource.clip = clip;

            // Fade in & play
            yield return StartCoroutine(FadeIn(bgmSource, .3f, 1.5f));

            // Wait for track to end (works regardless of timeScale)
            yield return new WaitWhile(() => bgmSource.isPlaying);

            // Fade out before next track
            yield return StartCoroutine(FadeOut(bgmSource, 1f));

            if (!loopPlaylist) break;
        }
    }

    // --- Fades use unscaled time so they still work if you ever pause the game ---

    IEnumerator FadeIn(AudioSource source, float targetVol, float duration)
    {
        source.Play();
        float startVol = source.volume;
        float t = 0f;
        while (t < duration)
        {
            t += Time.unscaledDeltaTime;
            source.volume = Mathf.Lerp(startVol, targetVol, t / duration);
            yield return null;
        }
        source.volume = targetVol;
    }

    IEnumerator FadeOut(AudioSource source, float duration)
    {
        float startVol = source.volume;
        float t = 0f;
        while (t < duration)
        {
            t += Time.unscaledDeltaTime;
            source.volume = Mathf.Lerp(startVol, 0f, t / duration);
            yield return null;
        }
        source.volume = 0f;
        source.Stop();
    }

    // Optional helper you can call from GameManager on Game Over
    public void FadeOutWindOnGameOver()
    {
        if (windSource && windSource.isPlaying)
            StartCoroutine(FadeOut(windSource, windFadeTime));
    }
}
