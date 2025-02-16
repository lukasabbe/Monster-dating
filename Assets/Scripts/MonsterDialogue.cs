using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "MonsterDialogue", menuName = "Scriptable Objects/MonsterDialogue")]
public class MonsterDialogue : ScriptableObject
{
    public string file_name;
    public string monster_name;
    public float base_rep;
}
