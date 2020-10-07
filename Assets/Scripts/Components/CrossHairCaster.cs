using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrossHairCaster : MonoCached
{
    [SerializeField] private float maxDistance = 15f;
    [SerializeField] private Transform crossHair;
    protected Ray ray;
    private RaycastHit hit;
    private RaycastHit[] allHits;
    public RaycastHit Hit
    {
        get { return hit; }
    }
    public RaycastHit[] AllHits
    {
        get { return allHits; }
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
        allHits = Physics.RaycastAll(ray, maxDistance);
        
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
