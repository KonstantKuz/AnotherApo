using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;
public class SpiderAst : Enemy, IDamageable
{
    [Header("Attack")] 
    [SerializeField] private float attackRadius;
    
    [Header("Movement")]
    [SerializeField] private float movementSpeed;
    [SerializeField] private float jumpSpeed;
    [SerializeField] private Transform explosionRoot;
    [SerializeField] private Animator animator;
    [SerializeField] private CharacterController controller;

    private Vector3 movementVelocity;
    private Vector3 verticalVelocity;
    private bool canAttack;
    
    public int TotalHealth { get; private set; }

    public override void ResetEnemy()
    {
        TotalHealth = Constants.TotalHealth.SpiderAst;
        SetMoveAnimationSpeed();
        UpdatePathPeriodically();
        SubscribeToBeat();
    }

    private void SubscribeToBeat()
    {
        GameBeatSequencer.OnGeneratedBeat += delegate { SetAttackPossibility(true); };
    }

    private void UnsubscribeFromBeat()
    {
        GameBeatSequencer.OnGeneratedBeat -= delegate { SetAttackPossibility(true); };
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
                UpdatePath(player.transform.position + player.transform.forward * Random.Range(-2,0));
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
        animator.SetTrigger(AnimatorHashes.JumpHash);
        verticalVelocity.y = jumpSpeed;
        
        StartCoroutine(DelayedAttack());
        IEnumerator DelayedAttack()
        {
            SetAttackPossibility(false);
            //yield return new WaitForSeconds(0.3f);
            while (!canAttack)
            {
                yield return null;
            }
            Attack();
        }
    }

    private void Attack()
    {
        OnDeath?.Invoke();
        ClearPlayer();
        StopAllCoroutines();

        SetAttackPossibility(false);
        SpawnDamagingExplosion();
        UnsubscribeFromBeat();
        ObjectPooler.Instance.ReturnObject(gameObject, gameObject.name);
    }

    private void SpawnDamagingExplosion()
    {
        Explosion explosion = ObjectPooler.Instance.SpawnObject(Constants.PoolExplosionMid).GetComponent<Explosion>();
        explosion.transform.position = explosionRoot.position;
        explosion.Explode(1 << LayerMasks.Player, Constants.DamagePerHit.SpiderAst, attackRadius);
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

    public void TakeDamage(int value)
    {
        TotalHealth-=value;
        if (TotalHealth > 0)
        {
            return;
        }
        
        Attack();
    }

    public override void OnDrawGizmos()
    {
        base.OnDrawGizmos();
        
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, attackRadius);
    }
}
