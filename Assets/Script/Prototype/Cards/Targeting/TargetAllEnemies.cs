using System.Collections.Generic;
using UnityEngine;
using System.Collections;

public class TargetAllEnemies : MonoBehaviour, ITarget
{
    public IEnumerator GetTargets(List<object> targets)
    {
        GameObject enemiesGameObject = GameObject.Find("Units/EnemyUnits/Enemies");
        targets.AddRange(enemiesGameObject.GetComponentsInChildren<Unit>());
        yield return null;
    }

}
