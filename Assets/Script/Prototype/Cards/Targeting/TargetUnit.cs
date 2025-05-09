using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;


public class TargetUnit : MonoBehaviour, ITarget
{
    Unit _clickedUnit;
    
    public IEnumerator GetTargets(List<object> targets)
    {
        _clickedUnit = null;
        foreach(Unit unit in StateMachine.Instance.units)
        {
            unit.OnUnitClicked += OnUnitClicked;
        }

        while (_clickedUnit == null)
        {
            yield return null;
        }
        targets.Add(_clickedUnit);

        foreach (Unit unit in StateMachine.Instance.units)
        {
            unit.OnUnitClicked -= OnUnitClicked;
        }

    }
    void OnUnitClicked(Unit unit)
    {
        _clickedUnit = unit;
    }

}
