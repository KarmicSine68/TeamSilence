using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MapGenerator : MonoBehaviour
{
    [SerializeField] private GameObject[] Tier2RoomsPrefabs;
    [SerializeField] private GameObject[] Tier3RoomsPrefabs;
    [SerializeField] private GameObject[] Tier1RoomsPrefabs;
    [SerializeField] private List<GameObject> Rooms;
    public GameObject StartingRoom;

    public GameObject WinText;

    public int RoomAmt;
    public int AlcoveRms;
    [SerializeField] private int spawnDistance;

    [SerializeField]
    private List<GameObject> availableRooms = new List<GameObject>();

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //BuildLevel(RoomAmt, AlcoveRms);
    }

    private void OnEnable()
    {
        RoomSelectManager.RoomTypesSelected += GenerateMapFromPool;
    }

    private void OnDisable()
    {
        RoomSelectManager.RoomTypesSelected -= GenerateMapFromPool;
    }

    private void GenerateMapFromPool(List<RoomSelectManager.RoomType> pool)
    {
        availableRooms.Clear();
        foreach (RoomSelectManager.RoomType roomType in pool)
        {
            GameObject roomprefab = GetRandomRoomFromType(roomType);

            if (availableRooms.Contains(roomprefab))
            {
                bool hasAllRooms = true;
                foreach (GameObject conflictRoom in GetRoomListFromType(roomType))
                {
                    if (!availableRooms.Contains(conflictRoom))
                    {
                        hasAllRooms = false;
                        break;
                    }
                }

                if (hasAllRooms)
                {
                    continue;
                }

                while (availableRooms.Contains(roomprefab))
                {
                    roomprefab = GetRandomRoomFromType(roomType);
                }
            }

            availableRooms.Add(roomprefab);
        }
    }

    private GameObject GetRandomRoomFromType(RoomSelectManager.RoomType type)
    {
        GameObject[] rooms = GetRoomListFromType(type);
        int rand = Random.Range(0, rooms.Length);

        return rooms[rand];
    }

    private GameObject[] GetRoomListFromType(RoomSelectManager.RoomType type)
    {
        switch (type)
        {
            case RoomSelectManager.RoomType.None:
                Debug.Log("Tried to get a random room where the type is none");
                return null;
            case RoomSelectManager.RoomType.Tier1:
                return Tier1RoomsPrefabs;
            case RoomSelectManager.RoomType.Tier2:
                return Tier2RoomsPrefabs;
            case RoomSelectManager.RoomType.Tier3:
                return Tier3RoomsPrefabs;
            default:
                throw new System.Exception("update switch statement in getrandomroom");
        }
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
                CurrentRoom = Instantiate(Tier3RoomsPrefabs[Random.Range(0, Tier3RoomsPrefabs.Length)], new Vector3(100 * i + 100, 0, 0), Quaternion.identity);
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
                GameObject OffShootRoom = Instantiate(Tier1RoomsPrefabs[Random.Range(0, Tier1RoomsPrefabs.Length)], new Vector3(100 * i +100, 0, 100), Quaternion.identity);
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
                CurrentRoom = Instantiate(Tier2RoomsPrefabs[Random.Range(0, Tier2RoomsPrefabs.Length)], new Vector3(spawnDistance*i + spawnDistance,0), Quaternion.identity);
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
            previousRoom.GetComponent<RoomData>().DoorForward.Room = CurrentRoom.GetComponent<RoomData>();
            CurrentRoom.GetComponent<RoomData>().DoorBackwards.TeleportSpot = previousRoom.GetComponent<RoomData>().roomBacktrackSpawn;
            previousRoom = CurrentRoom;
        }
    }
}
