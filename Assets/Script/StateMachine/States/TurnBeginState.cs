using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TurnBeginState : State
{
    PlayerUnit _playerUnit;

    public override IEnumerator Enter()
    {
        machine.CurrentUnit = null;

        // Find the first alive unit in the queue
        Unit aliveUnit = null;
        int unitsChecked = 0;

        while (machine.Units.Count > 0 && unitsChecked < machine.Units.Count)
        {
            Unit current = machine.Units.Dequeue();
            unitsChecked++;

            if (current == null) continue;

            if (current.GetStatValue(StatType.HP) > 0)
            {
                aliveUnit = current;
                machine.Units.Enqueue(current);
                break;
            }
            else
            {
                Debug.LogFormat("Unit {0} is dead", current.name);
                machine.Units.Enqueue(current); // Keep dead units in queue if needed
            }
        }

        machine.CurrentUnit = aliveUnit;

        // Try to get player unit reference if we don't have one
        if (_playerUnit == null && machine.CurrentUnit != null)
        {
            _playerUnit = machine.CurrentUnit as PlayerUnit;
        }

        yield return null;

        // Play relics at the start of each turn
        yield return StartCoroutine(PlayRelicsForTurn());

        // Check battle end conditions
        if (machine.Units.Count == 1 ||
          (_playerUnit != null && _playerUnit.GetStatValue(StatType.HP) <= 0))
        {
            StartCoroutine(WaitThenChangeState<EndBattleState>());
        }
        else if (machine.CurrentUnit != null)
        {
            StartCoroutine(WaitThenChangeState<RecoveryState>());
        }
        else
        {
            Debug.LogError("No valid units found in battle!");
            StartCoroutine(WaitThenChangeState<EndBattleState>());
        }
    }

    IEnumerator PlayRelicsForTurn()
    {
        // Create a copy of the list to avoid modification during iteration
        List<Card> relicsToPlay = new List<Card>(CardsController.Instance.RelicHolder.Cards);

        // Play all relics from the relic holder
        foreach (Card relic in relicsToPlay)
        {
            if (relic != null)
            {
                yield return StartCoroutine(PlayRelicDirectly(relic));
            }
        }
    }

    IEnumerator PlayRelicDirectly(Card relic)
    {
        // Store all necessary references BEFORE playing effects
        string relicName = relic.gameObject.name;
        Vector3 originalScale = relic.transform.localScale;

        // Get references to effect transforms BEFORE anything might get destroyed
        Transform playedTransform = relic.transform.Find(PlayCardsState.PlayedGameObject);
        Transform afterPlayedTransform = relic.transform.Find(PlayCardsState.AfterPlayedGameObject);

        Debug.Log($"Playing relic: {relicName}");

        // Visual feedback
        relic.transform.localScale = originalScale * 1.2f;
        yield return new WaitForSeconds(0.3f);

        // Play the relic effects using stored transform references
        yield return StartCoroutine(PlayCardEffects(relic, playedTransform, afterPlayedTransform));

        // Additional visual feedback - only if relic still exists
        if (relic != null)
        {
            relic.transform.localScale = originalScale;
            yield return new WaitForSeconds(0.2f);
        }

        // Clean up - remove from holder first, then destroy (if it's a one-time use relic)
        // Remove this line if you want relics to persist across turns
        if (relic != null)
        {
            CardsController.Instance.RelicHolder.RemoveCard(relic);
            Destroy(relic.gameObject);
        }
    }

    IEnumerator PlayCardEffects(Card card, Transform playedTransform, Transform afterPlayedTransform)
    {
        if (playedTransform != null)
        {
            yield return StartCoroutine(PlayCardEffect(card, playedTransform));
            yield return new WaitForSeconds(0.5f);
        }

        if (afterPlayedTransform != null)
        {
            yield return StartCoroutine(PlayCardEffect(card, afterPlayedTransform));
            yield return new WaitForSeconds(0.5f);
        }
    }

    IEnumerator PlayCardEffect(Card card, Transform playTransform)
    {
        // Check if the card or playTransform has been destroyed
        if (card == null || playTransform == null) yield break;

        int childCount = playTransform.childCount;

        for (int i = 0; i < childCount; i++)
        {
            // Check if the child still exists before accessing it
            if (i >= playTransform.childCount) yield break;

            Transform child = playTransform.GetChild(i);
            if (child == null) continue;

            ITarget targeter = child.GetComponent<ITarget>();
            List<object> targets = new List<object>();

            if (targeter == null) continue;

            yield return StartCoroutine(targeter.GetTargets(targets));

            // Get all effects from the child
            if (child == null) continue;

            CardEffect[] effects = child.GetComponents<CardEffect>();
            foreach (CardEffect effect in effects)
            {
                if (effect != null)
                {
                    yield return StartCoroutine(effect.Apply(targets));
                }
            }
        }
    }
}