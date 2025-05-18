using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class StatusEffect : MonoBehaviour
{
    public int Duration;
    public int Amount;
    protected Unit _host;
    private int _currentDuration;
    public int _currentAmount;
    void OnEnable()
    {
     _host = GetComponentInParent<Unit>();
        if (Duration >= 0)
        {
            _currentDuration = Duration;
            _host.OnUnitTakeTurn += DurationCountdown;

        }
     OnInflicted();
    }
    void OnDisable()
    {
    OnRemoved();
    }
public abstract void OnInflicted();

public abstract void OnRemoved();
    public virtual void OnDurationEnded()
    {
        _host.OnUnitTakeTurn -= DurationCountdown;
        Destroy(this.gameObject);
    }

    void DurationCountdown(Unit unit)
    {
        _currentDuration--;
        if(_currentDuration <= 0)
        {
            OnDurationEnded();
        }

    }
}