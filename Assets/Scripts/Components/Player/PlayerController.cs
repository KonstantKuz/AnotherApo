using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class PlayerController : MonoCached
{
    [SerializeField] private WeaponHolder weaponHolder;
    [SerializeField] private BodyData bodyData;
    [SerializeField] private CoverSensorsData coverSens;
    [SerializeField] private Animator animator;
    [SerializeField] private Rigidbody rigidbody;
    [SerializeField] private CapsuleCollider collider;
    [SerializeField] private AimingOverrider aimingHands;
    
    private static bool isGrounded;
    public static bool IsGrounded
    {
        get { return isGrounded; }
    }

    public override void OnEnable()
    {
        base.OnEnable();
        PlayerInput.OnJumped += TryJump;
        PlayerInput.OnSwordAttacked += SwordAttack;
        PlayerInput.OnWeaponSwitched += weaponHolder.SwitchWeapons;
    }

    private void Start()
    {
        SetUpAnimator();
        SetUpHands();
    }

    public void SetUpAnimator()
    {
        animator.SetFloat(AnimatorHashes.CoverSideHash, 1);
    }

    public void SetUpHands()
    {
        aimingHands.overridedChest.target = bodyData.mainCrossHair;
        aimingHands.overridedChest.weapon = weaponHolder.gun.transform;
        aimingHands.characterAnimator = animator;
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
        CheckIsGrounded();
        SetInputsToAnimator();
        Rotate();
        UpdateBodyAimPivot();

        if(PlayerInput.Firing)
        {
            Fire();
        }

        if(animator.GetBool(AnimatorHashes.CoverHash))
        {
            CoverTransforms();
        }
    }
    
    private void CheckIsGrounded()
    {
        Vector3 rayStart = animator.bodyPosition;
        
        if (Physics.Raycast(rayStart, Vector3.down, out RaycastHit hit, bodyData.distanceToGround, LayerMasks.interactionLayer))
        {
            Debug.DrawLine(rayStart, hit.point, Color.green);
            isGrounded = true;
        }
        else
        {
            Debug.DrawLine(rayStart, rayStart + Vector3.down * bodyData.distanceToGround, Color.red);
            isGrounded = false;
        }
    }

    private void SetInputsToAnimator()
    {
        animator.SetBool(AnimatorHashes.MeleeHash, PlayerInput.Melee);
        animator.SetBool(AnimatorHashes.AimingHash, PlayerInput.Aiming);
        animator.SetBool(AnimatorHashes.CrouchingHash, PlayerInput.Crouching);
        animator.SetBool(AnimatorHashes.ShiftingHash, PlayerInput.Shifting);
        animator.SetFloat(AnimatorHashes.VerticalHash, PlayerInput.Vertical, bodyData.movingDamp, Time.fixedDeltaTime * bodyData.movingDeltaTime);
        animator.SetFloat(AnimatorHashes.HorizontalHash, PlayerInput.Horizontal, bodyData.movingDamp, Time.fixedDeltaTime * bodyData.movingDeltaTime);
        animator.SetFloat(AnimatorHashes.Mouse_YHash, PlayerInput.MouseY); 
    }

    private void TryJump()
    {
        if (!IsGrounded)
            return;
        
        HandleRootMotionOnJump();
        animator.SetTrigger(AnimatorHashes.JumpHash);
        AddJumpForce();
        //ControlInAir();
        CheckForLanding();
    }

    private void HandleRootMotionOnJump()
    {
        animator.applyRootMotion = false;
        StartCoroutine(delayedRootMotionEnable());
        IEnumerator delayedRootMotionEnable()
        {
            yield return new WaitForSeconds(0.5f);
            while (!IsGrounded)
            {
                yield return new WaitForEndOfFrame();
            }
            animator.applyRootMotion = true;
        }
    }

    private void AddJumpForce()
    {
        Vector3 jumpDirection = transform.forward * PlayerInput.Vertical + transform.right * PlayerInput.Horizontal;
        rigidbody.AddForce(transform.up * bodyData.verticalJumpForce + jumpDirection * bodyData.horizontalJumpForce,
                           ForceMode.VelocityChange);
    }

    // private void ControlInAir()
    // {
    //     StartCoroutine(controlInAir());
    //     IEnumerator controlInAir()
    //     {
    //         yield return new WaitForSeconds(0.5f);
    //         while (!IsGrounded)
    //         {
    //             Vector3 airDirection = transform.forward * PlayerInput.Vertical + transform.right * PlayerInput.Horizontal;
    //             rigidbody.AddForce(airDirection * bodyData.horizontalJumpForce/2,
    //                                ForceMode.VelocityChange);
    //             yield return new WaitForEndOfFrame();
    //         }
    //     }
    // }

    private void CheckForLanding()
    {
        StartCoroutine(jumpHandle());

        IEnumerator jumpHandle()
        {
            yield return new WaitForSeconds(0.5f);
            while (!IsGrounded)
            {
                yield return new WaitForEndOfFrame();
            }
            animator.SetTrigger(AnimatorHashes.LandingHash);
        }
    }

    private void SwordAttack()
    {
        animator.SetTrigger(AnimatorHashes.SwordAttackHash);
    }

    private void Rotate()
    {
        transform.rotation *= Quaternion.AngleAxis(PlayerInput.MouseX * 15f, transform.up);
    }

    private void UpdateBodyAimPivot()
    {
        bodyData.bodyAimPivotPosition.y += PlayerInput.MouseY;
        bodyData.bodyAimPivotPosition.y = Mathf.Clamp(bodyData.bodyAimPivotPosition.y, -0.7f, 3.5f);
        bodyData.bodyAimPivotPosition.z = 5f;
        bodyData.bodyAimPivot.localPosition = bodyData.bodyAimPivotPosition;
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

            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(-coverSens.currentCover.forward), Time.deltaTime * 2f);
        }
    }

    public void AimingTransforms()
    {
        PlayerCameraBehaviour.FieldOfView(35f);

        if ((bodyData.mainCrossHair.position - weaponHolder.gun.transform.position).magnitude > 2f)
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
        weaponHolder.gun.Fire();
    }

    // private void OnDrawGizmos()
    // {
    //     Debug.DrawLine(aimingHands.overridedChest.chestTransform.position + aimingHands.overridedChest.chestTransform.up,
    //                    (aimingHands.overridedChest.chestTransform.position - aimingHands.overridedChest.chestTransform.up * bodyData.distanceToGround),
    //                    Color.green);
    // }
}
