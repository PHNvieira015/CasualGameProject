using UnityEngine;
using System.Collections;

public class EnemyAI2 : MonoBehaviour
{
    // Add this property to track if this enemy is dead
    public bool IsDead { get; private set; } = false;

    [Header("References")]
    [SerializeField] private EnemyActionHolder _actionHolder;

    [Header("Timing Settings")]
    [SerializeField] private float _turnStartDelay = 0.2f; // Delay before starting turn
    [SerializeField] private float _postActionDelay = 0.3f; // Delay after action completes

    [Header("Debug")]
    [SerializeField] private string _currentUnitDebug;
    [SerializeField] private bool _isMyTurnDebug;
    [SerializeField] private bool _actionCompletedDebug;

    private bool _isActionInProgress = false;
    private bool _turnProcessed = false;

    private void Update()
    {
        UpdateDebugInfo();

        if (IsMyTurn() && !_isActionInProgress && !_turnProcessed)
        {
            StartCoroutine(HandleEnemyTurn());
        }
        else if (!IsMyTurn())
        {
            _turnProcessed = false; // Reset for next turn
        }
    }

    private IEnumerator HandleEnemyTurn()
    {
        _isActionInProgress = true;
        _turnProcessed = true;

        // Wait before starting turn (helps with turn sequencing)
        yield return new WaitForSeconds(_turnStartDelay);

        yield return StartCoroutine(ExecuteEnemyAction());

        _isActionInProgress = false;
    }

    private IEnumerator ExecuteEnemyAction()
    {
        Debug.Log($"<color=cyan>[Enemy] Starting turn: {name}</color>");

        Card card = _actionHolder.GetCurrentAction();
        if (card == null)
        {
            Debug.LogWarning("[Enemy] No card available");
            yield break;
        }

        // Play card
        StateMachine.Instance.CardsdToPlay.Enqueue(card);
        Debug.Log($"[Enemy] Playing card: {card.name}");

        // Wait for completion
        yield return new WaitUntil(() => !StateMachine.Instance.CardsdToPlay.Contains(card));

        // Post-action sequence
        yield return new WaitForSeconds(_postActionDelay);
        _actionHolder.RotateCurrentAction();

        // End turn
        if (IsMyTurn())
        {
            StateMachine.Instance.ChangeState<EndTurnState>();
            Debug.Log("<color=green>[Enemy] Turn completed</color>");
        }
    }

    private bool IsMyTurn()
    {
        return StateMachine.Instance != null &&
               StateMachine.Instance.CurrentUnit != null &&
               StateMachine.Instance.CurrentUnit.gameObject == this.gameObject &&
               !IsDead; // Don't take turns if dead
    }

    private void UpdateDebugInfo()
    {
        _currentUnitDebug = StateMachine.Instance?.CurrentUnit?.name ?? "null";
        _isMyTurnDebug = IsMyTurn();
        _actionCompletedDebug = !_isActionInProgress;
    }

    // Add this method to mark the enemy as dead
    public void MarkAsDead()
    {
        IsDead = true;
        // Disable the AI so it doesn't try to take turns
        this.enabled = false;
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        if (_actionHolder == null)
            _actionHolder = GetComponentInChildren<EnemyActionHolder>();
    }
#endif
}