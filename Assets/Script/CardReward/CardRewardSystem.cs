using UnityEngine;
using System.Collections.Generic;
using System.Linq;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class CardRewardSystem : MonoBehaviour
{
    [Header("Settings")]
    public int rewardsToShow = 3;
    public List<Card> cardPrefabs = new List<Card>();

    [Header("References")]
    public Transform slotsParent;
    public GameObject rewardUI;
    public CardsList playerDeck;

    private List<CardRewardSlot> rewardSlots = new List<CardRewardSlot>();

    void Start()
    {
        rewardSlots = slotsParent.GetComponentsInChildren<CardRewardSlot>(true).ToList();
        rewardUI.SetActive(false);


    private void ValidatePrefabReferences()
    {
        List<Card> validPrefabs = new List<Card>();

        foreach (Card card in cardPrefabs)
        {
            if (card != null)
            {
                if (IsPrefabAsset(card))
                {
                    validPrefabs.Add(card);
                }
                else
                {
                    Debug.LogWarning($"Card {card.name} is a scene instance, not a prefab asset. This will cause issues.");
                }
            }
        }

        cardPrefabs = validPrefabs;

        if (cardPrefabs.Count == 0)
        {
            Debug.LogError("No valid card prefabs found! Make sure to assign prefabs from Project folder, not scene objects.");
        }
    }

    private bool IsPrefabAsset(Card card)
    {
        return card.gameObject.scene.name == null;
    }

    public void ShowRewards()
    {
        if (!ValidateCanGenerate()) return;

        rewardUI.SetActive(true);
        var randomCards = GetRandomCards();

        for (int i = 0; i < rewardsToShow && i < rewardSlots.Count; i++)
        {
            if (randomCards[i] != null)
            {
                rewardSlots[i].gameObject.SetActive(true);
                rewardSlots[i].Initialize(this, randomCards[i]);
            }
        }
    }

    public void OnCardSelected(Card cardPrefab)
    {
        if (cardPrefab == null)
        {
            Debug.LogError("Cannot add null card prefab!");
            HideRewards();
            return;
        }

        // CREATE A NEW INSTANCE OF THE CARD (but don't activate it yet)
        Card newCardInstance = Instantiate(cardPrefab);

        if (newCardInstance != null)
        {
            // Set the card to inactive - it will be activated when drawn from the deck
            newCardInstance.gameObject.SetActive(false);

            // Add the INSTANCE to the deck, not the prefab
            playerDeck.Cards.Add(newCardInstance);
            Debug.Log($"Added new instance of {cardPrefab.name} to player deck");
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
            Debug.LogError($"Not enough card prefabs! Need {rewardsToShow}, have {cardPrefabs.Count}");
            return false;
        }

        return true;
    }

    private List<Card> GetRandomCards()
    {
        return cardPrefabs
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
