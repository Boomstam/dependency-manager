using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class OrderByHierarchy : IComparer<Transform>
{
    public int Compare(Transform first, Transform second)
    {
        int[] firstSiblingIndices = ParentSiblingIndices(first);
        int[] secondSiblingIndices = ParentSiblingIndices(second);

        int shortestArrayLength = firstSiblingIndices.Length <= secondSiblingIndices.Length ?
            firstSiblingIndices.Length :
            secondSiblingIndices.Length;

        for (int i = 0; i < shortestArrayLength; i++)
        {
            int currentFirstIndex = firstSiblingIndices[i];
            int currentSecondIndex = secondSiblingIndices[i];
            
            if (currentFirstIndex != currentSecondIndex)
            {
                // Debug.Log($"{first.name} was found to not be equal to {second.name}, isGreatherThan = {currentFirstIndex > currentSecondIndex}");
                return currentFirstIndex > currentSecondIndex ? 1 : -1;
            }
        }
        // Debug.Log($"{first.name} was found to be equal to {second.name}");
        return 0;
    }

    private int[] ParentSiblingIndices(Transform injectBehaviour)
    {
        Transform nextParent = injectBehaviour.parent;
        List<int> parentSiblingIndices = new List<int>() { injectBehaviour.GetSiblingIndex() };

        while (nextParent != null)
        {
            parentSiblingIndices.Add(nextParent.GetSiblingIndex());
            
            nextParent = nextParent.parent;
        }

        /*
        Debug.Log($"Number of indices: {parentSiblingIndices.Count} for {injectBehaviour.name}");
        Debug.Log(
            $"Parent indices for {injectBehaviour.name}: " +
            $"{parentSiblingIndices.Select(ind => ind.ToString()).Aggregate((a, b) => a + "-" + b)}");
            */
        
        return parentSiblingIndices.ToArray();
    }
}