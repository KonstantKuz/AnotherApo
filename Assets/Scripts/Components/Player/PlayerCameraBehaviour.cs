using UnityEngine;

public class PlayerCameraBehaviour : MonoCached
{
    [SerializeField] private Transform lookAtPoint;
    [SerializeField] private Transform maxPositionTarget;
    [SerializeField] private Transform minPositionTarget;
    
    [SerializeField] private float positionUpdSpeed;
    [SerializeField] private float rotationUpdSpeed;

    private Vector3 currentPosition;
    
    public override void CustomLateUpdate()
    {
        CalculateCurrentPosition();
        SetCurrentRotation();
        SetCurrentPosition();
    }

    private void CalculateCurrentPosition()
    {
        if (Physics.Linecast(minPositionTarget.position, maxPositionTarget.position, out RaycastHit hit))
        {
            currentPosition = hit.point + hit.normal;
        }
        else
        {
            currentPosition = maxPositionTarget.position;
        }
    }
    
    private void SetCurrentPosition()
    {
        transform.position = Vector3.Lerp(transform.position, currentPosition, Time.deltaTime * 20);
    }

    private void SetCurrentRotation()
    {
        Quaternion lookRotation = Quaternion.LookRotation(lookAtPoint.position - transform.position);
        transform.rotation = lookRotation;
    }
}
