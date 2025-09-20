using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyCard : CardEffect
{
    public override IEnumerator Apply(List<object> targets)
    {
        foreach (object target in targets)
        {
            Card card = target as Card;
            if (card != null)
            {
                // Remove card from its current holder
                CardHolder currentHolder = card.GetComponentInParent<CardHolder>();
                if (currentHolder != null)
                {
                    currentHolder.RemoveCard(card);
                }

                // Optional: Add any destruction animation/effects here
                // For example, you could play a fade-out animation

                yield return new WaitForSeconds(0.1f); // Small delay between card destructions

                // Destroy the card game object
                Destroy(card.gameObject);
            }
        }
    }
}