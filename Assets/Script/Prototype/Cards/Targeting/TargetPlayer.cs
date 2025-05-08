using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class TargetPlayer : MonoBehaviour
{
public List<object> GetTargets()
    {
        List<object> targets = new List<object>();
        GameObject playerGameObject = GameObject.Find("Units/Player");
        targets.Add(playerGameObject.GetComponentInChildren<Unit>());
        return targets;
    }

}

