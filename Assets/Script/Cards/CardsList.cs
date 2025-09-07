using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[System.Serializable]
public class CardsList : MonoBehaviour
{
    public List<Card> cardPrefabs = new List<Card>(); // Prefab references
    public List<Card> Cards = new List<Card>(); // Actual card instances

    void Start()
    {
        InitializeDeck();
    }

    // Initialize the deck by creating instances from prefabs
    public void InitializeDeck()
    {
        ClearAllCards(); // Clear any existing instances first

        foreach (Card cardPrefab in cardPrefabs)
        {
            if (cardPrefab != null)
            {
                AddCardFromPrefab(cardPrefab);
            }
        }

        Debug.Log($"Initialized deck with {Cards.Count} cards");
    }

    // Add a card instance from a prefab
    public void AddCardFromPrefab(Card cardPrefab)
    {
        if (cardPrefab == null) return;

        Card newCard = Instantiate(cardPrefab, transform); // Create as child of this object
        newCard.gameObject.SetActive(false);
        newCard.name = cardPrefab.name; // Remove "(Clone)" from name
        Cards.Add(newCard);

        Debug.Log($"Added instance of {cardPrefab.name} to deck");
    }

    // Add an existing card instance
    public void AddCardInstance(Card cardInstance)
    {
        if (cardInstance != null && cardInstance.gameObject != null)
        {
            cardInstance.transform.SetParent(transform); // Make it a child
            cardInstance.gameObject.SetActive(false);
            Cards.Add(cardInstance);
            Debug.Log($"Added card instance {cardInstance.name} to deck");
        }
    }

    // Method to get all active card instances for gameplay
    public List<Card> GetActiveCards()
    {
        CleanupNullCards();
        return Cards.Where(card => card != null && card.gameObject != null).ToList();
    }

    // Method to clear destroyed or null cards
    public void CleanupNullCards()
    {
        int removed = Cards.RemoveAll(card => card == null || card.gameObject == null);
        if (removed > 0)
        {
            Debug.Log($"Cleaned up {removed} null cards from deck");
        }
    }

    // Clear all cards (destroy instances)
    public void ClearAllCards()
    {
        foreach (Card card in Cards)
        {
            if (card != null && card.gameObject != null)
            {
                // Cancel any animations first
                if (LeanTween.isTweening(card.gameObject))
                {
                    LeanTween.cancel(card.gameObject);
                }
                Destroy(card.gameObject);
            }
        }
        Cards.Clear();
        Debug.Log("Cleared all cards from deck");
    }

    // Get card count
    public int GetCount()
    {
        CleanupNullCards();
        return Cards.Count;
    }

    // Check if deck contains a card of specific type
    public bool ContainsCardType(Card cardPrefab)
    {
        if (cardPrefab == null) return false;

        CleanupNullCards();
        return Cards.Any(card => card != null && card.name.Replace("(Clone)", "").Trim() == cardPrefab.name);
    }

    // Optional: Method to shuffle the deck
    public void Shuffle()
    {
        CleanupNullCards();
        Cards = Cards.OrderBy(x => Random.value).ToList();
        Debug.Log("Shuffled deck");
    }

    // Optional: Method to draw a card
    public Card DrawCard()
    {
        CleanupNullCards();
        if (Cards.Count == 0) return null;

        Card drawnCard = Cards[0];
        Cards.RemoveAt(0);
        return drawnCard;
    }
}