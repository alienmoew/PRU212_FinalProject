using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSpawner : MonoBehaviour
{
    public GameObject[] playerPrefabs;
    public Transform[] spawnPoints;

    private void Start()
    {
        int randomNumber = Random.Range(0, spawnPoints.Length);
        Transform spawnPoint = spawnPoints[randomNumber];
        GameObject playerToSpawn;

        if (PhotonNetwork.LocalPlayer.CustomProperties["playerAvatar"] == null)
        {
            playerToSpawn = playerPrefabs[0];
        }
        else
        {
            playerToSpawn = playerPrefabs[(int)PhotonNetwork.LocalPlayer.CustomProperties["playerAvatar"]];
        }

        PhotonNetwork.Instantiate(playerToSpawn.name, spawnPoint.position, Quaternion.identity);

    }
}
