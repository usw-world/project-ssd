
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental;
using UnityEngine;

public class Enemy_Virus : MovableEnemy
{

    public GameObject check;
    public GameObject idleGameObject;
    public GameObject chaseGameObject;
    public GameObject inspectionObject;
    public GameObject inspectionImage;
    public Vector3 targetPos;
    
    private State idleState;
    private State chaseState;
    private State attackState;
    private State hitState;

    private Coroutine hitCoroutine;
    private Vector3 rotation;
    private Transform tmpTPlayerTransform;
    private string inspectionImagePoolerKey = "inspectionImage";
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
        PoolerManager.instance.InsertPooler("inspectionImage", inspectionImage, true);
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
        };

        chaseState.onStay += () =>
        {
            
            transform.LookAt(targetPos);
            if (chaseTimer > 0.3f)
            {
                chaseTimer = 0;
                enemyMovement.MoveToPoint(targetPos, moveSpeed);   
            }
            else
            {
                chaseTimer += Time.deltaTime;
            }
        };

        chaseState.onInactive += prevState =>
        {
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
        if (enemyStateMachine.currentState != chaseState && enemyStateMachine.currentState != attackState)
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
        List<GameObject> imageList = new List<GameObject>();
        for (int i = 0; i < 2; i++)
        {
            for (int j = 0; j < 10; j++)
            {
                var image = PoolerManager.instance.OutPool(inspectionImagePoolerKey);
                var imageTr = image.GetComponent<RectTransform>();
                image.transform.SetParent(inspectionObject.transform);
                imageList.Add(image);
                Vector3 pos = imageTr.position;
                pos.x += (i * 350)+(j * 20)+300f;
                pos.y -= (j*12)-200f+(i*40);
                imageTr.position = pos;
                yield return new WaitForSeconds(0.1f);
                Debug.Log("Image Add");
            }
        }
        yield return new WaitForSeconds(1);
        foreach (var image in imageList)
        {
            Destroy(image);
            yield return new WaitForSeconds(0.05f);
        }
        Debug.Log("Destroy virus self");
        this.gameObject.SetActive(false);
    }
}
