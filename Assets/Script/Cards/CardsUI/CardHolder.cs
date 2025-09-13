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
        Cards = new List<Card>();
        CardAmount.text = string.Format("{0}", Cards.Count);
    }
    public void AddCard(Card card)
    {
        if (card == null) return;

        RectTransform rect = card.transform as RectTransform;

        // Match anchors/pivots with Holder
        rect.anchorMax = Holder.anchorMax;
        rect.anchorMin = Holder.anchorMin;
        rect.pivot = Holder.pivot;

        // Find the previous CardHolder (if any)
        CardHolder oldHolder = card.GetComponentInParent<CardHolder>();

        //  Always re-parent immediately to Holder
        rect.SetParent(Holder, worldPositionStays: false);

        // Play animation if moving from another holder
        if (oldHolder != null && oldHolder != this)
        {
            card.Rotate(oldHolder.CardRotation - CardRotation, CardRotationDuration);
            card.Move(Vector3.zero, CardMoveDuration, () =>
            {
                Cards.Add(card);
                CardAmount.text = Cards.Count.ToString();
            });
        }
        else
        {
            // No animation case  snap into place
            rect.anchoredPosition3D = Vector3.zero;
            Cards.Add(card);
            CardAmount.text = Cards.Count.ToString();
        }
    }


    public void RemoveCard(Card card)
    {
        Cards.Remove(card);
        CardAmount.text = string.Format("{0}", Cards.Count);
    }
    public void SetInitialRotation()
    {
        foreach (Card card in Cards)
        {
            RectTransform rect = card.transform as RectTransform;
            rect.localRotation = Quaternion.Euler(0, CardRotation, 0);
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
        Debug.Log("All card holders cleared");
    }
    public void ClearCards()
    {
        // Destroy all card game objects
        foreach (Card card in Cards.ToArray()) // Use ToArray to avoid modification during iteration
        {
            if (card != null && card.gameObject != null)
            {
                Destroy(card.gameObject);
            }
        }

        // Clear the list
        Cards.Clear();
        CardAmount.text = "0";

        Debug.Log($"CardHolder {gameObject.name} cleared");
    }
}
