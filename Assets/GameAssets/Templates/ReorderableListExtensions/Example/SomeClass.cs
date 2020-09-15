using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SomeClass : MonoBehaviour
{
    public List<SomeData> someDataList;
    public GameObject[] gameObjectsArray;
}

[System.Serializable]
public class SomeData
{
    public int someInt;
    public GameObject someGameObject;
    public SomeType someType;
}

public enum SomeType
{
    Type1,
    Type2,
}

