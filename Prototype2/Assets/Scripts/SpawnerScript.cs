using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnerScript : MonoBehaviour
{
    public GameObject obstaclePrefab;
    [Header("Spawn Settings")]

    public int obstacleCount = 4;
    public float obstacleChunkHeight = 2f;
    public float obstacleChunkRadius = 3f;
    public float spawnInterval = 1f;
    public float spawnRange = 6f;
    public Vector3 spawnPosition = new Vector3(0f, -15f, 0f);
    public float maxRotationY = 90f;
    public float startingObstacleSpeed = 10f;

    public int seed = 1234;
    public bool enableSeedGeneration = false;

    // Start is called before the first frame update
    void Start()
    {
        if (obstaclePrefab == null)
        {
            Debug.LogError("ERROR: Obstacle Prefab is not assigned to the Spawner! Please drag a prefab into the Inspector.");
            return;
        }

        if (enableSeedGeneration)
        {
            Random.InitState(seed);
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

            //start
            float rand_rotation_y = Random.Range(-10f, 30f);
            for (int i = 0; i < obstacleCount; i += 1)
            {
                GameObject newObstacle = Instantiate(obstaclePrefab, spawnPosition, Quaternion.identity);
                float rand_position_y = Random.Range(-obstacleChunkHeight, obstacleChunkHeight);
                float rand_rotation_x = Random.Range(-60f, 60f) * (rand_position_y / obstacleChunkHeight / 2);
                newObstacle.transform.rotation = Quaternion.Euler(rand_rotation_x, rand_rotation_y, 0);
                newObstacle.transform.Translate(new Vector3(Random.Range(0f, obstacleChunkRadius), rand_position_y, 0f));
                rand_rotation_y += Random.Range(40f, 100f);

                // 5. Pass the speed to the obstacle movement script
                ObstacleBehavior movementScript = newObstacle.GetComponent<ObstacleBehavior>();
                if (movementScript != null)
                {
                    movementScript.SetFallSpeed(startingObstacleSpeed);
                }
            }

        }
    }
}
