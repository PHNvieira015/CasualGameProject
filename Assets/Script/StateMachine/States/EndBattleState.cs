using UnityEngine;
using System.Collections;

public class EndBattleState : State
{
    [SerializeField] private float _transitionDelay = 1.5f;

    public override IEnumerator Enter()
    {
        yield return null;
        Debug.Log("Battle ended - Preparing for reward screen");

        // Clear ALL card holders (including discard pile)
        ClearAllCardHolders();

        // Optional: Wait a moment before showing rewards
        yield return new WaitForSeconds(_transitionDelay);

        // Use the StateMachine's method to show reward screen
        StateMachine.Instance.ShowRewardScreen();

        yield return null;
    }

    private void ClearAllCardHolders()
    {
        // Try the simplest approach first - just call the static method
        // It will handle its own existence checks internally
        try
        {
            CardHolder.ClearAllHolders();
            return;
        }
        catch (System.Exception e)
        {
            Debug.LogWarning($"Static method failed: {e.Message}. Trying alternative methods...");
        }

        // Fallback: Clear through CardsController if available
        if (CardsController.Instance != null)
        {
            if (CardsController.Instance.DrawPile != null)
                CardsController.Instance.DrawPile.ClearCards();
            if (CardsController.Instance.Hand != null)
                CardsController.Instance.Hand.ClearCards();
            if (CardsController.Instance.DiscardPile != null)
                CardsController.Instance.DiscardPile.ClearCards();
            Debug.Log("All card holders cleared through CardsController");
            return;
        }

        // Final fallback: Find and clear all CardHolder objects
        CardHolder[] allHolders = FindObjectsOfType<CardHolder>();
        foreach (CardHolder holder in allHolders)
        {
            if (holder != null)
            {
                holder.ClearCards();
            }
        }
        Debug.Log("All card holders cleared via FindObjectsOfType");
    }
}