using UnityEngine;

[System.Serializable]
public class BodyData
{
    public Transform bodyAimPivot, mainCrossHair;
    public float movingDamp, movingDeltaTime;
    public float verticalJumpForce;
    public float distanceToGround;
    public Vector2 bodyAimPivotVerticalClamp;
    [HideInInspector]
    public Vector3 bodyAimPivotPosition;

}