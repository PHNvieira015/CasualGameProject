using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class TurnBeginState : State
{
    PlayerUnit _playerUnit;
    private bool _isProcessing = false;

    public override IEnumerator Enter()
    {
        if (_isProcessing)
        {
            Debug.LogWarning("TurnBeginState already processing - skipping");
            yield break;
        }

        _isProcessing = true;
        Debug.Log("=== TURN BEGIN STATE STARTED ===");

        // Use a flag to track if we encountered an error
        bool encounteredError = false;

        // RESET CARD SELECTION
        CardDrag.AllowSelection();

        machine.CurrentUnit = null;

        // Safety check - ensure units queue is valid
        if (machine.Units == null)
        {
            Debug.LogError("Units queue is null!");
            machine.Units = new Queue<Unit>();
        }

        // Find the first alive unit in the queue
        Unit aliveUnit = null;
        int unitsChecked = 0;
        int totalUnits = machine.Units.Count;

        Debug.Log($"Units in queue: {totalUnits}");

        // If no units in queue, handle battle end immediately
        if (totalUnits == 0)
        {
            Debug.Log("No units in battle queue");
            StartCoroutine(WaitThenChangeState<EndBattleState>());
            _isProcessing = false;
            yield break;
        }

        // First, check if player is dead
        if (_playerUnit == null)
        {
            // Try to find player unit in the queue if we don't have reference
            foreach (var unit in machine.Units)
            {
                if (unit is PlayerUnit playerUnit)
                {
                    _playerUnit = playerUnit;
                    break;
                }
            }
        }

        bool playerDead = (_playerUnit != null && _playerUnit.GetStatValue(StatType.HP) <= 0);

        // If player is dead, end battle immediately
        if (playerDead)
        {
            Debug.Log("Player is dead - ending battle");
            StartCoroutine(WaitThenChangeState<EndBattleState>());
            _isProcessing = false;
            yield break;
        }

        // Find next alive unit for the turn
        while (unitsChecked < totalUnits && aliveUnit == null)
        {
            Unit current = machine.Units.Dequeue();
            unitsChecked++;

            if (current == null)
            {
                Debug.LogWarning("Found null unit in queue");
                continue;
            }

            if (current.GetStatValue(StatType.HP) > 0)
            {
                aliveUnit = current;
                Debug.Log($"Next unit: {aliveUnit.name}");
            }
            else
            {
                Debug.LogFormat("Unit {0} is dead", current.name);
            }

            // Always enqueue back to maintain queue structure
            machine.Units.Enqueue(current);
        }

        machine.CurrentUnit = aliveUnit;

        // Check battle end conditions
        playerDead = (_playerUnit != null && _playerUnit.GetStatValue(StatType.HP) <= 0);
        bool allEnemiesDead = AreAllEnemiesDead();

        if (playerDead)
        {
            Debug.Log("Player died - ending battle");
            StartCoroutine(WaitThenChangeState<EndBattleState>());
            _isProcessing = false;
            yield break;
        }
        else if (allEnemiesDead)
        {
            Debug.Log("All enemies are dead - victory!");
            StartCoroutine(WaitThenChangeState<EndBattleState>());
            _isProcessing = false;
            yield break;
        }
        else if (aliveUnit == null)
        {
            Debug.Log("No alive units found in queue - ending battle");
            StartCoroutine(WaitThenChangeState<EndBattleState>());
            _isProcessing = false;
            yield break;
        }

        // Play relics at the start of each turn
        yield return StartCoroutine(PlayRelicsForTurn());

        // Re-check conditions after relics
        playerDead = (_playerUnit != null && _playerUnit.GetStatValue(StatType.HP) <= 0);
        allEnemiesDead = AreAllEnemiesDead();

        if (playerDead || allEnemiesDead)
        {
            Debug.Log("Battle ended during relic effects");
            StartCoroutine(WaitThenChangeState<EndBattleState>());
            _isProcessing = false;
            yield break;
        }

        // Proceed to recovery state
        Debug.Log("Transitioning to RecoveryState");
        StartCoroutine(WaitThenChangeState<RecoveryState>());

        _isProcessing = false;
    }

    private bool AreAllEnemiesDead()
    {
        bool foundAliveEnemy = false;

        foreach (var unit in machine.Units)
        {
            // Skip null units and the player unit
            if (unit == null || unit is PlayerUnit) continue;

            if (unit.GetStatValue(StatType.HP) > 0)
            {
                foundAliveEnemy = true;
                break;
            }
        }

        return !foundAliveEnemy;
    }

    IEnumerator PlayRelicsForTurn()
    {
        var relicHolder = CardsController.Instance?.RelicHolder;
        if (relicHolder == null) yield break;

        // Create a copy of the list to avoid modification during iteration
        List<Card> relicsToPlay = new List<Card>(relicHolder.Cards);
        Debug.Log($"Playing {relicsToPlay.Count} relics");

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
        if (relic == null) yield break;

        string relicName = relic.gameObject.name;
        Vector3 originalScale = relic.transform.localScale;

        // Get references to effect transforms
        Transform playedTransform = relic.transform.Find(PlayCardsState.PlayedGameObject);
        Transform afterPlayedTransform = relic.transform.Find(PlayCardsState.AfterPlayedGameObject);

        Debug.Log($"Playing relic: {relicName}");

        // Visual feedback
        relic.transform.localScale = originalScale * 1.2f;
        yield return new WaitForSeconds(0.3f);

        // Play the relic effects
        yield return StartCoroutine(PlayCardEffects(relic, playedTransform, afterPlayedTransform));

        // Reset scale if relic still exists
        if (relic != null)
        {
            relic.transform.localScale = originalScale;
            yield return new WaitForSeconds(0.2f);
        }

        // Clean up - remove from holder first, then destroy
        if (relic != null)
        {
            var relicHolder = CardsController.Instance?.RelicHolder;
            if (relicHolder != null)
            {
                relicHolder.RemoveCard(relic);
            }
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
        if (card == null || playTransform == null) yield break;

        int childCount = playTransform.childCount;

        for (int i = 0; i < childCount; i++)
        {
            if (i >= playTransform.childCount) yield break;

            Transform child = playTransform.GetChild(i);
            if (child == null) continue;

            ITarget targeter = child.GetComponent<ITarget>();
            List<object> targets = new List<object>();

            if (targeter == null) continue;

            yield return StartCoroutine(targeter.GetTargets(targets));

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