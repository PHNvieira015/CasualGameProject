using UnityEngine;
public class DamageIndicatorSpawner : MessageSpawner
{
    public void SpawnMessage(HealthChangeArgs args)
    {
        string message = GetFormattedMessage(args);
        var msgObject = Instantiate(_messagePrefab, GetSpawnPosition(), Quaternion.identity);

        var floatingMessage = msgObject.GetComponent<FloatingMessage>();
        if (floatingMessage != null)
        {
            floatingMessage.SetMessage(message);
            floatingMessage.SetColor(args.DisplayColor);
        }
    }

    private string GetFormattedMessage(HealthChangeArgs args)
    {
        string prefix = "";
        string value = Mathf.Abs(args.ActualChange).ToString();

        if (args.StatType == StatType.Block)
        {
            // Show negative sign for losing block
            prefix = args.ActualChange < 0 ? "-" : "+";
            return $"{prefix}{value} BLOCK";
        }

        if (args.ActualChange < 0)
        {
            prefix = "-";
            if (args.WasBlocked) value = $"{value} (BLOCKED)";
        }
        else if (args.ActualChange > 0)
        {
            prefix = "+";
        }

        return $"{prefix}{value} HP";
    }
}