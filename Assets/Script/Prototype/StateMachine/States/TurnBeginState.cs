using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TurnBeginState : State
{
    public override IEnumerator Enter()
    {
        machine.CurrentUnit = machine.units.Dequeue();
        machine.units.Enqueue(machine.CurrentUnit);

        yield return null;
        StartCoroutine(WaitThenChangeState<RecoveryState>());

    }


}