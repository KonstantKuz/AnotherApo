using UnityEngine;

[System.Serializable]
public class PlayerCharacterControllerCData : CharacterControllerData
{
    public float movingDamp, movingDeltaTime;
    public Vector2 bodyAimPivotVerticalClamp;
}

[System.Serializable]
public class CharacterControllerData
{
    public float gravityMultiplier;
    public float movementSpeed;
    public float jumpSpeed;
    public float dashSpeed;
    public float dashDuration;
}