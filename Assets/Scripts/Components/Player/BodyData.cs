using UnityEngine;

[System.Serializable]
public class BodyData
{
    public Transform bodyAimPivot, mainCrossHair;
    public float movingDamp, movingDeltaTime;

    public float gravityMultiplier;
    public float movementSpeed;
    public float jumpSpeed;
    public float dashSpeed;
    public float dashDuration;

    //public float gravityMultiplier;
    //public float distanceToGround;
    public Vector2 bodyAimPivotVerticalClamp;
    [HideInInspector]
    public Vector3 bodyAimPivotPosition;
}