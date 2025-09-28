using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class TargetUnit : MonoBehaviour, ITarget
{
    Unit _clickedUnit;
    private List<Unit> _subscribedUnits = new List<Unit>();

    public IEnumerator GetTargets(List<object> targets)
    {
        _clickedUnit = null;
        _subscribedUnits.Clear();

        // Subscribe only to enemies and track them separately
        foreach (Unit unit in StateMachine.Instance.Units)
        {
            if (unit != null && unit.gameObject != null && unit.CompareTag("Enemy"))
            {
                unit.OnUnitClicked += OnUnitClicked;
                _subscribedUnits.Add(unit);
            }
        }

        // Wait until an enemy is clicked
        while (_clickedUnit == null)
        {
            // Clean up any destroyed units from our subscription list
            _subscribedUnits.RemoveAll(u => u == null || u.gameObject == null);
            yield return null;
        }

        // Defensive: Only add if not destroyed
        if (_clickedUnit != null && _clickedUnit.gameObject != null)
            targets.Add(_clickedUnit);

        // Unsubscribe using our tracked list
        UnsubscribeFromTracked();
    }

    void OnUnitClicked(Unit unit)
    {
        // Double check it's an enemy (safety)
        if (unit.CompareTag("Enemy"))
        {
            _clickedUnit = unit;
        }
    }

    private void UnsubscribeFromTracked()
    {
        foreach (Unit unit in _subscribedUnits)
        {
            if (unit != null && unit.gameObject != null)
            {
                unit.OnUnitClicked -= OnUnitClicked;
            }
        }
        _subscribedUnits.Clear();
    }
}