using UnityEngine;

namespace DialogueSystem
{
    [CreateAssetMenu(fileName = "FoodItem", menuName = "Scriptable Objects/FoodItem", order = 1)]
    public class FoodItemScriptableObj : ScriptableObject
    {
        public FoodItemType type;
        public bool cooked;
        public bool ovened;
        public bool burned;
        public bool mixed;
        public bool shopped;
    }
}