using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetInterpolator : MonoCached
{
    private Transform target;
    private float speed;

    private bool isActive;

    public void SetConstraint(Transform target, float speed)
    {
        this.target = target;
        this.speed = speed;
        isActive = true;
    }

    public override void CustomUpdate()
    {
        if (!isActive)
            return;
        
        transform.position = Vector3.Lerp(transform.position, target.position, Time.deltaTime * speed);
    }
}
