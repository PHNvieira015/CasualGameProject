using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(BoxCollider2D))]
public class CardRewardSlot : MonoBehaviour, IPointerClickHandler
{
    [Header("References")]
    public Transform cardAnchor;

    private CardRewardSystem rewardSystem;
    private GameObject currentCard;
    private GameObject assignedPrefab;
    private BoxCollider2D slotCollider;

    void Awake()
    {
        slotCollider = GetComponent<BoxCollider2D>();
        slotCollider.isTrigger = true;
    }

    public void Initialize(CardRewardSystem system, GameObject prefab)
    {
        rewardSystem = system;
        assignedPrefab = prefab;
        SpawnCard();
    }

    void SpawnCard()
    {
        ClearCard();

        if (assignedPrefab == null) return;

        currentCard = Instantiate(assignedPrefab, cardAnchor);

        // Disable all card interactions
        foreach (var collider in currentCard.GetComponentsInChildren<Collider2D>())
            collider.enabled = false;

        // Make card visually appear behind the slot collider
        currentCard.transform.SetAsFirstSibling();
    }

    public void ClearCard()
    {
        if (currentCard != null)
            Destroy(currentCard);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (assignedPrefab != null)
            rewardSystem.OnCardSelected(assignedPrefab);
    }
}