using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeEnergyEffect : MonoBehaviour, ICardEffect
{
    public int Value;
    public IEnumerator Apply(List<object> targets)
    {
        foreach (object o in targets)
        {
            PlayerUnit player = o as PlayerUnit;
            player.CurrentEnergy += Value;
            yield return null;
        }
    }


}
