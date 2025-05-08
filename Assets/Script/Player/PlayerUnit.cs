using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerUnit : Unit
{
    public int MaxEnergy;

    public int CurrentEnergy;

    public int DrawAmount;
    
    public int MaxCards;

    public override IEnumerator Recover()
    {
        CurrentEnergy = MaxEnergy;
        int cardsToDraw = Mathf.Min(MaxCards - CardsController.Instance.Hand.Cards.Count, DrawAmount);
        yield return StartCoroutine(CardsController.Instance.Draw(DrawAmount));
        //yield return null;
    }

}
