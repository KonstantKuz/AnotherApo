using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using StateMachine;

public class PatrolState : State<EnemyController>
{
    public static PatrolState _instance;

    private PatrolState()
    {
        if (_instance != null)
            return;

        _instance = this;
    }

    public static PatrolState Instance
    {
        get
        {
            if (_instance == null)
                new PatrolState();

            return _instance;
        }
    }
    public override void EnterState(EnemyController owner)
    {
        owner.UpdatePath(owner.patrolPoints[Random.Range(0, owner.patrolPoints.Length)].position);
        owner.pathUpdPeriod = 30;
    }

    public override void ExitState(EnemyController owner)
    {

    }

    public override void UpdateState(EnemyController owner)
    {
        MoveToNextPoint(owner);
        LookAndHear(owner);
    }

    public void MoveToNextPoint(EnemyController owner)
    {
        owner.CleanPassedNodes();

        if (!owner.HasPath() || owner.PathFullyPassed())
        {
            Wait(owner);
            return;
        }
        owner.Move(1, 0);
        owner.RotateTowards(owner.currentPath.vectorPath[owner.currentPathNodeIndex]);
    }

    private void Wait(EnemyController owner)
    {
        owner.Move(0,0);

        if (Time.time > owner.pathUpdTimer)
        {
            owner.pathUpdTimer = Time.time + owner.pathUpdPeriod;
            EnterState(owner);
        }
    }

    private void LookAndHear(EnemyController owner)
    {
        if(Physics.CheckSphere(owner.transform.position, 5f, 1<<9))
        {
            Collider[] cols = Physics.OverlapSphere(owner.transform.position, 6f, 1 << 9);
            for (int i = 0; i < cols.Length; i++)
            {
                owner.TargetingCalculations(cols[i].transform.position);
                float horizontalAngle = Vector3.Angle(owner.transform.forward, owner.targetDirection_XZprojection);
                float verticalAngle = Vector3.Angle(owner.transform.forward, owner.targetDirection_ZYprojection);
                if(horizontalAngle < 70 || verticalAngle < 20)
                {
                    owner.stateMachine.ChangeState(AggressiveState.Instance);
                }
            }
        }
    }
}
