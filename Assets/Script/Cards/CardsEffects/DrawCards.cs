using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrawCards : CardEffect
{
    public int drawAmount = 1;
    public float delayBetweenDraws = 0.2f;
    public bool shuffleIfEmpty = true;

    public override IEnumerator Apply(List<object> targets)
    {
        if (CardsController.Instance == null)
        {
            Debug.LogError("CardsController instance not found!");
            yield break;
        }

        yield return StartCoroutine(DrawCardsRoutine(drawAmount));
    }

    private IEnumerator DrawCardsRoutine(int amount)
    {
        int cardsDrawn = 0;

        while (cardsDrawn < amount)
        {
            if (CardsController.Instance.DrawPile.Cards.Count == 0 && shuffleIfEmpty)
            {
                Debug.Log("Draw pile empty, shuffling discard pile...");
                yield return StartCoroutine(CardsController.Instance.ShuffleDiscardintoDrawPile());

                if (CardsController.Instance.DrawPile.Cards.Count == 0)
                {
                    Debug.Log("No cards available to draw after shuffling");
                    yield break;
                }
            }
            else if (CardsController.Instance.DrawPile.Cards.Count == 0 && !shuffleIfEmpty)
            {
                Debug.Log("Draw pile empty and shuffling disabled");
                yield break;
            }

            // Draw the top card
            Card card = CardsController.Instance.DrawPile.Cards[CardsController.Instance.DrawPile.Cards.Count - 1];

            // Remove from draw pile FIRST
            CardsController.Instance.DrawPile.RemoveCard(card);

            // Add to hand - the AddCard method should handle the rotation automatically
            CardsController.Instance.Hand.AddCard(card);

            cardsDrawn++;
            yield return new WaitForSeconds(delayBetweenDraws);
        }
    }
}