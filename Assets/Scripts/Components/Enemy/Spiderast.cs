using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spiderast : Enemy
{
    [SerializeField] private float movementSpeed;
    [SerializeField] private float jumpSpeed;
    [SerializeField] private Transform explosionRoot;
    [SerializeField] private Animator animator;
    [SerializeField] private CharacterController controller;

    private Vector3 movementVelocity;
    private Vector3 verticalVelocity;
    
    public override void Start()
    {
        base.Start();
        
        SetMoveAnimationSpeed();
        
        pathUpdPeriod = 0.5f;
        UpdatePathPeriodically();
    }

    private void UpdatePathPeriodically()
    {
        StartCoroutine(UpdatePathToPlayerPeriodically());
        IEnumerator UpdatePathToPlayerPeriodically()
        {
            UpdatePath(player.transform.position);
            yield return new WaitForSeconds(pathUpdPeriod);
            StartCoroutine(UpdatePathToPlayerPeriodically());
        }
    }

    private void SetMoveAnimationSpeed()
    {
        animator.SetFloat(AnimatorHashes.SpiderHashes.SpeedMultiplier, movementSpeed);
    }
    
    public override void CustomUpdate()
    {
        SetMoveAnimationSpeed();
        
        if (player == null)
            return;
        
        ApplyGravity();
        CleanPassedNodes();
        
        if (!HasPath() || PathFullyPassed())
        {
            JumpThenAttack();
            return;
        }
        
        RotateTowards(currentPath.vectorPath[currentPathNodeIndex]);
        MoveToNextNode();
    }

    private void JumpThenAttack()
    {
        animator.SetTrigger(AnimatorHashes.SpiderHashes.Jump);
        verticalVelocity.y += jumpSpeed;
        
        StartCoroutine(Attack());
        IEnumerator Attack()
        {
            yield return new WaitForSeconds(0.5f);
            ObjectPooler.Instance.SpawnObject(Constants.PoolMidExplosion, explosionRoot.position);
            ObjectPooler.Instance.ReturnObject(gameObject, gameObject.name);
        }
    }

    private void ApplyGravity()
    {
        verticalVelocity += Physics.gravity * Time.deltaTime;
        verticalVelocity.y = Mathf.Clamp(verticalVelocity.y, Physics.gravity.y, jumpSpeed);
        controller.Move(verticalVelocity * Time.deltaTime);
    }

    public override void MoveToNextNode()
    {
        movementVelocity = Vector3.zero;
        
        movementVelocity.z = movementSpeed;
        movementVelocity = transform.TransformDirection(movementVelocity);
        movementVelocity *= movementSpeed;

        controller.Move(movementVelocity * Time.deltaTime);
    }
}
