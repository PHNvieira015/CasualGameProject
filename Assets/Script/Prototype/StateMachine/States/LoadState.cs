using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LoadState : State
{
    public override IEnumerator Enter()
    {
        yield return StartCoroutine(InitializeDeck());
        yield return StartCoroutine(InitializeUnits());
        StartCoroutine(WaitThenChangeState<TurnBeginState>());

    }
    IEnumerator InitializeDeck()
    {
        foreach (Card card in CardsController.Instance.PlayerDeck.Cards)
        {
            Card newCard = Instantiate(card, Vector3.zero, Quaternion.identity, CardsController.Instance.DrawPile.Holder);
            CardsController.Instance.DrawPile.AddCard(newCard);
        }
        yield return new WaitForSeconds(CardHolder.CardMoveDuration);  
        CardsController.Instance.DrawPile.SetInitialRotation();
    }
    IEnumerator InitializeUnits()
    {
        machine.units = new Queue<Unit>();
        foreach (Unit unit in GameObject.Find("Units").GetComponentsInChildren<Unit>())
        {
            machine.units.Enqueue(unit);
        }
        yield return null;
        Debug.Log(machine.units.Count);
    }



}
