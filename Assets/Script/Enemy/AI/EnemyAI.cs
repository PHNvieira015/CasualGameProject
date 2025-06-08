using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Unit))]
public class EnemyAI : MonoBehaviour
{
    [System.Serializable]
    public class ActionSequence
    {
        public Card actionCard;
        public int weight = 1;
    }

    [Header("Action Settings")]
    [SerializeField] private List<ActionSequence> possibleActions;
    [SerializeField] private int actionsPerTurn = 1;

    private Unit unit;
    private Unit playerUnit;
    private List<Card> actionHistory = new List<Card>();

    private void Awake()
    {
        unit = GetComponent<Unit>();
    }

    public void Initialize(Unit player)
    {
        playerUnit = player;
    }

    public IEnumerator TakeTurn()
    {
        if (possibleActions.Count == 0)
        {
            Debug.LogWarning("No actions defined for enemy!");
            yield break;
        }

        for (int i = 0; i < actionsPerTurn; i++)
        {
            Card selectedCard = SelectActionCard();
            if (selectedCard != null)
            {
                yield return ExecuteCardAction(selectedCard);
                actionHistory.Add(selectedCard);
            }
        }

        yield return unit.Recover();
    }

    private Card SelectActionCard()
    {
        // Simple weighted random selection
        int totalWeight = 0;
        foreach (var action in possibleActions)
        {
            totalWeight += action.weight;
        }

        int randomValue = Random.Range(0, totalWeight);
        int currentWeight = 0;

        foreach (var action in possibleActions)
        {
            currentWeight += action.weight;
            if (randomValue < currentWeight)
            {
                return action.actionCard;
            }
        }

        return possibleActions[0].actionCard; // Fallback
    }

    private IEnumerator ExecuteCardAction(Card card)
    {
        List<object> targets = new List<object> { playerUnit };

        foreach (CardEffect effect in card.GetComponents<CardEffect>())
        {
            yield return effect.Apply(targets);
        }
    }
}