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
            newPosition = hit.point + transform.forward/2;
            //newPosition = hit.point + ( yPositionTarget.position - _transform.position)*hit.distance;
        }
        //if (Physics.Linecast(positionTarget.position, yPositionTarget.position, out hit))
        //{
        //    newPosition = hit.point;
        //    //newPosition = hit.point + (yPositionTarget.position - _transform.position) * hit.distance;
        //}
        transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(lookAtPivot.position - transform.position), Time.deltaTime * lookAtDelta);
        transform.position = Vector3.Lerp(transform.position, newPosition, Time.deltaTime * positionDelta);
    }
    
    public static void FieldOfView(float fieldOfView)
    {
        Camera.main.fieldOfView = Mathf.Lerp(Camera.main.fieldOfView, fieldOfView, Time.fixedDeltaTime);
    }
}
