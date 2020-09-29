using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Animations.Rigging;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private BodyData bodyData;
    [SerializeField] private PlayerWeaponHolder weaponHolder;
    [SerializeField] private Animator animator;
    [SerializeField] private CharacterController controller;
    [SerializeField] private RigBuilder rigBuilder;
    
    private Vector3 movementVelocity;
    private Vector3 dashVelocity;
    private Vector3 verticalVelocity;
    
    private Vector3 originalAnimatorLocalPosition;
    private Quaternion originalAnimatorLocalRotation;
    public Animator Animator
    {
        get => animator;
    }


    private void Awake()
    {
        originalAnimatorLocalPosition = animator.transform.localPosition;
        originalAnimatorLocalRotation = animator.transform.localRotation;
    }
    
    public void OnEnable()
    {
        PlayerInput.OnWeaponSwitched += SwitchWeapon;
        PlayerInput.OnSwordAttacked += SwordAttack;
        PlayerInput.OnJumped += TryJump;
        PlayerInput.OnDashed += Dash;
    }

    private void Dash()
    {
        dashVelocity.x = PlayerInput.Horizontal;
        dashVelocity.z = PlayerInput.Vertical;
        dashVelocity *= bodyData.dashSpeed;

        StartCoroutine(DelayedResetDashVelocity());
        IEnumerator DelayedResetDashVelocity()
        {
            yield return new WaitForSeconds(bodyData.dashDuration);
            dashVelocity = Vector3.zero;
        }
    }

    private void SwitchWeapon(bool Melee)
    {
        weaponHolder.SwitchWeapons(Melee);

        if (Melee)
        {
            rigBuilder.enabled = false;
            animator.applyRootMotion = true;
        }
        else
        {
            animator.applyRootMotion = false;
            rigBuilder.enabled = true;
        }
    }
    
    private void SwordAttack()
    {
        animator.SetTrigger(AnimatorHashes.SwordAttackHash);
    }

    private void TryJump()
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
    }
    
    public void Update()
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
        if (animator.applyRootMotion)
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
        if (animator.applyRootMotion)
        {
            MoveByAnimator();
        }
        else
        {
            MoveByController();
        }
    }

    private void MoveByAnimator()
    {
        controller.enabled = false;

        transform.position = animator.transform.position;

        animator.transform.RotateAround(animator.transform.position, animator.transform.forward,
                                        -originalAnimatorLocalRotation.eulerAngles.z);
        animator.transform.RotateAround(animator.transform.position, animator.transform.right,
                                        -originalAnimatorLocalRotation.eulerAngles.x);
        animator.transform.RotateAround(animator.transform.position, animator.transform.up,
                                        -originalAnimatorLocalRotation.eulerAngles.y);

        transform.rotation = animator.transform.rotation;

        transform.position += -transform.right * originalAnimatorLocalPosition.x;
        transform.position += -transform.up * originalAnimatorLocalPosition.y;
        transform.position += -transform.forward * originalAnimatorLocalPosition.z;

        animator.transform.localRotation = originalAnimatorLocalRotation;
        animator.transform.localPosition = originalAnimatorLocalPosition;
        
        controller.enabled = true;
    }

    private void MoveByController()
    {
        ResetMovementVelocity();

        movementVelocity.x = PlayerInput.Horizontal;
        movementVelocity.z = PlayerInput.Vertical;
        
        movementVelocity += dashVelocity;
        
        movementVelocity = transform.TransformDirection(movementVelocity);
        movementVelocity *= bodyData.movementSpeed;


        controller.Move(movementVelocity * Time.deltaTime);
    }

    private void ResetMovementVelocity()
    {
        movementVelocity = Vector3.zero;
        movementVelocity = transform.TransformDirection(movementVelocity);
        movementVelocity *= bodyData.movementSpeed;
        //controller.Move(movementVelocity * Time.deltaTime);
    }

    private void ApplyGravity()
    {
        verticalVelocity += Physics.gravity * Time.deltaTime;
        verticalVelocity.y = Mathf.Clamp(verticalVelocity.y, Physics.gravity.y, bodyData.jumpSpeed);
        controller.Move(verticalVelocity * Time.deltaTime);
    }

    private void SetInputsToAnimator()
    {
        animator.SetBool(AnimatorHashes.MeleeHash, PlayerInput.Melee);
        animator.SetBool(AnimatorHashes.AimingHash, true);
        
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
        DoActionOnLanding(delegate { animator.applyRootMotion = true; });
        
        ResetMovementVelocity();
        ActualJump();
    }

    private void ActualJump()
    {
        verticalVelocity.y = bodyData.jumpSpeed;
        animator.SetTrigger(AnimatorHashes.JumpHash);
        DoActionOnLanding(delegate { animator.SetTrigger(AnimatorHashes.LandingHash); });
    }

    private void DoActionOnLanding(Action onLanding)
    {
        StartCoroutine(waitForLanding());

        IEnumerator waitForLanding()
        {
            yield return null;
            while (!controller.isGrounded)
            {
                yield return null;
            }
            
            onLanding.Invoke();
        }
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
    //     if (other.CompareTag(Constants.Cover))
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
    //     if (other.CompareTag(Constants.Cover))
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
