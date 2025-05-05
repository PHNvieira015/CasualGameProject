using UnityEngine;

public class Tester : MonoBehaviour
{
    public Card Card;

    public int StartingHand=5;

    private void Start()
    {
    StartCoroutine(CardsController.Instance.Draw(StartingHand));
    }


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
    [ContextMenu("Shuffle Discard into Draw")]
    void ShuffleDiscard()
    {
        StartCoroutine (CardsController.Instance.ShuffleDiscardintoDrawPile());

    }
    [ContextMenu("PlayCard")]
    void PlayedCard()
    {
        CardsController.Instance.PlayedEffects(Card);

    }
    void AfterPlayedCard()
    {
        CardsController.Instance.AfterPlayedEffects(Card);

    }

}
