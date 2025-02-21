using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace DialogueSystem
{
    public class KitchenHandler : MonoBehaviour
    {
        public KitchenType currentKitchenType = KitchenType.none;
        public List<Transform> kitchenPoints;
        public List<bool> occupiedPlace = new List<bool>();

        public Color cookedFood;
        public Color burnedFood;

        public Color warning;

        public Image ProgBarStove;
        
        private void Start()
        {
            for(var i = 0; i < kitchenPoints.Count; i++) occupiedPlace.Add(false);
        }

        public kitchenPlace getCurrentKitchenPoint()
        {
            return currentKitchenType switch
            {
                KitchenType.none => new kitchenPlace(Vector2.zero, KitchenType.none, false),
                KitchenType.stove => new kitchenPlace(kitchenPoints[0].position, KitchenType.stove, occupiedPlace[0]),
                KitchenType.cutter => new kitchenPlace(kitchenPoints[1].position, KitchenType.cutter, occupiedPlace[1]),
                KitchenType.mixer => new kitchenPlace(kitchenPoints[2].position, KitchenType.mixer, occupiedPlace[2]),
                KitchenType.plate => new kitchenPlace(kitchenPoints[3].position, KitchenType.plate, occupiedPlace[3]),
                KitchenType.trachcan => new kitchenPlace(new Vector2(-1, -1), KitchenType.trachcan, false),
                _ => new kitchenPlace(Vector2.zero, KitchenType.none, false)
            };
        }

        public void setKitchenOccupied(KitchenType kitchenType, bool occupied)
        {
            switch (kitchenType)
            {
                case KitchenType.stove:
                    occupiedPlace[0] = occupied;
                    break;
                case KitchenType.cutter:
                    occupiedPlace[1] = occupied;
                    break;
                case KitchenType.mixer:
                    occupiedPlace[2] = occupied;
                    break;
                case KitchenType.plate:
                    occupiedPlace[3] = occupied;
                    break;
            }
        }

        public void ActiveStove(GameObject item)
        {
            StartCoroutine(_activeStove());
            IEnumerator _activeStove()
            {
                for (var i = 0; i < 8; i++)
                {
                    yield return new WaitForSeconds(1);
                    ProgBarStove.fillAmount = Mathf.Lerp(0, 1, i/8f);
                    if (occupiedPlace[0]) continue;
                    ProgBarStove.fillAmount = 0;
                    yield break;
                }
                
                if (!occupiedPlace[0])
                {
                    ProgBarStove.fillAmount = 0;
                    yield break;
                }
                
                item.GetComponent<Image>().color = cookedFood;
                item.GetComponent<FoodItem>().cooked = true;
                
                var orgColor = ProgBarStove.color;
                for (var i = 0; i < 5; i++)
                {
                    yield return new WaitForSeconds(0.5f);
                    ProgBarStove.color = warning;
                    yield return new WaitForSeconds(0.5f);
                    ProgBarStove.color = orgColor;
                    if (occupiedPlace[0]) continue;
                    ProgBarStove.fillAmount = 0;
                    yield break;
                }

                if (!occupiedPlace[0])
                {
                    ProgBarStove.fillAmount = 0;
                    yield break;
                }
                
                item.GetComponent<Image>().color = burnedFood;
                item.GetComponent<FoodItem>().burned = true;
                ProgBarStove.fillAmount = 0;

            }
        }
        
        public void MixerSlush(GameObject item)
        {
            StartCoroutine(_activeStove());
            IEnumerator _activeStove()
            {
                yield return new WaitForSeconds(3f); // first stage
                
                if(!occupiedPlace[2]) yield break;

                item.GetComponent<FoodItem>().mixed = true;
            }
        }

        public void aCutter(GameObject item)
        {
            StartCoroutine(_aCutter());
            IEnumerator _aCutter()
            {
                yield return new WaitForSeconds(3f); // first stage
                
                if(!occupiedPlace[1]) yield break;

                item.GetComponent<FoodItem>().shopped = true;
            }
        }
        
    }

    public class kitchenPlace
    {
        public Vector2 position;

        public KitchenType kitchenType;
        
        public bool occupied;

        public kitchenPlace(Vector2 pos, KitchenType kitchenType, bool occupied)
        {
            position = pos;
            this.kitchenType = kitchenType;
            this.occupied = occupied;
        }
    }
    
    public enum KitchenType
    {
        none,
        stove,
        cutter,
        mixer,
        plate,
        trachcan
    }
}