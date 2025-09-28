using UnityEngine;
using System.Collections;

public abstract class StatusEffect : MonoBehaviour
{
    public int Duration;
    public int Amount;
    public bool StacksIntensity;
    public bool StacksDuration;

    protected Unit _host;
    private int _currentDuration;
    public int _currentAmount;
    public int AppliedValue { get; protected set; }

    void OnEnable()
    {
        _host = GetComponentInParent<Unit>();
        if (_host == null) return;

        if (Duration >= 0)
        {
            _currentDuration = Duration;
            _host.OnUnitTakeTurn += DurationCountdown;
        }

        StartCoroutine(InflictAfterFrame());
    }

    void OnDisable()
    {
        OnRemoved();
        NotifyHolder();
    }

    void OnDestroy()
    {
        NotifyHolder();
    }

    private IEnumerator InflictAfterFrame()
    {
        yield return null;
        OnInflicted();
        NotifyHolder();
    }

    private void NotifyHolder()
    {
        if (_host != null)
        {
            BuffDebuffHolder holder = _host.GetComponentInChildren<BuffDebuffHolder>();
            if (holder != null)
            {
                holder.RefreshUI();
            }
        }
    }

    public abstract void OnInflicted();
    public abstract void OnRemoved();

    public virtual void OnDurationEnded()
    {
        if (_host != null)
        {
            _host.OnUnitTakeTurn -= DurationCountdown;
        }
        Destroy(gameObject);
    }

    void DurationCountdown(Unit unit)
    {
        _currentDuration--;
        if (_currentDuration <= 0)
        {
            OnDurationEnded();
        }
        else
        {
            NotifyHolder();
        }
    }

    protected void StacksChanged()
    {
        NotifyHolder();
    }
}