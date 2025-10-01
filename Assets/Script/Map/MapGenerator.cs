using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MapGenerator : MonoBehaviour
{
    [System.Serializable]
    public class NodeBlueprintMapping
    {
        public NodeType type;
        public NodeBlueprint blueprint;
    }

    [Header("Settings")]
    [SerializeField] private int totalRows = 9;
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

        // 2. Create forward connections using actual x positions
        for (int y = 0; y < rows.Count - 1; y++)
        {
            List<MapNode> currentRow = rows[y];
            List<MapNode> nextRow = rows[y + 1];

            // Special case: row before boss - all nodes connect to boss
            if (y == totalRows - 3)
            {
                MapNode bossNode = nextRow[0];
                foreach (var currentNode in currentRow)
                {
                    currentNode.AddConnection(bossNode);
                }
                continue;
            }
            // Special case: boss connects to victory
            else if (y == totalRows - 2)
            {
                MapNode victoryNode = nextRow[0];
                MapNode bossNode = currentRow[0];
                bossNode.AddConnection(victoryNode);
                continue;
            }

            // Normal connections based on x position
            foreach (var currentNode in currentRow)
            {
                List<MapNode> connections = GetNearbyNodesByXPosition(currentNode, nextRow);

                foreach (var connection in connections)
                {
                    currentNode.AddConnection(connection);
                }
            }

            // Ensure every node in next row has incoming connections
            foreach (var nextNode in nextRow)
            {
                bool hasIncoming = false;
                foreach (var currentNode in currentRow)
                {
                    if (currentNode.ConnectedNodes.Contains(nextNode))
                    {
                        hasIncoming = true;
                        break;
                    }
                }

                if (!hasIncoming)
                {
                    // Find the closest node in current row by x position
                    MapNode closestNode = GetClosestNodeByX(nextNode, currentRow);
                    closestNode.AddConnection(nextNode);
                }
            }
        }

        return nodes;
    }

    private List<MapNode> GetNearbyNodesByXPosition(MapNode currentNode, List<MapNode> nextRow)
    {
        List<MapNode> connections = new List<MapNode>();

        if (nextRow.Count == 0) return connections;

        // Get all nodes in next row sorted by x distance from current node
        List<MapNode> sortedNextRow = new List<MapNode>(nextRow);
        sortedNextRow.Sort((a, b) =>
        {
            float distA = Mathf.Abs(currentNode.position.x - a.position.x);
            float distB = Mathf.Abs(currentNode.position.x - b.position.x);
            return distA.CompareTo(distB);
        });

        // Always connect to the closest node
        if (sortedNextRow.Count > 0)
        {
            connections.Add(sortedNextRow[0]);
        }

        // Connect to additional nearby nodes (within ±1 x position)
        foreach (var node in sortedNextRow)
        {
            if (!connections.Contains(node))
            {
                int xDiff = Mathf.Abs(currentNode.position.x - node.position.x);
                if (xDiff <= 1) // Only connect to nodes within 1 x position
                {
                    connections.Add(node);
                }
            }
        }

        return connections;
    }

    private MapNode GetClosestNodeByX(MapNode targetNode, List<MapNode> nodes)
    {
        MapNode closest = nodes[0];
        float minDist = Mathf.Abs(targetNode.position.x - closest.position.x);

        foreach (var node in nodes)
        {
            float dist = Mathf.Abs(targetNode.position.x - node.position.x);
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
        return (row == 0 || row == 1 || row == 2 || row >= totalRows - 2) ? 1 : Random.Range(1, maxColumns + 1);
    }

    private int GetXPosition(int index, int nodeCount)
    {
        // Smaller spacing - nodes are closer together
        // This creates positions like: 
        // 1 node: [0]
        // 2 nodes: [-1, 1]  
        // 3 nodes: [-1, 0, 1]
        // 4 nodes: [-2, -1, 1, 2]
        // 5 nodes: [-2, -1, 0, 1, 2]
        if (nodeCount == 1) return 0;

        // Use smaller range for fewer nodes
        int maxOffset = (nodeCount - 1) / 2;
        return index - maxOffset;
    }

    private NodeType GetNodeTypeForRow(int row)
    {
        if (row == 0) return NodeType.RestSite;
        if (row == 1) return NodeType.MinorEnemy;
        if (row == 2) return NodeType.Store;
        if (row == 3) return NodeType.MinorEnemy;
        if (row == totalRows - 3) return NodeType.Store;
        if (row == totalRows - 2) return NodeType.Boss;
        if (row == totalRows - 1) return NodeType.Victory;
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
        MapNode node = new MapNode()
        {
            position = pos,
            nodeBlueprint = GetBlueprintForType(type),
            isActive = true
        };

        if (type == NodeType.Boss)
            node.isBossNode = true;
        else if (type == NodeType.RestSite)
            node.isRestNode = true;
        else if (type == NodeType.Victory)
            node.isVictoryNode = true;

        return node;
    }

    private NodeBlueprint GetBlueprintForType(NodeType type)
    {
        foreach (var mapping in blueprintMappings)
            if (mapping.type == type)
                return mapping.blueprint;
        return null;
    }
}