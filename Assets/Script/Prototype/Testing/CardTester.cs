using UnityEngine;

public class CardTester : MonoBehaviour
{
    public Card Card;

    [ContextMenu("Draw")]

    void DrawCard()
    {
        StartCoroutine (CardsController.Instance.Draw());
    }
    [ContextMenu("Discard")]
    void RemoveCard()
    {
        CardsController.Instance.Discard(Card);
    }
}
