using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAttackEffect : CardEffect
{
    [Header("Attack Settings")]
    public int damageAmount = 10;
    public bool ignoreBlock = false;
    public bool affectsAllTargets = false;

    [Header("Visual Feedback")]
    public float effectDelay = 0.5f;
    public GameObject hitVFXPrefab;

    public override IEnumerator Apply(List<object> targets)
    {
        if (targets == null || targets.Count == 0)
        {
            Debug.LogWarning("No targets provided for enemy attack!");
            yield break;
        }

        List<Unit> unitsToDamage = new List<Unit>();
        foreach (object target in targets)
        {
            if (target is Unit unit)
            {
                unitsToDamage.Add(unit);
                if (!affectsAllTargets) break;
            }
        }

        foreach (Unit unit in unitsToDamage)
        {
            yield return ApplyDamageToUnit(unit);
        }
    }

    private IEnumerator ApplyDamageToUnit(Unit targetUnit)
    {
        // Visual effect
        if (hitVFXPrefab != null)
        {
            Instantiate(hitVFXPrefab, targetUnit.transform.position, Quaternion.identity);
        }

        // Damage calculation
        int damage = damageAmount;
        int finalDamage = damage;

        if (!ignoreBlock)
        {
            int targetBlock = targetUnit.GetStatValue(StatType.Block);
            if (targetBlock > 0)
            {
                int remainingBlock = Mathf.Max(0, targetBlock - damage);
                targetUnit.SetStatValue(StatType.Block, remainingBlock);
                finalDamage = Mathf.Max(0, damage - targetBlock);
            }
        }

        // Apply damage
        if (finalDamage > 0)
        {
            int currentHP = targetUnit.GetStatValue(StatType.HP);
            targetUnit.SetStatValue(StatType.HP, currentHP - finalDamage);
        }

        yield return new WaitForSeconds(effectDelay);
    }
}