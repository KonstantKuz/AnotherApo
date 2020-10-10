using System;
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

[System.Serializable]
public class SoundSection
{
    public AudioSource source;
    public AudioClip jumpClip;
    public float jumpClipVolume;
    public AudioClip landingClip;
    public float landingClipVolume;
}

public class UMUBot : Enemy
{
    [SerializeField] private SoundSection soundSection;
    [Header("Movement")]
    [SerializeField] private float movementSpeed;
    [SerializeField] private float jumpSpeed;
    [SerializeField] private float gravityMultiplier;
    [SerializeField] private Animator animator;
    [SerializeField] private CharacterController controller;

    [Header("Targeting")]
    [SerializeField] private UMUGun[] guns;
    public UMUGun[] Guns
    {
        get { return guns; }
    }
    [SerializeField] private float targetingSpeed;
    [SerializeField] private LookAtSection lookAtSection;
    [SerializeField] private TargetingSection targetingSection;
    [SerializeField] private Transform fireShowStartPoint;
    private Transform currentAttackTarget;
    
    private Vector3 movementVelocity;
    private Vector3 verticalVelocity;

    private int damagedGunsCount;
    private bool alreadyJumped;
    private bool alreadyLanded;
    
    public override void ResetEnemy()
    {
        for (int i = 0; i < guns.Length; i++)
        {
            guns[i].ResetGun();
        }
        
        SubscribeToJumpBeat();
        SubscribeToCheckDeath();
        StartAiming();
        LookAt(currentAttackTarget);
        UpdatePathPeriodically();
    }

    private void SubscribeToJumpBeat()
    {
        GameBeatSequencer.OnBPM += TryJump;
    }

    private void UnsubscribeFromJumpBeat()
    {
        GameBeatSequencer.OnBPM -= TryJump;
    }

    private void TryJump()
    {
        if (GameBeatSequencer.CurrentBeat % 4 == 0)
        {
            animator.SetTrigger(AnimatorHashes.JumpHash);
        }
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
            StopAllCoroutines();
            ClearPlayer();

            animator.SetTrigger(AnimatorHashes.DeathHash);
            UnsubscribeFromJumpBeat();
            LookAt(fireShowStartPoint);
            DeathFireShow();
        }
    }

    private void DeathFireShow()
    {
        StartCoroutine(DelayedFireShow());
        IEnumerator DelayedFireShow()
        {
            yield return new WaitForSeconds(1f);

            while (!GameBeatSequencer.IsBeatedNow)
            {
                yield return null;
            }
            
            for (int i = 0; i < 3; i++)
            {
                SpawnHealExplosionsOnRndPos(
                    Constants.PoolExplosionMid, Constants.HealPerExplosion.UMUSmall, Constants.ExplosionRadiusSmall);
                yield return null;
            }
            yield return null;
            SpawnHealExplosionsOnRndPos(
                Constants.PoolExplosionBig, Constants.HealPerExplosion.UMUBig, Constants.ExplosionRadiusBig);
            
            OnDeath?.Invoke();
            ObjectPooler.Instance.ReturnObject(gameObject, gameObject.name);
        }

        void SpawnHealExplosionsOnRndPos(string explosionName, int heal, float radius)
        {
            Vector3 rndExplosionPosition = fireShowStartPoint.position +
                                           fireShowStartPoint.up * Random.Range(-1f, 1f) +
                                           fireShowStartPoint.right * Random.Range(-1f, 1f);
            
            Explosion explosion = ObjectPooler.Instance.SpawnObject(explosionName, rndExplosionPosition).GetComponent<Explosion>();
            explosion.Explode(1 << LayerMasks.Player, heal, radius);
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

        soundSection.source.volume = soundSection.jumpClipVolume;
        soundSection.source.PlayOneShot(soundSection.jumpClip);
        
        alreadyJumped = true;
        verticalVelocity.y = jumpSpeed;
        StartCoroutine(DelayedAllowJump());
        IEnumerator DelayedAllowJump()
        {
            yield return new WaitForSeconds(0.5f);
            alreadyJumped = false;
        }
    }

    public void Landing()
    {
        if (alreadyLanded)
            return;
        
        soundSection.source.volume = soundSection.landingClipVolume;
        soundSection.source.PlayOneShot(soundSection.landingClip);
        
        alreadyLanded = true;
        StartCoroutine(DelayedAllowLanding());
        IEnumerator DelayedAllowLanding()
        {
            yield return new WaitForSeconds(0.5f);
            alreadyLanded = false;
        }
    }
}
