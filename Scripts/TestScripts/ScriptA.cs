using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScriptA : InjectListBehaviour
{
    [Inject] private ScriptB scriptB;

    private void Awake()
    {
        Debug.Log($"On Script A: script b null = {scriptB == null}");

        StartCoroutine(InstantiateNewScriptAfterDelay());
    }
    
    private IEnumerator InstantiateNewScriptAfterDelay()
    {
        yield return new WaitForSeconds(0.1f);

        GameObject scriptC = new GameObject();
        scriptC.AddComponent<ScriptC>();
    }
}
