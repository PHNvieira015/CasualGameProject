using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class RewardState : State
{
    public override IEnumerator Enter()
    {
        yield return null;
        Debug.Log("Reward State entered");

        // Ensure reward screen is visible
        StateMachine.Instance.ShowRewardScreen();

        // Initialize reward logic here
        InitializeRewards();

        yield return null;
    }

    private void InitializeRewards()
    {
        // Add your reward generation and display logic here
        Debug.Log("Initializing rewards...");

        // Example: Generate gold, items, cards, etc.
        // RewardManager.GenerateRewards();
    }

    public override IEnumerator Exit()
    {
        // Hide reward screen when leaving this state
        StateMachine.Instance.HideRewardScreen();
        yield return null;
    }
}