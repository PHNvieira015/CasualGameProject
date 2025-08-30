using System.Collections;
using UnityEngine;
using TMPro;

public class PlayerUnit : Unit
{
    [Header("Energy Settings")]
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

    [Header("Card Settings")]
    public int DrawAmount;
    public int MaxCards;

    [Header("UI References")]
    [SerializeField] private TextMeshProUGUI _energyMeter;

    protected override void Awake()
    {
        base.Awake();

        if (_energyMeter == null)
        {
            Debug.LogError("Energy Meter is not assigned in the Inspector!", this);
        }
    }

    private void UpdateEnergyMeter()
    {
        if (_energyMeter != null)
        {
            _energyMeter.text = $"{CurrentEnergy}/{MaxEnergy}";
        }
    }

    public override IEnumerator Recover()
    {
        yield return StartCoroutine(base.Recover());

        // Restore energy to full
        CurrentEnergy = MaxEnergy;

        // Draw cards (but don’t exceed hand limit)
        int cardsToDraw = Mathf.Min(MaxCards - CardsController.Instance.Hand.Cards.Count, DrawAmount);
        if (cardsToDraw > 0)
        {
            yield return StartCoroutine(CardsController.Instance.Draw(cardsToDraw));
        }
    }
}
