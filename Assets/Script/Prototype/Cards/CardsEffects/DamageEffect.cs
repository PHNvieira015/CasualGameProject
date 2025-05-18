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

            ModifiedValues modifiedValues = new ModifiedValues(Amount);
            ApplyModifier(modifiedValues, ModifierTags.DoAttackDamage, StateMachine.Instance.CurrentUnit);
            ApplyModifier(modifiedValues, ModifierTags.TakeAttackDamage, unit);


            int block = unit.GetStatValue(StatType.Block);
            int leftoverBlock = Mathf.Max(0, block - modifiedValues.FinalValue);
            unit.SetStatValue(StatType.Block, leftoverBlock);

            int currentHP = unit.GetStatValue(StatType.HP);
            int leftoverDamage = Mathf.Max(0, modifiedValues.FinalValue - block);

            unit.SetStatValue(StatType.HP, Mathf.Max(currentHP - leftoverDamage));

            Debug.LogFormat("Unit {0} HP went from {1} to {2}; block went from {3} to {4} ",unit.name, currentHP, unit.GetStatValue(StatType.HP), block, leftoverBlock);
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
