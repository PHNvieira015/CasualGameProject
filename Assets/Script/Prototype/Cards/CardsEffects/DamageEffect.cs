using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageEffect : MonoBehaviour, ICardEffect
{
    public int Amount;
    public void Apply()
    {
        List<object> targets = GetComponent<ITarget>().GetTargets();
        foreach (Object o in targets)
        {
            Unit unit = o as Unit;
            
            
            //unit.TakeDamage(Amount);


        Debug.LogFormat("Unit [0] Took {1} Damage",unit.name, Amount);


        }
    }

}
