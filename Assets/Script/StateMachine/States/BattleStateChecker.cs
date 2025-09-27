using UnityEngine;
using System.Linq;
using System.Collections.Generic;

public class BattleStateChecker : MonoBehaviour
{
    private HashSet<EnemyAI2> deadEnemies = new HashSet<EnemyAI2>();

    private void OnEnable()
    {
        Unit.OnAnyUnitDeath += HandleUnitDeath;
    }

    private void OnDisable()
    {
        Unit.OnAnyUnitDeath -= HandleUnitDeath;
    }

    private void HandleUnitDeath(Unit deadUnit)
    {
        // Check if the dead unit is a boss
        Boss bossComponent = deadUnit.GetComponent<Boss>();
        if (bossComponent != null)
        {
            Debug.Log("Boss defeated! Handling boss death...");
            bossComponent.HandleBossDeath();
            return;
        }

        // For regular enemies, track them in our own list
        EnemyAI2 enemyAI = deadUnit.GetComponent<EnemyAI2>();
        if (enemyAI != null && enemyAI.GetComponent<Boss>() == null)
        {
            deadEnemies.Add(enemyAI);
            Debug.Log($"Enemy tracked as dead: {enemyAI.name}");
        }

        CheckBattleConditions();
    }

    private void CheckBattleConditions()
    {
        if (IsPlayerDefeated())
        {
            Debug.Log("Player defeated! Game Over!");
            StateMachine.Instance.ChangeState<GameOverState>();
            return;
        }

        if (AreAllEnemiesDefeated())
        {
            Debug.Log("All enemies defeated! Victory!");
            StateMachine.Instance.ChangeState<EndBattleState>();
            deadEnemies.Clear(); // Reset for next battle
            return;
        }

        Debug.Log("Battle continues... enemies remaining: " + GetRemainingEnemyCount());
    }

    private bool IsPlayerDefeated()
    {
        PlayerUnit player = FindObjectOfType<PlayerUnit>();
        return player == null || player.GetStatValue(StatType.HP) <= 0;
    }

    private bool AreAllEnemiesDefeated()
    {
        // Get all non-boss enemies in the scene
        EnemyAI2[] allEnemies = FindObjectsOfType<EnemyAI2>()
            .Where(enemy => enemy.GetComponent<Boss>() == null)
            .ToArray();

        // Check if all enemies are in our dead list
        return allEnemies.Length == 0 || allEnemies.All(enemy => deadEnemies.Contains(enemy));
    }

    private int GetRemainingEnemyCount()
    {
        EnemyAI2[] allEnemies = FindObjectsOfType<EnemyAI2>()
            .Where(enemy => enemy.GetComponent<Boss>() == null)
            .ToArray();

        return allEnemies.Count(enemy => !deadEnemies.Contains(enemy));
    }

    public void ManualBattleCheck()
    {
        Debug.Log("Manual battle check from End Turn");
        CheckBattleConditions();
    }
}