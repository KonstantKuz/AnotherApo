using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;
public class SpiderAst : Enemy, IDamageable
{
    [Header("Attack")] 
    [SerializeField] private float attackRadius;
    [SerializeField] private LayerMask mask;
    
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

    public override void Start()
    {
        base.Start();
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
        ObjectPooler.Instance.SpawnObject(Constants.PoolExplosionMid, explosionRoot.position);
        SetActualDamage();
        UnsubscribeFromBeat();
        ObjectPooler.Instance.ReturnObject(gameObject, gameObject.name);
    }

    private void SetActualDamage()
    {
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, attackRadius, mask);
        for (int i = 0; i < hitColliders.Length; i++)
        {
            IDamageable damageable;
            if (hitColliders[i].TryGetComponent(out damageable))
            {
                float minDamage = Constants.DamagePerHit.SpiderAst;
                float damageMultiplierByDistance =
                    attackRadius / (transform.position - hitColliders[i].transform.position).magnitude;
                float resultDamage = minDamage * damageMultiplierByDistance;
                damageable.TakeDamage((int)resultDamage);
            }
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

    public void TakeDamage(int value)
    {
        TotalHealth-=value;
        if (TotalHealth > 0)
        {
            return;
        }
        
        Attack();
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, attackRadius);
    }
}
