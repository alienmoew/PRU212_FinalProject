using UnityEngine;
using Photon.Pun;
using System.Collections.Generic;

public class EnemySpawner : MonoBehaviourPunCallbacks
{
    public GameObject enemyPrefab; // Reference to the enemy prefab
    public Transform[] spawnPoints; // Array of spawn points

    public float spawnInterval = 5f; // Time interval between spawns
    private float spawnTimer;

    private void Start()
    {
        spawnTimer = spawnInterval; // Initialize the spawn timer
    }

    private void Update()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            spawnTimer -= Time.deltaTime;

            if (spawnTimer <= 0f)
            {
                SpawnEnemy();
                spawnTimer = spawnInterval; // Reset the spawn timer
            }
        }
    }

    private void SpawnEnemy()
    {
        if (enemyPrefab != null && spawnPoints.Length > 0)
        {
            // List to keep track of used spawn points
            List<int> usedIndexes = new List<int>();

            // Iterate until a unique spawn point is found
            Transform spawnPoint = null;
            do
            {
                // Choose a random spawn point index
                int randomIndex = Random.Range(0, spawnPoints.Length);

                // Check if this spawn point index has already been used
                if (!usedIndexes.Contains(randomIndex))
                {
                    usedIndexes.Add(randomIndex); // Mark this index as used
                    spawnPoint = spawnPoints[randomIndex]; // Get the spawn point
                }

                // Ensure we do not get stuck in an infinite loop (all points used)
            } while (spawnPoint == null);

            // Instantiate the enemy prefab at the chosen spawn point
            PhotonNetwork.Instantiate(enemyPrefab.name, spawnPoint.position, spawnPoint.rotation);
        }
    }

}
