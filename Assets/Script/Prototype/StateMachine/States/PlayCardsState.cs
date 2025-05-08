using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class PlayCardsState : State
{
    public const string PlayedGameObject = "Effects/Played";

    public const string AfterPlayedGameObject = "Effects/AfterPlayed";

    Button endTurnButton;

    Coroutine cardSequencer;
    public override IEnumerator Enter()
    {
        yield return null;
        EndTurnButton(true);
        cardSequencer = StartCoroutine(CardSequencer());

    }
     public override IEnumerator Exit()
    {
        yield return null;
        EndTurnButton(false);
        StopCoroutine(cardSequencer);
    }
    IEnumerator CardSequencer()
    {
        while (true)
        {
            if(machine.CardsdToPlay.Count>0)
            {
                Card card = machine.CardsdToPlay.Dequeue();
                Debug.Log("Playing" + card);
                yield return StartCoroutine(PlayCardEffect(card, card.transform.Find(PlayedGameObject)));
                yield return StartCoroutine(PlayCardEffect(card, card.transform.Find(AfterPlayedGameObject)));
            }
            yield return null;
        }
    }
    IEnumerator PlayCardEffect(Card card, Transform playTransform)
    {
        for (int i = 0; i <playTransform.childCount; i++)
        {
            ITarget targeter = playTransform.GetChild(i).GetComponent<ITarget>();
            List<object> targets = new List<object>();
            if(targeter==null)
            {
                yield break;
            }
            yield return StartCoroutine(targeter.GetTargets(targets));
            ICardEffect effect = playTransform.GetChild(i).GetComponent<ICardEffect>();
            if (effect==null)
                yield break;
        yield return StartCoroutine(effect.Apply(targets));
        }
    }

    void EndTurnButton(bool interactiability)
    { 
    if (endTurnButton == null)
        {
            endTurnButton = GameObject.Find("Canvas/EndTurnButton").GetComponent<Button>();
        }
        endTurnButton.interactable = interactiability;
    }

}
