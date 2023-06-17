using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_Flower : MovableEnemy

{
    public GameObject throwableObj;
    private State idleState;
    private State attackState;
    private State hitState;
    private float attackInterval = 1.5f;
    private float timer;
    public Transform throwPivot;
    private Vector3 targetPos;
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
        PoolerManager.instance.InsertPooler(throwableObjKey, throwableObj, true);
    }
    
    private void Initialize()
    {
        idleState = new State("idle");
        hitState = new State("hit");
        attackState = new State("attack");
        enemyStatesMap.Add(idleState.stateName, idleState);
        enemyStatesMap.Add(attackState.stateName, attackState);
        enemyStatesMap.Add(hitState.stateName, hitState);
        StateInitialize();
        enemyStateMachine.SetIntialState(idleState);
        throwableObjKey = throwableObj.GetComponent<IPoolableObject>().GetKey();
    }

    private void StateInitialize()
    {
        idleState.onActive += prevState =>
        {
           enemyMovement.Stop();
        };
        

        attackState.onActive += state =>
        {
            enemyMovement.Stop();
            Debug.Log("attackState");
        };
        
        attackState.onStay += () =>
        {
            var rot = Vector3.Scale(new Vector3(1, 0, 1), targetPos);
            rot.y = 1.5f;
            transform.LookAt(rot);
            timer += Time.deltaTime;
            if (timer > attackInterval)
            {
                timer = 0;
                var throwObj = PoolerManager.instance.OutPool(throwableObjKey);
                throwObj.transform.position = transform.position;
                throwObj.transform.rotation = transform.localRotation;
                throwObj.GetComponent<Enemy_Flower_throwObj>().targetPos = targetPos;
                throwObj.gameObject.SetActive(true);
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
