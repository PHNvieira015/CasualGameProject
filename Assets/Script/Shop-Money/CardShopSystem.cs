using System.Collections.Generic;
using UnityEngine;

public class CardShopSystem : MonoBehaviour
{
    [Header("Shop Slots (Assign in Inspector)")]
    public List<CardShopSlot> cardSlots = new List<CardShopSlot>();
    public List<CardShopSlot> relicSlots = new List<CardShopSlot>();

    [Header("Shop Configuration")]
    public List<Card> availableShopCards = new List<Card>();
    public List<Card> availableRelics = new List<Card>();

    [Header("Player Deck Reference")]
    public CardsList playerDeck;
    public CardsList playerRelics;

    [Header("Shop Refresh")]
    public int maxRefreshesPerDay = 3;
    public int refreshCost = 25;

    private int currentRefreshes = 0;

    public delegate void CardPurchasedEventHandler(Card purchasedCard, CardShopSlot slot);
    public event CardPurchasedEventHandler CardPurchased;
    public event CardPurchasedEventHandler RelicPurchased;

    private void Start() => InitializeShop();

    public void InitializeShop()
    {
        // Clear all slots visually
        foreach (var slot in cardSlots) slot.ClearCard();
        foreach (var slot in relicSlots) slot.ClearCard();

        // Populate card slots
        foreach (var slot in cardSlots)
        {
            if (availableShopCards.Count == 0) break;

            Card randomCard = availableShopCards[Random.Range(0, availableShopCards.Count)];
            int randomCost = Random.Range(10, 101); // Random money cost between 10-100
            slot.Initialize(this, randomCard, randomCost, 0, CostType.Money);
        }

        // Populate relic slots
        foreach (var slot in relicSlots)
        {
            if (availableRelics.Count == 0) break;

            Card randomRelic = availableRelics[Random.Range(0, availableRelics.Count)];
            int randomKeyCost = Random.Range(1, 3); // 1 or 2 keys
            slot.Initialize(this, randomRelic, 0, randomKeyCost, CostType.Key);
        }
    }

    public void OnCardPurchasedHandler(Card card, CardShopSlot slot, bool isRelic = false)
    {
        if (isRelic) RelicPurchased?.Invoke(card, slot);
        else CardPurchased?.Invoke(card, slot);
    }

    public void RefreshShop()
    {
        if (currentRefreshes >= maxRefreshesPerDay) return;
        if (!MoneyKeySystem.Instance.SpendMoney(refreshCost)) return;

        currentRefreshes++;
        InitializeShop();
    }
}
