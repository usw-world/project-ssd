using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Enemy_Bomber : MovableEnemy
{

    private State idleState;
    private State chaseState;

    private float moveTimer = 0;
    private Vector3 targetPos;
    private Coroutine explode;
    protected override void Awake()
    {
        base.Awake();
        Initialize();
    }

    protected override void Start()
    {
        base.Start();
    }
    
    private void Initialize()
    {
        idleState = new State("idle");
        chaseState = new State("chase");
        enemyStatesMap.Add(idleState.stateName, idleState);
        enemyStatesMap.Add(chaseState.stateName, chaseState);
        StateInitialize();
        enemyStateMachine.SetIntialState(idleState);
    }

    private void StateInitialize()
    {
        idleState.onActive += prevState =>
        {
            enemyMovement.Stop();
        };

        chaseState.onActive += prevState =>
        {
            explode = StartCoroutine(CountDown());
        };

        chaseState.onStay += () =>
        {
            moveTimer += Time.deltaTime;
            if (moveTimer < 0.1f)
                return;
            moveTimer = 0;
            transform.LookAt(targetPos);
            enemyMovement.MoveToPoint(targetPos, moveSpeed);
        };

        chaseState.onInactive += prevState =>
        {
            StopCoroutine(explode);
        };

    }

    private void Explode()
    {
        var damage = new Damage(1, 0.5f, transform.forward, Damage.DamageType.Normal);
        base.OnDamage(damage);
    }

    protected override void ChaseTarget(Vector3 point)
    {
        targetPos = point;
        if(enemyStateMachine.currentState != chaseState)
            enemyStateMachine.ChangeState(chaseState);
    }

    protected override void OnLostTarget()
    {
        if(enemyStateMachine.currentState != idleState)
            enemyStateMachine.ChangeState(idleState);
    }


    IEnumerator CountDown()
    {
        for (int i = 0; i < 10; i++)
        {
            Debug.Log($"{2-i*0.2f}초 후 터짐");
            yield return new WaitForSeconds(0.2f);
        }
        Explode();
    }
}
