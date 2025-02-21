using System.Collections.Generic;
using DialogueSystem;
using UnityEngine;

[CreateAssetMenu(fileName = "MonsterDialogue", menuName = "Scriptable Objects/MonsterDialogue")]
public class MonsterDialogue : ScriptableObject
{
    public string file_name;
    public string monster_name;
    public float base_rep;
    public List<FoodItemScriptableObj> req_food_items = new();
}
