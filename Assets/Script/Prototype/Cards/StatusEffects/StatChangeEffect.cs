using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatChangeEffect : StatusEffect
{
    public StatType Type;
    public override void OnInflicted()
    {
        int currentValue = _host.GetStatValue(Type);
        _host.SetStatValue(Type, currentValue + Amount);
    }



    public override void OnRemoved()
    {
    int currentValue = _host.GetStatValue(Type);
    _host.SetStatValue(Type, currentValue - Amount);
    }
}
