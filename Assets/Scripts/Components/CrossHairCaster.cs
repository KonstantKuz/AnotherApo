using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Compilation;
using UnityEngine;

public class CrossHairCaster : MonoCached
{
    [SerializeField] private float maxDistance = 15f;
    [SerializeField] private Transform crossHair;
    private protected Ray ray;
    private RaycastHit hit;

    public RaycastHit Hit
    {
        get { return hit; }
    }

    public Action<Vector3> OnCrossHairUpdated;

    private void Awake()
    {
        ray = new Ray();
        hit = new RaycastHit();
    }

    public override void CustomUpdate()
    {
        SetRayTransform();
        CastCrossHair();
    }

    public virtual void SetRayTransform()
    {
        ray.origin = transform.position;
        ray.direction = transform.forward;
    }

    public virtual void CastCrossHair()
    {

        if(Physics.Raycast(ray, out hit, maxDistance))
        {
            crossHair.position = hit.point;
        }
        else
        {
            crossHair.position = ray.GetPoint(maxDistance);
        }

        OnCrossHairUpdated?.Invoke(crossHair.position);
    }
}
