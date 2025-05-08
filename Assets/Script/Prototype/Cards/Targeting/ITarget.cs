using NUnit.Framework;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public interface ITarget 

{
    IEnumerator GetTargets(List<object> targets);
}
