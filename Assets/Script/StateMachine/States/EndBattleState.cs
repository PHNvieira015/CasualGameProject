using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EndBattleState : State
{
    [SerializeField] private float _transitionDelay = 1.5f;

    public override IEnumerator Enter()
    {
        yield return null;
        Debug.Log("Battle ended - Preparing for reward screen");

        // Optional: Wait a moment before showing rewards
        yield return new WaitForSeconds(_transitionDelay);

        // Use the StateMachine's method to show reward screen
        StateMachine.Instance.ShowRewardScreen();

        // Optional: Change to a RewardState if you want more control
        // machine.ChangeState<RewardState>();

        yield return null;
    }
}