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
        currentKitchenPoint = Handler.getCurrentKitchenPoint();
        if (currentKitchenPoint.occupied && currentKitchenPoint.kitchenType != KitchenType.plate)
        {
            transform.position = startPosition;
            return;
        }

        if (currentKitchenPoint.kitchenType != KitchenType.plate)
        {
            GetComponent<Image>().raycastTarget = true;
        }
        else
        {
            Handler.DontUnload.Add(gameObject);
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
                switch (Handler.currentKitchenType)
                {
                    case KitchenType.stove:
                        Handler.ActiveStove(gameObject);
                        break;
                    case KitchenType.mixer:
                        Handler.MixerSlush(gameObject);
                        break;
                    case KitchenType.cutter:
                        Handler.aCutter(gameObject);
                        break;
                }
                Handler.setKitchenOccupied(currentKitchenPoint.kitchenType, true);
                break;
        }
    }
}
