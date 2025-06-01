using UnityEngine;

[System.Serializable]
public class HealthChangeArgs
{
    public int NewValue;
    public int OldValue;
    public int AttemptedChange;
    public int ActualChange => NewValue - OldValue;
    public StatType StatType;
    public bool WasBlocked;

    public Color DisplayColor
    {
        get
        {
            if (StatType == StatType.Block)
            {
                // BLUE for gaining block, WHITE for losing block
                return ActualChange >= 0 ? Color.blue : Color.white;
            }

            // RED for damage
            if (ActualChange < 0) return Color.red;

            // GREEN for healing
            if (ActualChange > 0) return Color.green;

            // WHITE as fallback
            return Color.white;
        }
    }

    public HealthChangeArgs(int oldValue, int newValue, int attemptedChange, StatType statType, bool wasBlocked = false)
    {
        OldValue = oldValue;
        NewValue = newValue;
        AttemptedChange = attemptedChange;
        StatType = statType;
        WasBlocked = wasBlocked;
    }
}