using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraHeightUpdater : MonoCached
{
    [SerializeField] private Transform bodyAimPivot;
    private Vector3 startLocalPosition;
    private Vector3 currentLocalPosition;

    private void Start()
    {
        startLocalPosition = transform.localPosition;
    }

    public override void CustomUpdate()
    {
        currentLocalPosition = transform.localPosition;
        currentLocalPosition.y = startLocalPosition.y - bodyAimPivot.localPosition.y/2;
        transform.localPosition = currentLocalPosition;
    }
}
