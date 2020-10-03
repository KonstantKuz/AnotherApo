using System.Collections;
using System.Collections.Generic;
using System.Data;
using UnityEngine;
using Pathfinding;
using StateMachine;
using UnityEngine.Animations;


public class Durashka : Enemy
{
    [Header("Movement")]
    [SerializeField] private float movementSpeed;
    [SerializeField] private float jumpSpeed;
    [SerializeField] private Animator animator;
    [SerializeField] private CharacterController Controller;
    
    [Header("Targeting")]
    [SerializeField] private Gun gun;
    [SerializeField] private float aimingSpeed;
    [SerializeField] private TargetInterpolator targetInterpolator;
    [SerializeField] private LookAtConstraint[] aimingConstraints;
    
    private Transform currentAttackTarget;

    private Vector3 movementVelocity;
    private Vector3 verticalVelocity;
    private Vector3 dashVelocity;
    
    public override void Start()
    {
        base.Start();
        StartAiming();
        pathUpdPeriod = 2;
        
        GoToRandomPlayerSidePeriodically();
        
        GameBeatSequencer.OnBPM += delegate
        {
            if (Random.value > 0.5f)
            {
                gun.Fire();
            }
        };
    }

    private void GoToRandomPlayerSidePeriodically()
    {
        StartCoroutine(GoToRandomSide());
        IEnumerator GoToRandomSide()
        {
            while (true)
            {
                GoToRandomPlayerSide();
                yield return new WaitForSeconds(pathUpdPeriod);
            }
        }
    }
    
    private void GoToRandomPlayerSide()
    {
        Vector3 side = player.transform.right * RandomSign() + player.transform.forward * RandomSign();
        Vector3 pointToGo = player.transform.position + side * Random.Range(5,8);
        
        NavGraph generalGraph = 
            AstarPath.active.data.FindGraph(graphToFind => graphToFind.name == Constants.GeneralGraph);
        
        Vector3 resultNode = (Vector3)generalGraph.GetNearest(pointToGo).node.position;

        UpdatePath(resultNode);
    }

    private int RandomSign()
    {
        return Random.value > 0.5f ? 1 : -1;
    }

    private void StartAiming()
    {
        animator.SetBool(AnimatorHashes.AimingHash, true);
        currentAttackTarget = player.Animator.GetBoneTransform(HumanBodyBones.Spine);
        targetInterpolator.SetConstraint(currentAttackTarget, aimingSpeed);
        for (int i = 0; i < aimingConstraints.Length; i++)
        {
            aimingConstraints[i].AddSource(Interpolator());
        }
    }

    private ConstraintSource Interpolator()
    {
        ConstraintSource interpolatorSource = new ConstraintSource();
        interpolatorSource.weight = 1;
        interpolatorSource.sourceTransform = targetInterpolator.transform;
        return interpolatorSource;
    }
    
    public override void CustomUpdate()
    {
        if (player == null)
            return;
        
        ApplyGravity();

        RotateTowards(player.transform.position);

        CleanPassedNodes();
        if (!HasPath() || PathFullyPassed())
        {
            Move(0,0);
            return;
        }
        
        CalculateMoveValuesToNextNode();
        MoveToNextNode();
    }

    private void ApplyGravity()
    {
        verticalVelocity += Physics.gravity * Time.deltaTime;
        verticalVelocity.y = Mathf.Clamp(verticalVelocity.y, Physics.gravity.y, jumpSpeed);
        Controller.Move(verticalVelocity * Time.deltaTime);
    }

    public override void MoveToNextNode()
    {
        Move(verticalMoveValue, horizontalMoveValue);
    }

    public void Move(float verticalValue, float horizontalValue)
    {
        float interpolatedVertical =
            Mathf.Lerp(animator.GetFloat(AnimatorHashes.VerticalHash), verticalValue, Time.deltaTime * 5f);
        
        float interpolatedHorizontal =
            Mathf.Lerp(animator.GetFloat(AnimatorHashes.HorizontalHash), horizontalValue, Time.deltaTime * 5f);
        
        animator.SetFloat(AnimatorHashes.VerticalHash, interpolatedVertical);
        animator.SetFloat(AnimatorHashes.HorizontalHash, interpolatedHorizontal);
        
        movementVelocity = Vector3.zero;

        movementVelocity.x = interpolatedHorizontal;
        movementVelocity.z = interpolatedVertical;
        
        movementVelocity = transform.TransformDirection(movementVelocity);
        movementVelocity *= movementSpeed;

        Controller.Move(movementVelocity * Time.deltaTime);
    }
}
