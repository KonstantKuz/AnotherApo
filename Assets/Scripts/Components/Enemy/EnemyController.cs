using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;
using StateMachine;


public class EnemyController : MonoCached
{
    public Transform[] patrolPoints;
    public Transform attackTarget;
    public Weapon weapon;

    [HideInInspector]
    public Animator animator;
    [HideInInspector]
    public AimingOverrider hands;

    public StateMachine<EnemyController> stateMachine;

    [HideInInspector]
    public Vector3 targetDirection, targetDirection_XZprojection, targetDirection_ZYprojection;
    [HideInInspector]
    public float angleBtwn_targetDirZY_FWD, angleBtwn_targetDirXZ_FWD;

    
    [Header("Navigation")]
    public AstarPath coverPoints;
    [HideInInspector]
    public float pathUpdTimer;
    [HideInInspector]
    public float pathUpdPeriod = 2f;
    [HideInInspector]
    public ABPath currentPath;
    [HideInInspector]
    public int currentPathPointIndex;
    [HideInInspector]
    public Vector3 currentPathTargetPosition;

    void Start()
    {
        _transform = transform;
        SetUpStateMachine();

        SetUpAnimtor();
        SetUpHands();
    }

    public void SetUpStateMachine()
    {
        stateMachine = new StateMachine<EnemyController>(this);
        stateMachine.ChangeState(PatrolState.Instance);
    }

    public void SetUpPathfinding()
    {
        
    }
    
    public void SetUpAnimtor()
    {
        animator = GetComponent<Animator>();
    }

    public void SetUpHands()
    {
        hands = GetComponentInChildren<AimingOverrider>();
        hands.overridedChest.target = attackTarget;
        hands.overridedChest.weapon = weapon._transform;
        hands.characterAnimator = animator;
    }

    private void OnAnimatorIK(int layerIndex)
    {
        if (animator.GetBool(AnimatorHashes.AimingHash))
        {
            BodyAiming();
        }
    }
    
    public override void CustomUpdate()
    {
        stateMachine.UpdateMachine();
    }

    public void TargetingCalculations(Vector3 target)
    {
        targetDirection = target - transform.position;

        targetDirection_XZprojection = Vector3.ProjectOnPlane(targetDirection, transform.up);
        targetDirection_ZYprojection = Vector3.ProjectOnPlane(targetDirection, transform.right);

        angleBtwn_targetDirZY_FWD = Vector3.SignedAngle(targetDirection_ZYprojection.normalized, transform.forward, transform.right);
        angleBtwn_targetDirXZ_FWD = Vector3.SignedAngle(targetDirection_XZprojection.normalized, transform.forward, transform.up);
    }

    public void BodyAiming()
    {
        //animator.SetFloat(AnimatorHashes.Mouse_YHash, angleBtwn_targetDirZY_FWD / 50f);

        if ((attackTarget.position - weapon._transform.position).magnitude > 2f)
        {
            animator.SetLookAtWeight(0.5f, 1f, 1f);
            animator.SetLookAtPosition(attackTarget.position);
        }
    }

    public void UpdatePath(Vector3 currentPathTarget)
    {
        currentPathTargetPosition = currentPathTarget;
        currentPathPointIndex = 0;
        currentPath = ABPath.Construct(_transform.position, currentPathTargetPosition);
        AstarPath.StartPath(currentPath);
    }

    private void OnDrawGizmos()
    {
        if (currentPath != null && currentPath.vectorPath.Count>0)
        {
            for (int i = 0; i < currentPath.vectorPath.Count -1; i++)
            {
                    Debug.DrawLine(currentPath.vectorPath[i], currentPath.vectorPath[i + 1], Color.green);
            }
        }
    }
}
