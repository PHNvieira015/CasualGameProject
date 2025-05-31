using NUnit.Framework;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TargetSelf : MonoBehaviour, ITarget
{
    public IEnumerator GetTargets(List<object> targets)
    {
        targets.Add(GetComponentInParent<Card>());
        yield return null;

    }

}
