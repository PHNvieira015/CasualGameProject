using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "NewCard", menuName = "ScriptableObjects/Card")]
public class CardScriptableObject : ScriptableObject
{
    public int energyCost;
    public Sprite sprite;
    public CardType cardType;

    [Header("Effects")]
    public List<Effect> effects = new List<Effect>();

    // You might add a method here to execute the card's effects
    // public void PlayCard(Character user, Character target)
    // {
    //     // Logic to apply effects based on type and target
    // }
}


