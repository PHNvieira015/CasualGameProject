using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
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
        Cards = new List<Card>(GetComponentsInChildren<Card>());
        CardAmount.text = string.Format("{0}", Cards.Count);
        SetInitialRotation();
    }
    public void AddCard(Card card)
    {
        RectTransform rect = card.transform as RectTransform;

        rect.anchorMax = Holder.anchorMax;
        rect.anchorMin = Holder.anchorMin;
        rect.pivot = Holder.pivot;

        CardHolder oldHolder = card.GetComponentInParent<CardHolder>(); // Changed to GetComponentInParent
        rect.SetParent(this.transform);

        //Card.Move
        // Only rotate if the card came from another holder
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
    }

    public void RemoveCard(Card card)
    {
        Cards.Remove(card);
        CardAmount.text = string.Format("{0}", Cards.Count);
    }
    void SetInitialRotation()
    {
        foreach (Card card in Cards)
        {
            RectTransform rect = card.transform as RectTransform;
            rect.localRotation = Quaternion.Euler(0, CardRotation, 0);
        }
    }

}
