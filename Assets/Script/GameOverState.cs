using UnityEngine;
using System.Collections;

public class GameOverState : State
{
    [SerializeField] private float _transitionDelay = 1f;

    public override IEnumerator Enter()
    {
        yield return null;
        Debug.Log("Game Over state entered");
        
        // Clear all cards
        ClearAllCardHolders();
        
        // Optional delay for dramatic effect
        yield return new WaitForSeconds(_transitionDelay);
        
        // Show game over screen
        MenuController.Instance.ChangeScreen(MenuController.Screens.Defeat);
    }
    
    private void ClearAllCardHolders()
    {
        // Use the same clear logic as EndBattleState
        try
        {
            CardHolder.ClearAllHolders();
            return;
        }
        catch (System.Exception e)
        {
            Debug.LogWarning($"Static method failed: {e.Message}");
        }

        // Fallback methods
        if (CardsController.Instance != null)
        {
            if (CardsController.Instance.DrawPile != null)
                CardsController.Instance.DrawPile.ClearCards();
            if (CardsController.Instance.Hand != null)
                CardsController.Instance.Hand.ClearCards();
            if (CardsController.Instance.DiscardPile != null)
                CardsController.Instance.DiscardPile.ClearCards();
        }
    }
}