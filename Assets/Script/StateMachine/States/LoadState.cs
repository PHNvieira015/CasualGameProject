using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq; // Add this for LINQ functionality

public class LoadState : State
{
    public override IEnumerator Enter()
    {
        yield return StartCoroutine(InitializeDeck());
        yield return StartCoroutine(InitializeUnits());
        yield return StartCoroutine(InitializeRelics());
        StartCoroutine(WaitThenChangeState<TurnBeginState>());
    }

    IEnumerator InitializeDeck()
    {
        // Create a list to store all instantiated cards
        List<Card> instantiatedCards = new List<Card>();

        foreach (Card card in CardsController.Instance.PlayerDeck.Cards)
        {
            Card newCard = Instantiate(card, Vector3.zero, Quaternion.identity, CardsController.Instance.DrawPile.Holder);
            instantiatedCards.Add(newCard);
        }

        // Shuffle the instantiated cards
        instantiatedCards = ShuffleCards(instantiatedCards);

        // Add shuffled cards to draw pile
        foreach (Card card in instantiatedCards)
        {
            CardsController.Instance.DrawPile.AddCard(card);
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
    }

    IEnumerator InitializeRelics()
    {
        foreach (Card card in CardsController.Instance.PlayerRelics.Cards)
        {
            Card newCard = Instantiate(card, Vector3.zero, Quaternion.identity, CardsController.Instance.RelicHolder.Holder);
            CardsController.Instance.RelicHolder.AddCard(newCard);
        }
        yield return new WaitForSeconds(CardHolder.CardMoveDuration);
        CardsController.Instance.RelicHolder.SetInitialRotation();
    }

    // Fisher-Yates shuffle algorithm for proper randomization
    private List<Card> ShuffleCards(List<Card> cards)
    {
        System.Random rng = new System.Random();
        int n = cards.Count;

        while (n > 1)
        {
            n--;
            int k = rng.Next(n + 1);
            Card value = cards[k];
            cards[k] = cards[n];
            cards[n] = value;
        }

        return cards;
    }

    // Alternative using LINQ (simpler but less control)
    private List<Card> ShuffleCardsLINQ(List<Card> cards)
    {
        System.Random rng = new System.Random();
        return cards.OrderBy(card => rng.Next()).ToList();
    }
}