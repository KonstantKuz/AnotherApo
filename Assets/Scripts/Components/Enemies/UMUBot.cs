using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;
using Random = UnityEngine.Random;

public class UMUBot : Enemy
{
    [Header("Movement")]
    [SerializeField] private float movementSpeed;
    [SerializeField] private float jumpSpeed;
    [SerializeField] private float gravityMultiplier;
    [SerializeField] private Animator animator;
    [SerializeField] private CharacterController controller;

    [Header("Targeting")]
    [SerializeField] private UMUGun[] guns;
    [SerializeField] private float aimingSpeed;
    [SerializeField] private Vector3 targetOffset;
    [SerializeField] private TargetInterpolator targetInterpolator;
    [SerializeField] private LookAtConstraint aimingConstraint;
    [SerializeField] private Transform fireShowStartPoint;
    private Transform currentAttackTarget;
    
    private Vector3 movementVelocity;
    private Vector3 verticalVelocity;

    private int damagedGunsCount;
    private bool alreadyJumped;

    public override void Start()
    {
        base.Start();
        SubscribeToCheckDeath();
        SubscribeToGameBeat();
        StartAiming();
        UpdatePathPeriodically();
    }

    private void SubscribeToCheckDeath()
    {
        for (int i = 0; i < guns.Length; i++)
        {
            guns[i].OnFullDamaged += CheckForDeath;
        }
    }

    private void CheckForDeath()
    {
        damagedGunsCount++;
        if (damagedGunsCount == guns.Length)
        {
            animator.SetTrigger(AnimatorHashes.DeathHash);
            UnsubscribeFromGameBeat();
            DeathFireShow();
            DisableActivities();
            LookDown();
        }
    }

    private void DeathFireShow()
    {
        StartCoroutine(DelayedFireShow());
        IEnumerator DelayedFireShow()
        {
            yield return new WaitForSeconds(1f);
            for (int i = 0; i < 3; i++)
            {
                SpawnExplosionsOnRandomPosition(Constants.PoolMidExplosion);
                yield return new WaitForSeconds(0.1f);
            }
            
            for (int i = 0; i < 2; i++)
            {
                SpawnExplosionsOnRandomPosition(Constants.PoolBigExplosion);
                yield return new WaitForSeconds(0.1f);
            }
            
            ObjectPooler.Instance.ReturnObject(gameObject, gameObject.name);
        }

        void SpawnExplosionsOnRandomPosition(string explosion)
        {
            Vector3 rndExplosionPosition = fireShowStartPoint.position +
                                           fireShowStartPoint.up * Random.Range(0.2f, 1f) +
                                           fireShowStartPoint.right * Random.Range(-1f, 1f);
            
            ObjectPooler.Instance.SpawnObject(explosion, rndExplosionPosition);
        }
    }
    
    private void DisableActivities()
    {
        controller.enabled = false;
        enabled = false;
    }

    private void LookDown()
    {
        currentAttackTarget = fireShowStartPoint;
        targetInterpolator.SetConstraint(currentAttackTarget, targetOffset, aimingSpeed);
        aimingConstraint.AddSource(Interpolator());
    }

    private void SubscribeToGameBeat()
    {
        for (int i = 0; i < guns.Length; i++)
        {
            GameBeatSequencer.OnBPM += guns[i].Fire;
        }
    }

    private void UnsubscribeFromGameBeat()
    {
        for (int i = 0; i < guns.Length; i++)
        {
            GameBeatSequencer.OnBPM -= guns[i].Fire;
        }
    }

    private void StartAiming()
    {
        currentAttackTarget = player.Animator.GetBoneTransform(HumanBodyBones.Spine);
        targetInterpolator.SetConstraint(currentAttackTarget, targetOffset, aimingSpeed);
        aimingConstraint.AddSource(Interpolator());
    }

    private ConstraintSource Interpolator()
    {
        ConstraintSource interpolatorSource = new ConstraintSource();
        interpolatorSource.weight = 1;
        interpolatorSource.sourceTransform = targetInterpolator.transform;
        return interpolatorSource;
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
        verticalVelocity += Physics.gravity * gravityMultiplier * Time.deltaTime;
        verticalVelocity.y = Mathf.Clamp(verticalVelocity.y, Physics.gravity.y * gravityMultiplier, jumpSpeed);
        controller.Move(verticalVelocity * Time.deltaTime);
    }

    public override void MoveToNextNode()
    {
        Move(verticalMoveValue, horizontalMoveValue);
    }

    private void Move(float verticalValue, float horizontalValue)
    {
        float interpolatedVertical =
            Mathf.Lerp(animator.GetFloat(AnimatorHashes.VerticalHash), verticalValue, Time.deltaTime * 2f);
        
        float interpolatedHorizontal =
            Mathf.Lerp(animator.GetFloat(AnimatorHashes.HorizontalHash), horizontalValue, Time.deltaTime *  2f);
        
        animator.SetFloat(AnimatorHashes.VerticalHash, interpolatedVertical);
        animator.SetFloat(AnimatorHashes.HorizontalHash, interpolatedHorizontal);
        
        movementVelocity = Vector3.zero;
        
        movementVelocity.x = interpolatedHorizontal;
        movementVelocity.z = interpolatedVertical;
        
        movementVelocity = transform.TransformDirection(movementVelocity);
        movementVelocity *= movementSpeed;
        
        controller.Move(movementVelocity * Time.deltaTime);
    }
    
    public void Jump()
    {
        if (alreadyJumped)
            return;
        
        alreadyJumped = true;
        verticalVelocity.y = jumpSpeed;
        StartCoroutine(DelayedAllowJump());
        IEnumerator DelayedAllowJump()
        {
            yield return new WaitForSeconds(0.5f);
            alreadyJumped = false;
        }
    }
}
