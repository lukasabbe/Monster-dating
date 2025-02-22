using System.Collections.Generic;
using UnityEngine;

public class GamerManager : MonoBehaviour
{
    public static int currentMonster;

    public static List<bool> completedMonsters = new();
    
    private void Start()
    {
        DontDestroyOnLoad(gameObject);
        for (var i = 0; i < 3; i++) completedMonsters.Add(false);
    }

    public static void setCurrentMonster(int monster)
    {
        currentMonster = monster;
    }

    public static int getMonsterScean(int currentMonster)
    {
        return currentMonster switch
        {
            0 => 4,
            1 => 5,
            2 => 6,
            _ => 0
        };
    }
}
