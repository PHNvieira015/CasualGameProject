using UnityEngine;

public class Relic : MonoBehaviour
{
    public string RelicName;
    public string Description;
    public Sprite Icon;

    public virtual void OnGameStart() { }
    public virtual void OnTurnStart() { }
    public virtual void OnTurnEnd() { }
    public virtual void OnCardPlayed(Card card) { }
    public virtual void OnCardDrawn(Card card) { }
    public virtual void OnDamageDealt(int damageAmount) { }
    public virtual void OnDamageTaken(int damageAmount) { }
}