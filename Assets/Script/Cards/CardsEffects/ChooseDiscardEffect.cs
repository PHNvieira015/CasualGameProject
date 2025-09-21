using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class ChooseDiscardEffectSimple : CardEffect
{
    public int NumberOfCardsToDiscard = 1;

    public override IEnumerator Apply(List<object> targets)
    {
        // Get all cards in hand
        List<Card> handCards = new List<Card>(CardsController.Instance.Hand.Cards);

        if (handCards.Count == 0)
        {
            Debug.Log("No cards in hand to discard");
            yield break;
        }

        // For simplicity, let's discard random cards
        // In a real implementation, you'd show a UI for player selection
        Debug.Log("Choose cards to discard (UI implementation needed)");

        // For now, discard random cards as a placeholder
        System.Random rng = new System.Random();
        var cardsToDiscard = handCards.OrderBy(x => rng.Next()).Take(NumberOfCardsToDiscard).ToList();

        foreach (Card card in cardsToDiscard)
        {
            CardsController.Instance.Discard(card);
            yield return new WaitForSeconds(0.1f);
        }

        Debug.Log($"Discarded {cardsToDiscard.Count} cards");
    }
}