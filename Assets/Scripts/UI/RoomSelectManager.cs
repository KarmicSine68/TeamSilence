using NaughtyAttributes;
using System.Collections.Generic;
using UnityEngine;

public class RoomSelectManager : MonoBehaviour
{
    public enum RoomType
    {
        None,
        Tier1,
        Tier2,
        Tier3
    }
    [SerializeField, Header("Prefabs"), HorizontalLine(4, EColor.Blue)]
    private HeldRoom HeldRoomPrefab;
    private HeldRoom heldRoom;

    [SerializeField]
    private int numberOfRoomSlots = 6;

    [SerializeField]
    private EquippedRoomSelection roomPrefab;

    [SerializeField]
    private GameObject equippedRoomContainer;

    private List<EquippedRoomSelection> equippedRoomButtons = new List<EquippedRoomSelection>();


    [SerializeField]
    private AvailableRoomButton roomButtonPrefab;

    [SerializeField]
    private GameObject AvailableButtonContainer;
    private List<AvailableRoomButton> availableButtons = new List<AvailableRoomButton>();

    private List<RoomType> Rooms = new List<RoomType>(6);

    [Header("Room Colors"), HorizontalLine(4, EColor.Red)]
    public List<Color> RoomColors = new List<Color>(3);

    private void Awake()
    {
        SetUpRooms();
    }

    public void SelectRoom(RoomType type, Vector3 position = new())
    {
        if (heldRoom != null)
        {
            return;
        }

        heldRoom = Instantiate(HeldRoomPrefab, position, Quaternion.identity, transform);
        heldRoom.InitHeldRoom(type);
    }

    public void RoomDropped()
    {
        heldRoom = null;
    }

    public void SetRoom(int id, RoomType type)
    {
        Rooms[id] = type;
    }

    public void SetUpRooms()
    {
        Rooms = new List<RoomType>();
        for (int i = 0; i < numberOfRoomSlots; i++)
        {
            Rooms.Add(RoomType.None);
            EquippedRoomSelection tempRm = Instantiate(roomPrefab, equippedRoomContainer.transform);
            equippedRoomButtons.Add(tempRm);
        }

        //grabs all the values of room type minus None
        for (int i = 0; i < System.Enum.GetValues(typeof(RoomType)).Length - 1; i++)
        {
            AvailableRoomButton tempRm = Instantiate(roomButtonPrefab, AvailableButtonContainer.transform);
            availableButtons.Add(tempRm);
            tempRm.Init((RoomType)i + 1);
        }
    }
}
