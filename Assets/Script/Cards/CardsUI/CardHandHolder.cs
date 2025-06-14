using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Splines;
using Unity.Mathematics;
using DG.Tweening; // Add this for DOMove

public class CardHandHolder : MonoBehaviour
{
    public const float CardMoveDuration = 0.5f;
    public const float CardRotationDuration = 0.5f;

    public int CardRotation;
    public List<Card> Cards;
    public RectTransform Holder;
    public TextMeshProUGUI CardAmount;

    [SerializeField] private SplineContainer splineContainer;

    private void Awake()
    {
        Cards = new List<Card>();
        CardAmount.text = string.Format("{0}", Cards.Count);
    }

    public void AddCard(Card card)
    {
        RectTransform rect = card.transform as RectTransform;

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

    private IEnumerator UpdateCardPosition(float duration)
    {
        if (Cards.Count == 0) yield break;

        float cardSpacing = 1f / 10f;
        float firstCardPosition = 0.5f - (Cards.Count - 1) * cardSpacing / 2;
        Spline spline = splineContainer.Spline;

        for (int i = 0; i < Cards.Count; i++)
        {
            float p = firstCardPosition + i * cardSpacing;
            Vector3 splinePosition = spline.EvaluatePosition(p);
            Vector3 forward = spline.EvaluateTangent(p);
            Vector3 up = spline.EvaluateUpVector(p);
            Quaternion rotation = Quaternion.LookRotation(-up, Vector3.Cross(-up, forward).normalized);

            // Use DOTween for DOMove
            Cards[i].transform.DOMove(splinePosition + transform.position + 0.01f * i * Vector3.back, duration);
            Cards[i].transform.DORotate(rotation.eulerAngles, duration);
        }

        yield return new WaitForSeconds(duration);
    }
}