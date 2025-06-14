using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class CardRewardSystem : MonoBehaviour
{
    [Header("Settings")]
    public int rewardsToShow = 3;
    public List<GameObject> cardPrefabs = new List<GameObject>();

    [Header("References")]
    public Transform slotsParent; // Parent of all slot objects
    public GameObject rewardUI; // The entire UI panel to show/hide
    public CardsList playerDeck; // Reference to player's deck

    private List<CardRewardSlot> rewardSlots = new List<CardRewardSlot>();

    void Start()
    {
        // Cache all slots
        rewardSlots = slotsParent.GetComponentsInChildren<CardRewardSlot>(true).ToList();
        rewardUI.SetActive(false);
    }

    public void ShowRewards()
    {
        if (!ValidateCanGenerate()) return;

        rewardUI.SetActive(true);

        // Get random cards and populate slots
        List<GameObject> randomCards = GetRandomCards();
        for (int i = 0; i < rewardsToShow; i++)
        {
            rewardSlots[i].gameObject.SetActive(true);
            rewardSlots[i].Initialize(this);
            rewardSlots[i].SpawnCard(randomCards[i]);
        }
    }

    private bool ValidateCanGenerate()
    {
        if (cardPrefabs.Count < rewardsToShow)
        {
            Debug.LogError("Not enough card prefabs!");
            return false;
        }
        return true;
    }

    private List<GameObject> GetRandomCards()
    {
        return cardPrefabs
            .OrderBy(x => Random.value)
            .Take(rewardsToShow)
            .ToList();
    }

    public void OnCardSelected(GameObject selectedCardObject)
    {
        // Get the original prefab name (without clone)
        string cardName = selectedCardObject.name.Replace("(Clone)", "").Trim();

        // Find the original prefab in your list
        GameObject cardPrefab = cardPrefabs.Find(p => p.name == cardName);
        if (cardPrefab == null)
        {
            Debug.LogError($"Card prefab not found: {cardName}");
            HideRewards();
            return;
        }

        // Create a NEW instance from the original prefab
        GameObject newCardInstance = Instantiate(cardPrefab);
        newCardInstance.name = cardName; // Clean name

        // Get the Card component
        Card cardComponent = newCardInstance.GetComponent<Card>();
        if (cardComponent == null)
        {
            Debug.LogError($"No Card component on {cardName}");
            Destroy(newCardInstance);
            HideRewards();
            return;
        }

        // Add to player deck
        try
        {
            playerDeck.Cards.Add(cardComponent);
            Debug.Log($"Successfully added {cardName} to deck. Deck now has {playerDeck.Cards.Count} cards.");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Failed to add card: {e.Message}");
        }

        // Optional: Initialize the card if needed
        // cardComponent.Initialize(); 

        HideRewards();
    }

    public void HideRewards()
    {
        rewardUI.SetActive(false);
        // Clear all slots
        rewardSlots.ForEach(slot => {
            slot.ClearCard();
            slot.gameObject.SetActive(false);
        });
    }
}