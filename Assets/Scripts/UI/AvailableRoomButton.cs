using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class AvailableRoomButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{

    private bool CanPurchase;
    private RoomSelectManager rmManager;

    [SerializeField]
    public RoomSelectManager.RoomType type;

    private int cost;

    private void OnEnable()
    {
        RoomSelectManager.currencyUpdated += UpdateInteractibility;
    }
    private void OnDisable()
    {
        RoomSelectManager.currencyUpdated -= UpdateInteractibility;
    }

    public void Init(RoomSelectManager.RoomType t)
    {
        type = t;
        rmManager = FindAnyObjectByType<RoomSelectManager>();
        GetComponent<Image>().color = rmManager.RoomColors[(int)t];
        cost = rmManager.tierCosts[(int)t - 1];
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (CanPurchase)
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

    private void UpdateInteractibility(int currentCurrency)
    {
        CanPurchase = currentCurrency >= cost;
    }
}
