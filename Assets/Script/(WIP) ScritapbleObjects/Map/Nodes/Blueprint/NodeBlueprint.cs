using UnityEngine;

[CreateAssetMenu(fileName = "NodeBlueprint", menuName = "ScriptableObjects/NodeBlueprint")]
public class NodeBlueprint : ScriptableObject
{
    public Sprite sprite;
    public NodeType nodeType;
    public Color color;
}


