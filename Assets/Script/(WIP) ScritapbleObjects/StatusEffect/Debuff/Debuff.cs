using UnityEngine;

[CreateAssetMenu(fileName = "NewDebuff", menuName = "ScriptableObjects/Debuff")]
public class Debuff : ScriptableObject
{
    public string debuffName;
    [TextArea] public string description;
    // Add other properties relevant to a debuff, e.g., duration, visual effects, etc.
    // public int duration;
    // public Sprite icon;
}


