using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Animations.Rigging;

public class PlayerCharacterController : MonoBehaviour
{
    [SerializeField] private Transform aimPivot;
    [SerializeField] private PlayerCharacterControllerCData controllerData;
    [SerializeField] private Animator animator;
    [SerializeField] private RigBuilder rigBuilder;
    [SerializeField] private CharacterController controller;
    public Animator Animator
    {
        get => animator;
    }
    
    private Vector3 movementVelocity;
    private Vector3 dashVelocity;
    private Vector3 verticalVelocity;
    private Vector3 bodyAimPivotPosition;
    
    public void OnEnable()
    {
        PlayerInput.OnJumped += TryJump;
        PlayerInput.OnDashed += Dash;
        
        GameBeatSequencer.OnBPM += delegate
        {
            if (PlayerInput.Firing)
            {
                AppendAimPivotHeight(0.05f);
            }
        };
    }

    private void Dash()
    {
        dashVelocity.x = PlayerInput.Horizontal;
        dashVelocity.z = PlayerInput.Vertical;
        dashVelocity *= controllerData.dashSpeed;

        StartCoroutine(DelayedResetDashVelocity());
        IEnumerator DelayedResetDashVelocity()
        {
            yield return new WaitForSeconds(controllerData.dashDuration);
            dashVelocity = Vector3.zero;
        }
    }

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
        AppendAimPivotHeight(PlayerInput.MouseY);
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
        movementVelocity *= controllerData.movementSpeed;
        
        controller.Move(movementVelocity * Time.deltaTime);
    }

    private void ResetMovementVelocity()
    {
        movementVelocity = Vector3.zero;
        movementVelocity = transform.TransformDirection(movementVelocity);
        movementVelocity *= controllerData.movementSpeed;
    }

    private void ApplyGravity()
    {
        verticalVelocity += Physics.gravity * controllerData.gravityMultiplier * Time.deltaTime;
        verticalVelocity.y = Mathf.Clamp(verticalVelocity.y, Physics.gravity.y * controllerData.gravityMultiplier,
                                         controllerData.jumpSpeed);
        controller.Move(verticalVelocity * Time.deltaTime);
    }

    private void SetInputsToAnimator()
    {
        animator.SetBool(AnimatorHashes.AimingHash, true);
        
        animator.SetFloat(AnimatorHashes.VerticalHash, PlayerInput.Vertical,
                          controllerData.movingDamp, Time.fixedDeltaTime * controllerData.movingDeltaTime);
        animator.SetFloat(AnimatorHashes.HorizontalHash, PlayerInput.Horizontal,
                          controllerData.movingDamp, Time.fixedDeltaTime * controllerData.movingDeltaTime);
    }

    private void AppendAimPivotHeight(float appendHeight)
    {
        bodyAimPivotPosition.y += appendHeight;
        bodyAimPivotPosition.y = Mathf.Clamp(bodyAimPivotPosition.y,
                                                      controllerData.bodyAimPivotVerticalClamp.x, 
                                                      controllerData.bodyAimPivotVerticalClamp.y);
        bodyAimPivotPosition.z = 5f;
        aimPivot.localPosition = bodyAimPivotPosition;
    }

    private void ActualJump()
    {
        verticalVelocity.y = controllerData.jumpSpeed;
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
