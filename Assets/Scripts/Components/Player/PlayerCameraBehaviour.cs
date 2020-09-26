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
        CalculateCurrentPosition();
        SetCurrentRotation();
        SetCurrentPosition();
    }

    private void CalculateCurrentPosition()
    {
        if (Physics.Linecast(pointForCheckIntersection.position, positionTarget.position, out RaycastHit hit))
        {
            currentPosition = hit.point + hit.normal;
        }
        else
        {
            currentPosition = positionTarget.position;
        }
    }
    
    private void SetCurrentPosition()
    {
        transform.position = currentPosition;
    }

    private void SetCurrentRotation()
    {
        Quaternion lookRotation = Quaternion.LookRotation(lookAtPoint.position - transform.position);
        transform.rotation = lookRotation;
    }
}
