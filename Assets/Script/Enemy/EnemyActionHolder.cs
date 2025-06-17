using UnityEngine;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif

[ExecuteInEditMode]
public class EnemyActionHolder : MonoBehaviour
{
    [Header("Card Settings")]
    [SerializeField] private List<GameObject> _cardPrefabs = new List<GameObject>();
    [SerializeField] private Transform _holder;

    private List<Card> _actionCards = new List<Card>();
    private GameObject _activeDisplay;
    private bool _isInitialized;

    private void Awake()
    {
        Initialize();
    }

    private void OnEnable()
    {
        Initialize();
    }

    private void Initialize()
    {
        if (_isInitialized) return;

        InitializeHolder();
        CleanupStrayObjects();
        InitializeCards();
        ShowNextAction();
        _isInitialized = true;
    }

    private void InitializeHolder()
    {
        if (_holder == null)
        {
            _holder = new GameObject("CardHolder").transform;
            _holder.SetParent(transform, false);
            _holder.localPosition = Vector3.zero;
        }
    }

    private void CleanupStrayObjects()
    {
        if (_holder == null) return;

        var children = new List<GameObject>();
        foreach (Transform child in _holder)
        {
            if (child == null) continue;

#if UNITY_EDITOR
            if (PrefabUtility.IsPartOfPrefabAsset(child.gameObject))
                continue;
#endif

            children.Add(child.gameObject);
        }

        foreach (var child in children)
        {
            if (child == null) continue;

            if (Application.isPlaying)
            {
                Destroy(child);
            }
            else
            {
#if UNITY_EDITOR
                if (!EditorUtility.IsPersistent(child) && child != null)
                {
                    DestroyImmediate(child);
                }
#endif
            }
        }
    }

    private void InitializeCards()
    {
        _actionCards.Clear();

        if (_cardPrefabs == null || _cardPrefabs.Count == 0)
        {
            Debug.LogWarning($"{name}: No card prefabs assigned", this);
            return;
        }

        foreach (var prefab in _cardPrefabs)
        {
            if (prefab == null)
            {
                Debug.LogWarning($"{name}: Null prefab in card prefabs list", this);
                continue;
            }

            var cardObj = Instantiate(prefab);
            if (cardObj != null && _holder != null)
            {
                cardObj.transform.SetParent(_holder, false);
                cardObj.name = $"{prefab.name}_Instance";
                cardObj.SetActive(false);

                var card = cardObj.GetComponent<Card>();
                if (card != null)
                {
                    _actionCards.Add(card);
                }
                else
                {
                    Debug.LogWarning($"{name}: Prefab {prefab.name} is missing Card component", this);
                    SafeDestroy(cardObj);
                }
            }
        }
    }

    public Card GetCurrentAction()
    {
        if (_actionCards.Count == 0)
        {
            Debug.LogWarning($"{name}: No actions available", this);
            return null;
        }
        return _actionCards[0];
    }

    public void RotateCurrentAction()
    {
        if (_actionCards.Count <= 1) return;

        if (_activeDisplay != null)
        {
            _activeDisplay.SetActive(false);
            _activeDisplay = null;
        }

        var firstCard = _actionCards[0];
        _actionCards.RemoveAt(0);
        _actionCards.Add(firstCard);
        firstCard.transform.SetAsLastSibling();

        ShowNextAction();
    }

    private void ShowNextAction()
    {
        if (_actionCards.Count == 0)
        {
            Debug.Log($"{name}: No cards to display");
            return;
        }

        if (_activeDisplay != null)
        {
            _activeDisplay.SetActive(false);
        }

        _activeDisplay = _actionCards[0].gameObject;
        _activeDisplay.SetActive(true);
        ResetTransform(_activeDisplay.transform);
    }

    private void ResetTransform(Transform t)
    {
        t.localPosition = Vector3.zero;
        t.localRotation = Quaternion.identity;
        t.localScale = Vector3.one;
    }

    private void SafeDestroy(Object obj)
    {
        if (obj == null) return;

        if (Application.isPlaying)
        {
            Destroy(obj);
        }
        else
        {
#if UNITY_EDITOR
            if (!EditorUtility.IsPersistent(obj))
            {
                DestroyImmediate(obj);
            }
#endif
        }
    }

    private void OnDestroy()
    {
        CleanupStrayObjects();
    }
}