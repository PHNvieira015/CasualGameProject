using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerUnit : Unit
{
    public int MaxEnergy;

    private int _currentEnergy;
    public int CurrentEnergy
    {
        get => _currentEnergy;
        set
        {
            _currentEnergy = Mathf.Clamp(value, 0, MaxEnergy);
            UpdateEnergyMeter();
        }
    }

    public int DrawAmount;
    public int MaxCards;

    private TextMeshProUGUI _energyMeter;
    private bool _energyMeterInitialized = false;

    protected override void Awake()
    {
        base.Awake();
        InitializeEnergyMeter();
    }

    private void InitializeEnergyMeter()
    {
        GameObject energyMeterObj = GameObject.Find("Canvas/EnergyMeter");
        if (energyMeterObj != null)
        {
            _energyMeter = energyMeterObj.GetComponent<TextMeshProUGUI>();
            _energyMeterInitialized = (_energyMeter != null);
        }

        if (!_energyMeterInitialized)
        {
            Debug.LogWarning("Energy meter not initialized - creating fallback");
            CreateFallbackEnergyMeter();
        }
    }

    private void CreateFallbackEnergyMeter()
    {
        GameObject meterObj = new GameObject("FallbackEnergyMeter");
        meterObj.transform.SetParent(GameObject.Find("Canvas")?.transform);
        _energyMeter = meterObj.AddComponent<TextMeshProUGUI>();
        _energyMeterInitialized = true;
    }

    void UpdateEnergyMeter()
    {
        if (!_energyMeterInitialized) return;

        _energyMeter.text = $"{CurrentEnergy}/{MaxEnergy}";
    }

    public override IEnumerator Recover()
    {
        yield return StartCoroutine(base.Recover());

        // Set energy after recovery completes
        CurrentEnergy = MaxEnergy;

        // Draw cards
        int cardsToDraw = Mathf.Min(MaxCards - CardsController.Instance.Hand.Cards.Count, DrawAmount);
        if (cardsToDraw > 0)
        {
            yield return StartCoroutine(CardsController.Instance.Draw(cardsToDraw));
        }
    }
}