using System.Collections;
using System.Collections.Generic;
using Pathfinding;
using UnityEngine;
using StateMachine;

public class AggressiveState : State<Durashka>
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

    public override void EnterState(Durashka owner)
    {
        owner.pathUpdPeriod = 2;
        owner.Animator.SetBool(AnimatorHashes.AimingHash, true);
        
        GoToRandomPlayerSidePeriodically(owner);
        
        GameBeatSequencer.OnBPM += delegate
        {
            if (Random.value > 0.5f)
            {
                owner.Gun.Fire();
            }
        };
    }

    private void GoToRandomPlayerSidePeriodically(Durashka owner)
    {
        owner.StartCoroutine(GoToRandomSide());
        IEnumerator GoToRandomSide()
        {
            while (true)
            {
                GoToRandomPlayerSide(owner);
                yield return new WaitForSeconds(owner.pathUpdPeriod);
            }
        }
    }

    public override void ExitState(Durashka owner)
    {

    }

    public override void UpdateState(Durashka owner)
    {
        owner.RotateTowards(owner.player.transform.position);

        MoveToNextPoint(owner);
    }

    private void MoveToNextPoint(Durashka owner)
    {
        owner.CleanPassedNodes();

        if (!owner.HasPath() || owner.PathFullyPassed())
        {
            Wait(owner);
            return;
        }
        owner.CalculateMoveValuesToNextNode();
        owner.MoveToNextNode();
    }

    private void Wait(Durashka owner)
    {
        owner.Move(0, 0);
    }

    private void GoToRandomPlayerSide(Durashka owner)
    {
        Vector3 side = owner.player.transform.right * RandomSign() + owner.player.transform.forward * RandomSign();
        Vector3 pointToGo = owner.player.transform.position + side * Random.Range(5,8);
        
        NavGraph generalGraph = 
            AstarPath.active.data.FindGraph(graphToFind => graphToFind.name == Constants.GeneralGraph);
        
        Vector3 resultNode = (Vector3)generalGraph.GetNearest(pointToGo).node.position;

        owner.UpdatePath(resultNode);
    }

    private int RandomSign()
    {
        return Random.value > 0.5f ? 1 : -1;
    }
}
