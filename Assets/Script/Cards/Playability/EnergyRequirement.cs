using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EnergyRequirement : MonoBehaviour, IPlayability
{
    public int Energy;

    public bool CanPlay()
    {
        // 0-cost cards should always be playable, even with 0 energy
        if (Energy == 0)
        {
            return true;
        }

        // Safety checks
        if (StateMachine.Instance == null)
        {
            Debug.LogWarning("StateMachine.Instance is null");
            return false;
        }

        if (StateMachine.Instance.CurrentUnit == null)
        {
            Debug.LogWarning("CurrentUnit is null");
            return false;
        }

        PlayerUnit player = StateMachine.Instance.CurrentUnit as PlayerUnit;
        if (player == null)
        {
            Debug.LogWarning("Current unit is not a PlayerUnit");
            return false;
        }

        bool canAfford = player.CurrentEnergy >= Energy;

        // Debug log to see what's happening
        if (!canAfford)
        {
            Debug.Log($"Not enough energy: Need {Energy}, Have {player.CurrentEnergy}");
        }

        return canAfford;
    }

    // Helper method to get the energy cost for UI display
    public int GetEnergyCost()
    {
        return Energy;
    }
}