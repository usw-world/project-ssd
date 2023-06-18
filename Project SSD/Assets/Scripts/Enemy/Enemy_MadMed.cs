
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.Experimental;
using UnityEngine;
using UnityEngine.Serialization;
using Random = System.Random;

[RequireComponent(typeof(Rigidbody))]
public class Enemy_MadMed : MovableEnemy
{

    public GameObject check;
    public GameObject idleGameObject;
    public GameObject chaseGameObject;
    public GameObject Canvas;
    public Vector3 targetPos;
    private State idleState;
    private State chaseState;
    private State attackState;
    private State hitState;
    private Coroutine hitCoroutine;
    private Vector3 rotation;
    [SerializeField] private Transform[] imageList;
    private float rotSpeed;
    private float chaseTimer;
    private float moveTimer;
    private Damage lastTakenDamage;
    private bool isSpin = false;
    


    protected override void Awake()
    {
        base.Awake();
    }

    protected override void Start()
    {
        base.Start();
        Initialize();
        enemyStateMachine.SetIntialState(idleState);
    }


    private void Initialize()
    {
        idleState = new State("idle");
        chaseState = new State("chase");
        attackState = new State("attack");
        hitState = new State("hit");
        enemyStatesMap.Add(idleState.stateName, idleState);
        enemyStatesMap.Add(chaseState.stateName, chaseState);
        enemyStatesMap.Add(attackState.stateName, attackState);
        enemyStatesMap.Add(hitState.stateName, hitState);

        // inspectionImagePullerKey
        StateInitialize();
        rotSpeed = 25.0f;
    }

    
    protected override void Update()
    {

    }


    private void StateInitialize()
    {
        idleState.onActive += prevState =>
        {
            enemyMovement.Stop();
        };

        idleState.onStay += () =>
        {
            rotation = transform.rotation.eulerAngles;
            rotation.y += Time.deltaTime * rotSpeed;
            transform.rotation = Quaternion.Euler(rotation);
        };

        idleState.onInactive += state =>
        {
            
        };

        chaseState.onActive += prevState =>
        {
            isSpin = false;
            idleGameObject.SetActive(false);
            chaseGameObject.SetActive(true);
            enemyAnimator.SetTrigger("walk");
        };

        chaseState.onStay += () =>
        {
            var rot = Vector3.Scale(new Vector3(1, 0, 1), targetPos);
            rot.y = transform.position.y;
            transform.LookAt(rot);
            moveTimer += Time.deltaTime;
            if (moveTimer < 0.5f)
                return;
            moveTimer = 0;
            enemyMovement.MoveToPoint(targetPos, moveSpeed);
            if(Vector3.Distance(transform.position, targetPos) < 1)
                enemyMovement.Stop();
        };

        chaseState.onInactive += prevState =>
        {
            enemyMovement.Stop();
            enemyAnimator.SetTrigger("idle");
        };

        attackState.onActive += state =>
        {
            enemyAnimator.SetTrigger("idle");
            StartCoroutine(CreateImage());
        };

        attackState.onStay += () =>
        {
            
        };

        attackState.onInactive += prevState =>
        {
        };

        hitState.onActive += state =>
        {
            
        };
        hitState.onInactive += prevState =>
        {
            try
            {
                StopCoroutine(hitCoroutine);
            }
            catch (Exception e)
            {
                Debug.LogError(e.StackTrace);
            }
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



    protected override void ChaseTarget(Vector3 point)
    {
        targetPos = point;
        if (enemyStateMachine.currentState != attackState && enemyStateMachine.currentState != chaseState && enemyStateMachine.currentState != hitState)
            SendChangeState(chaseState);
        float distance = Vector3.Distance(point, transform.position);
        if (distance <= 2 && enemyStateMachine.currentState != attackState)
        {
            SendChangeState(attackState);
        }
    }

    protected override void OnLostTarget()
    {
        if(enemyStateMachine.currentState != idleState)
            SendChangeState(idleState);
    }

    
    IEnumerator CreateImage()
    {
        chaseGameObject.SetActive(false);
        imageList = Canvas.GetComponentsInChildren<Transform>(true);
        var list = new List<GameObject>();
        Random random = new Random();
        foreach (var tr in imageList)
        {
           list.Add(tr.gameObject); 
        }
        while (list.Count > 0)
        {
            int index = random.Next(0, list.Count);
            yield return new WaitForSeconds(0.05f);
            list[index].SetActive(true);
            list.RemoveAt(index);
        }
        yield return new WaitForSeconds(1f);
        foreach (var image in imageList)
        {
            image.gameObject.SetActive(false);
        }
        this.gameObject.SetActive(false);
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
