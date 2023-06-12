
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental;
using UnityEngine;

public class Enemy_Virus : MovableEnemy
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
    private Transform tmpTPlayerTransform;
    private float rotSpeed;
    private float chaseTimer;
    private bool isSpin = false;


    protected override void Awake()
    {
        base.Awake();
        
        Debug.Log("Med awake");
    }

    protected override void Start()
    {
        base.Start();
        Initialize();
        enemyStateMachine.SetIntialState(idleState);
        Debug.Log("Med start");
        
    }


    private void Initialize()
    {
        idleState = new State("idle");
        chaseState = new State("chase");
        attackState = new State("attack");
        enemyStatesMap.Add(idleState.stateName, idleState);
        enemyStatesMap.Add(chaseState.stateName, chaseState);
        enemyStatesMap.Add(attackState.stateName, attackState);
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
            Debug.Log("Idle onActive");
            enemyMovement.Stop();
            chaseGameObject.SetActive(false);
            idleGameObject.SetActive(true);
            isSpin = true;
        };

        idleState.onStay += () =>
        {
            rotation = transform.rotation.eulerAngles;
            rotation.y += Time.deltaTime * rotSpeed;
            transform.rotation = Quaternion.Euler(rotation);
            Debug.Log("idle onStay");
        };

        idleState.onInactive += state =>
        {
            Debug.Log("idle inactive");
        };

        chaseState.onActive += prevState =>
        {
            tmpTPlayerTransform = TPlayer.instance.transform;
            Debug.Log("chase onActive");
            isSpin = false;
            idleGameObject.SetActive(false);
            chaseGameObject.SetActive(true);
            transform.LookAt(targetPos);
            enemyMovement.MoveToPoint(targetPos, moveSpeed); 
            // 이거 호출 계속되는데 이렇게 쓰는거 아닌거 같음
        };

        chaseState.onStay += () =>
        {

        };

        chaseState.onInactive += prevState =>
        {
            Debug.Log("Chase State onInactive");
            enemyMovement.Stop();
        };

        attackState.onActive += state =>
        {
            Debug.Log("Attack State OnActive");
            StartCoroutine(CreateImage());
        };

        attackState.onStay += () =>
        {
            
        };

        attackState.onInactive += prevState =>
        {
        };

    }


    protected override void ChaseTarget(Vector3 point)
    {
        targetPos = point;
        if (enemyStateMachine.currentState != attackState)
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
        foreach (var image in imageList)
        {
            image.gameObject.SetActive(true);
            yield return new WaitForSeconds(0.05f);
        }
        yield return new WaitForSeconds(1f);
        foreach (var image in imageList)
        {
            image.gameObject.SetActive(false);
        }
        this.gameObject.SetActive(false);
    }
    
}
