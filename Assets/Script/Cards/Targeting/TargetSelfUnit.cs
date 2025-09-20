using NUnit.Framework;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TargetSelfUnit : MonoBehaviour, ITarget
{
    public IEnumerator GetTargets(List<object> targets)
    {
        // Get the Unit component, not the Card component
        Unit unit = GetComponentInParent<Unit>();
        if (unit != null)
        {
            targets.Add(unit);
            Debug.Log($"TargetSelf: Added unit {unit.gameObject.name} to targets");
        }
        else
        {
            Debug.LogWarning("TargetSelf: No Unit component found in parent hierarchy");
        }

        yield return null;
    }
}