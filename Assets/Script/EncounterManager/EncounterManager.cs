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

    [Header("Reward Button")]
    [SerializeField] private MoneyRewardButton moneyRewardButton;

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
            return Random.Range(minReward, maxReward + 1); // +1 because Random.Range for int is exclusive max
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

            // Auto-find if not set
            if (moneyRewardButton == null)
                moneyRewardButton = FindObjectOfType<MoneyRewardButton>();
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
            Debug.LogWarning($"No valid group found for {type} encounter");
            return;
        }

        SpawnEnemies(selectedGroup);
        MenuController.Instance.ChangeScreen(MenuController.Screens.CombatMenu);
    }

    private void SpawnEnemies(EnemyGroup group)
    {
        if (group == null || group.enemyPrefabs.Count == 0) return;

        _currentEnemies.Clear();

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

            // Add to current enemies list for tracking
            Unit enemyUnit = enemyInstance.GetComponent<Unit>();
            if (enemyUnit != null)
            {
                _currentEnemies.Add(enemyUnit);
            }
        }
    }

    // Calculate reward for current encounter using only the predefined ranges
    public int CalculateBattleReward()
    {
        // Get base reward from encounter type range
        int baseReward = GetRandomRewardForEncounterType(_currentEncounterType);

        // Apply battle modifiers (completion bonus/penalty)
        int finalReward = ApplyBattleModifiers(baseReward);

        return Mathf.Max(0, finalReward); // Ensure non-negative
    }

    private int GetRandomRewardForEncounterType(EncounterType type)
    {
        return type switch
        {
            EncounterType.Normal => normalReward.GetRandomReward(),
            EncounterType.Elite => eliteReward.GetRandomReward(),
            EncounterType.Boss => bossReward.GetRandomReward(),
            _ => normalReward.GetRandomReward()
        };
    }

    // Get the reward range for UI display
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

    // Get minimum and maximum possible rewards
    public int GetMinRewardForCurrentEncounter()
    {
        return GetRewardRangeForEncounterType(_currentEncounterType).minReward;
    }

    public int GetMaxRewardForCurrentEncounter()
    {
        return GetRewardRangeForEncounterType(_currentEncounterType).maxReward;
    }

    private int ApplyBattleModifiers(int baseReward)
    {
        int defeatedCount = GetDefeatedEnemyCount();
        int totalEnemies = GetTotalEnemyCount();

        // Bonus for defeating all enemies
        if (defeatedCount == totalEnemies && totalEnemies > 1)
        {
            baseReward = Mathf.RoundToInt(baseReward * 1.2f); // 20% bonus for full clear
            Debug.Log($"Full clear bonus applied: {baseReward}");
        }

        // Penalty if not all enemies defeated (optional)
        else if (defeatedCount < totalEnemies)
        {
            float completionRatio = (float)defeatedCount / totalEnemies;
            baseReward = Mathf.RoundToInt(baseReward * completionRatio);
            Debug.Log($"Partial completion penalty applied: {baseReward} ({(completionRatio * 100):F0}% of reward)");
        }

        return baseReward;
    }

    // Get current encounter type for UI display
    public EncounterType GetCurrentEncounterType()
    {
        return _currentEncounterType;
    }

    // Get enemy count for UI display and completion calculation
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
        _currentEnemies.Clear();

        // Safe destruction - unparent first
        while (encounterParent.childCount > 0)
        {
            Transform child = encounterParent.GetChild(0);
            child.SetParent(null);
            Destroy(child.gameObject);
        }
    }
}