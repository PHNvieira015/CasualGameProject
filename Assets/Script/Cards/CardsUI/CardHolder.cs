using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class CardHolder : MonoBehaviour
{
    public const float CardMoveDuration = 0.5f;
    public const float CardRotationDuration = 0.5f;

    public int CardRotation;
    public List<Card> Cards;
    public RectTransform Holder;
    public TextMeshProUGUI CardAmount;

    private void Awake()
    {
        Cards = new List<Card>();
        CardAmount.text = string.Format("{0}", Cards.Count);
    }

    public void AddCard(Card card)
    {
        if (card == null)
        {
            Debug.LogError("Attempted to add null card to CardHolder");
            return;
        }

        RectTransform rect = card.transform as RectTransform;
        if (rect == null)
        {
            Debug.LogError("Card does not have RectTransform");
            return;
        }

        if (Holder == null)
        {
            Debug.LogError("Holder is not assigned");
            return;
        }

        rect.anchorMax = Holder.anchorMax;
        rect.anchorMin = Holder.anchorMin;
        rect.pivot = Holder.pivot;

        CardHolder oldHolder = card.GetComponentInParent<CardHolder>();
        rect.SetParent(this.transform);

        if (oldHolder != null)
        {
            card.Rotate(oldHolder.CardRotation - CardRotation, CardRotationDuration);
            card.Move(Holder.anchoredPosition3D, CardMoveDuration, () =>
            {
                Cards.Add(card);
                CardAmount.text = "" + Cards.Count;
                card.transform.SetParent(Holder);
            });
        }
        else
        {
            card.Move(Holder.anchoredPosition3D, CardMoveDuration, () =>
            {
                Cards.Add(card);
                CardAmount.text = "" + Cards.Count;
                card.transform.SetParent(Holder);
            });
        }
    }

    public void RemoveCard(Card card)
    {
        if (card != null)
        {
            Cards.Remove(card);
            CardAmount.text = string.Format("{0}", Cards.Count);
        }
    }

    public void SetInitialRotation()
    {
        foreach (Card card in Cards)
        {
            if (card != null)
            {
                RectTransform rect = card.transform as RectTransform;
                if (rect != null)
                {
                    rect.localRotation = Quaternion.Euler(0, CardRotation, 0);
                }
            }
        }
    }

    public static void ClearAllHolders()
    {
        CardHolder[] allHolders = FindObjectsOfType<CardHolder>();
        foreach (CardHolder holder in allHolders)
        {
            if (holder != null)
            {
                holder.ClearCards();
            }
        }

        GameObject discardPile = GameObject.FindGameObjectWithTag("DiscardPile");
        if (discardPile != null)
        {
            CardHolder discardHolder = discardPile.GetComponent<CardHolder>();
            if (discardHolder != null)
            {
                discardHolder.ClearCards();
            }
        }

        Debug.Log("All card holders cleared including discard pile");
    }

    public void ClearCards()
    {
        foreach (Card card in Cards.ToArray())
        {
            if (card != null && card.gameObject != null)
            {
                Destroy(card.gameObject);
            }
        }

        Cards.Clear();
        CardAmount.text = "0";

        Debug.Log("CardHolder " + gameObject.name + " cleared");
    }
}