using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpiderAst : Enemy, IDamageable
{
    [Header("Movement")]
    [SerializeField] private float movementSpeed;
    [SerializeField] private float jumpSpeed;
    [SerializeField] private Transform explosionRoot;
    [SerializeField] private Animator animator;
    [SerializeField] private CharacterController controller;

    private Vector3 movementVelocity;
    private Vector3 verticalVelocity;
    private bool canAttack;
    
    public int BulletCountToDie { get; private set; }

    public override void Start()
    {
        base.Start();
        BulletCountToDie = Constants.BulletCountsToDie.SpiderAst;
        SetMoveAnimationSpeed();
        UpdatePathPeriodically();

        GameBeatSequencer.OnGeneratedBeat += delegate { SetAttackPossibility(true); };
    }

    public void SetAttackPossibility(bool value)
    {
        canAttack = value;
    }

    private void UpdatePathPeriodically()
    {
        StartCoroutine(UpdatePathToPlayerPeriodically());
        IEnumerator UpdatePathToPlayerPeriodically()
        {
            while (true)
            {
                UpdatePath(player.transform.position);
                yield return new WaitForSeconds(pathUpdPeriod);
            }
        }
    }

    private void SetMoveAnimationSpeed()
    {
        animator.SetFloat(AnimatorHashes.SpiderHashes.SpeedMultiplier, movementSpeed);
    }
    
    public override void CustomUpdate()
    {
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
        verticalVelocity.y = jumpSpeed;
        
        StartCoroutine(DelayedAttack());
        IEnumerator DelayedAttack()
        {
            SetAttackPossibility(false);
            yield return new WaitForSeconds(0.3f);
            while (!canAttack)
            {
                yield return null;
            }
            Attack();
        }
    }

    private void Attack()
    {
        SetAttackPossibility(false);
        ObjectPooler.Instance.SpawnObject(Constants.PoolMidExplosion, explosionRoot.position);
        ObjectPooler.Instance.ReturnObject(gameObject, gameObject.name);
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

    public void TakeDamage()
    {
        BulletCountToDie--;
        if (BulletCountToDie > 0)
        {
            return;
        }
        
        Attack();
    }
}
