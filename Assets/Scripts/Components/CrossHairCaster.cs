using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrossHairCaster : MonoCached
{
    public Transform crossHair;
    private Ray ray;
    private RaycastHit hit;

    private void Awake()
    {
        ray = new Ray();
        hit = new RaycastHit();
        _transform = transform;
    }

    public override void CustomUpdate()
    {
        ray.origin = _transform.position;
        ray.direction = _transform.forward;

        if(Physics.Raycast(ray, out hit, 15f))
        {
            crossHair.position = hit.point;
        }
        else
        {
            crossHair.position = ray.GetPoint(15f);
        }
    }
}
