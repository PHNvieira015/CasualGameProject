using UnityEngine;
using System.Collections.Generic;

public class CardShopSystem : MonoBehaviour
{
    [Header("Shop Slots (Assign in Inspector)")]
    public List<CardShopSlot> cardSlots = new List<CardShopSlot>();
    public List<CardShopSlot> relicSlots = new List<CardShopSlot>();

    [Header("Shop Configuration")]
    public List<Card> availableShopCards = new List<Card>();
    public List<Card> availableRelics = new List<Card>();

    [Header("Pricing")]
    public int baseCardCost = 50;
    public int baseRelicCost = 100;

    public Vector2Int cardCostVariance = new Vector2Int(-10, 20);
    public Vector2Int relicCostVariance = new Vector2Int(-20, 30);

    [Header("Shop Refresh")]
    public int maxRefreshesPerDay = 3;
    public int refreshCost = 25;

    private int currentRefreshes = 0;

    public delegate void CardPurchasedEventHandler(Card purchasedCard, CardShopSlot slot);
    public event CardPurchasedEventHandler CardPurchased;
    public event CardPurchasedEventHandler RelicPurchased;

    private void Start()
    {
        InitializeShop();
    }

    public void InitializeShop()
    {
        // Clear slots visually
        foreach (var slot in cardSlots) slot.ResetSlot();
        foreach (var slot in relicSlots) slot.ResetSlot();

        // Populate card slots
        foreach (var slot in cardSlots)
        {
            if (availableShopCards.Count == 0) break;

            Card randomCard = availableShopCards[Random.Range(0, availableShopCards.Count)];

            // Randomize cost
            int randomizedCost = baseCardCost + Random.Range(cardCostVariance.x, cardCostVariance.y + 1);
            randomizedCost = Mathf.Max(1, randomizedCost);

            // Assign cost to slot
            slot.useMoney = true; // true = money, false = key (set in inspector if needed)
            slot.costAmount = randomizedCost;

            slot.Initialize(this, randomCard);
        }

        // Populate relic slots
        foreach (var slot in relicSlots)
        {
            if (availableRelics.Count == 0) break;

            Card randomRelic = availableRelics[Random.Range(0, availableRelics.Count)];

            int randomizedCost = baseRelicCost + Random.Range(relicCostVariance.x, relicCostVariance.y + 1);
            randomizedCost = Mathf.Max(1, randomizedCost);

            slot.useMoney = true; // or false for key
            slot.costAmount = randomizedCost;

            slot.Initialize(this, randomRelic);
        }
    }

    public void OnCardPurchasedHandler(Card card, CardShopSlot slot, bool isRelic = false)
    {
        if (isRelic)
            RelicPurchased?.Invoke(card, slot);
        else
            CardPurchased?.Invoke(card, slot);
    }

    public void RefreshShop()
    {
        if (currentRefreshes >= maxRefreshesPerDay) return;
        if (!MoneyKeySystem.Instance.SpendMoney(refreshCost)) return;

        currentRefreshes++;
        InitializeShop();
    }
}
