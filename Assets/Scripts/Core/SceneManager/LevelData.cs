using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Create LevelData", fileName = "LevelData", order = 0)]
public class LevelData : ScriptableObject
{
    public List<GameObject> levelEnemies;
    public List<GameObject> items;
}