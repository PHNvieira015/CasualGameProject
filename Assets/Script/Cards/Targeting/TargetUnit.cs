using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TargetUnit : MonoBehaviour, ITarget
{
    Unit _clickedUnit;

    public IEnumerator GetTargets(List<object> targets)
    {
        _clickedUnit = null;

        // Subscribe only to enemies
        foreach (Unit unit in StateMachine.Instance.Units)
        {
            if (unit.CompareTag("Enemy"))
            {
                unit.OnUnitClicked += OnUnitClicked;
            }
        }

        // Wait until an enemy is clicked
        while (_clickedUnit == null)
        {
            yield return null;
        }

        targets.Add(_clickedUnit);

        // Unsubscribe
        foreach (Unit unit in StateMachine.Instance.Units)
        {
            if (unit.CompareTag("Enemy"))
            {
                unit.OnUnitClicked -= OnUnitClicked;
            }
        }
    }

    void OnUnitClicked(Unit unit)
    {
        // Double check it’s an enemy (safety)
        if (unit.CompareTag("Enemy"))
        {
            _clickedUnit = unit;
        }
    }
}
