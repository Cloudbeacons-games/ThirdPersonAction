using System.Collections.Generic;
using UnityEngine;

public static class EnemyManager 
{
    static List<GameObject> enemies = new List<GameObject>();

    public static List<GameObject> ReturnList() => enemies;

    public static void RegisterEnemy(GameObject enemy) => enemies.Add(enemy);
    public static void DeregisterEnemy(GameObject enemy) => enemies.Remove(enemy);
    public static bool Contains(GameObject obj)=> enemies.Contains(obj);


    public static void Clear() => enemies.Clear();
}
