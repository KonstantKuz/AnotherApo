using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class CoverSensorsData
{
    public Transform currentCover { get; set; }
    public Transform leftSensor, rightSensor, coverHelper;
}
[System.Serializable]
public class BodyData
{
    public Transform bodyAimPivot, mainCrossHair;

    [HideInInspector]
    public Vector3 bodyAimPivotPosition;
    public float movingDamp, movingDeltaTime;
}


public class PlayerController : MonoCached
{
    public Weapon weapon;

    public BodyData bodyData;

    public CoverSensorsData coverSens;

    private Animator animator;
    private AimingOverrider hands;

    void Start()
    {
        _transform = transform;

        SetUpAnimtor();
        SetUpHands();
    }

    public void SetUpAnimtor()
    {
        animator = GetComponent<Animator>();
        animator.SetFloat(AnimatorHashes.CoverSideHash, 1);
    }

    public void SetUpHands()
    {
        hands = GetComponent<AimingOverrider>();
        hands.overridedChest.target = bodyData.mainCrossHair;
        hands.overridedChest.weapon = weapon._transform;
        hands.characterAnimator = animator;
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag(Tags.Cover))
        {
            coverSens.currentCover = other.transform;
            animator.SetBool(AnimatorHashes.CoverHash, true);
            if (PlayerInput.Horizontal == 0)
            {
                return;
            }
            animator.SetFloat(AnimatorHashes.CoverSideHash, PlayerInput.Horizontal > 0 ? 1 : -1);
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag(Tags.Cover))
        {
            animator.SetBool(AnimatorHashes.CoverHash, false);
        }
    }

    private void OnAnimatorIK(int layerIndex)
    {
        if(PlayerInput.Aiming)
        {
            AimingTransforms();
        }
        else
        {
            SimpleWalkingTransforms();
        }
    }

    public override void CustomFixedUpdate()
    {
        HandleInputs();
    }

    public void HandleInputs()
    {
        animator.SetBool(AnimatorHashes.AimingHash, PlayerInput.Aiming);
        animator.SetBool(AnimatorHashes.CrouchingHash, PlayerInput.Crouching);
        animator.SetFloat(AnimatorHashes.VerticalHash, PlayerInput.Vertical, bodyData.movingDamp, Time.fixedDeltaTime * bodyData.movingDeltaTime);
        animator.SetFloat(AnimatorHashes.HorizontalHash, PlayerInput.Horizontal, bodyData.movingDamp, Time.fixedDeltaTime * bodyData.movingDeltaTime);
        animator.SetFloat(AnimatorHashes.Mouse_YHash, PlayerInput.MouseY);

        _transform.rotation *= Quaternion.AngleAxis(PlayerInput.MouseX * 15f, transform.up);

        bodyData.bodyAimPivotPosition.y += PlayerInput.MouseY;
        bodyData.bodyAimPivotPosition.y = Mathf.Clamp(bodyData.bodyAimPivotPosition.y, -0.7f, 3.5f);
        bodyData.bodyAimPivotPosition.z = 5f;
        bodyData.bodyAimPivot.localPosition = bodyData.bodyAimPivotPosition;

        if(PlayerInput.Firing)
        {
            Fire();
        }

        if(animator.GetBool(AnimatorHashes.CoverHash))
        {
            CoverTransforms();
        }
    }

    public void CoverTransforms()
    {
        if(animator.GetFloat(AnimatorHashes.VerticalHash) < -0.7f)
        {
            return;
        }
        if (!animator.GetBool(AnimatorHashes.AimingHash) && animator.GetBool(AnimatorHashes.CrouchingHash))
        {
            RaycastHit hitCover;
            Debug.DrawRay(coverSens.rightSensor.position, coverSens.rightSensor.forward, Color.blue);
            Debug.DrawRay(coverSens.leftSensor.position, coverSens.leftSensor.forward, Color.blue);

            if (animator.GetFloat(AnimatorHashes.CoverSideHash) > 0)
            {
                if (Physics.Raycast(coverSens.rightSensor.position, coverSens.rightSensor.forward, out hitCover, 2f, 1<<10))
                {
                    coverSens.coverHelper.position = hitCover.point - coverSens.coverHelper.forward;
                    coverSens.currentCover = hitCover.transform;
                    coverSens.coverHelper.rotation = Quaternion.Lerp(coverSens.coverHelper.rotation, Quaternion.LookRotation(-hitCover.normal), Time.deltaTime * 10f);
                }
                else
                {
                    animator.SetFloat(AnimatorHashes.CoverSideHash, 2f);
                }
            }
            if (animator.GetFloat(AnimatorHashes.CoverSideHash) < 0)
            {
                if (Physics.Raycast(coverSens.leftSensor.position, coverSens.leftSensor.forward, out hitCover, 2f, 1 << 10))
                {
                    coverSens.coverHelper.position = hitCover.point - coverSens.coverHelper.forward;
                    coverSens.currentCover = hitCover.transform;
                    coverSens.coverHelper.rotation = Quaternion.Lerp(coverSens.coverHelper.rotation, Quaternion.LookRotation(-hitCover.normal), Time.deltaTime * 10f);
                }
                else
                {
                    animator.SetFloat(AnimatorHashes.CoverSideHash, -2f);
                }
            }

            _transform.rotation = Quaternion.Slerp(_transform.rotation, Quaternion.LookRotation(-coverSens.currentCover.forward), Time.deltaTime * 2f);

            //_transform.rotation = Quaternion.Slerp(_transform.rotation, Quaternion.LookRotation(coverSens.coverHelper.forward/2f), Time.deltaTime * 3f);
        }
        else
        {

        }
    }

    public void AimingTransforms()
    {
        PlayerCameraBehaviour.FieldOfView(35f);

        if ((bodyData.mainCrossHair.position - weapon._transform.position).magnitude > 2f)
        {
            animator.SetLookAtWeight(0.5f, 1f, 1f);
            animator.SetLookAtPosition(bodyData.mainCrossHair.position);
        }
    }

    public void SimpleWalkingTransforms()
    {
        PlayerCameraBehaviour.FieldOfView(60);
        animator.SetLookAtWeight(1, 0.4f, 0.4f);
        animator.SetLookAtPosition(bodyData.bodyAimPivot.position);
    }

    public void Fire()
    {
        weapon.Fire();
    }
}
