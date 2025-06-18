using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using DG.Tweening;

[RequireComponent(typeof(Button), typeof(Image))]
public class NodeVisual : MonoBehaviour, IPointerClickHandler
{
    [Header("Visual Settings")]
    [SerializeField] private GameObject selectionCircle;
    [SerializeField] private Color selectableColorModifier = new Color(1.2f, 1.2f, 1.2f, 1f);
    [SerializeField] private Color unselectableColorModifier = new Color(0.7f, 0.7f, 0.7f, 1f);

    [Header("Animation Settings")]
    [SerializeField] private float highlightScaleAmount = 1.1f;
    [SerializeField][Range(0.1f, 2f)] private float highlightPulseSpeed = 1f;
    [SerializeField][Range(0.1f, 2f)] private float clickAnimationDuration = 0.3f;
    [SerializeField] private float clickPunchStrength = 0.1f;

    private MapNode node;
    private bool isSelectable;
    private Image nodeImage;
    private Vector3 originalScale;
    private Color originalColor;
    private Sequence highlightSequence;

    private void Awake()
    {
        nodeImage = GetComponent<Image>();
        originalScale = transform.localScale;
        originalColor = nodeImage != null ? nodeImage.color : Color.white;

        if (selectionCircle != null)
            selectionCircle.SetActive(false);
    }

    public void Setup(MapNode node, bool isSelectable)
    {
        if (!gameObject.activeSelf)
            gameObject.SetActive(true);

        this.node = node;
        this.isSelectable = isSelectable;

        if (nodeImage == null)
            nodeImage = GetComponent<Image>();

        if (node?.nodeBlueprint != null && nodeImage != null)
        {
            nodeImage.sprite = node.nodeBlueprint.sprite;
            nodeImage.color = originalColor = node.nodeBlueprint.color;
        }

        SetSelected(false);
        SetSelectable(isSelectable);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (isSelectable && node != null)
        {
            var manager = FindObjectOfType<GameMapManager>();
            if (manager != null)
                manager.OnNodeClicked(node);

            PlayClickAnimation();
        }
    }

    public void SetSelectable(bool selectable)
    {
        isSelectable = selectable;

        if (nodeImage == null)
            return;

        nodeImage.color = originalColor * (selectable ?
            selectableColorModifier :
            unselectableColorModifier);

        if (selectable)
        {
            PlayHighlightAnimation();
        }
        else
        {
            StopHighlightAnimation();
        }
    }

    public void SetSelected(bool selected)
    {
        if (selectionCircle != null)
            selectionCircle.SetActive(selected);
    }

    private void PlayHighlightAnimation()
    {
        StopHighlightAnimation();

        if (!gameObject.activeInHierarchy)
            return;

        float duration = 1f / highlightPulseSpeed;

        highlightSequence = DOTween.Sequence()
            .Append(transform.DOScale(originalScale * highlightScaleAmount, duration / 2))
            .Append(transform.DOScale(originalScale, duration / 2))
            .SetLoops(-1, LoopType.Yoyo)
            .SetEase(Ease.InOutSine);
    }

    // Changed from private to public
    public void StopHighlightAnimation()
    {
        if (highlightSequence != null)
        {
            highlightSequence.Kill();
            highlightSequence = null;
        }

        if (this != null && transform != null)
            transform.localScale = originalScale;
    }

    private void PlayClickAnimation()
    {
        if (!gameObject.activeInHierarchy)
            return;

        transform.DOPunchScale(
            Vector3.one * clickPunchStrength,
            clickAnimationDuration,
            vibrato: 1,
            elasticity: 0.5f
        );
    }

    private void OnDisable()
    {
        StopHighlightAnimation();
    }

    private void OnDestroy()
    {
        StopHighlightAnimation();
    }
}