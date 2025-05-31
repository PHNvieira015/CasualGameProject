using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EndBattleState : State
{
    public override IEnumerator Enter()
    {
        yield return null;
        Debug.Log ("Battle ended");
    }

}

