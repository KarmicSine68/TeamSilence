using System.Collections.Generic;
using UnityEngine;

public class MapGenerator : MonoBehaviour
{
    public GameObject[] Room2DoorPrefabs;
    public GameObject[] Room3DoorPrefabs;
    public GameObject[] Room1DoorPrefabs;
    public List<GameObject> Rooms;
    public GameObject StartingRoom;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        BuildLevel(5, 2);
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
                CurrentRoom = Instantiate(Room3DoorPrefabs[Random.Range(0, Room3DoorPrefabs.Length)], new Vector3(100 * i + 100, 0, 0), Quaternion.identity);
                Rooms.Add(CurrentRoom);
                if (!previousRoom.GetComponent<RoomData>().DoorForward.left)
                {
                    CurrentRoom.transform.SetPositionAndRotation(CurrentRoom.transform.position, Quaternion.Euler(0, CurrentRoom.GetComponent<RoomData>().RotateAmount, 0));
                    CurrentRoom.GetComponent<RoomData>().DoorForward.left = !CurrentRoom.GetComponent<RoomData>().DoorForward.left;
                    Door TempDoor = CurrentRoom.GetComponent<RoomData>().DoorForward;
                    CurrentRoom.GetComponent<RoomData>().DoorForward = CurrentRoom.GetComponent<RoomData>().DoorBackwards;
                    CurrentRoom.GetComponent<RoomData>().DoorBackwards = TempDoor;
                    if (CurrentRoom.GetComponent<RoomData>().DoorOffshoot != null)
                    {
                        CurrentRoom.GetComponent<RoomData>().DoorOffshoot.left = !CurrentRoom.GetComponent<RoomData>().DoorOffshoot.left;
                    }
                }
                GameObject OffShootRoom = Instantiate(Room1DoorPrefabs[Random.Range(0, Room1DoorPrefabs.Length)], new Vector3(100 * i +100, 0, 100), Quaternion.identity);
                if (CurrentRoom.GetComponent<RoomData>().DoorOffshoot.left)
                {
                    OffShootRoom.transform.SetPositionAndRotation(OffShootRoom.transform.position, Quaternion.Euler(0, OffShootRoom.GetComponent<RoomData>().RotateAmount, 0));
                }
                CurrentRoom.GetComponent<RoomData>().DoorOffshoot.TeleportSpot = OffShootRoom.GetComponent<RoomData>().roomEntranceSpawn;
                OffShootRoom.GetComponent<RoomData>().DoorBackwards.TeleportSpot = CurrentRoom.GetComponent<RoomData>().roomBacktrackOffshootSpawn;
                alcoveRooms--;
            }
            else
            {
                CurrentRoom = Instantiate(Room2DoorPrefabs[Random.Range(0, Room2DoorPrefabs.Length)], new Vector3(100*i + 100,0), Quaternion.identity);
                Rooms.Add(CurrentRoom);
                if (!previousRoom.GetComponent<RoomData>().DoorForward.left)
                {
                    CurrentRoom.transform.SetPositionAndRotation(CurrentRoom.transform.position, Quaternion.Euler(0, CurrentRoom.GetComponent<RoomData>().RotateAmount, 0));
                    if(CurrentRoom.GetComponent<RoomData>().RotateAmount >= 180)
                    {
                        Door TempDoor = CurrentRoom.GetComponent<RoomData>().DoorForward;
                        CurrentRoom.GetComponent<RoomData>().DoorForward = CurrentRoom.GetComponent<RoomData>().DoorBackwards;
                        CurrentRoom.GetComponent<RoomData>().DoorBackwards = TempDoor;
                        GameObject TempSpawn = CurrentRoom.GetComponent<RoomData>().roomEntranceSpawn;
                        CurrentRoom.GetComponent<RoomData>().roomEntranceSpawn = CurrentRoom.GetComponent<RoomData>().roomBacktrackSpawn;
                        CurrentRoom.GetComponent<RoomData>().roomBacktrackSpawn = TempSpawn;
                    }
                    CurrentRoom.GetComponent<RoomData>().DoorForward.left = !CurrentRoom.GetComponent<RoomData>().DoorForward.left;
                }
            }

            previousRoom.GetComponent<RoomData>().DoorForward.TeleportSpot = CurrentRoom.GetComponent<RoomData>().roomEntranceSpawn;
            CurrentRoom.GetComponent<RoomData>().DoorBackwards.TeleportSpot = previousRoom.GetComponent<RoomData>().roomBacktrackSpawn;
            previousRoom = CurrentRoom;
        }
    }
}
