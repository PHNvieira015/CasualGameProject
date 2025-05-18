using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class InflictEffect : MonoBehaviour, ICardEffect
{
    //public int StatusEffect StatusEffectPrefab;
    public IEnumerator Apply(List<object> targets)
    {
        foreach (Object o in targets)
        {
            Unit unit = o as Unit;

 
            yield return null;
        }
    }

}
