using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class EquippedRoomSelection : MonoBehaviour, IPointerClickHandler
{
    [SerializeField]
    private int id;

    [SerializeField]
    private Sprite lobster;

    private RoomSelectManager.RoomType heldType;

    private RoomSelectManager roomSelectManager;
    private Image imgComp;

    private void Awake()
    {
        roomSelectManager = FindAnyObjectByType<RoomSelectManager>();
        imgComp = GetComponent<Image>();
        imgComp.sprite = null;
    }

    public void Init(int id = -1)
    {
        this.id = id;
    }
    public void EquipRoom(RoomSelectManager.RoomType type)
    {
        heldType = type;
        roomSelectManager.SetRoom(id, type);

        imgComp.color = roomSelectManager.RoomColors[(int)type];

        if (type == RoomSelectManager.RoomType.None)
        {
            imgComp.sprite = null;
        }
        else
        {
            imgComp.sprite = lobster;
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (heldType != RoomSelectManager.RoomType.None)
        {
            EquipRoom(RoomSelectManager.RoomType.None);
        }
    }
}
