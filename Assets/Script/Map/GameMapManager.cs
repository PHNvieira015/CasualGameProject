using System.Collections.Generic;
using UnityEngine;
using System.Linq; // Added this using directive

public class GameMapManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private MapGenerator mapGenerator;
    [SerializeField] private MapVisualizer mapVisualizer;

    private IReadOnlyList<MapNode> currentMap;
    private MapNode currentNode;

    private void Start()
    {
        if (mapGenerator == null || mapVisualizer == null)
        {
            Debug.LogError("MapGenerator or MapVisualizer not assigned!");
            return;
        }
        GenerateNewMap();
    }

    public void GenerateNewMap()
    {
        if (mapGenerator == null || mapVisualizer == null) return;

        currentMap = mapGenerator.GenerateMap();
        if (currentMap == null || currentMap.Count == 0)
        {
            Debug.LogError("Failed to generate map!");
            return;
        }

        mapVisualizer.VisualizeMap(currentMap.ToList());
        currentNode = currentMap.FirstOrDefault(n => n.position == Vector2Int.zero);
        UpdateNodeStates();
    }

    public void OnNodeClicked(MapNode node)
    {
        if (node == null || !IsValidNodeSelection(node)) return;

        currentNode = node;
        UpdateNodeStates();

        if (node.nodeBlueprint != null)
        {
            EventManager.Instance?.HandleNodeEvent(node.nodeBlueprint.nodeType);
        }
    }

    private bool IsValidNodeSelection(MapNode node)
    {
        return currentNode != null &&
               node != null &&
               currentNode.ConnectedNodes != null &&
               currentNode.ConnectedNodes.Contains(node);
    }

    private void UpdateNodeStates()
    {
        if (currentMap == null || mapVisualizer == null) return;

        foreach (var node in currentMap)
        {
            if (node != null)
            {
                mapVisualizer.SetNodeSelectable(node, IsValidNodeSelection(node));
            }
        }

        if (currentNode != null)
        {
            mapVisualizer.SetNodeSelected(currentNode, true);
        }
    }
}