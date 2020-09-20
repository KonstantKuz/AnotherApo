using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraCrossHairCaster : CrossHairCaster
{
    [SerializeField] private float rayStartOffset;

    public override void SetRayTransform()
    {
        ray.origin = transform.position + transform.forward * rayStartOffset;
        ray.direction = transform.forward;
    }
}
