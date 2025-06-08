using UnityEngine;
using System.Collections.Generic;

public class EnemyActionHolder : MonoBehaviour
{
    [SerializeField] private List<GameObject> _cardPrefabs;
    [SerializeField] private Transform _holder;

    private List<Card> _actionCards = new List<Card>();

    private void Awake()
    {
        if (_holder == null) _holder = transform;
        InitializeCards();
    }

    private void InitializeCards()
    {
        // Clear existing
        foreach (Transform child in _holder)
            Destroy(child.gameObject);
        _actionCards.Clear();

        // Create new cards
        foreach (var prefab in _cardPrefabs)
        {
            if (prefab == null) continue;

            var cardObj = Instantiate(prefab, _holder);
            var card = cardObj.GetComponent<Card>();
            if (card != null)
            {
                _actionCards.Add(card);
                cardObj.transform.SetAsLastSibling();
            }
        }
    }

    public Card GetCurrentAction()
    {
        return _actionCards.Count > 0 ? _actionCards[0] : null;
    }

    public void RotateCurrentAction()
    {
        if (_actionCards.Count <= 1) return;

        // Move first card to end
        var firstCard = _actionCards[0];
        _actionCards.RemoveAt(0);
        _actionCards.Add(firstCard);

        // Update hierarchy order
        firstCard.transform.SetAsLastSibling();
    }
}