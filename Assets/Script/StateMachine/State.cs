using System.Collections;
using UnityEngine;

public class State : MonoBehaviour
{
    protected StateMachine machine { get { return StateMachine.Instance; } }
    public virtual IEnumerator Enter()
    {
        yield return null;

    }

    public virtual IEnumerator Exit()
    {
        yield return null;
    }
    protected IEnumerator WaitThenChangeState<T>() where T : State
    {
        yield return new WaitForSeconds(0.1f);
        machine.ChangeState<T>();
    }
}
