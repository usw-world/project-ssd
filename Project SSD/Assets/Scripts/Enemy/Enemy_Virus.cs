using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.PlayerLoop;
using UnityEngine.Serialization;

public class Enemy_Virus : MovableEnemy
{
    private State idleState;
    private State chaseState;
    private State attackState;
    private State hitState;
    private Vector3 rotation;
    private float rotSpeed;
    private bool isRotate = false;

    public GameObject check;
    [FormerlySerializedAs("target")] public Transform targetTransform;
    public GameObject idleGameObject;
    public GameObject chaseGameObject;

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
            chaseGameObject.SetActive(false);
            idleGameObject.SetActive(true);
            isRotate = true;
        };

        idleState.onStay += () =>
        {
            rotation = transform.rotation.eulerAngles;
            rotation.y += Time.deltaTime * rotSpeed;
            transform.rotation = Quaternion.Euler(rotation);
            Debug.Log("onStay");
        };

        idleState.onInactive += state =>
        {
            Debug.Log("idle inactive");
        };

        chaseState.onActive += prevState =>
        {
            isRotate = false;
            idleGameObject.SetActive(false);
            chaseGameObject.SetActive(true);
            transform.LookAt(targetTransform);
            enemyMovement.MoveToPoint(targetTransform.position, moveSpeed);
        };

        attackState.onActive += state =>
        {

        };


    }
    
    
    
    
    protected override void ChaseTarget(Vector3 point)
    {
        // SendChangeState(chaseState);
    }

    protected override void OnLostTarget()
    {
        // SendChangeState(idleState);
    }
}
