using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class UpdateManager : MonoBehaviour
{
    private void Awake()
    {
       Application.targetFrameRate = 60;
    }
    
    private void Update()
    {
        for (int i = 0; i < MonoCached.customUpdates.Count; i++)
        {
            MonoCached.customUpdates[i].CallCustomUpdate();
        }
    }

    private void FixedUpdate()
    {
        for (int i = 0; i < MonoCached.customFixedUpdates.Count; i++)
        {
            MonoCached.customFixedUpdates[i].CallCustomFixedUpdate();
        }
    }

    private void LateUpdate()
    {
        for (int i = 0; i < MonoCached.customLateUpdates.Count; i++)
        {
            MonoCached.customLateUpdates[i].CallCustomLateUpdate();
        }
    }
}
