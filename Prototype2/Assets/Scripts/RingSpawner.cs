using UnityEngine;
using System.Collections;

public class RingSpawner : MonoBehaviour
{
    [Header("References")]
    public GameObject ringPrefab;   // your ring prefab (quad/torus)
    public Transform target;        // usually Main Camera (for reference only)

    [Header("Movement Settings")]
    public float spawnInterval = 0.5f;  // how often to spawn
    public float startY = -35f;         // start height
    public float endY = 0f;             // end height
    public float travelTime = 2f;       // time it takes to reach the player
    public float startScale = 1.5f;
    public float endScale = 0.1f;

    private float timer;

    void Update()
    {
        if (!ringPrefab) return;

        timer += Time.deltaTime;
        if (timer >= spawnInterval)
        {
            timer = 0f;
            SpawnRing();
        }
    }

    void SpawnRing()
    {
        // Always spawn exactly at (0, -35, 0)
        Vector3 spawnPos = new Vector3(0f, startY, 0f);
        GameObject ring = Instantiate(ringPrefab, spawnPos, Quaternion.identity);
        StartCoroutine(MoveRing(ring.transform));
    }

    IEnumerator MoveRing(Transform ring)
    {
        float elapsed = 0f;
        Vector3 startPos = new Vector3(0f, startY, 0f);
        Vector3 endPos = new Vector3(0f, endY, 0f);
        ring.localScale = Vector3.one * startScale;

        while (elapsed < travelTime)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / travelTime);

            // Move straight up
            ring.position = Vector3.Lerp(startPos, endPos, Mathf.SmoothStep(0f, 1f, t));

            // Scale down as it gets closer
            float scale = Mathf.Lerp(startScale, endScale, t);
            ring.localScale = Vector3.one * scale;

            yield return null;
        }

        Destroy(ring.gameObject);
    }
}
