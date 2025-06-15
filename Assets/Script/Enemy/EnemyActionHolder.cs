using UnityEngine;
using System.Collections.Generic;

public class EnemyActionHolder : MonoBehaviour
{
    [Header("Card Settings")]
    [SerializeField] private List<GameObject> _cardPrefabs;
    [SerializeField] private Transform _holder;

    private List<Card> _actionCards = new List<Card>();
    private GameObject _activeDisplay;

    private void Awake()
    {
        if (_holder == null) _holder = transform;
        InitializeCards();
        ShowNextAction();
    }

    private void InitializeCards()
    {
        // Clear existing
        foreach (Transform child in _holder)
            Destroy(child.gameObject);
        _actionCards.Clear();

        // Create new cards (but keep them inactive)
        foreach (var prefab in _cardPrefabs)
        {
            if (prefab == null) continue;

            var cardObj = Instantiate(prefab, _holder);
            var card = cardObj.GetComponent<Card>();
            if (card != null)
            {
                _actionCards.Add(card);
                cardObj.SetActive(false); // Start inactive
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

        // Hide current
        if (_activeDisplay != null)
            _activeDisplay.SetActive(false);

        // Rotate queue
        var firstCard = _actionCards[0];
        _actionCards.RemoveAt(0);
        _actionCards.Add(firstCard);
        firstCard.transform.SetAsLastSibling();

        ShowNextAction();
    }

    private void ShowNextAction()
    {
        // Hide previous
        if (_activeDisplay != null)
            _activeDisplay.SetActive(false);

        // Show next
        if (_actionCards.Count > 0)
        {
            _activeDisplay = _actionCards[0].gameObject;
            _activeDisplay.SetActive(true);

            // Reset transform in case it was moved
            _activeDisplay.transform.localPosition = Vector3.zero;
            _activeDisplay.transform.localRotation = Quaternion.identity;
        }
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        if (!Application.isPlaying && _holder != null && _cardPrefabs.Count > 0)
        {
            // Clear preview
            foreach (Transform child in _holder)
                DestroyImmediate(child.gameObject);

            // Create editor preview
            var preview = Instantiate(_cardPrefabs[0], _holder);
            preview.hideFlags = HideFlags.HideAndDontSave;
        }
    }
#endif
}