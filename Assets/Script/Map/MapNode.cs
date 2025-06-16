using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class MapNode
{
    public Vector2Int position;
    public NodeBlueprint nodeBlueprint;
    public bool isActive;
    public bool isBossNode;
    public bool isRestNode;

    [System.NonSerialized]
    private List<MapNode> connectedNodes = new List<MapNode>();

    public IReadOnlyList<MapNode> ConnectedNodes => connectedNodes;
    public NodeType NodeType => nodeBlueprint?.nodeType ?? NodeType.MinorEnemy;

    public void AddConnection(MapNode node)
    {
        if (node != null && !connectedNodes.Contains(node))
        {
            connectedNodes.Add(node);
        }
    }
}