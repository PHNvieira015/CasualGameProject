using UnityEngine;
using System.Collections;
using System.Linq;

public class BattleStateChecker : MonoBehaviour
{
    private void OnEnable()
    {
        // Subscribe to unit death events
        Unit.OnAnyUnitDeath += HandleUnitDeath;
    }

    private void OnDisable()
    {
        // Unsubscribe to prevent memory leaks
        Unit.OnAnyUnitDeath -= HandleUnitDeath;
    }

    private void HandleUnitDeath(Unit deadUnit)
    {
        // If an enemy died, mark it as dead in its EnemyAI2 component
        EnemyAI2 enemyAI = deadUnit.GetComponent<EnemyAI2>();
        if (enemyAI != null)
        {
            enemyAI.MarkAsDead();
        }

        // Check battle conditions when any unit dies
        CheckBattleConditions();
    }

    private void CheckBattleConditions()
    {
        // Check for player defeat first
        if (IsPlayerDefeated())
        {
            Debug.Log("Player defeated! Game Over!");
            StateMachine.Instance.ChangeState<GameOverState>();
            return;
        }

        // Check for player victory
        if (AreAllEnemiesDefeated())
        {
            Debug.Log("All enemies defeated! Victory!");
            StateMachine.Instance.ChangeState<EndBattleState>();
            return;
        }
    }

    private bool IsPlayerDefeated()
    {
        PlayerUnit player = FindObjectOfType<PlayerUnit>();
        return player == null || player.GetStatValue(StatType.HP) <= 0;
    }

    private bool AreAllEnemiesDefeated()
    {
        // Find all EnemyAI2 components and check if they're all dead
        EnemyAI2[] enemies = FindObjectsOfType<EnemyAI2>();
        return enemies.Length == 0 || enemies.All(enemy => enemy.IsDead);
    }

    // Optional: Backup checks that can be called from other states
    public static bool CheckVictory()
    {
        EnemyAI2[] enemies = FindObjectsOfType<EnemyAI2>();
        if (enemies.Length == 0 || enemies.All(enemy => enemy.IsDead))
        {
            StateMachine.Instance.ChangeState<EndBattleState>();
            return true;
        }
        return false;
    }

    public static bool CheckDefeat()
    {
        PlayerUnit player = FindObjectOfType<PlayerUnit>();
        if (player == null || player.GetStatValue(StatType.HP) <= 0)
        {
            StateMachine.Instance.ChangeState<GameOverState>();
            return true;
        }
        return false;
    }
}