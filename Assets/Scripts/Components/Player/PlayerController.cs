using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Animations.Rigging;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private BodyData bodyData;
    [SerializeField] private PlayerWeaponHolder weaponHolder;
    [SerializeField] private Animator animator;
    [SerializeField] private RigBuilder rigBuilder;
    private CharacterController controller;
    public Animator Animator
    {
        get => animator;
    }
    
    private Vector3 movementVelocity;
    private Vector3 dashVelocity;
    private Vector3 verticalVelocity;
    
    private Vector3 originalAnimatorLocalPosition;
    private Quaternion originalAnimatorLocalRotation;

    private void Awake()
    {
        controller = GetComponent<CharacterController>();

        originalAnimatorLocalPosition = animator.transform.localPosition;
        originalAnimatorLocalRotation = animator.transform.localRotation;
        
        weaponHolder.gun.SetDamageValue(Constants.DamagePerHit.Player);
    }
    
    public void OnEnable()
    {
        // PlayerInput.OnWeaponSwitched += SwitchWeapon;
        // PlayerInput.OnSwordAttacked += SwordAttack;
        PlayerInput.OnJumped += TryJump;
        PlayerInput.OnDashed += Dash;
        
        GameBeatSequencer.OnBPM += delegate
        {
            if (PlayerInput.Firing)
            {
                weaponHolder.gun.Fire();
                UpdateBodyAimPivot(0.05f);
            }
        };
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

    // private void SwitchWeapon(bool Melee)
    // {
    //     weaponHolder.SwitchWeapons(Melee);
    // }
    //
    // private void SwordAttack()
    // {
    //     animator.SetTrigger(AnimatorHashes.SwordAttackHash);
    // }

    private void TryJump()
    {
        if (!controller.isGrounded)
            return;
        
        ActualJump();
    }
    
    public void Update()
    {
        Rotate();
        Move();
        ApplyGravity();
        SetInputsToAnimator();
        UpdateBodyAimPivot(PlayerInput.MouseY);
    }

    private void Rotate()
    {
        transform.rotation *= Quaternion.AngleAxis(PlayerInput.MouseX * 15f, transform.up);
    }

    private void Move()
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
    }

    private void ApplyGravity()
    {
        verticalVelocity += Physics.gravity * bodyData.gravityMultiplier * Time.deltaTime;
        verticalVelocity.y = Mathf.Clamp(verticalVelocity.y, Physics.gravity.y * bodyData.gravityMultiplier,
                                         bodyData.jumpSpeed);
        controller.Move(verticalVelocity * Time.deltaTime);
    }

    private void SetInputsToAnimator()
    {
        animator.SetBool(AnimatorHashes.AimingHash, true);
        
        animator.SetFloat(AnimatorHashes.VerticalHash, PlayerInput.Vertical, bodyData.movingDamp, Time.fixedDeltaTime * bodyData.movingDeltaTime);
        animator.SetFloat(AnimatorHashes.HorizontalHash, PlayerInput.Horizontal, bodyData.movingDamp, Time.fixedDeltaTime * bodyData.movingDeltaTime);
    }

    private void UpdateBodyAimPivot(float appendHeight)
    {
        bodyData.bodyAimPivotPosition.y += appendHeight;
        bodyData.bodyAimPivotPosition.y = Mathf.Clamp(bodyData.bodyAimPivotPosition.y,
                                                      bodyData.bodyAimPivotVerticalClamp.x, 
                                                      bodyData.bodyAimPivotVerticalClamp.y);
        bodyData.bodyAimPivotPosition.z = 5f;
        bodyData.bodyAimPivot.localPosition = bodyData.bodyAimPivotPosition;
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
}
