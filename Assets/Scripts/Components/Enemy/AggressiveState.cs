using System.Collections;
using System.Collections.Generic;
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

        owner.animator.SetBool(AnimatorHashes.AimingHash, true);
        GoToPlayer(owner);
        
        StartFiring(owner);
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
                owner.gun.Fire();
                yield return null;
            }
            yield return owner.StartCoroutine(rndFire());
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
        
        if (Time.time > owner.pathUpdTimer)
        {
            owner.pathUpdTimer = Time.time + owner.pathUpdPeriod;
            EnterState(owner);
        }
    }

    private void GoToPlayer(Durashka owner)
    {
        PlayerController playerController = GameObject.FindObjectOfType<PlayerController>();
        Vector3 side = playerController.transform.right * RandomSign();
        Vector3 pointToGo = playerController.transform.position + side * Random.Range(5,8);
        
        var coverPointsGraph = AstarPath.active.data.FindGraph(graphToFind => graphToFind.name == "General");
        Vector3 resultNode = (Vector3)coverPointsGraph.GetNearest(pointToGo).node.position;
        resultNode.y = 0;

        owner.UpdatePath(resultNode);
    }

    public int RandomSign()
    {
        return Random.Range(0, 2) * 2 - 1;
        return Random.value > 0.5f ? 1 : -1;
    }
}
