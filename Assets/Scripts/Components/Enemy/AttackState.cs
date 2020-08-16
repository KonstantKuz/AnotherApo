using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using StateMachine;

public class AttackState : State<EnemyController>
{
    public static AttackState _instance;

    private AttackState()
    {
        if (_instance != null)
            return;

        _instance = this;
    }

    public static AttackState Instance
    {
        get
        {
            if (_instance == null)
                new AttackState();

            return _instance;
        }
    }

    public override void EnterState(EnemyController _owner)
    {
        _owner.animator.SetBool(AnimatorHashes.AimingHash, true);

        FindAndTakeCover(_owner);
    }

    public override void ExitState(EnemyController _owner)
    {

    }

    public override void UpdateState(EnemyController _owner)
    {
        MoveToNextPoint(_owner);
    }

    public void MoveToNextPoint(EnemyController _owner)
    {
        if (!ReferenceEquals(_owner.currentPath, null) && _owner.currentPath.vectorPath.Count > 0)
        {
            if ((_owner.currentPath.vectorPath[_owner.currentPathPointIndex] - _owner.currentPathTargetPosition).magnitude < 1f)
            {
                Wait(_owner);
                return;
            }


            if ((_owner.transform.position - _owner.currentPath.vectorPath[_owner.currentPathPointIndex]).magnitude < 1f)
            {
                _owner.currentPath.vectorPath.Remove(_owner.currentPath.vectorPath[_owner.currentPathPointIndex]);
            }
            if (_owner.currentPathPointIndex > _owner.currentPath.vectorPath.Count - 1)
            {
                return;
            }

            AttackTargeting(_owner);
           
            float angleBtwn_FWD_NextPathPoint = Vector3.SignedAngle(_owner.transform.forward, _owner.currentPath.vectorPath[_owner.currentPathPointIndex] - _owner.transform.position, _owner.transform.up);

            if (angleBtwn_FWD_NextPathPoint > 0)
            {
                if (angleBtwn_FWD_NextPathPoint > 90)
                {
                    _owner.animator.SetFloat(AnimatorHashes.VerticalHash, Mathf.Lerp(_owner.animator.GetFloat(AnimatorHashes.VerticalHash), -1, Time.deltaTime * 2f));
                }
                else
                {
                    _owner.animator.SetFloat(AnimatorHashes.VerticalHash, Mathf.Lerp(_owner.animator.GetFloat(AnimatorHashes.VerticalHash), 1, Time.deltaTime * 2f));
                }

                _owner.animator.SetFloat(AnimatorHashes.HorizontalHash, Mathf.Lerp(_owner.animator.GetFloat(AnimatorHashes.HorizontalHash), 1, Time.deltaTime * 2f));
            }
            else
            {
                if (angleBtwn_FWD_NextPathPoint > -90)
                {
                    _owner.animator.SetFloat(AnimatorHashes.VerticalHash, Mathf.Lerp(_owner.animator.GetFloat(AnimatorHashes.VerticalHash), 1, Time.deltaTime * 2f));
                }
                else
                {
                    _owner.animator.SetFloat(AnimatorHashes.VerticalHash, Mathf.Lerp(_owner.animator.GetFloat(AnimatorHashes.VerticalHash), -1, Time.deltaTime * 2f));
                }
                _owner.animator.SetFloat(AnimatorHashes.HorizontalHash, Mathf.Lerp(_owner.animator.GetFloat(AnimatorHashes.HorizontalHash), -1, Time.deltaTime * 2f));
            }

        }
        else
        {
            Wait(_owner);
        }
    }


    public void AttackTargeting(EnemyController _owner)
    {
        _owner.TargetingCalculations(_owner.attackTarget.position);
        _owner.transform.rotation = Quaternion.Lerp(_owner.transform.rotation, Quaternion.LookRotation(_owner.targetDirection_XZprojection, _owner.transform.up), Time.deltaTime * 2f);

    }
    public void Wait(EnemyController _owner)
    {
        _owner.animator.SetFloat(AnimatorHashes.VerticalHash, Mathf.Lerp(_owner.animator.GetFloat(AnimatorHashes.VerticalHash), 0f, Time.deltaTime * 5f));
        _owner.animator.SetFloat(AnimatorHashes.HorizontalHash, Mathf.Lerp(_owner.animator.GetFloat(AnimatorHashes.HorizontalHash), 0f, Time.deltaTime * 5f));
        _owner.animator.SetBool(AnimatorHashes.CrouchingHash, true);

        AttackTargeting(_owner);
    }

    public void FindAndTakeCover(EnemyController _owner)
    {
        var coverPointsGraph = AstarPath.active.data.FindGraph(graphToFind => graphToFind.name == "Cover Points");
        List<Pathfinding.GraphNode> nodesdata = new List<Pathfinding.GraphNode>();
        coverPointsGraph.GetNodes(nodes => nodesdata.Add(nodes));
        for (int i = 0; i < nodesdata.Count; i++)
        {
            Debug.Log("Node added" + (Vector3)nodesdata[i].position);
        }
        Vector3 nearestCover = (Vector3)coverPointsGraph.GetNearest(_owner.transform.position).node.position;
        nearestCover.y = 0;

        _owner.UpdatePath(nearestCover);
    }

    public void AttackFromWalk()
    {

    }

    public void AttackFromCover()
    {

    }

}
