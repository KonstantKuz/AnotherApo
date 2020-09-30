using System.Collections;
using System.Collections.Generic;
using System.Data;
using UnityEngine;
using Pathfinding;
using StateMachine;
using UnityEngine.Animations;


public class Durashka : Enemy
{
    public Gun Gun;
    public Animator Animator;
    public CharacterController Controller;
    [SerializeField] private float movementSpeed;
    [SerializeField] private float jumpSpeed;
    [SerializeField] private LookAtConstraint[] aimingConstraints;
    
    private StateMachine<Durashka> stateMachine;
    private Transform currentAttackTarget;

    private Vector3 movementVelocity;
    private Vector3 verticalVelocity;
    private Vector3 dashVelocity;
    
    public override void Start()
    {
        base.Start();
        StartAiming();
        SetUpStateMachine();
    }

    private void StartAiming()
    {
        currentAttackTarget = player.Animator.GetBoneTransform(HumanBodyBones.Spine);
        
        for (int i = 0; i < aimingConstraints.Length; i++)
        { 
            ConstraintSource source = new ConstraintSource();
            source.weight = 1;
            source.sourceTransform = currentAttackTarget;
            aimingConstraints[i].SetSource(0, source);
        }
    }

    public void SetUpStateMachine()
    {
        stateMachine = new StateMachine<Durashka>(this);
        stateMachine.ChangeState(AggressiveState.Instance);
    }
    
    public override void CustomUpdate()
    {
        ApplyGravity();
        stateMachine.UpdateMachine();
    }

    private void ApplyGravity()
    {
        verticalVelocity += Physics.gravity * Time.deltaTime;
        verticalVelocity.y = Mathf.Clamp(verticalVelocity.y, Physics.gravity.y, jumpSpeed);
        Controller.Move(verticalVelocity * Time.deltaTime);
    }

    private void OnAnimatorIK(int layerIndex)
    {
        if (Animator.GetBool(AnimatorHashes.AimingHash))
        {
            BodyAiming();
        }
    }

    public void BodyAiming()
    {
        if ((currentAttackTarget.position - Gun.transform.position).magnitude > 2f)
        {
            Animator.SetLookAtWeight(0.5f, 1f, 1f);
            Animator.SetLookAtPosition(currentAttackTarget.position);
        }
    }

    public override void MoveToNextNode()
    {
        Move(verticalMoveValue, horizontalMoveValue);
    }

    public void Move(float verticalValue, float horizontalValue)
    {
        float interpolatedVertical =
            Mathf.Lerp(Animator.GetFloat(AnimatorHashes.VerticalHash), verticalValue, Time.deltaTime * 5f);
        
        float interpolatedHorizontal =
            Mathf.Lerp(Animator.GetFloat(AnimatorHashes.HorizontalHash), horizontalValue, Time.deltaTime * 5f);
        
        Animator.SetFloat(AnimatorHashes.VerticalHash, interpolatedVertical);
        Animator.SetFloat(AnimatorHashes.HorizontalHash, interpolatedHorizontal);
        
        movementVelocity = Vector3.zero;

        movementVelocity.x = interpolatedHorizontal;
        movementVelocity.z = interpolatedVertical;
        
        movementVelocity = transform.TransformDirection(movementVelocity);
        movementVelocity *= movementSpeed;

        Controller.Move(movementVelocity * Time.deltaTime);
    }
}
