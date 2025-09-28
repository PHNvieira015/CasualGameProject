using UnityEngine;

public class BattleButtons : MonoBehaviour
{
    public void Endturn()
    {
        StateMachine.Instance.CardsdToPlay.Clear();
        StateMachine.Instance.ChangeState<EndTurnState>();
        
    }

}
