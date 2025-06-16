using System.Collections.Generic;
using UnityEngine;

public class MapGenerator : MonoBehaviour
{
    [System.Serializable]
    public class NodeBlueprintMapping
    {
        public NodeType type;
        public NodeBlueprint blueprint;
    }

    [Header("Dimensions")]
    [SerializeField] private int totalRows = 6;
    [SerializeField] private int maxWidth = 5;

    [Header("Node Blueprints")]
    [SerializeField] private List<NodeBlueprintMapping> blueprintMappings = new List<NodeBlueprintMapping>();

    [Header("Generation Settings")]
    [SerializeField, Range(0.1f, 0.9f)] private float nodeSpawnChance = 0.6f;

    public List<MapNode> GenerateMap()
    {
        List<MapNode> nodes = new List<MapNode>();
        MapNode startNode = CreateNode(Vector2Int.zero, NodeType.MinorEnemy);
        nodes.Add(startNode);

        for (int y = 1; y < totalRows; y++)
        {
            int nodesThisRow = GetNodesCountForRow(y);
            List<MapNode> currentRow = CreateRow(y, nodesThisRow);
            ConnectRows(nodes, currentRow, y);
            nodes.AddRange(currentRow);
        }

        return nodes;
    }

    public int GetMapColumns() => maxWidth;

    private int GetNodesCountForRow(int y)
    {
        if (y == totalRows - 1) return 1; // Boss row
        return Random.Range(1, maxWidth + 1);
    }

    private List<MapNode> CreateRow(int y, int count)
    {
        List<MapNode> row = new List<MapNode>();
        for (int i = 0; i < count; i++)
        {
            Vector2Int pos = new Vector2Int(CalculateXOffset(i, count), y);
            row.Add(CreateNode(pos, GetNodeTypeForPosition(y)));
        }
        return row;
    }

    private int CalculateXOffset(int index, int totalNodes)
    {
        if (totalNodes == 1) return 0;
        return index - (totalNodes - 1) / 2;
    }

    private NodeType GetNodeTypeForPosition(int y)
    {
        if (y == totalRows - 1) return NodeType.Boss;
        if (y == totalRows - 2) return NodeType.RestSite;

        float roll = Random.value;
        if (roll < 0.5f) return NodeType.MinorEnemy;
        if (roll < 0.7f) return NodeType.Event;
        if (roll < 0.8f) return NodeType.EliteEnemy;
        if (roll < 0.9f) return NodeType.Store;
        return NodeType.Treasure;
    }

    private NodeBlueprint GetBlueprintForType(NodeType type)
    {
        foreach (var mapping in blueprintMappings)
            if (mapping.type == type)
                return mapping.blueprint;
        return null;
    }

    private MapNode CreateNode(Vector2Int pos, NodeType type)
    {
        return new MapNode()
        {
            position = pos,
            nodeBlueprint = GetBlueprintForType(type),
            isActive = true,
            isBossNode = (type == NodeType.Boss),
            isRestNode = (type == NodeType.RestSite)
        };
    }

    private void ConnectRows(List<MapNode> allNodes, List<MapNode> currentRow, int currentY)
    {
        foreach (var node in currentRow)
        {
            foreach (var prevNode in allNodes.FindAll(n => n.position.y == currentY - 1))
            {
                if (Mathf.Abs(prevNode.position.x - node.position.x) <= 1)
                {
                    prevNode.AddConnection(node);
                }
            }
        }
    }
}