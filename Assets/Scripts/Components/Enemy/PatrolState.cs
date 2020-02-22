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
    public override void EnterState(EnemyController _owner)
    {
        _owner.UpdatePath(_owner.patrolPoints[Random.Range(0, _owner.patrolPoints.Length)].position);
        _owner.pathUpdPeriod = 30;
        Debug.Log(_owner.currentPath.vectorPath.Count);
    }

    public override void ExitState(EnemyController _owner)
    {

    }

    public override void UpdateState(EnemyController _owner)
    {
        MoveToNextPoint(_owner);
        LookAndHear(_owner);
    }

    public void MoveToNextPoint(EnemyController _owner)
    {
        if(!ReferenceEquals(_owner.currentPath, null) && _owner.currentPath.vectorPath.Count>0)
        {
            if ((_owner.currentPath.vectorPath[_owner.currentPathPointIndex] - _owner.currentPathTargetPosition).magnitude < 1f)
            {
                Wait(_owner);
                return;
            }
            

            if ((_owner._transform.position - _owner.currentPath.vectorPath[_owner.currentPathPointIndex]).magnitude < 1f)
            {
                _owner.currentPath.vectorPath.Remove(_owner.currentPath.vectorPath[_owner.currentPathPointIndex]);
            }
            
            _owner.animator.SetFloat(AnimatorHashes.VerticalHash, Mathf.Lerp(_owner.animator.GetFloat(AnimatorHashes.VerticalHash), 1f, Time.deltaTime * 5f));
            _owner.TargetingCalculations(_owner.currentPath.vectorPath[_owner.currentPathPointIndex]);
            _owner._transform.rotation = Quaternion.Lerp(_owner._transform.rotation, Quaternion.LookRotation(_owner.targetDirection_XZprojection/*, _owner._transform.up*/), Time.deltaTime * 2f);
        }
        else
        {
            Wait(_owner);
        }
    }

    public void Wait(EnemyController _owner)
    {
        _owner.animator.SetFloat(AnimatorHashes.VerticalHash, Mathf.Lerp(_owner.animator.GetFloat(AnimatorHashes.VerticalHash), 0f, Time.deltaTime *5f));
        _owner.animator.SetFloat(AnimatorHashes.HorizontalHash, Mathf.Lerp(_owner.animator.GetFloat(AnimatorHashes.HorizontalHash), 0f, Time.deltaTime * 5f));

        if (Time.time > _owner.pathUpdTimer)
        {
            _owner.pathUpdTimer = Time.time + _owner.pathUpdPeriod;
            EnterState(_owner);
        }
    }

    public void LookAndHear(EnemyController _owner)
    {
        if(Physics.CheckSphere(_owner._transform.position, 5f, 1<<9))
        {
            Collider[] cols = Physics.OverlapSphere(_owner._transform.position, 6f, 1 << 9);
            for (int i = 0; i < cols.Length; i++)
            {
                _owner.TargetingCalculations(cols[i].transform.position);
                float horizontalAngle = Vector3.Angle(_owner._transform.forward, _owner.targetDirection_XZprojection);
                float verticalAngle = Vector3.Angle(_owner._transform.forward, _owner.targetDirection_ZYprojection);
                if(horizontalAngle < 70 || verticalAngle < 20)
                {
                    _owner.stateMachine.ChangeState(AttackState.Instance);
                }
            }
        }
    }

   

}
