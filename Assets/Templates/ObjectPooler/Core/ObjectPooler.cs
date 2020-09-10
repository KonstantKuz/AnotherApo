using System.Collections;
using System.Collections.Generic;
using Singleton;
using UnityEngine;

public class ObjectPooler : Singleton<ObjectPooler>
{
    //[DrawAsReorderable("pools")] 
    [SerializeField] private List<Pool> pools;
    [SerializeField] private List<PoolGroup> poolGroups;

    private Dictionary<string, Pool> poolsDictionary;
    private Dictionary<string, PoolGroup> poolGroupsDictionary;
    private Dictionary<string, List<string>> groupTagToHisPoolTagsDictionary;
    
    #region Initialization

    private void Awake()
    {
        InitializePooler();
    }

    private void InitializePooler()
    {
        //TryResolveUnassignedPools();
        InitializeSingleObjectPools();
        InitializeObjectPoolGroups();
    }

    public void TryResolveUnassignedPools()
    {
        if (pools == null || poolGroups == null)
            return;
        
        for (int i = 0; i < poolGroups.Count; i++)
        {
            for (int j = 0; j < poolGroups[i].poolsInGroup.Count; j++)
            {
                Pool poolInGroup = poolGroups[i].poolsInGroup[j].pool;
                if(!pools.Contains(poolInGroup))
                {
                    Debug.LogWarning($"There is {poolInGroup.poolTag} pool in {poolGroups[i].groupTag} pool group, " +
                                     $"but not in Pools! Added automatically.");
                    pools.Add(poolInGroup);
                }
            }

        }
    }
    private void InitializeSingleObjectPools()
    {
        poolsDictionary = new Dictionary<string, Pool>();

        for (int i = 0; i < pools.Count; i++)
        {
            pools[i].poolQueue = new Queue<GameObject>();

            if (pools[i].nameAsTag)
            {
                pools[i].poolTag = pools[i].prefab.name;
            }

            poolsDictionary.Add(pools[i].poolTag, pools[i]);
        }
    }
    private void InitializeObjectPoolGroups()
    {
        poolGroupsDictionary = new Dictionary<string, PoolGroup>();
        groupTagToHisPoolTagsDictionary = new Dictionary<string, List<string>>();

        for (int groupIndex = 0; groupIndex < poolGroups.Count; groupIndex++)
        {
            poolGroups[groupIndex].CalculateTotalWeight();
            poolGroupsDictionary.Add(poolGroups[groupIndex].groupTag, poolGroups[groupIndex]);
            
            List<string> poolTagsInGroup = new List<string>();
            for (int singleIndex = 0; singleIndex < poolGroups[groupIndex].poolsInGroup.Count; singleIndex++)
            {
                poolTagsInGroup.Add(poolGroups[groupIndex].poolsInGroup[singleIndex].pool.poolTag);
            }
            groupTagToHisPoolTagsDictionary.Add(poolGroups[groupIndex].groupTag, poolTagsInGroup);
        }
    }
    #endregion

    #region Spawning from concrete object pool (General)
    public GameObject SpawnObject(string poolTag)
    {
        GameObject objToReturn;

        TryFindPoolTag(poolTag);

        if (poolsDictionary[poolTag].poolQueue.Count > 0)
        {
            objToReturn = poolsDictionary[poolTag].poolQueue.Dequeue();
            objToReturn.gameObject.SetActive(true);
        }
        else
        {
            if(poolsDictionary[poolTag].parent == null)
            {
                poolsDictionary[poolTag].parent = new GameObject(poolTag + "Pool").transform;
                poolsDictionary[poolTag].parent.parent = transform;
            }
            objToReturn = Instantiate(poolsDictionary[poolTag].prefab, poolsDictionary[poolTag].parent);
            objToReturn.name = poolsDictionary[poolTag].prefab.name;
        }

        if(poolsDictionary[poolTag].autoReturn)
        {
            DelayedReturnObject(objToReturn, poolTag, poolsDictionary[poolTag].autoReturnDelay);
        }

        return objToReturn;
    }
    private void TryFindPoolTag(string poolTag)
    {
        if (!poolsDictionary.ContainsKey(poolTag))
        {
            Debug.LogError($"Threse is no pool with pooltag == {poolTag}");
        }
    }
    public GameObject SpawnObject(string poolTag, Vector3 position)
    {
        GameObject objToReturn;

        objToReturn = SpawnObject(poolTag);
        objToReturn.transform.position = position;

        return objToReturn;
    }
    public GameObject SpawnObject(string poolTag, Vector3 position, Quaternion rotation)
    {
        GameObject objToReturn;

        objToReturn = SpawnObject(poolTag);
        objToReturn.transform.position = position;
        objToReturn.transform.rotation = rotation;

        return objToReturn;
    }
    #endregion

