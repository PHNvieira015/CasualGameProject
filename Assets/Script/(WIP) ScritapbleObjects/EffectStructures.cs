using UnityEngine;

public enum EffectType
{
    Damage,
    Block,
    Debuff,
    ReduceCardBuy,
    CreateSpecialDebuffCard
    // Add other effect types as needed
}

[System.Serializable]
public class Effect
{
    public EffectType type;
    public int value;
    public Debuff debuff;
    public int duration; // Added duration for debuffs
    public TargetType target; // Added target for effects
    // Add other properties relevant to specific effects, e.g., visual effects, etc.
}

public enum TargetType
{
    SingleEnemy,
    AllEnemies,
    Player
}




public enum CardType
{
    Attack,
    Skill,
    Power
    // Add other card types as needed
}


