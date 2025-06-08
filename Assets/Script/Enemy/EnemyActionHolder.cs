using System.Collections.Generic;
using UnityEngine;

public class EnemyActionHolder : MonoBehaviour
{
    public List<Card> Actions = new List<Card>();

    public Card GetCurrentActionAndRotate()
    {
        if (Actions.Count == 0) return null;

        var action = Actions[0];
        Actions.RemoveAt(0);
        Actions.Add(action);
        return action;
    }
}