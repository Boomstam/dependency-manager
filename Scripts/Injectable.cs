using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Injectable : MonoBehaviour
{
    // Holds a flag to avoid double injection of in-scene placed objects
    public bool AddedOnSceneAwake { get; set; }

    private void Awake()
    {
        if (AddedOnSceneAwake == false)
        {
            DependencyManager.Register(this);  
        }

        OnInjectAwake();
    }

    /// <summary>
    /// Alternative for Awake, called on Awake right after fixing dependencies
    /// </summary>
    protected virtual void OnInjectAwake()
    {
    }

    private void OnDestroy()
    {
        DependencyManager.Unregister(this);

        OnInjectDestroy();
    }
    
    /// <summary>
    /// Alternative for OnDestroy, called on OnDestroy right after unregistering from dependencies
    /// </summary>
    protected virtual void OnInjectDestroy()
    {
    }
}

// There might be a better pattern than using these empty classes
public class InjectBehaviour : Injectable
{
    
}

public class InjectListBehaviour : Injectable
{
    
}

public class ConsumerBehaviour : Injectable
{
    
}

[AttributeUsage(AttributeTargets.Field)]
public class Inject : Attribute {}
