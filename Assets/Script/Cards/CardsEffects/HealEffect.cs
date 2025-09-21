using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealEffect : CardEffect
{
    public int Amount;

    public override IEnumerator Apply(List<object> targets)
    {
        foreach (object target in targets)
        {
            Unit unit = target as Unit;
            if (unit == null) continue;

            ModifiedValues modifiedValues = new ModifiedValues(Amount);
            ApplyModifier(modifiedValues, ModifierTags.Heal, unit);

            int currentHP = unit.GetStatValue(StatType.HP);
            int maxHP = unit.GetStatValue(StatType.MaxHP);
            int newHP = Mathf.Min(maxHP, currentHP + modifiedValues.FinalValue);

            unit.SetStatValue(StatType.HP, newHP);

            Debug.LogFormat("Unit {0} HP went from {1} to {2} (healed for {3})",
                unit.name, currentHP, newHP, modifiedValues.FinalValue);

            yield return null;
        }
    }

    void ApplyModifier(ModifiedValues modifiedValues, ModifierTags tag, Unit unit)
    {
        TagModifier modifier = unit.Modify[(int)tag];
        if (modifier != null)
        {
            modifier(modifiedValues);
            Debug.LogFormat("Base Heal Value:{0}, Modified Heal Value: {1}",
                modifiedValues.BaseValue, modifiedValues.FinalValue);
        }
    }
}