using UnityEngine;

public class DiscardSelf : MonoBehaviour
{
    public void Apply()
    {
        CardsController.Instance.Discard(GetComponentInParent<Card>());
    }

}
