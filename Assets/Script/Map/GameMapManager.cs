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

    private void Start() => GenerateNewMap();

    public void GenerateNewMap()
    {
        currentMap = mapGenerator.GenerateMap();
        mapVisualizer.VisualizeMap(currentMap);
        currentNode = currentMap.Find(n => n.position == Vector2Int.zero);
        UpdateNodeStates();
    }

    public void OnNodeClicked(MapNode node)
    {
        if (IsValidNodeSelection(node))
        {
            currentNode = node;
            UpdateNodeStates();
            EventManager.Instance.HandleNodeEvent(node.nodeBlueprint.nodeType);  // Changed to nodeType
        }
    }

    private bool IsValidNodeSelection(MapNode node)
    {
        return currentNode != null &&
               currentNode.ConnectedNodes.Contains(node);
    }

    private void UpdateNodeStates()
    {
        foreach (var node in currentMap)
        {
            bool isSelectable = IsValidNodeSelection(node);
            mapVisualizer.SetNodeSelectable(node, isSelectable);
        }
        mapVisualizer.SetNodeSelected(currentNode, true);
    }
}