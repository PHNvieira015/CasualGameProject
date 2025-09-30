using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EndTurnState : State
{
    public override IEnumerator Enter()
    {
        Debug.Log("=== END TURN STATE STARTED ===");

        // Force complete all card animations immediately
        yield return StartCoroutine(ForceCompleteAllCardAnimations());

        // Clear any card selections
        CardDrag.AllowSelection();

        // Small delay to ensure everything settles
        yield return new WaitForSeconds(0.1f);

        Debug.Log("Transitioning to TurnBeginState");
        StartCoroutine(WaitThenChangeState<TurnBeginState>());
    }

    private IEnumerator ForceCompleteAllCardAnimations()
    {
        Debug.Log("Completing all card animations...");

        // Force complete all card movements
        Card[] allCards = FindObjectsOfType<Card>();
        int movingCards = 0;

        foreach (Card card in allCards)
        {
            if (card != null && card.IsMoving())
            {
                movingCards++;
                card.ForceCompleteMove();
            }
        }

        Debug.Log($"Force completed {movingCards} moving cards");

        yield return null;
    }
}