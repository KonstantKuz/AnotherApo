using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Animations.Rigging;
using Random = UnityEngine.Random;

[System.Serializable]
public class LookAtSection
{
    public TargetInterpolator lookAtInterpolator;
    public Vector3 lookAtOffset;
}

[System.Serializable]
public class TargetingSection
{
    public TargetInterpolator targetInterpolator;
    public Vector3 targetOffset;
}

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
    [SerializeField] private float targetingSpeed;
    [SerializeField] private LookAtSection lookAtSection;
    [SerializeField] private TargetingSection targetingSection;
    [SerializeField] private Transform fireShowStartPoint;
    private Transform currentAttackTarget;
    
    private Vector3 movementVelocity;
    private Vector3 verticalVelocity;

    private int damagedGunsCount;
    private bool alreadyJumped;

    public override void Start()
    {
        base.Start();
        
        for (int i = 0; i < guns.Length; i++)
        {
            guns[i].SubscribeToBeat();
        }
        
        SubscribeToCheckDeath();
        StartAiming();
        LookAt(currentAttackTarget);
        UpdatePathPeriodically();
        
        GameBeatSequencer.OnBPM += delegate
        {
            if (GameBeatSequencer.CurrentBeat % 4 == 0)
            {
                animator.SetTrigger(AnimatorHashes.JumpHash);
            }
        };
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
            LookAt(fireShowStartPoint);
            DeathFireShow();
            enabled = false;
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
                SpawnExplosionsOnRandomPosition(Constants.PoolExplosionMid);
                yield return null;
            }
            for (int i = 0; i < 2; i++)
            {
                SpawnExplosionsOnRandomPosition(Constants.PoolExplosionBig);
            }
            
            ObjectPooler.Instance.ReturnObject(gameObject, gameObject.name);
        }

        void SpawnExplosionsOnRandomPosition(string explosion)
        {
            Vector3 rndExplosionPosition = fireShowStartPoint.position +
                                           fireShowStartPoint.up * Random.Range(-1f, 1f) +
                                           fireShowStartPoint.right * Random.Range(-1f, 1f);
            
            ObjectPooler.Instance.SpawnObject(explosion, rndExplosionPosition);
        }
    }

    private void StartAiming()
    {
        currentAttackTarget = player.Animator.GetBoneTransform(HumanBodyBones.Spine);
        targetingSection.targetInterpolator.SetConstraint(currentAttackTarget, targetingSection.targetOffset, targetingSpeed);
    }

    private void LookAt(Transform target)
    {
        lookAtSection.lookAtInterpolator.SetConstraint(target, lookAtSection.lookAtOffset, targetingSpeed);
    }

    private void UpdatePathPeriodically()
    {
        StartCoroutine(UpdatePathToPlayerPeriodically());
        IEnumerator UpdatePathToPlayerPeriodically()
        {
            while (true)
            {
                UpdatePath(player.transform.position + player.transform.forward*Random.Range(2,10));
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
