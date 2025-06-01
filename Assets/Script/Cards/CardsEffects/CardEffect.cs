using NUnit.Framework;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public abstract class CardEffect: MonoBehaviour
{
    public abstract IEnumerator Apply(List<object> targets);

}

