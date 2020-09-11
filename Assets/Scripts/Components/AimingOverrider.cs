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
public class SpineOverride
{
    public Transform spineTransform;
    [HideInInspector]
    public Transform rightUpperArm;
}

public class AimingOverrider : MonoCached
{
    [SerializeField] private Animator characterAnimator;
    [SerializeField] private HandIKOverride hadnsIKOverrider;
    [SerializeField] private SpineOverride spineOverrider;
    [SerializeField] private WeaponHolder weaponHolder;
    [SerializeField] private Transform target;

    private float deltaTime;
    
    private void Start()
    {
        spineOverrider.rightUpperArm = characterAnimator.GetBoneTransform(HumanBodyBones.RightUpperArm);
    }

    private void OnAnimatorIK(int layerIndex)
    {
        if (IsMelee())
            return;
        
        if (IsAiming())
        {
            SetWeight(1f);

            CalculateLerpedIKs(hadnsIKOverrider.rHandAiming, hadnsIKOverrider.lHandAiming);
        }
        else
        {
            SetWeight(0.75f);

            CalculateLerpedIKs(hadnsIKOverrider.rHandSimple, hadnsIKOverrider.lHandSimple);
        }

        SetLerpedIK();
    }
    
    private void CalculateLerpedIKs(Transform targetR, Transform targetL)
    {
        deltaTime = Time.deltaTime * 20f;

        hadnsIKOverrider.rHandLerpedPos = Vector3.Lerp(hadnsIKOverrider.rHandLerpedPos, targetR.position, deltaTime);
        hadnsIKOverrider.lHandLerpedPos = Vector3.Lerp(hadnsIKOverrider.lHandLerpedPos, targetL.position, deltaTime);

        hadnsIKOverrider.rHandLerpedRot = Quaternion.Lerp(hadnsIKOverrider.rHandLerpedRot, targetR.rotation, deltaTime);
        hadnsIKOverrider.lHandLerpedRot = Quaternion.Lerp(hadnsIKOverrider.lHandLerpedRot, targetL.rotation, deltaTime);
    }

    private void SetLerpedIK()
    {
        characterAnimator.SetIKPosition(AvatarIKGoal.LeftHand, hadnsIKOverrider.lHandLerpedPos);
        characterAnimator.SetIKPosition(AvatarIKGoal.RightHand, hadnsIKOverrider.rHandLerpedPos);

        characterAnimator.SetIKRotation(AvatarIKGoal.LeftHand, hadnsIKOverrider.lHandLerpedRot);
        characterAnimator.SetIKRotation(AvatarIKGoal.RightHand, hadnsIKOverrider.rHandLerpedRot);
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
        if (IsMelee() || !IsAiming())
            return;
        
        SetSpinePositionAsRightUpperArm();
        if (TargetIsAhead())
        {
            RotateSpineTowardsTarget();

            RotateGunTowardsTarget();
        }
    }

    private void RotateSpineTowardsTarget()
    {
        Quaternion spineLookRotation = Quaternion.LookRotation(target.position - spineOverrider.spineTransform.position);
        spineOverrider.spineTransform.rotation =
            Quaternion.Lerp(spineOverrider.spineTransform.rotation, spineLookRotation, deltaTime);
    }

    private void RotateGunTowardsTarget()
    {
        Quaternion gunLookRotation =
            Quaternion.LookRotation(target.position -
                                    weaponHolder.gun.transform.position); //Quaternion.LookRotation(projectedDirection_ZY);
        weaponHolder.gun.transform.rotation =
            Quaternion.Lerp(weaponHolder.gun.transform.rotation, gunLookRotation, deltaTime);
    }

    private void SetSpinePositionAsRightUpperArm()
    {
        spineOverrider.spineTransform.position = spineOverrider.rightUpperArm.position;
    }

    private bool TargetIsAhead()
    {
        Vector3 targetDirection = target.position - weaponHolder.gun.transform.position;
        // Debug.Log($"Angle btw animator & target == {SignBtnw(targetDirection, characterAnimator.transform.forward, characterAnimator.transform.up)}");

        float distanceToTarget = targetDirection.magnitude;
        // float direction = SignBtnw(targetDirection, characterAnimator.transform.forward,
        //                            characterAnimator.transform.up);
        
        if (distanceToTarget > 1f)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    private bool IsMelee()
    {
        return characterAnimator.GetBool(AnimatorHashes.MeleeHash);
    }

    private bool IsAiming()
    {
        return characterAnimator.GetBool(AnimatorHashes.AimingHash);
    }
    
    public static float SignBtnw(Vector3 from, Vector3 to, Vector3 axis)
    {
        float cross_x = from.y * to.z - from.z * to.y;
        float cross_y = from.z * to.x - from.x * to.z;
        float cross_z = from.x * to.y - from.y * to.x;
        float sign = Mathf.Sign(axis.x * cross_x + axis.y * cross_y + axis.z * cross_z);
        return sign;
    }
}