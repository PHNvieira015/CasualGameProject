using UnityEngine;

public class BuffDebuffMessageSpawner : MessageSpawner
{
    public float verticalOffset = -2.0f;

    public void SpawnBuffMessage(string message, int value)
    {
        string finalMessage = value > 0 ? $"+{value} {message}" : message;
        SpawnColoredMessage(finalMessage, Color.green);
    }

    public void SpawnDebuffMessage(string message, int value)
    {
        string finalMessage = value > 0 ? $"+{value} {message}" : message;
        SpawnColoredMessage(finalMessage, Color.blue);
    }

    public void SpawnExpirationMessage(string effectName)
    {
        string message = $"{effectName} Faded";
        SpawnColoredMessage(message, Color.gray);
    }

    private void SpawnColoredMessage(string msg, Color color)
    {
        Vector3 spawnPosition = GetSpawnPosition() + Vector3.up * verticalOffset;

        var msgObject = Instantiate(_messagePrefab, spawnPosition, Quaternion.identity);
        var floatingMsg = msgObject.GetComponent<FloatingMessage>();
        if (floatingMsg != null)
        {
            floatingMsg.SetMessage(msg);
            floatingMsg.SetColor(color);
        }
    }

    private Vector3 GetSpawnPosition()
    {
        return transform.position + Vector3.up * 1.0f;
    }
}