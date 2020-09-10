using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ObjectPooler/Pool", fileName = "NewPool")]
[System.Serializable]
public class Pool : ScriptableObject
{
    public GameObject prefab = null;
    public bool nameAsTag = true;
    public string poolTag = "";
    public bool autoReturn = false;
    public float autoReturnDelay = 1f;

    [HideInInspector] public Queue<GameObject> poolQueue;
    [HideInInspector] public Transform parent;
}