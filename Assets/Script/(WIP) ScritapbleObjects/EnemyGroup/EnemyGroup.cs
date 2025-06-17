using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "NewEnemyGroup", menuName = "Encounter System/Enemy Group")]
public class EnemyGroup : ScriptableObject
{
    [Header("Group Identification")]
    public string groupName = "Default Group";

    [Header("Enemies")]
    public List<GameObject> enemyPrefabs; // All enemies that will spawn

    [Header("Formation Settings")]
    public bool useCustomFormation = false;
    public Vector3[] customPositions; // Optional custom positions
}