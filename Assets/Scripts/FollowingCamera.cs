/******************************************************************************
 * Author: Brad Dixon
 * File Name: FollowingCamera.cs
 * Creation Date: 8/31/2026
 * Last Modified: 9/1/2026
 * Brief: Has the camera follow the player. Stays with the confines 
 *        of the room.
 * External Resources: N/A
 * ***************************************************************************/
using UnityEngine;

public class FollowingCamera : MonoBehaviour
{
    GameObject playerRef;
    [SerializeField] Vector3 cameraOffset;
    public Vector3 CameraOffset
    {
        get => cameraOffset;
    }
    [SerializeField] Vector2 roomDimensions;

    /// <summary>
    /// Gets reference to the player
    /// </summary>
    private void Start()
    {
        playerRef = FindAnyObjectByType<PlayerBehaviour>().gameObject;
    }

    /// <summary>
    /// Camera follows player. Makes sure to clamp camera to room size and offset the position from the player
    /// </summary>
    private void Update()
    {
        //Offsets the camera's position from the player's position
        Vector3 camPosition = playerRef.transform.position + cameraOffset;

        //Delete when clamping works with relative player position
        transform.position = camPosition;

        //Clamping causes the mouse position to not be fully relative to the player.
        //Will work on fixing later, prioritizing the functionality of the player first
        //Clamps the camera to the boundaries of the room
        //transform.position = new Vector3(Mathf.Clamp(camPosition.x, -roomDimensions.x + cameraOffset.x, roomDimensions.x + cameraOffset.x)
        //    , transform.position.y, Mathf.Clamp(camPosition.z, -roomDimensions.y + cameraOffset.z, roomDimensions.y + cameraOffset.z));
    }
}
