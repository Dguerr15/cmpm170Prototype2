using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlashLightController : MonoBehaviour
{
    public Light flashLight;
    public KeyCode toggleKey = KeyCode.F;

    public AudioClip toggleSound;
    private AudioSource audioSource;

    [Header("Flicker Settings")]
    public float minWaitTime = 5f;
    public float maxWaitTime = 10f;
    public int flickerCount = 2;
    public float flickerDuration = 0.1f;
    public float pauseBetweenFlicker = 0.1f;
    // Start is called before the first frame update
    void Start()
    {
        flashLight = GetComponent<Light>();
        audioSource = GetComponent<AudioSource>();

        StartCoroutine(FlickerRoutine());
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(toggleKey))
        {
            flashLight.enabled = !flashLight.enabled;
            if (toggleSound != null && audioSource != null)
            {
                audioSource.PlayOneShot(toggleSound);
            }
        }
    }

    IEnumerator FlickerRoutine()
    {
        while (true)
        {
            float waitTime = Random.Range(minWaitTime, maxWaitTime);
            yield return new WaitForSeconds(waitTime);
            if (flashLight.enabled)
            {
                for(int i = 0; i < flickerCount; i++)
                {
                    flashLight.enabled = false;
                    yield return new WaitForSeconds(flickerDuration);
                    flashLight.enabled = true;
                    yield return new WaitForSeconds(pauseBetweenFlicker);
                }   
            }
        }
    }
}
