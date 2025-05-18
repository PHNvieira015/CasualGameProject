using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class PlayCardsState : State
{
    public const string PlayedGameObject = "Effects/Played";

    public const string AfterPlayedGameObject = "Effects/AfterPlayed";

    public Button endTurnButton;

    Coroutine _cardSequencer;

    HorizontalLayoutGroup _handLayout;

    private void Awake()
    {
        _handLayout = CardsController.Instance.Hand.Holder.GetComponent<HorizontalLayoutGroup>();
    }


    public override IEnumerator Enter()
    {
        yield return new WaitForSeconds(0.5f);
        EndTurnButton(true);
        _handLayout.enabled = false;
        _cardSequencer = StartCoroutine(CardSequencer());

    }
     public override IEnumerator Exit()
    {
        yield return null;
        EndTurnButton(false);
        _handLayout.enabled = true;
        StopCoroutine(_cardSequencer);
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
                yield return new WaitForSeconds(0.5f);
                yield return StartCoroutine(PlayCardEffect(card, card.transform.Find(AfterPlayedGameObject)));
                yield return new WaitForSeconds(0.5f);
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
                continue;
            }
            yield return StartCoroutine(targeter.GetTargets(targets));
            ICardEffect effect = playTransform.GetChild(i).GetComponent<ICardEffect>();
            if (effect==null)
                continue;

        yield return StartCoroutine(effect.Apply(targets));
            _handLayout.enabled = true;
        }
    }

    void EndTurnButton(bool interactiability)
    { 
    if (endTurnButton == null)
        {
            endTurnButton = GameObject.Find("Canvas/EndTurnbutton").GetComponent<Button>();
        }
        endTurnButton.interactable = interactiability;
    }

}
