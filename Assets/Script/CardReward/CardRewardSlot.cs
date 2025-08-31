using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(BoxCollider2D))]
public class CardRewardSlot : MonoBehaviour, IPointerClickHandler
{
    [Header("References")]
    public Transform cardAnchor;

    private CardRewardSystem rewardSystem;
    private GameObject currentCard;
    private Card currentCardComponent;
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
        currentCardComponent = currentCard.GetComponent<Card>();

        // Disable all card interactions and make it non-playable
        DisableCardInteractions();

        // Optional: Add visual effects to indicate it's a reward card
        ApplyRewardCardVisuals();

        // Make card visually appear behind the slot collider
        currentCard.transform.SetAsFirstSibling();
    }

    void DisableCardInteractions()
    {
        if (currentCard == null) return;

        // Only disable what's absolutely necessary
        CardDrag cardDrag = currentCard.GetComponent<CardDrag>();
        if (cardDrag != null)
            cardDrag.enabled = false;

        // Remove the CanvasGroup approach - it might be interfering
        CanvasGroup canvasGroup = currentCard.GetComponent<CanvasGroup>();
        if (canvasGroup != null)
        {
            Destroy(canvasGroup);
        }

        // Just disable colliders and let the slot handle clicks
        foreach (var collider in currentCard.GetComponentsInChildren<Collider2D>())
            collider.enabled = false;
    }

    void ApplyRewardCardVisuals()
    {
        if (currentCard == null) return;

        // Optional: Add a glow, frame, or other visual indicator
        // that this is a reward card (not playable in this context)

        // Example: Add a subtle glow or outline
        // Outline outline = currentCard.AddComponent<Outline>();
        // outline.effectColor = Color.yellow;
        // outline.effectDistance = new Vector2(2, 2);
    }

    public void ClearCard()
    {
        if (currentCard != null)
            Destroy(currentCard);

        currentCardComponent = null;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (assignedPrefab != null)
            rewardSystem.OnCardSelected(assignedPrefab);
    }

    // Optional: Add hover effects to the slot instead of the card
    public void OnPointerEnter(PointerEventData eventData)
    {
        // Highlight the slot when hovering
         //transform.localScale = Vector3.one * 1.1f;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        // Reset slot appearance
         //transform.localScale = Vector3.one;
    }
}