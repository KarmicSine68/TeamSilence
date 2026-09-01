using System.Collections.Generic;
using UnityEngine;

public class MapGenerator : MonoBehaviour
{
    public GameObject[] Room2DoorPrefabs;
    public GameObject[] Room3LeftDoorPrefabs;
    public GameObject[] Room3RightDoorPrefabs;
    public List<GameObject> Rooms;
    public GameObject StartingRoom;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void BuildLevel(int roomAmount, int alcoveRooms)
    {
        GameObject previousRoom = Instantiate(StartingRoom, Vector3.zero, Quaternion.identity);
        Rooms.Add(previousRoom);
        for (int i = 0; i < roomAmount; i++)
        {
            GameObject CurrentRoom = null;
            if (alcoveRooms >= 1 && Random.Range(0,1) <= (2 / roomAmount - i))
            {
                //CurrentRoom =
                if (previousRoom.GetComponent<RoomData>().DoorForward.left)
                {
                    //need to use different room sets to make sure they are both visible
                }
                else
                {

                }
            }
            else
            {
                
                CurrentRoom = Instantiate(Room2DoorPrefabs[Random.Range(0, Room2DoorPrefabs.Length)], new Vector3(20*i,0), Quaternion.identity);
                Rooms.Add(CurrentRoom);
                if (previousRoom.GetComponent<RoomData>().DoorForward.left)
                {
                    //rotate the room 90 degrees to have the entrance on the bottom right
                }
                previousRoom.GetComponent<RoomData>().DoorForward.TeleportSpot = CurrentRoom.GetComponent<RoomData>().roomEntranceSpawn;
                CurrentRoom.GetComponent<RoomData>().DoorBackwards.TeleportSpot = previousRoom.GetComponent<RoomData>().roomBacktrackSpawn;
            }


            previousRoom = CurrentRoom;
        }
    }
}
