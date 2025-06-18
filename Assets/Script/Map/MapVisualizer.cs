using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MapVisualizer : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private RectTransform mapContainer;
    [SerializeField] private Vector2 nodeSize = new Vector2(120, 120);
    [SerializeField] private float horizontalSpacing = 200f;
    [SerializeField] private float verticalSpacing = 250f;
    [SerializeField] private float verticalStartOffset = 300f;
    [SerializeField] private Color connectionColor = new Color(1, 1, 1, 0.5f);
    [SerializeField] private float connectionThickness = 5f;

    [Header("Prefabs")]
    [SerializeField] private GameObject nodePrefab;
    [SerializeField] private GameObject connectionPrefab;

    private Dictionary<MapNode, RectTransform> nodeTransforms = new Dictionary<MapNode, RectTransform>();

    public void VisualizeMap(List<MapNode> map)
    {
        if (mapContainer == null || nodePrefab == null)
        {
            Debug.LogError("Essential components not assigned in MapVisualizer");
            return;
        }

        ClearMap();
        Vector2 startPos = new Vector2(0, -mapContainer.rect.height / 2 + verticalStartOffset);

        // Create all nodes first
        foreach (var node in map)
        {
            if (node == null) continue;

            Vector2 nodePos = new Vector2(
                node.position.x * horizontalSpacing,
                startPos.y + node.position.y * verticalSpacing
            );

            GameObject nodeObj = Instantiate(nodePrefab, mapContainer);
            if (nodeObj == null) continue;

            RectTransform rt = nodeObj.GetComponent<RectTransform>();
            if (rt == null) continue;

            rt.anchoredPosition = nodePos;
            rt.sizeDelta = nodeSize;

            NodeVisual visual = nodeObj.GetComponent<NodeVisual>();
            if (visual != null)
            {
                nodeObj.SetActive(true); // Ensure active before setup
                visual.Setup(node, false);
            }

            nodeTransforms[node] = rt;
        }

        // Create connections after all nodes exist
        foreach (var node in map)
        {
            if (node == null || !nodeTransforms.ContainsKey(node)) continue;

            foreach (var connectedNode in node.ConnectedNodes)
            {
                if (connectedNode != null &&
                    nodeTransforms.ContainsKey(connectedNode) &&
                    nodeTransforms.ContainsKey(node))
                {
                    CreateConnection(nodeTransforms[node], nodeTransforms[connectedNode]);
                }
            }
        }
    }

    private void CreateConnection(RectTransform start, RectTransform end)
    {
        if (connectionPrefab == null || start == null || end == null) return;

        GameObject connection = Instantiate(connectionPrefab, mapContainer);
        if (connection == null) return;

        connection.transform.SetAsFirstSibling();

        RectTransform connectionRT = connection.GetComponent<RectTransform>();
        Image connectionImage = connection.GetComponent<Image>();

        if (connectionRT == null || connectionImage == null)
        {
            Destroy(connection);
            return;
        }

        connectionImage.color = connectionColor;

        Vector2 startPos = start.anchoredPosition;
        Vector2 endPos = end.anchoredPosition;
        Vector2 direction = endPos - startPos;
        float distance = direction.magnitude;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        connectionRT.anchoredPosition = (startPos + endPos) / 2f;
        connectionRT.sizeDelta = new Vector2(distance, connectionThickness);
        connectionRT.localEulerAngles = new Vector3(0, 0, angle);
    }

    public void SetNodeSelectable(MapNode node, bool selectable)
    {
        if (node != null && nodeTransforms.TryGetValue(node, out RectTransform rt))
        {
            NodeVisual visual = rt.GetComponent<NodeVisual>();
            if (visual != null)
            {
                visual.SetSelectable(selectable);
            }
        }
    }

    public void SetNodeSelected(MapNode node, bool selected)
    {
        if (node != null && nodeTransforms.TryGetValue(node, out RectTransform rt))
        {
            NodeVisual visual = rt.GetComponent<NodeVisual>();
            if (visual != null)
            {
                visual.SetSelected(selected);
            }
        }
    }

    private void ClearMap()
    {
        if (mapContainer == null) return;

        foreach (Transform child in mapContainer)
        {
            if (child != null)
            {
                NodeVisual visual = child.GetComponent<NodeVisual>();
                if (visual != null)
                {
                    visual.StopHighlightAnimation();
                }
                Destroy(child.gameObject);
            }
        }
        nodeTransforms.Clear();
    }
}