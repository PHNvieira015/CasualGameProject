using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(BoxCollider2D), typeof(Image))]
public class CardRewardSlot : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
    [Header("References")]
    public Transform cardAnchor;
    public Image slotBackground;

    private CardRewardSystem rewardSystem;
    private Card currentCardInstance;
    private Card assignedCardPrefab;
    private BoxCollider2D slotCollider;
    private Image slotImage;
    private RectTransform rectTransform;

    private Color normalColor = new Color(1, 1, 1, 0.5f);
    private Color hoverColor = new Color(0.8f, 0.9f, 1, 0.8f);
    private Color selectedColor = new Color(0.6f, 0.8f, 1, 1f);

    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        slotCollider = GetComponent<BoxCollider2D>();
        slotImage = GetComponent<Image>();

        SetupCollider(); // Properly size the collider

        if (slotImage != null)
        {
            slotImage.color = normalColor;
            slotImage.raycastTarget = true;
        }

        if (slotBackground == null)
            slotBackground = slotImage;
    }

    void Start()
    {
        // Ensure collider is properly sized after layout is calculated
        Invoke("SetupCollider", 0.1f);
    }

    void SetupCollider()
    {
        if (slotCollider != null && rectTransform != null)
        {
            slotCollider.isTrigger = true;

            // Set collider size to match RectTransform size
            Vector2 size = rectTransform.rect.size;
            slotCollider.size = size;

            // Center the collider
            slotCollider.offset = Vector2.zero;

            Debug.Log($"Collider size set to: {size} for slot: {gameObject.name}");
        }
    }

    // Optional: Update collider if rect transform changes size
    void OnRectTransformDimensionsChange()
    {
        SetupCollider();
    }

    public void Initialize(CardRewardSystem system, Card cardPrefab)
    {
        rewardSystem = system;
        assignedCardPrefab = cardPrefab;
        SpawnCard();
    }

    void SpawnCard()
    {
        ClearCard();

        if (assignedCardPrefab == null) return;

        currentCardInstance = Instantiate(assignedCardPrefab, cardAnchor);
        if (currentCardInstance != null)
        {
            currentCardInstance.transform.localPosition = Vector3.zero;
            currentCardInstance.transform.localRotation = Quaternion.identity;
            currentCardInstance.transform.localScale = Vector3.one;

            FixCardTransparency(currentCardInstance);
            DisableCardInteractions();
            MakeCardNonBlocking();
        }
    }

    void FixCardTransparency(Card card)
    {
        if (card == null) return;

        CanvasGroup canvasGroup = card.GetComponent<CanvasGroup>();
        if (canvasGroup != null)
        {
            canvasGroup.alpha = 1f;
            canvasGroup.blocksRaycasts = false;
            canvasGroup.interactable = false;
        }

        Image[] images = card.GetComponentsInChildren<Image>();
        foreach (Image img in images)
        {
            img.raycastTarget = false;
            Color color = img.color;
            color.a = 1f;
            img.color = color;
        }
    }

    void DisableCardInteractions()
    {
        if (currentCardInstance == null) return;

        CardDrag cardDrag = currentCardInstance.GetComponent<CardDrag>();
        if (cardDrag != null) cardDrag.enabled = false;

        foreach (var collider in currentCardInstance.GetComponentsInChildren<Collider2D>())
            collider.enabled = false;

        Button cardButton = currentCardInstance.GetComponent<Button>();
        if (cardButton != null) cardButton.enabled = false;
    }

    void MakeCardNonBlocking()
    {
        if (currentCardInstance == null) return;

        CanvasGroup cardCanvasGroup = currentCardInstance.GetComponent<CanvasGroup>();
        if (cardCanvasGroup == null)
            cardCanvasGroup = currentCardInstance.gameObject.AddComponent<CanvasGroup>();

        cardCanvasGroup.blocksRaycasts = false;
        cardCanvasGroup.interactable = false;

        Image[] cardImages = currentCardInstance.GetComponentsInChildren<Image>();
        foreach (Image img in cardImages)
        {
            img.raycastTarget = false;
        }
    }

    public void ClearCard()
    {
        if (currentCardInstance != null)
        {
            Destroy(currentCardInstance.gameObject);
            currentCardInstance = null;
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        Debug.Log($"Slot {gameObject.name} clicked!");

        if (assignedCardPrefab != null && rewardSystem != null)
        {
            if (slotBackground != null)
                slotBackground.color = selectedColor;

            // Pass the original prefab, not the instance
            rewardSystem.OnCardSelected(assignedCardPrefab);
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (slotBackground != null)
            slotBackground.color = hoverColor;

        transform.localScale = Vector3.one * 1.05f;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (slotBackground != null)
            slotBackground.color = normalColor;

        transform.localScale = Vector3.one;
    }

    public void ResetVisuals()
    {
        if (slotBackground != null)
            slotBackground.color = normalColor;

        transform.localScale = Vector3.one;
    }

    // DEBUG: Draw gizmo to visualize collider size
#if UNITY_EDITOR
    void OnDrawGizmosSelected()
    {
        if (slotCollider != null)
        {
            Gizmos.color = Color.green;
            Gizmos.matrix = transform.localToWorldMatrix;
            Gizmos.DrawWireCube(slotCollider.offset, slotCollider.size);
        }
    }
#endif
}