    #region Spawning from object pool group
    // just using a group tag that points to an array of Concrete object pools tags
    public GameObject SpawnRandomObject(string groupTag)
    {
        GameObject objToReturn;

        TryFindGroupTag(groupTag);

        int rndSingleObjectPoolTagIndex = Random.Range(0, groupTagToHisPoolTagsDictionary[groupTag].Count);
        string rndSingleObjectPoolTag = groupTagToHisPoolTagsDictionary[groupTag][rndSingleObjectPoolTagIndex];

        objToReturn = SpawnObject(rndSingleObjectPoolTag);

        return objToReturn;
    }
    private void TryFindGroupTag(string groupTag)
    {
        if (!groupTagToHisPoolTagsDictionary.ContainsKey(groupTag))
        {
            Debug.LogError($"Threse is no poolgroup with grouptag == {groupTag}");
        }
    }
    public GameObject SpawnRandomObject(string groupTag, Vector3 position)
    {
        GameObject objToReturn;

        objToReturn = SpawnRandomObject(groupTag);
        objToReturn.transform.position = position;

        return objToReturn;
    }
    public GameObject SpawnRandomObject(string groupTag, Vector3 position, Quaternion rotation)
    {
        GameObject objToReturn;

        objToReturn = SpawnRandomObject(groupTag);
        objToReturn.transform.position = position;
        objToReturn.transform.rotation = rotation;

        return objToReturn;
    }
    
    public GameObject SpawnWeightedRandomObject(string groupTag)
    {
        TryFindGroupTag(groupTag);
        
        int randomValue = Random.Range(0, poolGroupsDictionary[groupTag].totalWeight + 1);

        for (int i = 0; i < poolGroupsDictionary[groupTag].poolsInGroup.Count; i++)
        {
            WeightedPool weightedPool = poolGroupsDictionary[groupTag].poolsInGroup[i];
            if (randomValue <=  weightedPool.weight)
            {
                return SpawnObject(weightedPool.pool.poolTag);
            }

            randomValue -= weightedPool.weight;
        }
        
        Debug.LogError($"Return null with {randomValue} weight. Maybe weight are not assigned.");
        return null;
    }
    #endregion

    #region Return object
    public void ReturnObject(GameObject toReturn, string poolTag)
    {
        TryFindPoolTag(poolTag);

        if (!poolsDictionary[poolTag].poolQueue.Contains(toReturn))
        {
            toReturn.SetActive(false);
            poolsDictionary[poolTag].poolQueue.Enqueue(toReturn);
        }
    }
    public void DelayedReturnObject(GameObject toReturn, string poolTag, float delay)
    {
        TryFindPoolTag(poolTag);

        StartCoroutine(DelayedReturn(toReturn, poolTag, delay));
    }
    private IEnumerator DelayedReturn(GameObject toReturn, string poolTag, float delay)
    {
        yield return new WaitForSeconds(delay);
        ReturnObject(toReturn, poolTag);
    }
    #endregion
    
#if UNITY_EDITOR
    #region EditorOnly
    public List<Pool> EditorOnlyPools
    {
        get { return pools; }
        set { pools = value; }
    }

    public List<PoolGroup> EditorOnlyPoolGroups
    {
        get { return poolGroups; }
        set { poolGroups = value; }
    }
    #endregion
#endif
}