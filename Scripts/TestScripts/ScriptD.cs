using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ScriptD : InjectBehaviour
{
    [Inject] private List<ScriptA> scriptAs;

    protected override void OnInjectAwake()
    {
        Debug.Log($"ScriptAs null: {scriptAs == null}");
        scriptAs.ForEach(a =>  Debug.Log(a.name));
    }
}
