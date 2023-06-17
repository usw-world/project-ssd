using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_Flower : MovableEnemy

{
    public GameObject throwableObj;
    private State idleState;
    private State attackState;
    private State hitState;
    private float attackInterval = 0;
    private float timer;
    private Transform throwPivot;
    private Vector3 targetPos;
    private Coroutine explode;
    private Coroutine hitCoroutine;
    private string throwableObjKey;
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
        hitState = new State("hit");
        enemyStatesMap.Add(idleState.stateName, idleState);
        enemyStatesMap.Add(attackState.stateName, attackState);
        enemyStatesMap.Add(hitState.stateName, hitState);
        StateInitialize();
        throwableObjKey = throwableObj.GetComponent<IPoolableObject>().GetKey();
        PoolerManager.instance.InPool(throwableObjKey, throwableObj);
        enemyStateMachine.SetIntialState(idleState);
    }

    private void StateInitialize()
    {
        idleState.onActive += prevState =>
        {
           
        };
        

        attackState.onActive += state =>
        {
            
        };
        
        attackState.onStay += () =>
        {
            if (timer > attackInterval)
            {
                timer = 0;
                var throwObj = PoolerManager.instance.OutPool(throwableObjKey).GetComponent<Enemy_Flower_throwObj>();
                throwObj.targetPos = targetPos;
                
            }
        };


    }

    public override void OnDamage(Damage damage) {
        base.OnDamage(damage);
    }

    public override void TakeDamage(Damage damage) {
        base.TakeDamage(damage);
        if(isDead)
            gameObject.SetActive(false);
        else
        {
            // 피격 애니메이션 설정
        }
        
        
    }
    


    protected override void ChaseTarget(Vector3 point)
    {
        targetPos = point;
        if(enemyStateMachine.currentState != attackState)
            enemyStateMachine.ChangeState(attackState);
    }

    protected override void OnLostTarget()
    {
        if(enemyStateMachine.currentState != idleState)
            enemyStateMachine.ChangeState(idleState);
    }


}
