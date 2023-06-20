using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.Serialization;


[RequireComponent(typeof(Rigidbody))]
public class Enemy_Bomber : MovableEnemy
{
    public DecalProjector rangeProjector;
    public DecalProjector delayProjector;
    public GameObject effect;
    public GameObject deadEffect;
    public GameObject model;
    private State idleState;
    private State chaseState;
    private State hitState;
    private float moveTimer = 0;
    private Vector3 targetPos;
    private Coroutine hitCoroutine;
    [SerializeField] private float boomTimer;
    [SerializeField] private float boomRange;
    protected override void Awake()
    {
        base.Awake();
        Initialize();
    }

    protected override void Start()
    {
        base.Start();
        rangeProjector.size = new Vector3(boomRange, boomRange, .1f);
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
            enemyAnimator.SetTrigger("idle");
            enemyMovement.Stop();
        };

        chaseState.onActive += prevState =>
        {
            effect.SetActive(true);
            enemyAnimator.SetTrigger("attack");
            StartCoroutine(CountDown());
        };

        chaseState.onStay += () =>
        {
            moveTimer += Time.deltaTime;
            var rot = Vector3.Scale(new Vector3(1, 0, 1), targetPos);
            rot.y = transform.position.y;
            transform.LookAt(rot);
            
            if (moveTimer < 0.5f)
                return;
            moveTimer = 0;
            enemyMovement.MoveToPoint(targetPos, moveSpeed);
            if(Vector3.Distance(transform.position, targetPos) < 2)
                enemyMovement.Stop();
        };

        chaseState.onInactive += prevState =>
        {
            // StopCoroutine(explode);
            // delayProjector.gameObject.SetActive(false);
            // rangeProjector.gameObject.SetActive(false);
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
        else{
            this.gameObject.SetActive(false);
            // model.gameObject.SetActive(false);
            Debug.Log(this.gameObject.name+" is dead");
        }
    }
    
    public void Explode()
    {
        Damage damage = new Damage(5, 2.0f, Vector3.forward * 5, Damage.DamageType.Normal);
        var targets = Physics.OverlapSphere(transform.position, boomRange, 1<<7);
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
            SendChangeState(chaseState);
    }

    protected override void OnLostTarget()
    {
        if(enemyStateMachine.currentState != idleState)
            SendChangeState(idleState);
    }


    IEnumerator CountDown()
    {
        rangeProjector.gameObject.SetActive(true);
        delayProjector.gameObject.SetActive(true);
        float timer = 0.0f;
        while(true)
        {
            if (timer < boomTimer)
            {
                Debug.Log($"{boomTimer-timer}초 후 터짐");
                delayProjector.size = new Vector3(timer*(boomRange/boomTimer), timer*(boomRange/boomTimer), .1f);
                timer += Time.deltaTime;
                yield return new WaitForSeconds(Time.deltaTime);
            }
            else
            {
                break;
            }
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
