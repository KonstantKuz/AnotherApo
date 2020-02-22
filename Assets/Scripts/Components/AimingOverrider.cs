using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class HandIKOverride
{
    public Transform rHandSimple;
    public Transform lHandSimple;

    public Transform rHandAiming;
    public Transform lHandAiming;

    [HideInInspector]
    public Vector3 rHandLerpedPos;
    [HideInInspector]
    public Vector3 lHandLerpedPos;
    [HideInInspector]
    public Quaternion rHandLerpedRot;
    [HideInInspector]
    public Quaternion lHandLerpedRot;
}
[System.Serializable]
public class ChestOverride
{
    public Transform overridedChestTransform;
    public Transform weapon;
    public Transform target;
    [HideInInspector]
    public Transform rightUpperArm;
}

public class AimingOverrider : MonoCached
{
    [HideInInspector]
    public Animator characterAnimator;

    public HandIKOverride hadnsIKOverrides;
    public ChestOverride overridedChest;

    private float deltaTime;

    void Start()
    {
        characterAnimator = GetComponent<Animator>();

        overridedChest.rightUpperArm = characterAnimator.GetBoneTransform(HumanBodyBones.RightUpperArm);
    }

    private void OnAnimatorIK(int layerIndex)
    {
        characterAnimator.SetIKPositionWeight(AvatarIKGoal.LeftHand, 1);
        characterAnimator.SetIKPositionWeight(AvatarIKGoal.RightHand, 1);
        characterAnimator.SetIKRotationWeight(AvatarIKGoal.LeftHand, 1);
        characterAnimator.SetIKRotationWeight(AvatarIKGoal.RightHand, 1);

        deltaTime = Time.deltaTime * 20f;

        if (characterAnimator.GetBool(AnimatorHashes.AimingHash))
        {
            hadnsIKOverrides.rHandLerpedPos = Vector3.Lerp(hadnsIKOverrides.rHandLerpedPos, hadnsIKOverrides.rHandAiming.position, deltaTime);
            hadnsIKOverrides.lHandLerpedPos = Vector3.Lerp(hadnsIKOverrides.lHandLerpedPos, hadnsIKOverrides.lHandAiming.position, deltaTime);

            hadnsIKOverrides.rHandLerpedRot = Quaternion.Lerp(hadnsIKOverrides.rHandLerpedRot, hadnsIKOverrides.rHandAiming.rotation, deltaTime);
            hadnsIKOverrides.lHandLerpedRot = Quaternion.Lerp(hadnsIKOverrides.lHandLerpedRot, hadnsIKOverrides.lHandAiming.rotation, deltaTime);
        }
        else
        {
            hadnsIKOverrides.rHandLerpedPos = Vector3.Lerp(hadnsIKOverrides.rHandLerpedPos, hadnsIKOverrides.rHandSimple.position, deltaTime);
            hadnsIKOverrides.lHandLerpedPos = Vector3.Lerp(hadnsIKOverrides.lHandLerpedPos, hadnsIKOverrides.lHandSimple.position, deltaTime);

            hadnsIKOverrides.rHandLerpedRot = Quaternion.Lerp(hadnsIKOverrides.rHandLerpedRot, hadnsIKOverrides.rHandSimple.rotation, deltaTime);
            hadnsIKOverrides.lHandLerpedRot = Quaternion.Lerp(hadnsIKOverrides.lHandLerpedRot, hadnsIKOverrides.lHandSimple.rotation, deltaTime);
        }


        characterAnimator.SetIKPosition(AvatarIKGoal.LeftHand, hadnsIKOverrides.lHandLerpedPos);
        characterAnimator.SetIKPosition(AvatarIKGoal.RightHand, hadnsIKOverrides.rHandLerpedPos);

        characterAnimator.SetIKRotation(AvatarIKGoal.LeftHand, hadnsIKOverrides.lHandLerpedRot);
        characterAnimator.SetIKRotation(AvatarIKGoal.RightHand, hadnsIKOverrides.rHandLerpedRot);
    }

    public override void CustomUpdate()
    {
        if (characterAnimator.GetBool(AnimatorHashes.AimingHash))
        {
            if (deltaTime < 15f)
                deltaTime += Time.deltaTime/*Mathf.Lerp(deltaTime, Time.deltaTime * 15f, Time.deltaTime)*/;

            overridedChest.overridedChestTransform.position = overridedChest.rightUpperArm.position;
            if ((overridedChest.target.position - overridedChest.weapon.position).magnitude > 2f)
            {
                overridedChest.overridedChestTransform.rotation = Quaternion.Lerp(overridedChest.overridedChestTransform.rotation, Quaternion.LookRotation(overridedChest.target.position - overridedChest.overridedChestTransform.position), deltaTime);
                Vector3 ZYDir = Vector3.ProjectOnPlane(overridedChest.target.position - overridedChest.overridedChestTransform.position, overridedChest.overridedChestTransform.right);
                overridedChest.weapon.rotation = Quaternion.Slerp(overridedChest.weapon.rotation, Quaternion.LookRotation(ZYDir), deltaTime);
            }
        }
        else
        {
            deltaTime = 0f;
        }
    }
}
