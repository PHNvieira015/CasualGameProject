using UnityEngine;
using System.Collections.Generic;
using System.Linq;

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

        // Validate that we're using prefab assets, not scene instances
        ValidatePrefabReferences();
    }

    // This method checks if we're using proper prefab assets
    private void ValidatePrefabReferences()
    {
        List<Card> validPrefabs = new List<Card>();

        foreach (Card card in cardPrefabs)
        {
            if (card != null)
            {
                // Check if this is a prefab asset (not a scene instance)
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

    // Helper method to check if an object is a prefab asset
    private bool IsPrefabAsset(Card card)
    {
        // Prefab assets won't have a scene (they exist outside scenes)
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
            Debug.LogError("Cannot instantiate null card prefab!");
            HideRewards();
            return;
        }

        // Instantiate from the prefab asset
        Card newCard = Instantiate(cardPrefab);

        if (newCard != null)
        {
            playerDeck.Cards.Add(newCard);
            newCard.gameObject.SetActive(false);
            Debug.Log($"Added {newCard.name} to player deck");
        }

        HideRewards();
    }

    private bool ValidateCanGenerate()
    {
        RemoveNullPrefabs();

        if (cardPrefabs.Count < rewardsToShow)
        {
            Debug.LogError($"Not enough card prefabs! Need {rewardsToShow}, have {cardPrefabs.Count}");
            return false;
        }

        return true;
    }

    private List<Card> GetRandomCards()
    {
        RemoveNullPrefabs();

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

    public void RemoveNullPrefabs()
    {
        cardPrefabs.RemoveAll(prefab => prefab == null);
    }

    // Editor method to help with setup (call this from editor scripts if needed)
#if UNITY_EDITOR
    public void FindAllCardPrefabsInProject()
    {
        RemoveNullPrefabs();

        // Find all Card prefabs in the project
        string[] guids = UnityEditor.AssetDatabase.FindAssets("t:Prefab");
        foreach (string guid in guids)
        {
            string path = UnityEditor.AssetDatabase.GUIDToAssetPath(guid);
            GameObject prefab = UnityEditor.AssetDatabase.LoadAssetAtPath<GameObject>(path);

            if (prefab != null)
            {
                Card card = prefab.GetComponent<Card>();
                if (card != null && !cardPrefabs.Contains(card))
                {
                    cardPrefabs.Add(card);
                }
            }
        }

        Debug.Log($"Found {cardPrefabs.Count} card prefabs in project");
    }
#endif
}