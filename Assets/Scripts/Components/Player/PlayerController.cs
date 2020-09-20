using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Animations.Rigging;

public class PlayerController : MonoCached
{
    [SerializeField] private WeaponHolder weaponHolder;
    [SerializeField] private BodyData bodyData;
    [SerializeField] private CoverSensorsData coverSens;
    //[SerializeField] private AimingAndIKOverrider aimingAndIkOverrider;
    [SerializeField] private Animator animator;
    //[SerializeField] private Rigidbody rigidbody;
    [SerializeField] private CharacterController controller;
    [SerializeField] private RigBuilder rigBuilder;
    private Vector3 movementVelocity;
    private Vector3 verticalVelocity;
    
    private Vector3 originalAnimatorLocalPosition;
    private Quaternion originalAnimatorLocalRotation;

    private void Awake()
    {
        originalAnimatorLocalPosition = animator.transform.localPosition;
        originalAnimatorLocalRotation = animator.transform.localRotation;
    }
    
    public override void OnEnable()
    {
        base.OnEnable();
        PlayerInput.OnJumped += delegate
        {
            if (!controller.isGrounded)
                return;
            if (PlayerInput.Melee)
            {
                JumpWithSword();
            }
            else
            {
                JumpWithGun();
            }
        };
        
        PlayerInput.OnSwordAttacked += SwordAttack;
        PlayerInput.OnWeaponSwitched += delegate (bool Melee)
        {
            weaponHolder.SwitchWeapons();
            
            if (Melee)
            { 
                rigBuilder.enabled = false;
                animator.applyRootMotion = true;
            }
            else
            {
                animator.applyRootMotion = false;
                rigBuilder.enabled = true;

                animator.transform.localPosition = originalAnimatorLocalPosition;
                animator.transform.localRotation = originalAnimatorLocalRotation;
            }

            ResetVelocities();
        };
    }

    private void ResetVelocities()
    {
        verticalVelocity = Vector3.zero;
        movementVelocity = Vector3.zero;
    }
    
    public override void CustomUpdate()
    {
        Rotate();
        TryMove();
        ApplyGravity();
        SetInputsToAnimator();
        UpdateBodyAimPivot();

        if(!PlayerInput.Melee && PlayerInput.Firing)
        {
            weaponHolder.gun.Fire();
        }
    }

    private void Rotate()
    {
        if (PlayerInput.Melee)
        {
            animator.transform.rotation *= Quaternion.AngleAxis(PlayerInput.MouseX * 15f, animator.transform.up);
        }
        else
        {
            transform.rotation *= Quaternion.AngleAxis(PlayerInput.MouseX * 15f, transform.up);
        }
    }

    private void TryMove()
    {
        if (PlayerInput.Melee)
        {
            // if (!controller.isGrounded)
            // {
            //     animator.applyRootMotion = false;
            //     MoveByController();
            //     animator.applyRootMotion = true;
            // }
            return;
        }
        
        MoveByController();
    }

    private void MoveByController()
    {
        movementVelocity.x = PlayerInput.Horizontal;
        movementVelocity.z = PlayerInput.Vertical;
        movementVelocity = transform.TransformDirection(movementVelocity);
        movementVelocity *= bodyData.movementSpeed;
        controller.Move(movementVelocity * Time.deltaTime);
    }

    // private void MoveToAnimator()
    // {
    //     controller.enabled = false;
    //     
    //     controller.transform.position = animator.transform.position;
    //     
    //     animator.transform.RotateAround(animator.transform.position, animator.transform.forward, -originalAnimatorLocalRotation.eulerAngles.z);
    //     animator.transform.RotateAround(animator.transform.position, animator.transform.right, -originalAnimatorLocalRotation.eulerAngles.x);
    //     animator.transform.RotateAround(animator.transform.position, animator.transform.up, -originalAnimatorLocalRotation.eulerAngles.y);
    //
    //     controller.transform.rotation = animator.transform.rotation;
    //     
    //     controller.transform.position += -controller.transform.right * originalAnimatorLocalPosition.x;
    //     controller.transform.position += -controller.transform.up * originalAnimatorLocalPosition.y;
    //     controller.transform.position += -controller.transform.forward * originalAnimatorLocalPosition.z;
    //     
    //     animator.transform.localPosition = originalAnimatorLocalPosition;
    //     
    //     controller.enabled = true;
    // }

    private void ApplyGravity()
    {
        verticalVelocity += Physics.gravity * Time.deltaTime;
        verticalVelocity.y = Mathf.Clamp(verticalVelocity.y, Physics.gravity.y, bodyData.speedOnJump);
        controller.Move(verticalVelocity * Time.deltaTime);
    }

    private void SetInputsToAnimator()
    {
        animator.SetBool(AnimatorHashes.MeleeHash, PlayerInput.Melee);
        animator.SetBool(AnimatorHashes.AimingHash, true);
        animator.SetBool(AnimatorHashes.CrouchingHash, PlayerInput.Crouching);
        animator.SetBool(AnimatorHashes.ShiftingHash, PlayerInput.Shifting);
        
        animator.SetFloat(AnimatorHashes.VerticalHash, PlayerInput.Vertical, bodyData.movingDamp, Time.fixedDeltaTime * bodyData.movingDeltaTime);
        animator.SetFloat(AnimatorHashes.HorizontalHash, PlayerInput.Horizontal, bodyData.movingDamp, Time.fixedDeltaTime * bodyData.movingDeltaTime);
    }

