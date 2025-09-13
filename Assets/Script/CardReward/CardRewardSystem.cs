using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class CardRewardSystem : MonoBehaviour
{
    [Header("Settings")]
    public int rewardsToShow = 3;
    public List<Card> cardPrefabs = new List<Card>(); // Changed from GameObject to Card

    [Header("References")]
    public Transform slotsParent;
    public GameObject rewardUI;
    public CardsList playerDeck;

    private List<CardRewardSlot> rewardSlots = new List<CardRewardSlot>();

    void Start()
    {
        rewardSlots = slotsParent.GetComponentsInChildren<CardRewardSlot>(true).ToList();
        rewardUI.SetActive(false);
    }

    public void ShowRewards()
    {
        if (!ValidateCanGenerate()) return;

        rewardUI.SetActive(true);
        var randomCards = GetRandomCards();

        for (int i = 0; i < rewardsToShow; i++)
        {
            if (i < rewardSlots.Count && randomCards[i] != null)
            {
                rewardSlots[i].gameObject.SetActive(true);
                rewardSlots[i].Initialize(this, randomCards[i]);
            }
        }
    }

    public void OnCardSelected(Card cardPrefab) // Changed parameter type
    {
        if (cardPrefab == null)
        {
            Debug.LogError("Cannot instantiate null card prefab!");
            HideRewards();
            return;
        }

        // Check if the prefab is valid (not destroyed)
        if (cardPrefab == null || cardPrefab.gameObject == null)
        {
            Debug.LogError("Card prefab is destroyed or invalid!");
            HideRewards();
            return;
        }

        // Instantiate the card directly as a Card component
        Card newCard = Instantiate(cardPrefab);

        if (newCard != null)
        {
            playerDeck.Cards.Add(newCard);
            newCard.gameObject.SetActive(false); // Hide until played
            Debug.Log($"Added {newCard.name} to player deck");
        }
        else
        {
            Debug.LogError("Failed to instantiate card!");
        }

        HideRewards();
    }

    private bool ValidateCanGenerate()
    {
        if (cardPrefabs.Count < rewardsToShow)
        {
            Debug.LogError("Not enough card prefabs!");
            return false;
        }

        // Check if any prefabs are null or destroyed
        if (cardPrefabs.Any(prefab => prefab == null || prefab.gameObject == null))
        {
            Debug.LogError("Some card prefabs are null or destroyed!");
            return false;
        }

        return true;
    }

    private List<Card> GetRandomCards() // Changed return type
    {
        // Filter out any null or destroyed prefabs first
        var validPrefabs = cardPrefabs.Where(prefab => prefab != null && prefab.gameObject != null).ToList();

        if (validPrefabs.Count < rewardsToShow)
        {
            Debug.LogError($"Not enough valid card prefabs! Need {rewardsToShow}, have {validPrefabs.Count}");
            return new List<Card>();
        }

        return validPrefabs
            .OrderBy(x => Random.value)
            .Take(rewardsToShow)
            .ToList();
    }

    public void HideRewards()
    {
        rewardUI.SetActive(false);
        foreach (var slot in rewardSlots)
        {
            if (slot != null)
            {
                slot.ClearCard();
                slot.gameObject.SetActive(false);
            }
        }
    }

    // Optional: Method to refresh card prefabs list (remove null entries)
    public void RefreshCardPrefabs()
    {
        cardPrefabs = cardPrefabs.Where(prefab => prefab != null && prefab.gameObject != null).ToList();
        Debug.Log($"Refreshed card prefabs list. Now contains {cardPrefabs.Count} valid prefabs.");
    }
}