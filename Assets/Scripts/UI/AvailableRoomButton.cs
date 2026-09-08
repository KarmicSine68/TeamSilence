using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class AvailableRoomButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler
{

    private bool isHoveredOver;
    private RoomSelectManager rmManager;

    [SerializeField]
    public RoomSelectManager.RoomType type;

    public void Init(RoomSelectManager.RoomType t)
    {
        type = t;
        rmManager = FindAnyObjectByType<RoomSelectManager>();
        GetComponent<Image>().color = rmManager.RoomColors[(int)t];
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        isHoveredOver = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        isHoveredOver = false;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (isHoveredOver)
        {
            rmManager.SelectRoom(type, eventData.position);
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        HeldRoom heldObj = FindAnyObjectByType<HeldRoom>();

        if (heldObj == null)
        {
            return;
        }
        else
        {
            heldObj.HandleMouseUp();
        }
    }
}
