using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

// Meant to make this script run before all others
[DefaultExecutionOrder(-95)]
public class DependencyManager : MonoBehaviour
{
    private static Dictionary<Type, InjectBehaviour> registeredSingletons = new();
    // The order of the InjectListBehaviours in the lists is determined by hierarchy on awake.
    // After that, objects are just added chronologically.
    private static Dictionary<Type, List<InjectListBehaviour>> registeredLists = new();

    private void Awake()
    {
        Debug.Log($"Awake dependency manager");

        AddInitialInjectables();
    }
    
    // Dependencies order on awake is determined by hierarchy
    private static T[] FindOrderedInjectables<T>() where T : Injectable
    {
        Injectable[] injectBehaviours = FindObjectsOfType<Injectable>();
        
        injectBehaviours = injectBehaviours.Select(injTrans => injTrans.transform)
            .OrderBy(injTrans => injTrans, new OrderByHierarchy())
            .Select(injTrans => injTrans.GetComponent<Injectable>()).ToArray();

        Debug.Log($"Found InjectBehaviours:");
        injectBehaviours.ToList().ForEach(Debug.Log);
        
        return (T[])injectBehaviours;
    }

    // Add all already existing InjectBehaviours to the registered objects.
    private static void AddInitialInjectables()
    {
        Injectable[] injectables = FindOrderedInjectables<Injectable>();
        
        foreach (Injectable injectable in injectables)
        {
            Debug.Log($"Found {injectable}");

            Add(injectable);
            injectable.AddedOnSceneAwake = true;
        }
        
        foreach (Injectable injectable in injectables)
        {
            InjectFields(injectable);
        };
    }

    private static void Add(Injectable injectable)
    {
        if (injectable is InjectBehaviour injectBehaviour)
        {
            if (registeredSingletons.ContainsKey(injectBehaviour.GetType()))
            {
                throw new Exception($"Tried to inject duplicate of singleton: {injectBehaviour.name}");
            }
            else
            {
                registeredSingletons.Add(injectBehaviour.GetType(), injectBehaviour);
                Debug.Log($"Added singleton: {injectable.name}");
            }
        }
        else if(injectable is InjectListBehaviour injectListBehaviour)
        {
            if (registeredLists.ContainsKey(injectListBehaviour.GetType()))
            {
                registeredLists[injectListBehaviour.GetType()].Add(injectListBehaviour);
                Debug.Log($"Added to existing list: {injectable.name}");
            }
            else
            {
                registeredLists.Add(injectListBehaviour.GetType(), new List<InjectListBehaviour>(){ injectListBehaviour });
                Debug.Log($"Added new list: {injectable.name}");
            }
        }
    }

    public static void Register(Injectable injectable)
    {
        if (injectable is ConsumerBehaviour)
        {
            InjectFields(injectable);
        }
        else
        {
            Type type = injectable.GetType();
            Debug.Log($"Register {injectable}, type: {type}");

            Add(injectable);
        
            InjectFields(injectable);
        }
    }

    public static void Unregister(Injectable injectable)
    {
        if(injectable is ConsumerBehaviour) return;
        
        Type type = injectable.GetType();
        Debug.Log($"Unregister {injectable.name}, type: {type}");
        
        if (registeredSingletons.ContainsKey(type))
        {
            registeredSingletons.Remove(type);
        }
        else
        {
            registeredLists[type].Remove((InjectListBehaviour)injectable);
        }

        throw new Exception($"Couldn't unregister injectable {injectable.name}");
    }

    private static void InjectFields(Injectable injectable)
    {
        FieldInfo[] objectFields = GetFields(injectable);

        foreach (FieldInfo field in objectFields)
        {
            bool isList = IsList(field.FieldType);
            Debug.Log($"Field for {injectable}: {field.Name}, type: {field.FieldType} isList: {isList}");

            if (isList)
            {
                // check this
                Type underlyingType = field.FieldType.GetGenericArguments()[0];
                Debug.Log($"List: {field}, {field.FieldType} on {injectable.name}, underlying type: {underlyingType}");

                if (registeredLists.ContainsKey(underlyingType) == false)
                {
                    throw new Exception($"Couldn't find list {field.FieldType} of type {underlyingType} on {injectable.name}");
                }

                List<InjectListBehaviour> foundList = registeredLists[underlyingType];
                    // .Select(injectListBehaviour => injectListBehaviour as ).ToList();
                
                field.SetValue(foundList, foundList);
            }
            else
            {
                if (registeredSingletons.ContainsKey(field.FieldType) == false)
                {
                    throw new Exception($"Couldn't find singleton {field.FieldType} on {injectable.name}");
                }

                InjectBehaviour foundClass = registeredSingletons[field.FieldType];

                field.SetValue(injectable, foundClass);

                Debug.Log($"Set value of {field.Name} on {injectable}");
            }
        }
    }

    private static FieldInfo[] GetFields(Injectable injectable)
    {
        // Get all fields with a certain attribute through reflection
        FieldInfo[] objectFields = injectable.GetType()
            .GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
        
        return objectFields;
    }

    private static bool IsList(Type type)
    {
        return type.IsGenericType && type.GetGenericTypeDefinition() == typeof(List<>);
    }
}