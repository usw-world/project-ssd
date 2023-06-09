using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_dummy : MovableEnemy
{
    private State idleState;
    private State detectingState;

    private Vector3 spawnPos;
    
    protected NavigateableMovement enemyMovement;
    private void InitializeState()
    {
        idleState.onActive += state =>
        {
            enemyMovement.Stop();
        };
        detectingState.onActive += state =>
        {

        };


    }

    protected override void Awake()
    {
        base.Awake();
        spawnPos = transform.position;
    }

    protected override void ChaseTarget(Vector3 point)
    {
        throw new System.NotImplementedException();
    }

    protected override void OnLostTarget()
    {
        throw new System.NotImplementedException();
    }
}
