using UnityEngine;

[System.Serializable]
public class BodyData
{
    public Transform bodyAimPivot, mainCrossHair;
    public float movingDamp, movingDeltaTime;
    public float verticalJumpForce;
    public float horizontalJumpForce;
    //public float rootMotionEnableDelayOnJump;
    public float distanceToGround;
    [HideInInspector]
    public Vector3 bodyAimPivotPosition;

}