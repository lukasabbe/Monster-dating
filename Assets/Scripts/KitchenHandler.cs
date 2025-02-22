using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

namespace DialogueSystem
{
    public class KitchenHandler : MonoBehaviour
    {
        public KitchenType currentKitchenType = KitchenType.none;
        public List<Transform> kitchenPoints;
        public List<bool> occupiedPlace = new List<bool>();

        public Color cookedFood;
        public Color burnedFood;
        public Sprite MixedShit;
        public Sprite CuttShit;

        public Color warning;

        public Image ProgBarStove;
        public Image ProgBarCutter;
        public Image ProgBarMixer;

        public TMP_Text timer;

        public int timeToCook = 25;
        
        public List<GameObject> DontUnload = new List<GameObject>();

        public AudioClip mixerSound;
        public AudioClip cutterSound;
        public AudioClip stoveSound;
        
        private void Start()
        {
            for(var i = 0; i < kitchenPoints.Count; i++) occupiedPlace.Add(false);
            StartCoroutine(waitClock());

            IEnumerator waitClock()
            {
                timer.text = $"{timeToCook}";
                for (var i = 0; i < timeToCook; i++)
                {
                    yield return new WaitForSeconds(1);
                    timer.text = $"{timeToCook - i}";
                }
                loadNextScene();
            }
        }
        
        public void loadNextScene()
        {
            StartCoroutine(fade());

            IEnumerator fade()
            {
                yield return new WaitForSeconds(1f);
                foreach (var g in DontUnload)
                {
                    g.transform.SetParent(null);
                    DontDestroyOnLoad(g);
                }
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);   
            }
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
                var audio = GetComponent<AudioSource>();
                audio.PlayOneShot(stoveSound);
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
                
                ProgBarMixer.fillAmount = 1;
                
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
                var savedSprite = item.GetComponent<Image>().sprite;
                item.GetComponent<Image>().sprite = MixedShit;
                
                GetComponent<AudioSource>().PlayOneShot(mixerSound);
                
                
                for (var i = 0; i < 4; i++)
                {
                    yield return new WaitForSeconds(1);
                    ProgBarMixer.fillAmount = Mathf.Lerp(0, 1, i/4f);
                    
                    if (occupiedPlace[2]) continue;
                    ProgBarMixer.fillAmount = 0;
                    item.GetComponent<Image>().sprite = savedSprite;
                    yield break;
                }
                
                if (!occupiedPlace[2])
                {
                    item.GetComponent<Image>().sprite = savedSprite;
                    ProgBarStove.fillAmount = 0;
                    yield break;
                }

                ProgBarMixer.fillAmount = 0;

                item.GetComponent<FoodItem>().mixed = true;
            }
        }

        public void aCutter(GameObject item)
        {
            StartCoroutine(_aCutter());
            IEnumerator _aCutter()
            {
                var audio = GetComponent<AudioSource>();
                audio.PlayOneShot(cutterSound);
                for (var i = 0; i < 4; i++)
                {
                    yield return new WaitForSeconds(1);
                    ProgBarCutter.fillAmount = Mathf.Lerp(0, 1, i/4f);
                    
                    if (occupiedPlace[1]) continue;
                    ProgBarCutter.fillAmount = 0;
                    yield break;
                }
                
                if (!occupiedPlace[1])
                {
                    ProgBarCutter.fillAmount = 0;
                    yield break;
                }
                
                item.GetComponent<Image>().sprite = CuttShit;

                ProgBarCutter.fillAmount = 0;

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