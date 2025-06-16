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
    [SerializeField] private GameObject connectionPrefab; // Simple UI Image prefab

    private Dictionary<MapNode, RectTransform> nodeTransforms = new Dictionary<MapNode, RectTransform>();

    public void VisualizeMap(List<MapNode> map)
    {
        ClearMap();
        Vector2 startPos = new Vector2(0, -mapContainer.rect.height / 2 + verticalStartOffset);

        // Create all nodes first
        foreach (var node in map)
        {
            Vector2 nodePos = new Vector2(
                node.position.x * horizontalSpacing,
                startPos.y + node.position.y * verticalSpacing
            );

            GameObject nodeObj = Instantiate(nodePrefab, mapContainer);
            RectTransform rt = nodeObj.GetComponent<RectTransform>();
            rt.anchoredPosition = nodePos;
            rt.sizeDelta = nodeSize;

            nodeObj.GetComponent<NodeVisual>().Setup(node, false);
            nodeTransforms[node] = rt;
        }

        // Create connections after all nodes exist
        foreach (var node in map)
        {
            if (!nodeTransforms.TryGetValue(node, out RectTransform startRT)) continue;

            foreach (var connectedNode in node.ConnectedNodes)
            {
                if (nodeTransforms.TryGetValue(connectedNode, out RectTransform endRT))
                {
                    CreateConnection(startRT, endRT);
                }
            }
        }
    }

    private void CreateConnection(RectTransform start, RectTransform end)
    {
        if (connectionPrefab == null)
        {
            Debug.LogError("Connection prefab is not assigned!");
            return;
        }

        GameObject connection = Instantiate(connectionPrefab, mapContainer);
        connection.transform.SetAsFirstSibling(); // Draw behind nodes

        RectTransform connectionRT = connection.GetComponent<RectTransform>();
        Image connectionImage = connection.GetComponent<Image>();

        if (connectionRT == null || connectionImage == null)
        {
            Debug.LogError("Connection prefab missing required components!");
            Destroy(connection);
            return;
        }

        connectionImage.color = connectionColor;

        // Calculate connection parameters
        Vector2 startPos = start.anchoredPosition;
        Vector2 endPos = end.anchoredPosition;
        Vector2 direction = endPos - startPos;
        float distance = direction.magnitude;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        // Position and size the connection
        connectionRT.anchoredPosition = (startPos + endPos) / 2f;
        connectionRT.sizeDelta = new Vector2(distance, connectionThickness);
        connectionRT.localEulerAngles = new Vector3(0, 0, angle);
    }

    public void SetNodeSelectable(MapNode node, bool selectable)
    {
        if (nodeTransforms.TryGetValue(node, out RectTransform rt))
        {
            NodeVisual visual = rt.GetComponent<NodeVisual>();
            if (visual != null) visual.SetSelectable(selectable);
        }
    }

    public void SetNodeSelected(MapNode node, bool selected)
    {
        if (nodeTransforms.TryGetValue(node, out RectTransform rt))
        {
            NodeVisual visual = rt.GetComponent<NodeVisual>();
            if (visual != null) visual.SetSelected(selected);
        }
    }

    private void ClearMap()
    {
        foreach (Transform child in mapContainer)
            Destroy(child.gameObject);
        nodeTransforms.Clear();
    }
}