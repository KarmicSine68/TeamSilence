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
    [SerializeField] Vector2 roomDimensions;

    private void Start()
    {
        playerRef = FindAnyObjectByType<PlayerBehaviour>().gameObject;
    }

    private void Update()
    {
        Vector3 camPosition = playerRef.transform.position + cameraOffset;
        transform.position = new Vector3(Mathf.Clamp(camPosition.x, -roomDimensions.x + cameraOffset.x, roomDimensions.x + cameraOffset.x)
            , transform.position.y, Mathf.Clamp(camPosition.z, -roomDimensions.y + cameraOffset.z, roomDimensions.y + cameraOffset.z));
        //transform.position = playerRef.transform.position + cameraOffset;
    }
}
