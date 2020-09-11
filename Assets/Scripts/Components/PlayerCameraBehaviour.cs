using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCameraBehaviour : MonoCached
{
    [SerializeField] private Transform lookAtPoint;
    [SerializeField] private Transform positionTarget;
    [SerializeField] private Transform player;
    
    //[SerializeField] private float lookAtDelta;
    //[SerializeField] private float positionDelta;
    
    private Vector3 currentPosition;

    public override void CustomUpdate()
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
        currentPosition = positionTarget.position;
        currentPosition.y -= lookAtPoint.localPosition.y/2;
        if (Physics.Linecast(currentPosition, player.position, out RaycastHit hit))
        {
            currentPosition = hit.point + transform.forward;
        }
    }

    private void SetCurrentPosition()
    {
        transform.position = currentPosition;
        //transform.position = Vector3.Lerp(transform.position, currentPosition, Time.deltaTime * positionDelta);
    }

    private void SetCurrentRotation()
    {
        Quaternion lookRotation = Quaternion.LookRotation(lookAtPoint.position - transform.position);
        transform.rotation = lookRotation;
        // transform.rotation = Quaternion.Lerp(transform.rotation, lookRotation, Time.deltaTime * lookAtDelta);
    }

    public static void FieldOfView(float fieldOfView)
    {
        Camera.main.fieldOfView = Mathf.Lerp(Camera.main.fieldOfView, fieldOfView, Time.fixedDeltaTime);
    }
}
