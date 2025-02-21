using UnityEngine;

namespace DialogueSystem
{
    public class FoodItem : MonoBehaviour
    {
        public FoodItemType type;
        public bool cooked = false;
        public bool ovened = false;
        public bool burned = false;
        public bool mixed = false;
        public bool shopped = false;
        
    }
    public enum FoodItemType
    {
        musroom,
        beef,
        bolt,
        leaf
    }
}