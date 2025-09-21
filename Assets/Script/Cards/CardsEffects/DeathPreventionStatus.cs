using UnityEngine;
using System.Collections;

public class DeathPreventionStatus : StatusEffect
{
    private bool _hasPreventedDeath = false;

    public override void OnInflicted()
    {
        Debug.Log($"{_host.name} gained Death Prevention for {Duration} turns");

        // Subscribe to the TakeAttackDamage modifier to intercept lethal damage
        _host.Modify[(int)ModifierTags.TakeAttackDamage] += PreventLethalDamage;
    }

    public override void OnRemoved()
    {
        Debug.Log($"Death Prevention removed from {_host.name}");

        // Unsubscribe from the modifier
        _host.Modify[(int)ModifierTags.TakeAttackDamage] -= PreventLethalDamage;
    }

    void PreventLethalDamage(ModifiedValues modifiedValues)
    {
        if (_hasPreventedDeath) return;

        // Check if this damage would kill the unit
        int currentHP = _host.GetStatValue(StatType.HP);
        int damageAfterModifiers = modifiedValues.FinalValue;

        if (currentHP <= damageAfterModifiers)
        {
            // This is lethal damage - prevent it and set HP to 1
            _hasPreventedDeath = true;
            modifiedValues.FinalValue = currentHP - 1; // Reduce damage to leave 1 HP

            Debug.Log($"Death Prevention saved {_host.name} from lethal damage!");

            // Visual feedback
            StartCoroutine(ShowPreventionEffect());

            // Remove the status after use (if not stackable)
            if (!StacksIntensity)
            {
                // Schedule removal at the end of the frame
                StartCoroutine(RemoveAfterFrame());
            }
        }
    }

    private IEnumerator RemoveAfterFrame()
    {
        yield return null; // Wait until end of frame
        OnDurationEnded();
    }

    private IEnumerator ShowPreventionEffect()
    {
        // Visual feedback - flash the unit white
        SpriteRenderer sprite = _host.GetComponent<SpriteRenderer>();
        if (sprite != null)
        {
            Color originalColor = sprite.color;
            sprite.color = Color.white;
            yield return new WaitForSeconds(0.2f);
            sprite.color = originalColor;
        }
        yield return new WaitForSeconds(0.3f);
    }
}