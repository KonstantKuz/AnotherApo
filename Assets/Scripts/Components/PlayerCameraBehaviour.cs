using UnityEngine;

public class PlayerCameraBehaviour : MonoCached
{
    [SerializeField] private Transform lookAtPoint;
    [SerializeField] private Transform positionTarget;
    [SerializeField] private Transform pointForCheckIntersection;
    
    [SerializeField] private float rotationUpdSpeed;
    [SerializeField] private float positionUpdSpeed;
    
    private Vector3 currentPosition;

    public override void CustomLateUpdate()
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
        if (Physics.Linecast(currentPosition, pointForCheckIntersection.position, out RaycastHit hit))
        {
            currentPosition = hit.point + transform.forward;
        }
    }

    private void SetCurrentPosition()
    {
        transform.position = currentPosition;
        //transform.position = Vector3.Lerp(transform.position, currentPosition, Time.deltaTime * positionUpdSpeed);
    }

    private void SetCurrentRotation()
    {
        Quaternion lookRotation = Quaternion.LookRotation(lookAtPoint.position - transform.position);
        transform.rotation = lookRotation;
        //transform.rotation = Quaternion.Lerp(transform.rotation, lookRotation, Time.deltaTime * rotationUpdSpeed);
    }

    public static void FieldOfView(float fieldOfView)
    {
        Camera.main.fieldOfView = Mathf.Lerp(Camera.main.fieldOfView, fieldOfView, Time.fixedDeltaTime);
    }
}
