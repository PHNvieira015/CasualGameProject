using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

[RequireComponent(typeof(Button), typeof(Image))]
public class NodeVisual : MonoBehaviour, IPointerClickHandler
{
    [Header("References")]
    [SerializeField] private GameObject selectionHighlight;

    private MapNode node;
    private bool isSelectable;
    private Image nodeImage;

    private void Awake()
    {
        nodeImage = GetComponent<Image>();
    }

    public void Setup(MapNode node, bool isSelectable)
    {
        this.node = node;
        this.isSelectable = isSelectable;

        if (node?.nodeBlueprint != null)
        {
            nodeImage.color = node.nodeBlueprint.color;
            nodeImage.sprite = node.nodeBlueprint.sprite;
        }

        if (selectionHighlight != null)
            selectionHighlight.SetActive(false);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (isSelectable)
            FindObjectOfType<GameMapManager>()?.OnNodeClicked(node);
    }

    public void SetSelectable(bool selectable)
    {
        isSelectable = selectable;
        if (node?.nodeBlueprint != null)
            nodeImage.color = selectable ? node.nodeBlueprint.color : node.nodeBlueprint.color * 0.7f;
    }

    public void SetSelected(bool selected)
    {
        if (selectionHighlight != null)
            selectionHighlight.SetActive(selected);
    }
}