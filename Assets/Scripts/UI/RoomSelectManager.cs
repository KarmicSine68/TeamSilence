using NaughtyAttributes;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RoomSelectManager : MonoBehaviour
{
    public enum RoomType
    {
        None,
        Tier1,
        Tier2,
        Tier3
    }
    [SerializeField]
    private Button generateBtn;

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

    [Header("Cost"), HorizontalLine(4, EColor.Indigo), SerializeField]
    private TMP_Text costText;

    [SerializeField]
    public List<int> tierCosts;

    [SerializeField]
    private int startingCurrency;

    [HideInInspector]
    public int currentCurrency;
    public static Action<int> currencyUpdated;
    public static Action<List<RoomType>> RoomTypesSelected;

    [SerializeField]
    private GameObject InSceneUI;
    [SerializeField]
    private TMP_Text health;
    [SerializeField]
    private TMP_Text dash;

    private void Awake()
    {
        currentCurrency = startingCurrency;

        
        SetUpRooms();
        UpdateCostUI();
        SetGenerateBtnSelectability();
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
        if (Rooms[id] != RoomType.None)
        {
            currentCurrency += tierCosts[(int)Rooms[id] - 1];
        }

        if (type != RoomType.None)
        {
            currentCurrency -= tierCosts[(int)type - 1];
        }

        /*//clearing a room
        if (type == RoomType.None && Rooms[id] != RoomType.None)
        {
            currentCurrency += tierCosts[(int)Rooms[id] - 1];
        }
        else
        {
            //setting a room
            if (type != RoomType.None)
            {
                currentCurrency -= tierCosts[(int)type - 1];
            }
            
        }*/

        Rooms[id] = type;


        SetGenerateBtnSelectability();
        UpdateCostUI();
    }

    public void SetUpRooms()
    {
        Rooms = new List<RoomType>();
        for (int i = 0; i < numberOfRoomSlots; i++)
        {
            Rooms.Add(RoomType.None);
            EquippedRoomSelection tempRm = Instantiate(roomPrefab, equippedRoomContainer.transform);
            tempRm.Init(i);
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

    private void UpdateCostUI()
    {
        currencyUpdated?.Invoke(currentCurrency);
        costText.text = $"Currency: {currentCurrency}";
    }

    private void SetGenerateBtnSelectability()
    {
        generateBtn.interactable = !Rooms.Contains(RoomType.None);
    }

    public void ClearAllRooms()
    {
        for (int i = 0; i < equippedRoomButtons.Count; i++)
        {
            equippedRoomButtons[i].EquipRoom(RoomType.None);
        }
    }

    public void StartRun()
    {
        RoomTypesSelected?.Invoke(Rooms);
        
    }

    public TMP_Text[] ReturnPlayerUI()
    {
        InSceneUI.SetActive(true);

        TMP_Text[] uiElements = new TMP_Text[2];

        uiElements[0] = health;
        uiElements[1] = dash;

        gameObject.SetActive(false);

        return uiElements;
    }
}
