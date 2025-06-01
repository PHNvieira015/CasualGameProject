using UnityEngine;

public class BuffDebuffMessageSpawner : MessageSpawner
{
    public void SpawnBuffMessage(string buffName, int stacks)
    {
        string message = $"+{stacks} {buffName}";
        SpawnColoredMessage(message, Color.green);
    }

    public void SpawnDebuffMessage(string debuffName, int stacks)
    {
        string message = $"+{stacks} {debuffName}";
        SpawnColoredMessage(message, Color.red);
    }

    public void SpawnExpirationMessage(string effectName)
    {
        string message = $"{effectName} Faded";
        SpawnColoredMessage(message, Color.gray);
    }

    private void SpawnColoredMessage(string msg, Color color)
    {
        var msgObject = Instantiate(_messagePrefab, GetSpawnPosition(), Quaternion.identity);
        var floatingMsg = msgObject.GetComponent<FloatingMessage>();
        if (floatingMsg != null)
        {
            floatingMsg.SetMessage(msg);
            floatingMsg.SetColor(color);
        }
    }
}
