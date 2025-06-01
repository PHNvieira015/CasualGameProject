using UnityEngine;

public class MessageSpawner : MonoBehaviour
{
    [SerializeField] protected Vector2 _initialPosition;
    [SerializeField] protected GameObject _messagePrefab;

    protected Vector3 GetSpawnPosition()
    {
        return transform.position + (Vector3)_initialPosition;
    }

    public void SpawnMessage(string msg)
    {
        var msgObject = Instantiate(_messagePrefab, GetSpawnPosition(), Quaternion.identity);
        var inGameMessage = msgObject.GetComponent<IInGameMessage>();
        if (inGameMessage != null)
        {
            inGameMessage.SetMessage(msg);
        }
    }
}