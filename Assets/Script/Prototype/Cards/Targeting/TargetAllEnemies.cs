using System.Collections.Generic;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TargetAllEnemies : MonoBehaviour, ITarget
{
    public IEnumerator GetTargets(List<object> targets)
    {
        GameObject enemiesGameObject = GameObject.Find("Units/Enemies");
        targets.AddRange(enemiesGameObject.GetComponentsInChildren<Unit>());
        yield return null;
    }

}
