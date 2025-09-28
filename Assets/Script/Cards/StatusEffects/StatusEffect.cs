using UnityEngine;
using System.Collections;

public abstract class StatusEffect : MonoBehaviour
{
    public int Duration;
    public int Amount;
    public bool StacksIntensity;
    public bool StacksDuration;

    protected Unit _host;
    public int CurrentDuration { get; protected set; } // Changed to protected set
    public int CurrentAmount { get; protected set; }   // Changed to protected set
    public int AppliedValue { get; protected set; }

    void OnEnable()
    {
        _host = GetComponentInParent<Unit>();
        if (_host == null) return;

        // Initialize current duration and amount
        CurrentDuration = Duration;
        CurrentAmount = Amount;

        if (Duration >= 0)
        {
            _host.OnUnitTakeTurn += DurationCountdown;
        }

        StartCoroutine(InflictAfterFrame());
    }

    void OnDisable()
    {
        OnRemoved();
    }

    void OnDestroy()
    {
        // Notify when destroyed so icon gets removed
        NotifyHolder();
    }

    private IEnumerator InflictAfterFrame()
    {
        yield return null;
        OnInflicted();
        NotifyHolder(); // Create icon when applied
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
        CurrentDuration--;
        if (CurrentDuration <= 0)
        {
            OnDurationEnded();
        }
        else
        {
            // Update icon every turn to show remaining duration
            NotifyHolder();
        }
    }

    protected void StacksChanged()
    {
        // Update icon when stacks change
        NotifyHolder();
    }
}