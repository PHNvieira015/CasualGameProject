using NUnit.Framework;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public interface ICardEffect
{
    IEnumerator Apply(List<object> targets);

}
