using System.Collections.Generic;
using Objects;
using UnityEngine;

[CreateAssetMenu(menuName = "Create LevelData", fileName = "LevelData", order = 0)]
public class LevelData : ScriptableObject
{
    public List<GameObject> levelEnemies;
    public List<Item> items;
}