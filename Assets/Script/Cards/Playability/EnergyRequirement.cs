using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EnergyRequirement : MonoBehaviour, IPlayability
{
    public int Energy;

    public bool CanPlay()
    {
        PlayerUnit player = StateMachine.Instance.CurrentUnit as PlayerUnit;
        if (player)
        {
            return player.CurrentEnergy >= Energy;
        }
        return false;
    }
 
}
