using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class EnemyGroup
{
    public string groupName;
    public List<GameObject> enemyPrefabs;
}

public class EncounterManager : MonoBehaviour
{
    public static EncounterManager Instance { get; private set; }

    [Header("Enemy Groups")]
    public List<EnemyGroup> normalEnemyGroups;
    public List<EnemyGroup> eliteEnemyGroups;
    public List<GameObject> bossEnemies;

    [Header("References")]
    [SerializeField] private Transform encounterParent;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void StartRandomEncounter(EncounterType type)
    {
        ClearEncounter();

        List<GameObject> enemiesToSpawn = new List<GameObject>();

        switch (type)
        {
            case EncounterType.Normal:
                if (normalEnemyGroups.Count > 0)
                {
                    EnemyGroup selectedGroup = normalEnemyGroups[Random.Range(0, normalEnemyGroups.Count)];
                    enemiesToSpawn.AddRange(selectedGroup.enemyPrefabs);
                }
                break;

            case EncounterType.Elite:
                if (eliteEnemyGroups.Count > 0)
                {
                    EnemyGroup selectedGroup = eliteEnemyGroups[Random.Range(0, eliteEnemyGroups.Count)];
                    enemiesToSpawn.AddRange(selectedGroup.enemyPrefabs);
                }
                break;

            case EncounterType.Boss:
                if (bossEnemies.Count > 0)
                {
                    enemiesToSpawn.Add(bossEnemies[Random.Range(0, bossEnemies.Count)]);
                }
                break;
        }

        foreach (var enemyPrefab in enemiesToSpawn)
        {
            Instantiate(enemyPrefab, encounterParent);
        }

        MenuController.Instance.ChangeScreen(MenuController.Screens.CombatMenu);
    }

    private void ClearEncounter()
    {
        foreach (Transform child in encounterParent)
        {
            Destroy(child.gameObject);
        }
    }
}

public enum EncounterType
{
    Normal,
    Elite,
    Boss
}