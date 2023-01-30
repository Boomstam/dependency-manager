using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScriptC : InjectBehaviour
{
    [Inject] private ScriptA scriptA;
    [SerializeField] private string fieldWithOtherAttribute;
    private string fieldWithoutAttribute;

    protected override void OnInjectAwake()
    {
        Debug.Log($"In C: scriptA null = {scriptA == null}");
    }
}
