using System.Collections.Generic;
using UnityEngine;

public class MapGenerator : MonoBehaviour
{
    [SerializeField] private GameObject[] Room2DoorPrefabs;
    [SerializeField] private GameObject[] Room3DoorPrefabs;
    [SerializeField] private GameObject[] Room1DoorPrefabs;
    [SerializeField] private List<GameObject> Rooms;
    public GameObject StartingRoom;

    public GameObject WinText;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        BuildLevel(10, 5);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    /// <summary>
    /// Takes in numbers for roomAmount and spawns that amount of rooms picking randomly from the selection + the amount of Single offshoot rooms
    /// </summary>
    /// <param name="roomAmount"></param>
    /// <param name="alcoveRooms"></param>
    public void BuildLevel(int roomAmount, int alcoveRooms)
    {
        //Spawns in the first starting room and gives that data to previous room to make sure the rooms can start being linked
        GameObject previousRoom = Instantiate(StartingRoom, Vector3.zero, Quaternion.identity);
        Rooms.Add(previousRoom);
        //Loops through until all the rooms get spawned
        for (int i = 0; i < roomAmount; i++)
        {
            GameObject CurrentRoom = null;
            //Roll to see if a room will have an offshoot room to know weather to spawn a room with 3 or 2 doors in
            if (alcoveRooms >= 1 && Random.Range(0f,1f) <= ((alcoveRooms +1) / (roomAmount - i)))
            {
                //make the room from the array of prefabs that have the 3 doors needed to have an offshoot room
                CurrentRoom = Instantiate(Room3DoorPrefabs[Random.Range(0, Room3DoorPrefabs.Length)], new Vector3(100 * i + 100, 0, 0), Quaternion.identity);
                Rooms.Add(CurrentRoom);

                //Check which way the door is facing so that the way the player enters feels like it make sense
                if (!previousRoom.GetComponent<RoomData>().DoorForward.left)
                {
                    CurrentRoom.transform.SetPositionAndRotation(CurrentRoom.transform.position, Quaternion.Euler(0, CurrentRoom.GetComponent<RoomData>().RotateAmount, 0));
                    //change which door is the door facing forward because the only way to make sure that both doors are visible from an angle is to have whichever door was the entrance to the room now be the exit.
                    Door TempDoor = CurrentRoom.GetComponent<RoomData>().DoorForward;
                    CurrentRoom.GetComponent<RoomData>().DoorForward = CurrentRoom.GetComponent<RoomData>().DoorBackwards;
                    CurrentRoom.GetComponent<RoomData>().DoorBackwards = TempDoor;
                    GameObject TempSpawn = CurrentRoom.GetComponent<RoomData>().roomEntranceSpawn;
                    CurrentRoom.GetComponent<RoomData>().roomEntranceSpawn = CurrentRoom.GetComponent<RoomData>().roomBacktrackSpawn;
                    CurrentRoom.GetComponent<RoomData>().roomBacktrackSpawn = TempSpawn;
                    if (CurrentRoom.GetComponent<RoomData>().DoorOffshoot != null)
                    {
                        CurrentRoom.GetComponent<RoomData>().DoorOffshoot.left = !CurrentRoom.GetComponent<RoomData>().DoorOffshoot.left;
                    }
                    CurrentRoom.GetComponent<RoomData>().DoorForward.left = !CurrentRoom.GetComponent<RoomData>().DoorForward.left;
                }
                //make the Offshoot room here so we don't have to loop back through
                GameObject OffShootRoom = Instantiate(Room1DoorPrefabs[Random.Range(0, Room1DoorPrefabs.Length)], new Vector3(100 * i +100, 0, 100), Quaternion.identity);
                if (CurrentRoom.GetComponent<RoomData>().DoorOffshoot.left)
                {
                    OffShootRoom.transform.SetPositionAndRotation(OffShootRoom.transform.position, Quaternion.Euler(0, OffShootRoom.GetComponent<RoomData>().RotateAmount, 0));
                }
                //connect all of the data to be able to go back and forth
                CurrentRoom.GetComponent<RoomData>().DoorOffshoot.TeleportSpot = OffShootRoom.GetComponent<RoomData>().roomEntranceSpawn;
                OffShootRoom.GetComponent<RoomData>().DoorBackwards.TeleportSpot = CurrentRoom.GetComponent<RoomData>().roomBacktrackOffshootSpawn;
                alcoveRooms--;
            }
            else
            {
                //spawn in a room out of the rooms we have that has 2 doors
                CurrentRoom = Instantiate(Room2DoorPrefabs[Random.Range(0, Room2DoorPrefabs.Length)], new Vector3(100*i + 100,0), Quaternion.identity);
                Rooms.Add(CurrentRoom);
                //rotate if the room needs to face the other way to have it fit in with the walking through a door
                if (!previousRoom.GetComponent<RoomData>().DoorForward.left)
                {
                    CurrentRoom.transform.SetPositionAndRotation(CurrentRoom.transform.position, Quaternion.Euler(0, CurrentRoom.GetComponent<RoomData>().RotateAmount, 0));
                    //if the room has to roatate 180 degrees the door that would normally be the exit would now be the entrance
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

            //connect the doors between the rooms
            previousRoom.GetComponent<RoomData>().DoorForward.TeleportSpot = CurrentRoom.GetComponent<RoomData>().roomEntranceSpawn;
            CurrentRoom.GetComponent<RoomData>().DoorBackwards.TeleportSpot = previousRoom.GetComponent<RoomData>().roomBacktrackSpawn;
            previousRoom = CurrentRoom;
        }
    }
}
