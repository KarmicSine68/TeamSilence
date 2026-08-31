/******************************************************************************
 * Author: Brad Dixon
 * File Name: FollowingCamera.cs
 * Creation Date: 8/31/2026
 * Brief: Has the camera follow the player. Stays with the confines 
 * of the room.
 * External Resources: N/A
 * ***************************************************************************/
using UnityEngine;

public class FollowingCamera : MonoBehaviour
{
    GameObject playerRef;
    [SerializeField] Vector3 cameraOffset;

    private void Start()
    {
        playerRef = FindAnyObjectByType<PlayerBehaviour>().gameObject;
    }

    private void Update()
    {
        transform.position = playerRef.transform.position + cameraOffset;
    }
}
