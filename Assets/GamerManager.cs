using System.Collections.Generic;
using System.IO;
using DialogueSystem;
using UnityEngine;

public class GamerManager : MonoBehaviour
{
    public static int currentMonster;

    public static List<bool> completedMonsters = new();
    
    private void Start()
    {
        DontDestroyOnLoad(gameObject);
        for (var i = 0; i < 3; i++) completedMonsters.Add(false);
        LoadGame();
    }

    public static void setCurrentMonster(int monster)
    {
        currentMonster = monster;
    }

    public static int getMonsterScean(int currentMonster)
    {
        return currentMonster switch
        {
            0 => 6,
            1 => 7,
            2 => 8,
            _ => 0
        };
    }

    public static void SaveGame()
    {
        var save_path = Application.persistentDataPath + "/monsters_dating.dat";
        var buffer = new Buffer(Buffer.KiB * 10);
        buffer.Write(completedMonsters[0]);
        buffer.Write(completedMonsters[1]);
        buffer.Write(completedMonsters[2]);
        buffer.Shrink();
        File.WriteAllBytes(save_path, buffer);
    }

    public static void LoadGame()
    {
        var save_path = Application.persistentDataPath + "/monsters_dating.dat";
        if(!File.Exists(save_path)) return;  
        var bytes = File.ReadAllBytes(save_path);
        var buffer = new Buffer(bytes);
        buffer.Read(out bool temp1);
        completedMonsters[0] = temp1;
        buffer.Read(out bool temp2);
        completedMonsters[1] = temp2;
        buffer.Read(out bool temp3);
        completedMonsters[2] = temp3;
    }

    public void removeData()
    {
        var save_path = Application.persistentDataPath + "/monsters_dating.dat";
        if(!File.Exists(save_path)) return;
        File.Delete(save_path);
        for (var i = 0; i < 3; i++) completedMonsters[i] = false;
    }
}
