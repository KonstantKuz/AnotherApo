using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCameraBehaviour : MonoCached
{
    [SerializeField] private Transform lookAtPoint;
    [SerializeField] private Transform positionTarget;
    
    public float lookAtDelta;
    public float positionDelta;
    private float YRotation;
    private float XRotation;
    private Vector3 currentPosition;
    private RaycastHit hit;

    public override void CustomFixedUpdate()
    {
        HandleTransforms();
    }

    public void HandleTransforms()
    {
        CalculateCurrentPosition();

        SetCurrentRotation();
        SetCurrentPosition();
    }

    private void CalculateCurrentPosition()
    {
        YRotation += PlayerInput.MouseY;
        YRotation = Mathf.Clamp(YRotation, -8, 5);
        // currentPosition = Quaternion.AngleAxis(YRotation, positionTarget.right) * positionTarget.position;
        currentPosition = positionTarget.position;
        currentPosition.y -= lookAtPoint.localPosition.y/2;
    }

    private void SetCurrentPosition()
    {
        transform.position = Vector3.Lerp(transform.position, currentPosition, Time.deltaTime * positionDelta);
    }

    private void SetCurrentRotation()
    {
        Quaternion lookRotation = Quaternion.LookRotation(lookAtPoint.position - transform.position);
        transform.rotation = Quaternion.Lerp(transform.rotation, lookRotation, Time.deltaTime * lookAtDelta);
    }

    public static void FieldOfView(float fieldOfView)
    {
        Camera.main.fieldOfView = Mathf.Lerp(Camera.main.fieldOfView, fieldOfView, Time.fixedDeltaTime);
    }
}
