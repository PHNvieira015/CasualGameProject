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

    [Header("Reward Settings")]
    [SerializeField] private RewardRange normalReward = new RewardRange(8, 15);
    [SerializeField] private RewardRange eliteReward = new RewardRange(20, 35);
    [SerializeField] private RewardRange bossReward = new RewardRange(45, 60);

    [System.Serializable]
    public class RewardRange
    {
        public int minReward;
        public int maxReward;

        public RewardRange(int min, int max)
        {
            minReward = min;
            maxReward = max;
        }

        public int GetRandomReward()
        {
            return Random.Range(minReward, maxReward + 1);
        }
    }

    private List<Unit> _currentEnemies = new List<Unit>();
    private EncounterType _currentEncounterType;

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
        _currentEncounterType = type;

        EnemyGroup selectedGroup = GetEnemyGroup(type, specificGroup);

        if (selectedGroup == null || selectedGroup.enemyPrefabs.Count == 0)
        {
            Debug.LogWarning("No valid group found for " + type + " encounter");
            return;
        }

        SpawnEnemies(selectedGroup);
        MenuController.Instance.ChangeScreen(MenuController.Screens.CombatMenu);
    }

    private void SpawnEnemies(EnemyGroup group)
    {
        if (group == null || group.enemyPrefabs.Count == 0) return;

        _currentEnemies.Clear();

        Vector3[] positions = new Vector3[group.enemyPrefabs.Count];
        for (int i = 0; i < positions.Length; i++)
        {
            positions[i] = CalculateSpawnPosition(i, positions.Length);
        }

        for (int i = 0; i < group.enemyPrefabs.Count; i++)
        {
            if (group.enemyPrefabs[i] == null) continue;

            GameObject enemyInstance = Instantiate(group.enemyPrefabs[i], encounterParent);
            enemyInstance.transform.localPosition = positions[i];
            enemyInstance.name = $"{group.enemyPrefabs[i].name}_{i}";

            Unit enemyUnit = enemyInstance.GetComponent<Unit>();
            if (enemyUnit != null)
            {
                _currentEnemies.Add(enemyUnit);
            }
        }
    }

    public int CalculateBattleReward()
    {
        return GetRandomRewardForEncounterType(_currentEncounterType);
    }

    private int GetRandomRewardForEncounterType(EncounterType type)
    {
        RewardRange range = GetRewardRangeForEncounterType(type);
        return range.GetRandomReward();
    }

    public RewardRange GetRewardRangeForEncounterType(EncounterType type)
    {
        return type switch
        {
            EncounterType.Normal => normalReward,
            EncounterType.Elite => eliteReward,
            EncounterType.Boss => bossReward,
            _ => normalReward
        };
    }

    public EncounterType GetCurrentEncounterType()
    {
        return _currentEncounterType;
    }

    public int GetDefeatedEnemyCount()
    {
        int count = 0;
        foreach (Unit enemy in _currentEnemies)
        {
            if (enemy != null && enemy.GetStatValue(StatType.HP) <= 0)
            {
                count++;
            }
        }
        return count;
    }

    public int GetTotalEnemyCount()
    {
        return _currentEnemies.Count;
    }

    public int GetMinRewardForCurrentEncounter()
    {
        return GetRewardRangeForEncounterType(_currentEncounterType).minReward;
    }

    public int GetMaxRewardForCurrentEncounter()
    {
        return GetRewardRangeForEncounterType(_currentEncounterType).maxReward;
    }

    private Vector3 CalculateSpawnPosition(int index, int total)
    {
        if (total <= 1)
        {
            return new Vector3(3, 0, spawnArea.y / 2);
        }

        float lerpValue = Mathf.Clamp01((float)index / (total - 1));
        float xPos = Mathf.Lerp(-spawnArea.x / 2, spawnArea.x / 2, lerpValue);

        Vector3 position = new Vector3(xPos, 0, spawnArea.y / 2);
        if (float.IsNaN(position.x))
        {
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
        _currentEnemies.Clear();

        while (encounterParent.childCount > 0)
        {
            Transform child = encounterParent.GetChild(0);
            child.SetParent(null);
            Destroy(child.gameObject);
        }
    }
}