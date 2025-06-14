using UnityEngine;
using UnityEngine.EventSystems;

public class CardRewardSlot : MonoBehaviour, IPointerClickHandler
{
    [Header("References")]
    public Transform cardAnchor; // Where to spawn the card

    private CardRewardSystem rewardSystem;
    private GameObject currentCard;


    public void Initialize(CardRewardSystem system)
    {
        rewardSystem = system;
    }

    public void SpawnCard(GameObject cardPrefab)
    {
        ClearCard();
        currentCard = Instantiate(cardPrefab, cardAnchor);

        // Disable card's own collider if it has one
        var cardCollider = currentCard.GetComponent<Collider>();
        if (cardCollider != null) cardCollider.enabled = false;
    }

    public void ClearCard()
    {
        if (currentCard != null)
        {
            Destroy(currentCard);
            currentCard = null;
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (currentCard != null)
        {
            rewardSystem.OnCardSelected(currentCard);
        }
    }
}