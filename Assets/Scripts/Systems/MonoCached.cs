using System.Collections.Generic;
using UnityEngine;

public class MonoCached : MonoBehaviour
{
    public static List<MonoCached> customUpdates = new List<MonoCached>(1000);
    public static List<MonoCached> customFixedUpdates = new List<MonoCached>(1000);
    public static List<MonoCached> customLateUpdates = new List<MonoCached>(1000);
    public virtual void OnEnable()
    {
        customUpdates.Add(this);
        customFixedUpdates.Add(this);
        customLateUpdates.Add(this);
    }

    public virtual void OnDisable()
    {
        customUpdates.Remove(this);
        customFixedUpdates.Remove(this);
        customLateUpdates.Remove(this);
    }
    
    public void CallCustomUpdate()
    {
        CustomUpdate();
    }

    public void CallCustomFixedUpdate()
    {
        CustomFixedUpdate();
    }

    public void CallCustomLateUpdate()
    {
        CustomLateUpdate();
    }

    public virtual void CustomUpdate()
    {

    }

    public virtual void CustomFixedUpdate()
    {

    }

    public virtual void CustomLateUpdate()
    {
        
    }
}
