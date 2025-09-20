using NUnit.Framework;
using UnityEngine;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

public class CardsController : MonoBehaviour
{
    #region Fields/Properties

    public static CardsController Instance;

    public CardHolder Hand;

    public CardHolder DrawPile;

    public CardHolder DiscardPile;

    public CardsList PlayerDeck;

    public CardsList PlayerRelics;

    public CardHolder RelicHolder;

    #endregion

    private void Awake()
    {
        Instance = this;

    }

    #region Card Controls
    public IEnumerator Draw(int amount = 1)
    {
        while (amount>0)
        {
            if (DrawPile.Cards.Count == 0)
            {
             yield return   StartCoroutine(ShuffleDiscardintoDrawPile());
                yield return new WaitForSeconds(CardHolder.CardMoveDuration);
                if (DrawPile.Cards.Count == 0)
                {
                    Debug.Log("No cards to draw");
                    yield break;
                } 
            }
            Card card = DrawPile.Cards[DrawPile.Cards.Count - 1];
            DrawPile.RemoveCard(card);
            Hand.AddCard(card);
            amount--;
            yield return new WaitForSeconds(0.1f);
        }
    }
    public void Discard(Card card)
    {
        Hand.RemoveCard(card);
        DrawPile.RemoveCard(card);
        DiscardPile.AddCard(card);
    }

    public IEnumerator ShuffleDiscardintoDrawPile()
    {
        //Randomize Cards
        List<Card> cards = DiscardPile.Cards;

        System.Random rand = new System.Random();

        List<Card> shuffledDiscardIntoDeck = new List<Card>(cards.OrderBy(x => rand.Next()).ToList());

        //Put into discard
        foreach (Card card in shuffledDiscardIntoDeck)
        {
            DiscardPile.RemoveCard(card);
            DrawPile.AddCard(card);
            yield return new WaitForSeconds(0.1f);
        }
    }
    #endregion
}
