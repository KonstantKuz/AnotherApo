using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ObjectPooler/PoolGroup", fileName = "NewPoolGroup")]
[System.Serializable]
public class PoolGroup : ScriptableObject
{
    public string groupTag;
    public List<WeightedPool> poolsInGroup;
    
    [HideInInspector] public int totalWeight;

    public void CalculateTotalWeight()
    {
        totalWeight = 0;
        for (int i = 0; i < poolsInGroup.Count; i++)
        {
            totalWeight += poolsInGroup[i].weight;
        }
    }
}

[System.Serializable]
public class WeightedPool
{
    public int weight;
    public Pool pool;
}