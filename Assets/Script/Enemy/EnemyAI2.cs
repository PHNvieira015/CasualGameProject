using UnityEngine;

public class EnemyAI2 : MonoBehaviour
{
    private EnemyActionHolder actionHolder;
    private bool isPerformingAction = false;

    void Awake()
    {
        actionHolder = GetComponentInChildren<EnemyActionHolder>(true);
        if (actionHolder == null)
        {
            Debug.LogError($"EnemyAI: Could not find EnemyActionHolder in children of {gameObject.name}");
            enabled = false;
        }
    }

    public void PerformEnemyTurn()
    {
        if (isPerformingAction) return;

        if (actionHolder == null)
        {
            Debug.LogWarning("EnemyAI: Cannot perform turn - no action holder available");
            return;
        }

        if (actionHolder.Actions.Count == 0)
        {
            Debug.LogWarning("EnemyAI: No actions available to perform");
            return;
        }

        isPerformingAction = true;

        // Get and rotate the action
        Card currentActionCard = actionHolder.GetCurrentActionAndRotate();

        if (currentActionCard != null)
        {
            Debug.Log($"Enemy performing action: {currentActionCard.name}");
            // Execute the action here

            // You might want to start a coroutine for the action animation/effects
            StartCoroutine(ExecuteAction(currentActionCard));
        }
    }

    private IEnumerator ExecuteAction(Card action)
    {
        // Implement your action logic here
        Debug.Log($"Executing action: {action.name}");

        // Example: Wait for animation to complete
        yield return new WaitForSeconds(0.5f);

        // Action complete
        isPerformingAction = false;
    }
}