using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageEffect : MonoBehaviour, ICardEffect
{
    public int Amount;
    public IEnumerator Apply(List<object> targets)
    {
        foreach (Object o in targets) 
        {
            Unit unit = o as Unit;

        Debug.LogFormat("Unit [0] Took {1} Damage",unit.name, Amount);
            yield return null;
        }
    }

}
