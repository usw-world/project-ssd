using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;


[RequireComponent(typeof(Rigidbody))]
public class Enemy_Bomber : MovableEnemy
{
    private State idleState;
    private State chaseState;
    private State hitState;
    private float moveTimer = 0;
    private Vector3 targetPos;
    private Coroutine explode;
    private Coroutine hitCoroutine;
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
        hitState = new State("hit");
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
            
            if (moveTimer < 0.5f)
                return;
            moveTimer = 0;
            var rot = Vector3.Scale(new Vector3(1, 0, 1), targetPos);
            rot.y = transform.position.y;
            enemyMovement.MoveToPoint(targetPos, moveSpeed);
            transform.LookAt(rot);
            if(Vector3.Distance(transform.position, targetPos) < 3)
                enemyMovement.Stop();
        };

        chaseState.onInactive += prevState =>
        {
            StopCoroutine(explode);
        };
        hitState.onActive += state =>
        {
            
        };


    }

    public override void OnDamage(Damage damage) {
        base.OnDamage(damage);
    }

    public override void TakeDamage(Damage damage) {
        base.TakeDamage(damage);
        if(!isDead) {
            if(hitCoroutine != null)
                StopCoroutine(hitCoroutine);
            hitCoroutine = StartCoroutine(HitCoroutine(damage));
        }
        else
            this.gameObject.SetActive(false);
    }
    
    public void Explode()
    {
        Damage damage = new Damage(5, 2.0f, Vector3.forward * 5, Damage.DamageType.Normal);
        var targets = Physics.OverlapSphere(transform.position, 5, 1<<7);
        foreach (var obj in targets)
        {
            obj.TryGetComponent(out IDamageable damageable);
            damageable?.OnDamage(damage);
        }
        this.gameObject.SetActive(false);
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
        for (int i = 0; i < 15; i++)
        {
            Debug.Log($"{3-i*0.2f}초 후 터짐");
            yield return new WaitForSeconds(0.2f);
        }
        Explode();
    }
    private IEnumerator HitCoroutine(Damage damage) {
        float offset = 0;
        float pushedOffset = 0;
        Vector3 pushedDestination = Vector3.Scale(new Vector3(1, 0, 1), damage.forceVector);
        if(damage.hittingDuration > 0)
            SendChangeState(hitState);
        while(offset < damage.hittingDuration) {
            enemyMovement.MoveToward(Vector3.Lerp(pushedDestination, Vector3.zero, pushedOffset) * Time.deltaTime, Space.World, moveLayerMask);
            pushedOffset += Time.deltaTime * 2;
            offset += Time.deltaTime;
            yield return null;
        }
        SendChangeState(idleState);
    }
}
