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
    public Transform chestTransform;
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
        if (PlayerInput.Melee)
            return;
        
        if (PlayerInput.Aiming)
        {
            SetWeight(1f);

            CalculateLerpedIKs(hadnsIKOverrides.rHandAiming, hadnsIKOverrides.lHandAiming);
        }
        else
        {
            SetWeight(0.75f);

            CalculateLerpedIKs(hadnsIKOverrides.rHandSimple, hadnsIKOverrides.lHandSimple);
        }

        SetLerpedIK();
    }
    
    private void CalculateLerpedIKs(Transform targetR, Transform targetL)
    {
        deltaTime = Time.deltaTime * 20f;

        hadnsIKOverrides.rHandLerpedPos = Vector3.Lerp(hadnsIKOverrides.rHandLerpedPos, targetR.position, deltaTime);
        hadnsIKOverrides.lHandLerpedPos = Vector3.Lerp(hadnsIKOverrides.lHandLerpedPos, targetL.position, deltaTime);

        hadnsIKOverrides.rHandLerpedRot = Quaternion.Lerp(hadnsIKOverrides.rHandLerpedRot, targetR.rotation, deltaTime);
        hadnsIKOverrides.lHandLerpedRot = Quaternion.Lerp(hadnsIKOverrides.lHandLerpedRot, targetL.rotation, deltaTime);
    }

    private void SetLerpedIK()
    {
        characterAnimator.SetIKPosition(AvatarIKGoal.LeftHand, hadnsIKOverrides.lHandLerpedPos);
        characterAnimator.SetIKPosition(AvatarIKGoal.RightHand, hadnsIKOverrides.rHandLerpedPos);

        characterAnimator.SetIKRotation(AvatarIKGoal.LeftHand, hadnsIKOverrides.lHandLerpedRot);
        characterAnimator.SetIKRotation(AvatarIKGoal.RightHand, hadnsIKOverrides.rHandLerpedRot);
    }

    private void SetWeight(float weight)
    {
        characterAnimator.SetIKPositionWeight(AvatarIKGoal.LeftHand, weight);
        characterAnimator.SetIKPositionWeight(AvatarIKGoal.RightHand, weight);
        characterAnimator.SetIKRotationWeight(AvatarIKGoal.LeftHand, weight);
        characterAnimator.SetIKRotationWeight(AvatarIKGoal.RightHand, weight);
    }

    public override void CustomUpdate()
    {
        if (!PlayerInput.Melee && !PlayerInput.Aiming)
            return;
        
        overridedChest.chestTransform.position = overridedChest.rightUpperArm.position;
        if ((overridedChest.target.position - overridedChest.weapon.position).magnitude > 2f)
        {
            overridedChest.chestTransform.rotation = Quaternion.Lerp(overridedChest.chestTransform.rotation, Quaternion.LookRotation(overridedChest.target.position - overridedChest.chestTransform.position), deltaTime);
            Vector3 ZYDir = Vector3.ProjectOnPlane(overridedChest.target.position - overridedChest.chestTransform.position, overridedChest.chestTransform.right);
            overridedChest.weapon.rotation = Quaternion.Slerp(overridedChest.weapon.rotation, Quaternion.LookRotation(ZYDir), 15f);
        }
    }
}
