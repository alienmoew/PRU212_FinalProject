using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class GameHandle : MonoBehaviourPunCallbacks
{
    public CameraFollow cameraFollow;
    private Transform playerTransform;

    private void Start()
    {
        // Lắng nghe sự kiện OnPlayerSpawned của Photon để cập nhật playerTransform
        StartCoroutine(FindPlayerTransform());
    }

    private IEnumerator FindPlayerTransform()
    {
        while (playerTransform == null)
        {
            GameObject playerObject = GameObject.FindWithTag("Player");
            if (playerObject != null)
            {
                playerTransform = playerObject.transform;
                cameraFollow.Setup(() => GetCameraFollowPosition());
                yield break;
            }
            yield return null;
        }
    }

    private Vector3 GetCameraFollowPosition()
    {
        if (playerTransform != null)
        {
            Vector3 playerPosition = playerTransform.position;

            // Lấy vị trí của con trỏ chuột trong không gian thế giới
            Vector3 mouseWorldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mouseWorldPosition.z = playerPosition.z;

            // Tính toán vị trí trung bình giữa người chơi và con trỏ chuột
            Vector3 targetPosition = playerPosition + (mouseWorldPosition - playerPosition) / 4f;

            return targetPosition;
        }
        return Vector3.zero;
    }
}
