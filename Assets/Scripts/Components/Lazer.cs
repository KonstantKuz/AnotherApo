using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lazer : MonoCached
{
    [SerializeField] private LineRenderer lazerLine;
    [SerializeField] private CrossHairCaster crossHairCaster;
    public override void OnEnable()
    {
        base.OnEnable();
        crossHairCaster.OnCrossHairUpdated += OnCrossHairUpdated;
    }

    public override void OnDisable()
    {
        base.OnDisable();
        crossHairCaster.OnCrossHairUpdated -= OnCrossHairUpdated;
    }

    private void OnCrossHairUpdated(Vector3 crossHairPosition)
    {
        lazerLine.SetPosition(0, transform.position);
        lazerLine.SetPosition(1, crossHairPosition);
    }
}
