using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class CardRewardSystem : MonoBehaviour
{
    [Header("Settings")]
    public int rewardsToShow = 3;
    public List<GameObject> cardPrefabs = new List<GameObject>();

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
            rewardSlots[i].gameObject.SetActive(true);
            rewardSlots[i].Initialize(this, randomCards[i]);
        }
    }

    public void OnCardSelected(GameObject cardPrefab)
    {
        GameObject newCard = Instantiate(cardPrefab);
        Card cardComponent = newCard.GetComponent<Card>();

        if (cardComponent != null)
        {
            playerDeck.Cards.Add(cardComponent);
            newCard.SetActive(false); // Hide until played
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
        return true;
    }

    private List<GameObject> GetRandomCards()
    {
        return cardPrefabs
            .OrderBy(x => Random.value)
            .Take(rewardsToShow)
            .ToList();
    }

    public void HideRewards()
    {
        rewardUI.SetActive(false);
        rewardSlots.ForEach(slot => {
            slot.ClearCard();
            slot.gameObject.SetActive(false);
        });
    }
}