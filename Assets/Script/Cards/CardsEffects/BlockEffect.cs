using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockEffect : CardEffect
{
    public int Amount;
    public override IEnumerator Apply(List<object> targets)
    {
        foreach (object o in targets)
        {
            Unit unit = o as Unit;
            if (unit == null) continue;

            ModifiedValues modifiedValues = new ModifiedValues(Amount);
            ApplyModifier(modifiedValues, ModifierTags.GainBlock, StateMachine.Instance.CurrentUnit);
            Debug.LogFormat("Unit {0} gain {1} block", unit.name, modifiedValues.FinalValue);

            int currentBlock = unit.GetStatValue(StatType.Block);
            unit.SetStatValue(StatType.Block, currentBlock+modifiedValues.FinalValue);
            
            yield return null;
        }
    }
    void ApplyModifier(ModifiedValues modifiedValues, ModifierTags tag, Unit unit)
    {
        TagModifier modifier = unit.Modify[(int)tag];
        if (modifier != null)
        {
            modifier(modifiedValues);
            Debug.LogFormat("Base Value:{0}, Modified Value: {1}", modifiedValues.BaseValue, modifiedValues.FinalValue);
        }

    }

}
