using DialogueSystem;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class DragScript : MonoBehaviour, IDragHandler , IBeginDragHandler, IEndDragHandler
{
    public KitchenHandler Handler;
    private Vector3 startPosition;
    private kitchenPlace currentKitchenPoint;
    private void OnEnable()
    {
        SceneManager.sceneLoaded += scene_load;
    }
    
    private void OnDisable()
    {
        SceneManager.sceneLoaded -= scene_load;
    }
    
    private void scene_load(Scene scene, LoadSceneMode mode)
    {
        if (SceneManager.GetActiveScene().buildIndex == GamerManager.getMonsterScean(GamerManager.currentMonster))
        {
            GameObject.Find("DialogueHandler").GetComponent<DialogueHandler>().foodItems.Add(gameObject.GetComponent<FoodItem>());
        }
    }

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