    private void UpdateBodyAimPivot()
    {
        bodyData.bodyAimPivotPosition.y += PlayerInput.MouseY;
        bodyData.bodyAimPivotPosition.y = Mathf.Clamp(bodyData.bodyAimPivotPosition.y,
                                                      bodyData.bodyAimPivotVerticalClamp.x, 
                                                      bodyData.bodyAimPivotVerticalClamp.y);
        bodyData.bodyAimPivotPosition.z = 5f;
        bodyData.bodyAimPivot.localPosition = bodyData.bodyAimPivotPosition;
    }

    private void JumpWithGun()
    {
        ActualJump();
    }

    private void JumpWithSword()
    {
        animator.applyRootMotion = false;
        controller.enabled = true;
        
        DoActionOnLanding(
            delegate
            {
                ResetVelocities();
                controller.enabled = false;
                animator.applyRootMotion = true; 
            }, 
            0);
        
        ActualJump();
    }

    private void ActualJump()
    {
        verticalVelocity.y = bodyData.speedOnJump;
        animator.SetTrigger(AnimatorHashes.JumpHash);
        DoActionOnLanding(delegate { animator.SetTrigger(AnimatorHashes.LandingHash); }, 0);
    }

    private void DoActionOnLanding(Action onLanding, float delay)
    {
        StartCoroutine(waitForLanding());

        IEnumerator waitForLanding()
        {
            yield return null;
            while (!controller.isGrounded)
            {
                yield return null;
            }
            yield return new WaitForSeconds(delay);
            
            onLanding.Invoke();
        }
    }


    private void SwordAttack()
    {
        animator.SetTrigger(AnimatorHashes.SwordAttackHash);
    }


    // private void Start()
    // {
    //     SetUpAnimator();
    // }
    //
    // public void SetUpAnimator()
    // {
    //     animator.SetFloat(AnimatorHashes.CoverSideHash, 1);
    // }

    // private void OnTriggerStay(Collider other)
    // {
    //     if (other.CompareTag(Tags.Cover))
    //     {
    //         coverSens.currentCover = other.transform;
    //         animator.SetBool(AnimatorHashes.CoverHash, true);
    //         if (PlayerInput.Horizontal == 0)
    //         {
    //             return;
    //         }
    //         animator.SetFloat(AnimatorHashes.CoverSideHash, PlayerInput.Horizontal > 0 ? 1 : -1);
    //     }
    // }
    // private void OnTriggerExit(Collider other)
    // {
    //     if (other.CompareTag(Tags.Cover))
    //     {
    //         animator.SetBool(AnimatorHashes.CoverHash, false);
    //     }
    // }

    // private void OnAnimatorIK(int layerIndex)
    // {
    //     if(!PlayerInput.Melee /*&& PlayerInput.Aiming*/)
    //     {
    //         AimingTransforms();
    //     }
    //     // else
    //     // {
    //     //     SimpleWalkingTransforms();
    //     // }
    // }

    // public void CoverTransforms()
    // {
    //     if(animator.GetFloat(AnimatorHashes.VerticalHash) < -0.7f)
    //     {
    //         return;
    //     }
    //     if (!animator.GetBool(AnimatorHashes.AimingHash) && animator.GetBool(AnimatorHashes.CrouchingHash))
    //     {
    //         RaycastHit hitCover;
    //         Debug.DrawRay(coverSens.rightSensor.position, coverSens.rightSensor.forward, Color.blue);
    //         Debug.DrawRay(coverSens.leftSensor.position, coverSens.leftSensor.forward, Color.blue);
    //
    //         if (animator.GetFloat(AnimatorHashes.CoverSideHash) > 0)
    //         {
    //             if (Physics.Raycast(coverSens.rightSensor.position, coverSens.rightSensor.forward, out hitCover, 2f, 1<<10))
    //             {
    //                 coverSens.coverHelper.position = hitCover.point - coverSens.coverHelper.forward;
    //                 coverSens.currentCover = hitCover.transform;
    //                 coverSens.coverHelper.rotation = Quaternion.Lerp(coverSens.coverHelper.rotation, Quaternion.LookRotation(-hitCover.normal), Time.deltaTime * 10f);
    //             }
    //             else
    //             {
    //                 animator.SetFloat(AnimatorHashes.CoverSideHash, 2f);
    //             }
    //         }
    //         if (animator.GetFloat(AnimatorHashes.CoverSideHash) < 0)
    //         {
    //             if (Physics.Raycast(coverSens.leftSensor.position, coverSens.leftSensor.forward, out hitCover, 2f, 1 << 10))
    //             {
    //                 coverSens.coverHelper.position = hitCover.point - coverSens.coverHelper.forward;
    //                 coverSens.currentCover = hitCover.transform;
    //                 coverSens.coverHelper.rotation = Quaternion.Lerp(coverSens.coverHelper.rotation, Quaternion.LookRotation(-hitCover.normal), Time.deltaTime * 10f);
    //             }
    //             else
    //             {
    //                 animator.SetFloat(AnimatorHashes.CoverSideHash, -2f);
    //             }
    //         }
    //
    //         transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(-coverSens.currentCover.forward), Time.deltaTime * 2f);
    //     }
    // }

    // public void AimingTransforms()
    // {
    //     //PlayerCameraBehaviour.FieldOfView(35f);
    //
    //     if ((bodyData.mainCrossHair.position - weaponHolder.gun.transform.position).magnitude > 2f)
    //     {
    //         animator.SetLookAtWeight(0.5f, 1f, 1f);
    //         animator.SetLookAtPosition(bodyData.mainCrossHair.position);
    //     }
    // }

    // public void SimpleWalkingTransforms()
    // {
    //     //PlayerCameraBehaviour.FieldOfView(60);
    //     animator.SetLookAtWeight(1, 0.4f, 0.4f);
    //     animator.SetLookAtPosition(bodyData.bodyAimPivot.position);
    // }
}
