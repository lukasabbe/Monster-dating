using DialogueSystem;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DragScript : MonoBehaviour, IDragHandler , IBeginDragHandler, IEndDragHandler
{
    public KitchenHandler Handler;
    private Vector3 startPosition;
    private kitchenPlace currentKitchenPoint;
    
    public void OnDrag(PointerEventData eventData)
    {
        transform.position = eventData.position;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        GetComponent<Image>().raycastTarget = false;
        startPosition = transform.position;
        if (currentKitchenPoint == null) return;
        Handler.setKitchenOccupied(currentKitchenPoint.kitchenType, false);
        currentKitchenPoint = null;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        GetComponent<Image>().raycastTarget = true;
        currentKitchenPoint = Handler.getCurrentKitchenPoint();
        if (currentKitchenPoint.occupied)
        {
            transform.position = startPosition;
            return;
        }
        switch (currentKitchenPoint.kitchenType)
        {
            case KitchenType.none:
                transform.position = startPosition;
                return;
            case KitchenType.trachcan:
                Destroy(gameObject);
                return;
            default:
                transform.position = currentKitchenPoint.position;
                Handler.setKitchenOccupied(currentKitchenPoint.kitchenType, true);
                break;
        }
    }
}
