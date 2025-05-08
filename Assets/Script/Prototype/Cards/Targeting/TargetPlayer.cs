using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class TargetPlayer : MonoBehaviour, ITarget
{
public IEnumerator GetTargets(List<object> targets)
    {
        GameObject playerGameObject = GameObject.Find("Units/Player");
        targets.Add(playerGameObject.GetComponentInChildren<Unit>());
        yield return null;
    }

}

