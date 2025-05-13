using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class PlayerUnit : Unit
{
    public int MaxEnergy;

    public int CurrentEnergy
    {
        get
        {
            return _currentEnergy;
        }
        set
        {
            //modify/add/control
            _currentEnergy = value;
            UpdateEnergyMeter();
        }
    }
    [SerializeField]
    int _currentEnergy;

    public int DrawAmount;
    
    public int MaxCards;

    Text _energyMeter;
    
    void awake()
    {
        _energyMeter = GameObject.Find("Canvas/EnergyMeter Text (TMP").GetComponent<Text>();
    }

    public override IEnumerator Recover()
    {
        CurrentEnergy = MaxEnergy;
        int cardsToDraw = Mathf.Min(MaxCards - CardsController.Instance.Hand.Cards.Count, DrawAmount);
        yield return StartCoroutine(CardsController.Instance.Draw(DrawAmount));
        //yield return null;
    }
    void UpdateEnergyMeter()
    {
    _energyMeter.text = string.Format("{0}/{1}", CurrentEnergy, MaxEnergy);
    }

}
