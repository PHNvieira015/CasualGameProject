using System.Collections.Generic;
using UnityEngine;

public class TargetAllEnemies : MonoBehaviour, ITarget
{
    public List<object> GetTargets()
    {
        List<object> targets = new List<object>();
        GameObject enemiesGameObject = GameObject.Find("Units/Enemies");
        targets.AddRange(enemiesGameObject.GetComponentsInChildren<Unit>());
        return targets;
    }

}
