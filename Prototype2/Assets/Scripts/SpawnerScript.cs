using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnerScript : MonoBehaviour
{
    public GameObject obstaclePrefab;
    [Header("Spawn Settings")]
    public float spawnInterval = 1f;
    public float spawnRange = 6f;
    public float spawnYPosition = -15f;
    public float maxRotationY = 90f;
    public float startingObstacleSpeed = 10f;

    // Start is called before the first frame update
    void Start()
    {
        if (obstaclePrefab == null)
        {
            Debug.LogError("ERROR: Obstacle Prefab is not assigned to the Spawner! Please drag a prefab into the Inspector.");
            return;
        }

        // Start the continuous spawning coroutine
        StartCoroutine(SpawnObstacles());
    }

    IEnumerator SpawnObstacles()
    {
        // Infinite loop to keep spawning while the game is running
        while (true)
        {
            // 1. Wait for the defined interval before spawning
            yield return new WaitForSeconds(spawnInterval);

            // 2. Calculate a random horizontal position (X and Z)
            float randomX = Random.Range(-spawnRange, spawnRange);
            float randomZ = Random.Range(-spawnRange, spawnRange);
            Vector3 spawnPosition = new Vector3(randomX, spawnYPosition, randomZ);

            // 3. Calculate random rotation around Y axis
            float rotationY = Random.Range(-maxRotationY, maxRotationY);
            Quaternion randomRotation = Quaternion.Euler(0f, rotationY, 0f);


            // 4. Instantiate the obstacle at the calculated position
            GameObject newObstacle = Instantiate(obstaclePrefab, spawnPosition, randomRotation);

            // 5. Pass the speed to the obstacle movement script
            ObstacleBehavior movementScript = newObstacle.GetComponent<ObstacleBehavior>();
            if (movementScript != null)
            {
                movementScript.SetFallSpeed(startingObstacleSpeed);
            }
        }
    }
}
