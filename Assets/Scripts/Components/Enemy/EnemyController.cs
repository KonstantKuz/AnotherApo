using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;
using StateMachine;


public class EnemyController : MonoCached
{
    public Transform[] patrolPoints;
    public Transform attackTarget;
    public Gun gun;

    public Animator animator;
    [SerializeField] private AimingAndIKOverrider hands;

    public StateMachine<EnemyController> stateMachine;

    [HideInInspector]
    public Vector3 targetDirection, targetDirection_XZprojection, targetDirection_ZYprojection;
    [HideInInspector]
    public float angleBtwn_targetDirZY_FWD, angleBtwn_targetDirXZ_FWD;
    
    [HideInInspector]
    public float pathUpdTimer;
    [HideInInspector]
    public float pathUpdPeriod = 2f;
    [HideInInspector]
    public ABPath currentPath;
    [HideInInspector]
    public int currentPathNodeIndex;
    [HideInInspector]
    public Vector3 currentPathTarget;

    void Start()
    {
        SetUpStateMachine();
    }

    public void SetUpStateMachine()
    {
        stateMachine = new StateMachine<EnemyController>(this);
        stateMachine.ChangeState(AggressiveState.Instance);
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

    public void BodyAiming()
    {
        //animator.SetFloat(AnimatorHashes.Mouse_YHash, angleBtwn_targetDirZY_FWD / 50f);

        if ((attackTarget.position - gun.transform.position).magnitude > 2f)
        {
            animator.SetLookAtWeight(0.5f, 1f, 1f);
            animator.SetLookAtPosition(attackTarget.position);
        }
    }

    public void UpdatePath(Vector3 currentPathTarget)
    {
        this.currentPathTarget = currentPathTarget;
        currentPathNodeIndex = 0;
        currentPath = ABPath.Construct(transform.position, this.currentPathTarget);
        AstarPath.StartPath(currentPath);
    }

    public bool HasPath()
    {
        return !ReferenceEquals(currentPath, null) && currentPath.vectorPath.Count > 0;
    }

    public bool PathFullyPassed()
    {
        return currentPathNodeIndex > currentPath.vectorPath.Count - 1 ||
                (currentPath.vectorPath[currentPathNodeIndex] - currentPathTarget).magnitude < 1f;
    }

    public void CleanPassedNodes()
    {
        if (PathFullyPassed())
            return;
        
        if ((transform.position - currentPath.vectorPath[currentPathNodeIndex]).magnitude < 1f)
        {
            currentPath.vectorPath.Remove(currentPath.vectorPath[currentPathNodeIndex]);
        }
    }

    public float ForwardAngleToNextNode()
    {
        return Vector3.SignedAngle(transform.forward,
                                   currentPath.vectorPath[currentPathNodeIndex] - transform.position,
                                   transform.up);
    }

    public void Move(float verticalValue, float horizontalValue)
    {
        animator.SetFloat(AnimatorHashes.VerticalHash, Mathf.Lerp(animator.GetFloat(AnimatorHashes.VerticalHash), verticalValue, Time.deltaTime * 5f));
        animator.SetFloat(AnimatorHashes.HorizontalHash, Mathf.Lerp(animator.GetFloat(AnimatorHashes.HorizontalHash), horizontalValue, Time.deltaTime * 5f));
    }

    public void RotateTowards(Vector3 target)
    {
        TargetingCalculations(target);
        Quaternion lookRotation = Quaternion.LookRotation(targetDirection_XZprojection, transform.up);
        transform.rotation = Quaternion.Lerp(transform.rotation, lookRotation, Time.deltaTime * 2f);
    }

    public void TargetingCalculations(Vector3 target)
    {
        targetDirection = target - transform.position;

        targetDirection_XZprojection = Vector3.ProjectOnPlane(targetDirection, transform.up);
        targetDirection_ZYprojection = Vector3.ProjectOnPlane(targetDirection, transform.right);

        angleBtwn_targetDirZY_FWD = Vector3.SignedAngle(targetDirection_ZYprojection.normalized, transform.forward, transform.right);
        angleBtwn_targetDirXZ_FWD = Vector3.SignedAngle(targetDirection_XZprojection.normalized, transform.forward, transform.up);
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
