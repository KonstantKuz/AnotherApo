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
        GoToRandomPlayerSide(owner);
        
        StartFiring(owner);
        UpdatePathPeriodically(owner);
    }

    private void StartFiring(Durashka owner)
    {
        owner.StartCoroutine(rndFire());
        IEnumerator rndFire()
        {
            yield return new WaitForSeconds(Random.Range(2f, 5f));
            float fireTime = Random.Range(0.5f, 1f);
            while (fireTime>0)
            {
                fireTime -= Time.deltaTime;
                owner.Gun.Fire();
                yield return null;
            }
            yield return owner.StartCoroutine(rndFire());
        }
    }
    
    private void UpdatePathPeriodically(Durashka owner)
    {
        owner.StartCoroutine(UpdatePathToPlayerPeriodically());
        IEnumerator UpdatePathToPlayerPeriodically()
        {
            EnterState(owner);
            yield return new WaitForSeconds(owner.pathUpdPeriod);
            owner.StartCoroutine(UpdatePathToPlayerPeriodically());
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
        Vector3 side = owner.player.transform.right * RandomSign();
        Vector3 pointToGo = owner.player.transform.position + side * Random.Range(5,8);
        
        NavGraph coverPointsGraph = 
            AstarPath.active.data.FindGraph(graphToFind => graphToFind.name == Constants.GeneralGraph);
        
        Vector3 resultNode = (Vector3)coverPointsGraph.GetNearest(pointToGo).node.position;
        resultNode.y = 0;

        owner.UpdatePath(resultNode);
    }

    private int RandomSign()
    {
        return Random.value > 0.5f ? 1 : -1;
    }
}
