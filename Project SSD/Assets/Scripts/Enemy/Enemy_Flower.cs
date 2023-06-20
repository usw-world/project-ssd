using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_Flower : MovableEnemy

{
    public GameObject throwableObj;
    public GameObject deadEffect;
    public GameObject model;
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
    
    protected override void Initialize()
    {
        base.Initialize();
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
        };
        
        attackState.onStay += () =>
        {
            Vector3 targetPosition = Vector3.Scale(new Vector3(1, 0, 1), targetPos);
            targetPosition.y = transform.position.y;
            transform.LookAt(targetPosition);
            targetPosition.y = throwPivot.position.y;
            throwPivot.transform.LookAt(targetPosition);
            timer += Time.deltaTime;
            if (timer > attackInterval)
            {
                timer = 0;
                enemyAnimator.SetTrigger("attack");
            }
        };

        attackState.onInactive += state =>
        {
            enemyAnimator.SetTrigger("idle");
        };


    }

    public override void OnDamage(Damage damage) {
        base.OnDamage(damage);
    }

    public override void TakeDamage(Damage damage) {
        base.TakeDamage(damage);
        if (isDead)
        {
            // model.gameObject.SetActive(false);
            gameObject.SetActive(false);
        }
        else
        {
            // 피격 애니메이션 설정
        }
    }
    


    protected override void ChaseTarget(Vector3 point)
    {
        targetPos = point;
        if(enemyStateMachine.currentState != attackState)
            SendChangeState(attackState);
    }

    protected override void OnLostTarget()
    {
        if(enemyStateMachine.currentState != idleState)
            SendChangeState(idleState);
    }

    public void Attack()
    {
        var throwObj = PoolerManager.instance.OutPool(throwableObjKey);
        throwObj.transform.position = throwPivot.position;
        throwObj.transform.rotation = throwPivot.rotation;
        // throwObj.transform.rotation = transform.localRotation;
        throwObj.GetComponent<Enemy_Flower_throwObj>().targetPos = targetPos;
        throwObj.gameObject.SetActive(true);
    }
    
    


}
