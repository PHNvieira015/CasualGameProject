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
                yield return new WaitForSeconds(4);
            }
            yield return null;
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
