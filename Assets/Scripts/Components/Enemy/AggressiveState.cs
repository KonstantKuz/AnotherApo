using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using StateMachine;

public class AggressiveState : State<EnemyController>
{
    public static AggressiveState _instance;

    private AggressiveState()
    {
        if (_instance != null)
            return;

        _instance = this;
    }

    public static AggressiveState Instance
    {
        get
        {
            if (_instance == null)
                new AggressiveState();

            return _instance;
        }
    }
    
    public override void EnterState(EnemyController owner)
    {
        owner.pathUpdPeriod = 2;

        owner.animator.SetBool(AnimatorHashes.AimingHash, true);
        GoToPlayer(owner);
    }

    public override void ExitState(EnemyController owner)
    {

    }

    public override void UpdateState(EnemyController owner)
    {
        owner.RotateTowards(owner.attackTarget.position);

        MoveToNextPoint(owner);
    }

    private float verticalMoveValue;
    private float horizontalMoveValue;

    private void MoveToNextPoint(EnemyController owner)
    {
        owner.CleanPassedNodes();

        if (!owner.HasPath() || owner.PathFullyPassed())
        {
            Wait(owner);
            return;
        }
        
        CalculateMoveValues(owner);
        owner.Move(verticalMoveValue, horizontalMoveValue);
    }

    private void CalculateMoveValues(EnemyController owner)
    {
        float fwdAngleToNextNode = owner.ForwardAngleToNextNode();

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

    private void Wait(EnemyController owner)
    {
        owner.Move(0, 0);
        
        if (Time.time > owner.pathUpdTimer)
        {
            owner.pathUpdTimer = Time.time + owner.pathUpdPeriod;
            EnterState(owner);
        }
    }

    private void GoToPlayer(EnemyController owner)
    {
        PlayerController playerController = GameObject.FindObjectOfType<PlayerController>();
        Vector3 pointToGo = playerController.transform.position + playerController.transform.right * 5f;
        
        var coverPointsGraph = AstarPath.active.data.FindGraph(graphToFind => graphToFind.name == "General");
        Vector3 resultNode = (Vector3)coverPointsGraph.GetNearest(pointToGo).node.position;
        resultNode.y = 0;

        owner.UpdatePath(resultNode);
    }
}
