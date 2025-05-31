using UnityEngine;
using UnityEngine.EventSystems;

public class SelectableUnit : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [Header("Visual Settings")]
    public Color highlightedColor = Color.red;
    [SerializeField] private string childObjectName = ""; // Optional: specify which child to search

    private SpriteRenderer _spriteRenderer;
    private Color _originalColor;

    private void Awake()
    {
        // Try to get SpriteRenderer from specified child or any child
        if (!string.IsNullOrEmpty(childObjectName))
        {
            Transform child = transform.Find(childObjectName);
            if (child != null)
            {
                _spriteRenderer = child.GetComponent<SpriteRenderer>();
            }
        }

        // If not found by name or name not specified, get from any child
        if (_spriteRenderer == null)
        {
            _spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        }

        if (_spriteRenderer == null)
        {
            Debug.LogError("No SpriteRenderer found in children!", this);
            enabled = false; // Disable the script if no renderer found
            return;
        }

        _originalColor = _spriteRenderer.color;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (_spriteRenderer != null)
        {
            _spriteRenderer.color = highlightedColor;
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (_spriteRenderer != null)
        {
            _spriteRenderer.color = _originalColor;
        }
    }

    // Optional: Add methods to change the target child at runtime
    public void SetTargetChild(string childName)
    {
        Transform child = transform.Find(childName);
        if (child != null)
        {
            var newRenderer = child.GetComponent<SpriteRenderer>();
            if (newRenderer != null)
            {
                _spriteRenderer = newRenderer;
                _originalColor = _spriteRenderer.color;
            }
        }
    }
}