using System.Collections.Generic;
using DialogueSystem;
using NUnit.Framework;
using UnityEngine;

[CreateAssetMenu(fileName = "MonsterDialogue", menuName = "Scriptable Objects/MonsterDialogue")]
public class MonsterDialogue : ScriptableObject
{
    public string file_name;
    public string monster_name;
    public float base_rep;
    public List<FoodItemScriptableObj> req_food_items = new();
    public List<string> food_comments = new();
    public List<float> end_succes_values = new(3);
    public List<string> end_comments = new(3);
}
