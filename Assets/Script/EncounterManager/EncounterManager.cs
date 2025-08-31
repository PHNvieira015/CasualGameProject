using UnityEngine;
using System.Collections.Generic;

public class EncounterManager : MonoBehaviour
{
    public static EncounterManager Instance { get; private set; }

    [Header("Spawn Settings")]
    [SerializeField] private Transform encounterParent;
    [SerializeField] private Vector2 spawnArea = new Vector2(5f, 3f);

    [Header("Enemy Groups")]
    [SerializeField] private List<EnemyGroup> normalEnemyGroups;
    [SerializeField] private List<EnemyGroup> eliteEnemyGroups;
    [SerializeField] private List<EnemyGroup> bossGroups;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            InitializeEncounterParent();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void StartEncounter(EncounterType type, string specificGroup = null)
    {
        ClearEncounter();
        EnemyGroup selectedGroup = GetEnemyGroup(type, specificGroup);

        if (selectedGroup == null || selectedGroup.enemyPrefabs.Count == 0)
        {
            Debug.LogWarning($"No valid group found for {type} encounter");
            return;
        }

        SpawnEnemies(selectedGroup);
        MenuController.Instance.ChangeScreen(MenuController.Screens.CombatMenu);
    }

    private void SpawnEnemies(EnemyGroup group)
    {
        if (group == null || group.enemyPrefabs.Count == 0) return;

        // Calculate all positions first
        Vector3[] positions = new Vector3[group.enemyPrefabs.Count];
        for (int i = 0; i < positions.Length; i++)
        {
            positions[i] = CalculateSpawnPosition(i, positions.Length);
        }

        // Then instantiate
        for (int i = 0; i < group.enemyPrefabs.Count; i++)
        {
            if (group.enemyPrefabs[i] == null) continue;

            GameObject enemyInstance = Instantiate(group.enemyPrefabs[i], encounterParent);
            enemyInstance.transform.localPosition = positions[i];
            enemyInstance.name = $"{group.enemyPrefabs[i].name}_{i}";
        }
    }

    private Vector3 CalculateSpawnPosition(int index, int total)
    {
        // Handle single enemy case
        if (total <= 1)
        {
            return new Vector3(3, 0, spawnArea.y / 2);
        }

        // Prevent division by zero in Lerp
        float lerpValue = Mathf.Clamp01((float)index / (total - 1));
        float xPos = Mathf.Lerp(-spawnArea.x / 2, spawnArea.x / 2, lerpValue);

        // Final NaN check just in case
        Vector3 position = new Vector3(xPos, 0, spawnArea.y / 2);
        if (float.IsNaN(position.x))
        {
            Debug.LogWarning($"Invalid position calculated for enemy {index}, defaulting to center");
            return Vector3.zero;
        }

        return position;
    }

    private EnemyGroup GetEnemyGroup(EncounterType type, string groupName)
    {
        List<EnemyGroup> groups = type switch
        {
            EncounterType.Normal => normalEnemyGroups,
            EncounterType.Elite => eliteEnemyGroups,
            EncounterType.Boss => bossGroups,
            _ => null
        };

        if (groups == null || groups.Count == 0) return null;

        return string.IsNullOrEmpty(groupName) ?
            groups[Random.Range(0, groups.Count)] :
            groups.Find(g => g.groupName.Equals(groupName, System.StringComparison.OrdinalIgnoreCase));
    }

    private void InitializeEncounterParent()
    {
        if (encounterParent == null)
        {
            GameObject parent = new GameObject("EncounterParent");
            parent.transform.position = Vector3.zero;
            encounterParent = parent.transform;
        }
    }

    private void ClearEncounter()
    {
        // Safe destruction - unparent first
        while (encounterParent.childCount > 0)
        {
            Transform child = encounterParent.GetChild(0);
            child.SetParent(null);
            Destroy(child.gameObject);
        }
    }
}