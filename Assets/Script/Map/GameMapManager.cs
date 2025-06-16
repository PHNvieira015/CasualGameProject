using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GameMapManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private MapGenerator mapGenerator;
    [SerializeField] private MapVisualizer mapVisualizer;

    private List<MapNode> currentMap;
    private MapNode currentNode;

    private void Start()
    {
        GenerateNewMap();
    }

    public void GenerateNewMap()
    {
        currentMap = mapGenerator.GenerateMap();
        mapVisualizer.VisualizeMap(currentMap);

        // Set starting node (bottom center)
        int startX = 0;
        currentNode = currentMap.Find(n => n.position == new Vector2Int(startX, 0));

        UpdateNavigation();
    }

    public void OnNodeClicked(MapNode node)
    {
        if (currentNode != null && currentNode.ConnectedNodes.Contains(node))
        {
            currentNode = node;
            UpdateNavigation();
            HandleNodeType(node.NodeType);
        }
    }

    private void UpdateNavigation()
    {
        foreach (var node in currentMap)
        {
            bool isSelectable = currentNode != null && currentNode.ConnectedNodes.Contains(node);
            mapVisualizer.SetNodeSelectable(node, isSelectable);
        }

        if (currentNode != null)
        {
            mapVisualizer.SetNodeSelected(currentNode, true);
        }
    }

    private void HandleNodeType(NodeType nodeType)
    {
        Debug.Log($"Handling node type: {nodeType}");
    }
}