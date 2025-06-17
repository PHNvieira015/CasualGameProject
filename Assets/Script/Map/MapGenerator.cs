using UnityEngine;
using System.Collections.Generic;

public class MapGenerator : MonoBehaviour
{
    [System.Serializable]
    public class NodeBlueprintMapping
    {
        public NodeType type;
        public NodeBlueprint blueprint;
    }

    [Header("Settings")]
    [SerializeField] private int totalRows = 5; // Minimum 3 rows
    [SerializeField] private int maxColumns = 3; // Maximum nodes per row
    [SerializeField] private List<NodeBlueprintMapping> blueprintMappings = new List<NodeBlueprintMapping>();

    public List<MapNode> GenerateMap()
    {
        List<MapNode> nodes = new List<MapNode>();
        List<MapNode> previousRowNodes = new List<MapNode>();

        // 1. Start at bottom (row 0)
        MapNode startNode = CreateNode(new Vector2Int(0, 0), NodeType.MinorEnemy);
        nodes.Add(startNode);
        previousRowNodes.Add(startNode);

        // 2. Random middle rows
        for (int y = 1; y < totalRows - 2; y++)
        {
            int nodesInThisRow = Random.Range(1, maxColumns + 1);
            List<MapNode> currentRowNodes = new List<MapNode>();

            // Create nodes for this row
            for (int x = 0; x < nodesInThisRow; x++)
            {
                // Center the nodes by offsetting based on count
                int xOffset = x - nodesInThisRow / 2;
                MapNode newNode = CreateNode(new Vector2Int(xOffset, y), GetRandomNodeType());
                nodes.Add(newNode);
                currentRowNodes.Add(newNode);
            }

            // Connect to previous row
            foreach (var previousNode in previousRowNodes)
            {
                // Connect each previous node to all nodes in current row (or implement smarter connection logic)
                foreach (var currentNode in currentRowNodes)
                {
                    // Simple connection logic - connect if roughly above or nearby
                    if (Mathf.Abs(currentNode.position.x - previousNode.position.x) <= 1)
                    {
                        previousNode.AddConnection(currentNode);
                    }
                }
            }

            previousRowNodes = currentRowNodes;
        }

        // 3. Rest node (second from top)
        int restRow = totalRows - 2;
        MapNode restNode = CreateNode(new Vector2Int(0, restRow), NodeType.RestSite);
        nodes.Add(restNode);

        // Connect to previous row
        foreach (var previousNode in previousRowNodes)
        {
            if (Mathf.Abs(restNode.position.x - previousNode.position.x) <= 1)
            {
                previousNode.AddConnection(restNode);
            }
        }

        // 4. Boss node (top)
        MapNode bossNode = CreateNode(new Vector2Int(0, totalRows - 1), NodeType.Boss);
        restNode.AddConnection(bossNode);
        nodes.Add(bossNode);

        return nodes;
    }

    private NodeType GetRandomNodeType()
    {
        // Never returns boss type for middle nodes
        float roll = Random.value;
        if (roll < 0.7f) return NodeType.MinorEnemy;
        if (roll < 0.9f) return NodeType.EliteEnemy;
        return NodeType.Treasure;
    }

    private MapNode CreateNode(Vector2Int pos, NodeType type)
    {
        return new MapNode()
        {
            position = pos,
            nodeBlueprint = GetBlueprintForType(type),
            isActive = true
        };
    }

    private NodeBlueprint GetBlueprintForType(NodeType type)
    {
        foreach (var mapping in blueprintMappings)
            if (mapping.type == type)
                return mapping.blueprint;
        return null;
    }
}