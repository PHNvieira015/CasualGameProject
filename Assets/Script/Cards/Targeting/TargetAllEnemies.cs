using System.Collections.Generic;
using UnityEngine;
using System.Collections;

public class TargetAllEnemies : MonoBehaviour, ITarget
{
    public IEnumerator GetTargets(List<object> targets)
    {
        GameObject enemiesGameObject = GameObject.Find("Units/Encounter");
        foreach (Unit unit in enemiesGameObject.GetComponentsInChildren<Unit>())
        {
            if (unit.GetStatValue(StatType.HP) > 0)
            {
                targets.Add(unit);
            }
        }
        yield return null;
    }

}
