using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetInterpolator : MonoCached
{
    private Vector3 offset;
    private Transform target;
    private float speed;

    private bool isActive;

    public void SetConstraint(Transform target, float speed)
    {
        this.target = target;
        this.speed = speed;
        isActive = true;
    }
    
    public void SetConstraint(Transform target, Vector3 offset, float speed)
    {
        this.target = target;
        this.speed = speed;
        this.offset = offset;
        isActive = true;
    }

    public override void CustomUpdate()
    {
        if (!isActive)
            return;
        
        transform.position = Vector3.Lerp(transform.position, target.position + offset, Time.deltaTime * speed);
    }
}
