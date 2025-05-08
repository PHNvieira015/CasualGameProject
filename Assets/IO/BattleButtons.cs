using UnityEngine;

public class BattleButtons : MonoBehaviour
{
    public void Endturn()
    {
        StateMachine.Instance.ChangeState<EndTurnState>();
    }

}
