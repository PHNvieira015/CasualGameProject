using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

[RequireComponent(typeof(BoxCollider2D), typeof(Image))]
public class CardShopSlot : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
    [Header("Card Display")]
    public Transform cardAnchor;
    public Image slotBackground;

    [Header("Cost Display")]
    public GameObject costPanel;
    public TMP_Text costText;

    [Header("Cost Settings")]
    public bool useMoney = true;   // true = money, false = key
    public int costAmount = 0;     // set by CardShopSystem

    private CardShopSystem shopSystem;
    private Card currentCardInstance;
    private Card assignedCardPrefab;

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

    void Start()
    {
        Invoke("SetupCollider", 0.1f);
    }

    void SetupCollider()
    {
        if (slotCollider != null && rectTransform != null)
        {
            slotCollider.isTrigger = true;
            slotCollider.size = rectTransform.rect.size;
            slotCollider.offset = Vector2.zero;
        }
    }

    public void Initialize(CardShopSystem system, Card cardPrefab)
    {
        shopSystem = system;
        assignedCardPrefab = cardPrefab;
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

        // Disable interactions
        CanvasGroup canvasGroup = currentCardInstance.GetComponent<CanvasGroup>();
        if (canvasGroup == null)
            canvasGroup = currentCardInstance.gameObject.AddComponent<CanvasGroup>();

        canvasGroup.blocksRaycasts = false;
        canvasGroup.interactable = false;

        foreach (var collider in currentCardInstance.GetComponentsInChildren<Collider2D>())
            collider.enabled = false;

        Button button = currentCardInstance.GetComponent<Button>();
        if (button != null) button.enabled = false;

        Image[] images = currentCardInstance.GetComponentsInChildren<Image>();
        foreach (var img in images)
            img.raycastTarget = false;
    }

    public void ClearCard()
    {
        if (currentCardInstance != null)
        {
            Destroy(currentCardInstance.gameObject);
            currentCardInstance = null;
        }
    }

    void CheckAffordability()
    {
        if (MoneyKeySystem.Instance != null)
            canAfford = !isPurchased && (useMoney ?
                MoneyKeySystem.Instance.CanAfford(costAmount, 0) :
                MoneyKeySystem.Instance.CanAfford(0, costAmount));
    }

    void UpdateCostDisplay()
    {
        if (costPanel != null && costText != null)
        {
            costPanel.SetActive(!isPurchased);

            string text = "";
            if (useMoney) text = $"{costAmount}G";
            else text = $"{costAmount}K";

            costText.text = text;
        }
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
            slotBackground.color = affordableColor;
        else
            slotBackground.color = unaffordableColor;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (isPurchased) return;

        if (!canAfford) return;

        bool purchased = useMoney ?
            MoneyKeySystem.Instance.MakePurchase(costAmount, 0) :
            MoneyKeySystem.Instance.MakePurchase(0, costAmount);

        if (!purchased) return;

        isPurchased = true;
        UpdateVisualState();
        UpdateCostDisplay();

        shopSystem?.OnCardPurchasedHandler(assignedCardPrefab, this);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (isPurchased) return;
        if (slotBackground != null)
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
        UpdateCostDisplay();
        UpdateVisualState();
    }

    void OnEnable()
    {
        if (MoneyKeySystem.Instance != null)
            MoneyKeySystem.Instance.OnValuesChanged += OnResourcesChanged;
    }

    void OnDisable()
    {
        if (MoneyKeySystem.Instance != null)
            MoneyKeySystem.Instance.OnValuesChanged -= OnResourcesChanged;
    }

    void OnResourcesChanged(int money, int keys)
    {
        CheckAffordability();
        UpdateVisualState();
        UpdateCostDisplay();
    }
}
