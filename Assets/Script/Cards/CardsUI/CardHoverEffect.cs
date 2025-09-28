using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

public class CardHoverEffect : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private float cardScaleMultiplier = 1.2f;
    [SerializeField] private float scaleDuration = 0.2f;
    [SerializeField] private float liftDuration = 0.2f;

    private Vector3 _originalScale;
    private RectTransform _cardRect;
    private Card _card;
    private Coroutine _effectCoroutine;
    private bool _isHovering;

    private float hoverY = 200f;
    private float normalY = 0f;

    void Awake()
    {
        _card = GetComponentInParent<Card>();
        if (_card != null)
        {
            _cardRect = _card.transform as RectTransform;
            _originalScale = _cardRect.localScale;
            // Set initial Y to normal
            _cardRect.localPosition = new Vector3(_cardRect.localPosition.x, normalY, _cardRect.localPosition.z);
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (_cardRect == null || _card.transform.parent.tag != "Hand") return;

        _isHovering = true;
        ApplyHoverEffect(true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (_cardRect == null || _card.transform.parent.tag != "Hand") return;

        _isHovering = false;
        ApplyHoverEffect(false);
    }

    private void ApplyHoverEffect(bool hover)
    {
        if (_cardRect == null) return;

        if (_effectCoroutine != null)
        {
            StopCoroutine(_effectCoroutine);
            _effectCoroutine = null;
        }

        Vector3 targetScale = hover ? _originalScale * cardScaleMultiplier : _originalScale;
        float targetY = hover ? hoverY : normalY;

        if (gameObject.activeInHierarchy)
            _effectCoroutine = StartCoroutine(AnimateTo(targetScale, targetY, hover ? liftDuration : scaleDuration));
        else
        {
            _cardRect.localScale = targetScale;
            _cardRect.localPosition = new Vector3(_cardRect.localPosition.x, targetY, _cardRect.localPosition.z);
        }
    }

    private IEnumerator AnimateTo(Vector3 targetScale, float targetY, float duration)
    {
        Vector3 startScale = _cardRect.localScale;
        float startY = _cardRect.localPosition.y;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / duration);

            _cardRect.localScale = Vector3.Lerp(startScale, targetScale, t);
            _cardRect.localPosition = new Vector3(_cardRect.localPosition.x, Mathf.Lerp(startY, targetY, t), _cardRect.localPosition.z);

            yield return null;
        }

        _cardRect.localScale = targetScale;
        _cardRect.localPosition = new Vector3(_cardRect.localPosition.x, targetY, _cardRect.localPosition.z);
        _effectCoroutine = null;
    }
}
