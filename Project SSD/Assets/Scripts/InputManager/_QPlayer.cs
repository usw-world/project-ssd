using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using Unity.IO.LowLevel.Unsafe;
using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(StateMachine))]
public class _QPlayer : MonoBehaviour
{
    [SerializeField] float speed = 3f;
    [SerializeField] float jumpPower = 5f;

    Rigidbody rbody;
    StateMachine qplayerStateMachine;
    InputManager inputManager;
    Vector3 moveDir;
    Animator qplayerAni;

    bool canJump = true;

    public State moveState { get; private set; } = new State("Move");
    public State idleState { get; private set; } = new State("Idle");
    public State attackState { get; private set; } = new State("Attack");
    public State jumpState { get; private set; } = new State("Jump");
    public State basicState
    {
        get
        {
            return idleState;
        }
    }
    public State currentState 
    { 
        get 
        { 
            return qplayerStateMachine.currentState; 
        } 
    }

    private void Awake()
    {
        qplayerAni = GetComponent<Animator>();
        rbody = GetComponent<Rigidbody>();

        inputManager = GameObject.FindObjectOfType<InputManager>();     // 나중에 직접 참조로 변경
        if (inputManager == null)
        {
            Debug.LogError("InputManager not found in scene.");
        }


        if (TryGetComponent<StateMachine>(out qplayerStateMachine))
        {
            qplayerStateMachine.SetIntialState(idleState);
        }
        else
        {
            Debug.LogError("Player hasn't any 'StateMachine'.");
        }
        InitialState();
    }

    private void InitialState()
    {
        attackState.onActive += (prevState) =>
        {

        };
        attackState.onInactive += (nextState) =>
        {

        };
        idleState.onActive += (prevState) =>
        {
        };
        idleState.onInactive += (nextState) =>
        {
        };
        moveState.onActive += (prevState) =>
        {
            qplayerAni.SetBool("Move", true);
        };
        moveState.onInactive += (nextState) =>
        {
            qplayerAni.SetBool("Move", false);
        };
        jumpState.onActive += (prevState) =>
        {
            qplayerAni.SetTrigger("Jump");
        };
        jumpState.onInactive += (nextState) =>
        {
            
        };
    }

    //private void Attack()
    //{
    //    qplayerAni.SetBool("Attack", true);
    //}

    private void FixedUpdate()
    {
        Debug.Log("current state : " + qplayerStateMachine.currentState);
        Move();
        Jump();
    }

    private void Move()
    {
        if (qplayerStateMachine.Compare(moveState) || qplayerStateMachine.Compare(jumpState))
        {
            transform.Translate(new Vector3(moveDir.x, 0f, moveDir.y) * speed * Time.deltaTime);
            
        }

    }

    private void Jump()
    {
        if (qplayerStateMachine.Compare(jumpState) && canJump)
        {
            rbody.AddForce(Vector3.up * jumpPower, ForceMode.Impulse);
            canJump = false;
        }
    }

    private void HandleJump()
    {
        qplayerStateMachine.ChangeState(jumpState, true);
    }

 
    private void HandleMove(Vector3 vec)
    {
        moveDir = vec;
        qplayerStateMachine.ChangeState(moveState, false);

        if (vec == Vector3.zero)
        {
            qplayerStateMachine.ChangeState(idleState);
        }
    }

    private void HandleAttack()
    {
        qplayerStateMachine.ChangeState(attackState);
    }

    private void OnEnable()
    {
        inputManager.Jump += HandleJump;
        inputManager.Move += HandleMove;
        inputManager.Attack += HandleAttack;
    }

    private void OnDisable()
    {
        inputManager.Jump -= HandleJump;
        inputManager.Move -= HandleMove;
        inputManager.Attack -= HandleAttack;
    }

    private void OnCollisionEnter(Collision collision)
    {
        canJump = true;
        qplayerStateMachine.ChangeState(idleState);
    }
}
