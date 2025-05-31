using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TurnBeginState : State
{
    PlayerUnit _playerUnit;

    public override IEnumerator Enter()
    {
        machine.CurrentUnit = null;

        // Find the first alive unit in the queue
        Unit aliveUnit = null;
        int unitsChecked = 0;

        while (machine.Units.Count > 0 && unitsChecked < machine.Units.Count)
        {
            Unit current = machine.Units.Dequeue();
            unitsChecked++;

            if (current == null) continue;

            if (current.GetStatValue(StatType.HP) > 0)
            {
                aliveUnit = current;
                machine.Units.Enqueue(current);
                break;
            }
            else
            {
                Debug.LogFormat("Unit {0} is dead", current.name);
                machine.Units.Enqueue(current); // Keep dead units in queue if needed
            }
        }

        machine.CurrentUnit = aliveUnit;

        // Try to get player unit reference if we don't have one
        if (_playerUnit == null && machine.CurrentUnit != null)
        {
            _playerUnit = machine.CurrentUnit as PlayerUnit;
        }

        yield return null;

        // Check battle end conditions
        if (machine.Units.Count == 1 ||
          (_playerUnit != null && _playerUnit.GetStatValue(StatType.HP) <= 0))
        {
            StartCoroutine(WaitThenChangeState<EndBattleState>());
        }
        else if (machine.CurrentUnit != null)
        {
            StartCoroutine(WaitThenChangeState<RecoveryState>());
        }
        else
        {
            Debug.LogError("No valid units found in battle!");
            StartCoroutine(WaitThenChangeState<EndBattleState>());
        }
    }
}