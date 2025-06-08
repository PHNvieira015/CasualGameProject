using System;
using UnityEngine;

[CreateAssetMenu(menuName = "Status Effect")]
public class StatusEffectData : ScriptableObject
{
    public string effectName;
    public Sprite Icon;
    public float DotAmount;

    public enum BuffDebuffType
    {
        Buff,
        Debuff
    }

    public BuffDebuffType effectType;
    public float duration;

    public GameObject effectPrefab;
}
