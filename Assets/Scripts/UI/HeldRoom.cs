using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using NaughtyAttributes;

public class HeldRoom : MonoBehaviour, IPointerMoveHandler
{
    [SerializeField, Layer]
    private int EquippedRoomsLayer;
    private RoomSelectManager.RoomType roomType;
    RoomSelectManager rmManager;

    public void InitHeldRoom(RoomSelectManager.RoomType type)
    {
        roomType = type;
        rmManager = GetComponentInParent<RoomSelectManager>();
        GetComponent<Image>().color = rmManager.RoomColors[(int)type];
    }

    public void OnPointerMove(PointerEventData eventData)
    {
        transform.position = eventData.position;
    }

    public void HandleMouseUp()
    {
        
        EquippedRoomSelection overlappedRoom = CheckOverlap(new Vector2(transform.position.x, transform.position.y));
        if (overlappedRoom != null)
        {
            overlappedRoom.EquipRoom(roomType);
        }

        rmManager.RoomDropped();
        Destroy(gameObject);
    }

    public EquippedRoomSelection CheckOverlap(Vector2 MousePos)
    {
        PointerEventData pointerData = new PointerEventData(EventSystem.current);
        pointerData.position = MousePos;

        List<RaycastResult> results = new List<RaycastResult>();

        GetComponentInParent<GraphicRaycaster>().Raycast(pointerData, results);

        foreach (RaycastResult result in results) {
            if (result.gameObject.layer == EquippedRoomsLayer)
            {
                EquippedRoomSelection room = result.gameObject.GetComponent<EquippedRoomSelection>();

                if (room != null)
                {
                    return room;
                }

                
            }
        }

        return null;
    }
}
