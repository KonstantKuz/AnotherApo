using System;
using System.Collections;
using System.Collections.Generic;
using Pathfinding;
using UnityEngine;

public abstract class Enemy : MonoCached
{
    [HideInInspector] public Vector3 targetDirection, targetDirection_XZprojection, targetDirection_ZYprojection;
    [HideInInspector] public float angleBtwn_targetDirZY_FWD, angleBtwn_targetDirXZ_FWD;

    [HideInInspector] public float verticalMoveValue, horizontalMoveValue;

    protected PlayerCharacterController player;
    
    [Header("Pathfinding")]
    [SerializeField] protected string patgGraphName;
    [SerializeField] protected float pathUpdPeriod;
    protected float pathUpdTimer;
    protected float pathCleanDistance = 1f;
    protected Path currentPath;
    protected int currentPathNodeIndex;
    protected Vector3 currentPathTarget;

    public Action OnDeath;
    public override void OnEnable()
    {
        base.OnEnable();
        player = FindObjectOfType<PlayerCharacterController>();
        ResetEnemy();
    }

    public virtual void ResetEnemy()
    {
        
    }

    public void ClearPlayer()
    {
        player = null;
    }

    protected void UpdatePath(Vector3 currentPathTarget)
    {
        this.currentPathTarget = currentPathTarget;
        currentPathNodeIndex = 0;
        
        currentPath = ABPath.Construct(transform.position, currentPathTarget);
        currentPath.nnConstraint.graphMask = GraphMask.FromGraphName(patgGraphName);
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

    protected void CleanPassedNodes()
    {
        if (PathFullyPassed())
            return;
        
        if ((transform.position - currentPath.vectorPath[currentPathNodeIndex]).magnitude < pathCleanDistance)
        {
            currentPath.vectorPath.Remove(currentPath.vectorPath[currentPathNodeIndex]);
        }
    }

    public void CalculateMoveValuesToNextNode()
    {
        float fwdAngleToNextNode = ForwardAngleToNextNode();

        verticalMoveValue = 0;
        horizontalMoveValue = 0;
        
        if (fwdAngleToNextNode > 0)
        {
            if (fwdAngleToNextNode > 90)
            {
                verticalMoveValue = -1;
            }
            else
            {
                verticalMoveValue = 1;
            }

            horizontalMoveValue = 1;
        }
        else
        {
            if (fwdAngleToNextNode > -90)
            {
                verticalMoveValue = 1;
            }
            else
            {
                verticalMoveValue = -1;
            }

            horizontalMoveValue = -1;
        }
    }

    public float ForwardAngleToNextNode()
    {
        return Vector3.SignedAngle(transform.forward,
                                   currentPath.vectorPath[currentPathNodeIndex] - transform.position,
                                   transform.up);
    }

    public void RotateTowards(Vector3 target)
    {
        TargetingCalculations(target);
        Quaternion lookRotation = Quaternion.LookRotation(targetDirection_XZprojection, transform.up);
        transform.rotation = Quaternion.Lerp(transform.rotation, lookRotation, Time.deltaTime * 5f);
    }

    public void TargetingCalculations(Vector3 target)
    {
        targetDirection = target - transform.position;

        targetDirection_XZprojection = Vector3.ProjectOnPlane(targetDirection, transform.up);
        targetDirection_ZYprojection = Vector3.ProjectOnPlane(targetDirection, transform.right);

        angleBtwn_targetDirZY_FWD = Vector3.SignedAngle(targetDirection_ZYprojection.normalized, transform.forward, transform.right);
        angleBtwn_targetDirXZ_FWD = Vector3.SignedAngle(targetDirection_XZprojection.normalized, transform.forward, transform.up);
    }

    public virtual void MoveToNextNode()
    {
        
    }
    
    public virtual void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position + transform.up/2, 1f);
        
        if (currentPath != null && currentPath.vectorPath.Count>0)
        {
            for (int i = 0; i < currentPath.vectorPath.Count -1; i++)
            {
                Debug.DrawLine(currentPath.vectorPath[i], currentPath.vectorPath[i + 1], Color.green);
            }
        }
    }
}