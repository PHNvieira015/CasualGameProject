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
    [SerializeField] private int totalRows = 5;
    [SerializeField] private int maxColumns = 3;
    [SerializeField] private List<NodeBlueprintMapping> blueprintMappings = new List<NodeBlueprintMapping>();

    public List<MapNode> GenerateMap()
    {
        List<MapNode> nodes = new List<MapNode>();
        List<List<MapNode>> rows = new List<List<MapNode>>();

        // 1. Create all nodes row by row (bottom to top)
        for (int y = 0; y < totalRows; y++)
        {
            List<MapNode> currentRow = new List<MapNode>();
            int nodeCount = GetNodeCountForRow(y);

            for (int x = 0; x < nodeCount; x++)
            {
                int xPos = GetXPosition(x, nodeCount);
                NodeType type = GetNodeTypeForRow(y);
                MapNode node = CreateNode(new Vector2Int(xPos, y), type);
                nodes.Add(node);
                currentRow.Add(node);
            }
            rows.Add(currentRow);
        }

        // 2. Connect each node to at least one node above
        for (int y = 0; y < rows.Count - 1; y++) // Skip last row (boss)
        {
            List<MapNode> currentRow = rows[y];
            List<MapNode> rowAbove = rows[y + 1];

            foreach (var node in currentRow)
            {
                // Connect to closest node above
                MapNode closestAbove = GetClosestNode(node, rowAbove);
                node.AddConnection(closestAbove);
            }
        }

        // 3. Connect each node to at least one node below (except start)
        for (int y = 1; y < rows.Count; y++) // Skip first row
        {
            List<MapNode> currentRow = rows[y];
            List<MapNode> rowBelow = rows[y - 1];

            foreach (var node in currentRow)
            {
                // Connect to closest node below
                MapNode closestBelow = GetClosestNode(node, rowBelow);
                closestBelow.AddConnection(node); // Reverse connection
            }
        }

        return nodes;
    }

    private MapNode GetClosestNode(MapNode source, List<MapNode> targets)
    {
        MapNode closest = targets[0];
        float minDist = Mathf.Abs(source.position.x - closest.position.x);

        foreach (var node in targets)
        {
            float dist = Mathf.Abs(source.position.x - node.position.x);
            if (dist < minDist)
            {
                minDist = dist;
                closest = node;
            }
        }
        return closest;
    }

    private int GetNodeCountForRow(int row)
    {
        // First and last two rows always have 1 node
        return (row == 0 || row >= totalRows - 2) ? 1 : Random.Range(1, maxColumns + 1);
    }

    private int GetXPosition(int index, int nodeCount)
    {
        // Center nodes in their row
        return nodeCount > 1 ? index - (nodeCount - 1) / 2 : 0;
    }

    private NodeType GetNodeTypeForRow(int row)
    {
        if (row == 0) return NodeType.MinorEnemy;
        if (row == totalRows - 2) return NodeType.RestSite;
        if (row == totalRows - 1) return NodeType.Boss;
        return GetRandomNodeType();
    }

    private NodeType GetRandomNodeType()
    {
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