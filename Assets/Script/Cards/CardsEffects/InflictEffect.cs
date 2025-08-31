using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InflictEffect : CardEffect
{
    public StatusEffect StatusEffectPrefab;
    public int AppliesXTimes;

    public override IEnumerator Apply(List<object> targets)
    {
        foreach (object target in targets)
        {
            // Check if the target is null or has been destroyed
            if (target == null)
            {
                Debug.LogWarning("Attempted to apply effect to null target");
                continue;
            }

            Unit unit = target as Unit;

            // Check if the unit reference is valid and not destroyed
            if (unit == null)
            {
                Debug.LogWarning("Target is not a Unit or is null");
                continue;
            }

            // Additional check to see if the GameObject has been destroyed
            if (unit.gameObject == null)
            {
                Debug.LogWarning("Attempted to apply effect to destroyed Unit");
                continue;
            }

            for (int i = 0; i < AppliesXTimes; i++)
            {
                TryToApply(unit);
            }

            yield return null;
        }
    }

    void TryToApply(Unit unit)
    {
        // Double-check unit is still valid before applying effect
        if (unit == null || unit.gameObject == null)
        {
            Debug.LogWarning("Cannot apply effect - Unit is null or destroyed");
            return;
        }

        StatusEffect status = GetEffect(unit);
        if (status == null)
        {
            status = CreateEffect(unit);
        }
        else
        {
            if (StatusEffectPrefab.StacksDuration)
            {
                status.Duration += StatusEffectPrefab.Duration;
            }
            if (StatusEffectPrefab.StacksIntensity)
            {
                status.Amount += StatusEffectPrefab.Amount;
            }
        }
    }

    StatusEffect GetEffect(Unit unit)
    {
        // Check if unit is still valid before accessing its components
        if (unit == null || unit.gameObject == null)
            return null;

        foreach (StatusEffect status in unit.GetComponentsInChildren<StatusEffect>())
        {
            if (status == null) continue;

            if (status.name == StatusEffectPrefab.name)
            {
                return status;
            }
        }
        return null;
    }

    StatusEffect CreateEffect(Unit unit)
    {
        // Final safety check before instantiating
        if (unit == null || unit.gameObject == null)
            return null;

        StatusEffect instantiated = Instantiate(StatusEffectPrefab, Vector3.zero, Quaternion.identity, unit.transform);
        instantiated.name = StatusEffectPrefab.name;
        return instantiated;
    }
}