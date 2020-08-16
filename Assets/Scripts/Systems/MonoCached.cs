using System.Collections.Generic;
using UnityEngine;

public class MonoCached : MonoBehaviour
{
    public static List<MonoCached> customUpdates = new List<MonoCached>(1000);
    public static List<MonoCached> customFixedUpdates = new List<MonoCached>(1000);

    public virtual void OnEnable()
    {
        customUpdates.Add(this);
        customFixedUpdates.Add(this);
    }

    public virtual void OnDisable()
    {
        customUpdates.Remove(this);
        customFixedUpdates.Remove(this);
    }
    
    public void CustomUpdatesCall()
    {
        CustomUpdate();
    }

    public void CustomFixedUpdatesCall()
    {
        CustomFixedUpdate();
    }

    public virtual void CustomUpdate()
    {

    }

    public virtual void CustomFixedUpdate()
    {

    }
}
