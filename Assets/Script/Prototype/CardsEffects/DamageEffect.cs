using UnityEngine;

public class DamageEffect : MonoBehaviour, ICardEffect
{
    public int Amount;
    public void Apply()
    {
        Debug.LogFormat("Deals {0} Damage", Amount);
    }

}
