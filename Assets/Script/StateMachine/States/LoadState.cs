using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LoadState : State
{
    public override IEnumerator Enter()
    {
        yield return StartCoroutine(InitializeDeck());
        yield return StartCoroutine(InitializeUnits());
        StartCoroutine(WaitThenChangeState<TurnBeginState>());

    }
    IEnumerator InitializeDeck()
    {
        // Clear the draw pile first
        if (CardsController.Instance != null)
        {
            CardsController.Instance.DrawPile.ClearCards();
        }

        // Ensure player deck is initialized
        CardsController.Instance.PlayerDeck.CleanupNullCards();

        // Get all valid cards from player's deck
        List<Card> playerCards = CardsController.Instance.PlayerDeck.GetActiveCards();

        Debug.Log($"Initializing deck with {playerCards.Count} cards");

        // Add each card to the draw pile
        foreach (Card card in playerCards)
        {
            if (card != null && card.gameObject != null)
            {
                // ACTIVATE THE CARD FIRST before adding to draw pile
                card.gameObject.SetActive(true);
                CardsController.Instance.DrawPile.AddCard(card);
                yield return new WaitForSeconds(0.1f);
            }
        }

        yield return new WaitForSeconds(CardHolder.CardMoveDuration);
        CardsController.Instance.DrawPile.SetInitialRotation();
    }
    IEnumerator InitializeUnits()
    {
        machine.Units = new Queue<Unit>();
        foreach (Unit unit in GameObject.Find("Units").GetComponentsInChildren<Unit>())
        {
            machine.Units.Enqueue(unit);
        }
        yield return null;
        //Debug.Log(machine.Units.Count);
    }



}
