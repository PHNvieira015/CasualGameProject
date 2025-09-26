using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;
using UnityEngine.UI;

public enum CostType { Money, Key }

[RequireComponent(typeof(BoxCollider2D), typeof(Image))]
public class CardShopSlot : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
    [Header("References")]
    public Transform cardAnchor;
    public Image slotBackground;
    public GameObject costPanel;
    public TextMeshProUGUI costText;

    [HideInInspector] public int costAmount;
    [HideInInspector] public CostType costType;

    private CardShopSystem shopSystem;
    private Card assignedCardPrefab;
    private Card currentCardInstance;

    private BoxCollider2D slotCollider;
    private Image slotImage;
    private RectTransform rectTransform;

    private bool isPurchased = false;
    private bool canAfford = false;

    private Color normalColor = new Color(1, 1, 1, 0.7f);
    private Color hoverColor = new Color(0.8f, 0.9f, 1, 0.9f);
    private Color affordableColor = new Color(0.9f, 1, 0.9f, 0.8f);
    private Color unaffordableColor = new Color(1, 0.8f, 0.8f, 0.6f);
    private Color purchasedColor = new Color(0.7f, 0.7f, 0.7f, 0.5f);

    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        slotCollider = GetComponent<BoxCollider2D>();
        slotImage = GetComponent<Image>();

        SetupCollider();

        if (slotImage != null)
            slotImage.color = normalColor;

        if (slotBackground == null)
            slotBackground = slotImage;
    }

    void Start() => Invoke("SetupCollider", 0.1f);

    void SetupCollider()
    {
        if (slotCollider != null && rectTransform != null)
        {
            slotCollider.isTrigger = true;
            slotCollider.size = rectTransform.rect.size;
            slotCollider.offset = Vector2.zero;
        }
    }

    // Correct Initialize method with 5 arguments
    public void Initialize(CardShopSystem system, Card cardPrefab, int moneyCost, int keyCost, CostType type)
    {
        shopSystem = system;
        assignedCardPrefab = cardPrefab;
        costType = type;
        costAmount = (type == CostType.Money) ? moneyCost : keyCost;
        isPurchased = false;

        SpawnCard();
        UpdateCostDisplay();
        CheckAffordability();
        UpdateVisualState();
    }

    void SpawnCard()
    {
        ClearCard();
        if (assignedCardPrefab == null) return;

        currentCardInstance = Instantiate(assignedCardPrefab, cardAnchor);
        currentCardInstance.transform.localPosition = Vector3.zero;
        currentCardInstance.transform.localRotation = Quaternion.identity;
        currentCardInstance.transform.localScale = Vector3.one;

        CanvasGroup cg = currentCardInstance.GetComponent<CanvasGroup>();
        if (!cg) cg = currentCardInstance.gameObject.AddComponent<CanvasGroup>();
        cg.blocksRaycasts = false;
        cg.interactable = false;

        foreach (var img in currentCardInstance.GetComponentsInChildren<Image>())
            img.raycastTarget = false;

        foreach (var col in currentCardInstance.GetComponentsInChildren<Collider2D>())
            col.enabled = false;
    }

    void UpdateCostDisplay()
    {
        if (costPanel != null && costText != null)
        {
            costPanel.SetActive(true);
            costText.text = $"{costAmount} {(costType == CostType.Money ? "G" : "K")}";
        }
    }

    void CheckAffordability()
    {
        canAfford = !isPurchased && (costType == CostType.Money ?
            MoneyKeySystem.Instance.Money >= costAmount :
            MoneyKeySystem.Instance.Keys >= costAmount);
    }

    void UpdateVisualState()
    {
        if (slotBackground == null) return;

        if (isPurchased)
        {
            slotBackground.color = purchasedColor;
            if (costText != null) costText.text = "SOLD";
        }
        else if (canAfford)
        {
            slotBackground.color = affordableColor;
        }
        else
        {
            slotBackground.color = unaffordableColor;
        }
    }

    public void ClearCard()
    {
        if (currentCardInstance != null)
            Destroy(currentCardInstance.gameObject);
        currentCardInstance = null;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        Debug.Log($"Clicked slot for card: {assignedCardPrefab?.name}, Purchased: {isPurchased}, CanAfford: {canAfford}");

        if (isPurchased || !canAfford) return;

        bool purchaseSuccess = costType == CostType.Money ?
            MoneyKeySystem.Instance.MakePurchase(costAmount, 0) :
            MoneyKeySystem.Instance.MakePurchase(0, costAmount);

        if (!purchaseSuccess)
        {
            Debug.Log("Purchase failed: not enough resources");
            return;
        }

        isPurchased = true;
        UpdateVisualState();

        if (shopSystem != null)
        {
            CardsList targetList = (costType == CostType.Money) ? shopSystem.playerDeck : shopSystem.playerRelics;
            if (targetList != null)
            {
                targetList.Cards.Add(assignedCardPrefab);
                Debug.Log($"Added {assignedCardPrefab.name} to {(costType == CostType.Money ? "PlayerDeck" : "PlayerRelics")}");
            }

            shopSystem.OnCardPurchasedHandler(assignedCardPrefab, this, costType == CostType.Key);
        }

        if (currentCardInstance != null)
            currentCardInstance.gameObject.SetActive(false);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (!isPurchased && slotBackground != null)
            slotBackground.color = hoverColor;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        UpdateVisualState();
    }

    public void ResetSlot()
    {
        isPurchased = false;
        ClearCard();
        SpawnCard();
        CheckAffordability();
        UpdateVisualState();
        UpdateCostDisplay();
    }
}
