using UnityEngine;
using Photon.Pun;

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
            // Choose a random spawn point
            Transform spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];

            // Instantiate the enemy prefab at the chosen spawn point
            PhotonNetwork.Instantiate(enemyPrefab.name, spawnPoint.position, spawnPoint.rotation);
        }
    }
}
