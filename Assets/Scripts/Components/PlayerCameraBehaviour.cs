using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCameraBehaviour : MonoCached
{
    public Transform lookAtPivot;
    public Transform heightTarget;
    public Transform positionTarget;
    public float lookAtDelta;
    public float positionDelta;
    private Vector3 newPosition;
    private RaycastHit hit;
    private void Awake()
    {
        _transform = transform;
    }

    public override void CustomFixedUpdate()
    {
        HandleTransforms();
    }

    public void HandleTransforms()
    {
        newPosition = positionTarget.position;
        newPosition.y = heightTarget.position.y;

        Debug.DrawLine(heightTarget.position, newPosition, Color.red);
        if (Physics.Linecast(heightTarget.position, newPosition, out hit))
        {
            newPosition = hit.point + _transform.forward/2;
            //newPosition = hit.point + ( yPositionTarget.position - _transform.position)*hit.distance;
        }
        //if (Physics.Linecast(positionTarget.position, yPositionTarget.position, out hit))
        //{
        //    newPosition = hit.point;
        //    //newPosition = hit.point + (yPositionTarget.position - _transform.position) * hit.distance;
        //}
        _transform.rotation = Quaternion.Lerp(_transform.rotation, Quaternion.LookRotation(lookAtPivot.position - _transform.position), Time.deltaTime * lookAtDelta);
        _transform.position = Vector3.Lerp(_transform.position, newPosition, Time.deltaTime * positionDelta);
    }
    
    public static void FieldOfView(float fieldOfView)
    {
        Camera.main.fieldOfView = Mathf.Lerp(Camera.main.fieldOfView, fieldOfView, Time.fixedDeltaTime);
    }
}
