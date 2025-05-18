using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using TMPro; // Add this if using TextMeshPro

public class PlayerUnit : Unit
{
    public int MaxEnergy;
    [SerializeField] int _currentEnergy;
    public int DrawAmount;
    public int MaxCards;

    // Changed to TMP_Text if using TextMeshPro
    [SerializeField] private Text _energyMeter;
    // OR if using TextMeshPro:
    // [SerializeField] private TMP_Text _energyMeter;

    public int CurrentEnergy
    {
        get => _currentEnergy;
        set
        {
            _currentEnergy = Mathf.Clamp(value, 0, MaxEnergy);
            UpdateEnergyMeter();
        }
    }

    void Awake()
    {
        // Better initialization methods:
        // Option 1: Assign via inspector (recommended)
        // Option 2: Find with more reliable path
        if (_energyMeter == null)
        {
            var meterObj = GameObject.Find("EnergyMeter Text (TMP)");
            if (meterObj != null)
            {
                _energyMeter = meterObj.GetComponent<Text>();
                // OR if using TextMeshPro:
                // _energyMeter = meterObj.GetComponent<TMP_Text>();
            }
        }

        // Initialize energy display
        UpdateEnergyMeter();
    }

    public override IEnumerator Recover()
    {
        yield return StartCoroutine(base.Recover());
        CurrentEnergy = MaxEnergy;

        int cardsToDraw = Mathf.Min(MaxCards - CardsController.Instance.Hand.Cards.Count, DrawAmount);
        if (cardsToDraw > 0)
        {
            yield return StartCoroutine(CardsController.Instance.Draw(cardsToDraw));
        }
    }

    void UpdateEnergyMeter()
    {
        if (_energyMeter != null)
        {
            _energyMeter.text = $"{CurrentEnergy}/{MaxEnergy}";
        }
        else
        {
            Debug.LogWarning("Energy meter reference is missing!");
        }
    }
}