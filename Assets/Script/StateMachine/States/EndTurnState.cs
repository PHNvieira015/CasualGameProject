using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EndTurnState : State
{
    public override IEnumerator Enter()
    {
        yield return null;
        StartCoroutine(WaitThenChangeState<TurnBeginState>());
    }

}
