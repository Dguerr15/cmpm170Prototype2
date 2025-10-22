using UnityEngine;
using System.Collections;

[RequireComponent(typeof(AudioSource))]
public class BGMPlayer : MonoBehaviour
{
    [Header("BGM Tracks")]
    public AudioClip[] bgmClips;          // drag your 3 bgm files here
    public bool loopPlaylist = true;      // keep playing random tracks forever

    private AudioSource audioSource;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        audioSource.loop = false; // we¡¦ll handle looping manually
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
            // pick a random clip
            AudioClip clip = bgmClips[Random.Range(0, bgmClips.Length)];

            // play it
            audioSource.clip = clip;
            audioSource.Play();

            // wait until it finishes
            yield return new WaitWhile(() => audioSource.isPlaying);

            if (!loopPlaylist)
                break; // stop after one random track
        }
    }
}
