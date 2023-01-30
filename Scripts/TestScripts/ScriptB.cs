using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScriptB : InjectBehaviour
{
    [Inject] private ScriptD scriptD;

    protected override void OnInjectAwake()
    {
        Debug.Log($"On script B: scriptD null = {scriptD == null}");
    }
}
