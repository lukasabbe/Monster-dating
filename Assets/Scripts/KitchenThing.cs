using UnityEngine;
using UnityEngine.EventSystems;

namespace DialogueSystem
{
    public class KitchenThing : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        public KitchenHandler Handler;
        public KitchenType type;
        public int index;
        
        public void OnPointerEnter(PointerEventData eventData)
        {
            Handler.currentKitchenType = type;
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            Handler.currentKitchenType = KitchenType.none;
        }
    }
}