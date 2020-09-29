using System.Collections;
using System.Collections.Generic;
using System.Data;
using UnityEngine;
using Pathfinding;
using StateMachine;
using UnityEngine.Animations;


public class Durashka : Enemy
{
    public Gun gun;
    public Animator animator;

    [SerializeField] private LookAtConstraint[] aimingConstraints;
    
    private StateMachine<Durashka> stateMachine;
    private Transform currentAttackTarget;
    
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
        stateMachine.UpdateMachine();
    }

    private void OnAnimatorIK(int layerIndex)
    {
        if (animator.GetBool(AnimatorHashes.AimingHash))
        {
            BodyAiming();
        }
    }

    public void BodyAiming()
    {
        if ((currentAttackTarget.position - gun.transform.position).magnitude > 2f)
        {
            animator.SetLookAtWeight(0.5f, 1f, 1f);
            animator.SetLookAtPosition(currentAttackTarget.position);
        }
    }

    public override void MoveToNextNode()
    {
        Move(verticalMoveValue, horizontalMoveValue);
    }

    public void Move(float verticalValue, float horizontalValue)
    {
        animator.SetFloat(AnimatorHashes.VerticalHash, Mathf.Lerp(animator.GetFloat(AnimatorHashes.VerticalHash), verticalValue, Time.deltaTime * 5f));
        animator.SetFloat(AnimatorHashes.HorizontalHash, Mathf.Lerp(animator.GetFloat(AnimatorHashes.HorizontalHash), horizontalValue, Time.deltaTime * 5f));
    }
}
