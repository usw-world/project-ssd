using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(SphereCollider))]
[RequireComponent(typeof(CapsuleCollider))]
[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(TPlayerInput))]
[RequireComponent(typeof(StateMachine))]
public class TPlayer : MonoBehaviour
{
    [Header("Player Status")]        
    private float moveSpeed = 3f;       // 캐릭터 이동속도
    private float rotSpeed = 30f;       // 캐릭터 회전속도
    private float idleTime = 0;         // idle 유지 시간
    private float idleActionTime = 10;  // idle 액션 나오는 시간
    private bool isSuperArmour = false; // 슈퍼 아머
    private bool isSword = false;       // 무기를 들고 있는가
    private bool isNotDamage = false;   // 무적인가
    private bool isAttack = false;      // 공격중인가
    private int hitCount = 0;           // 연속으로 hit 획수

    private Vector3 lookVecter;

    [SerializeField] private Transform sword;
    [SerializeField] private Transform swordUnUse;
    [SerializeField] private Transform swordUse;

    private Animator ani;
    private Rigidbody rigi;

    private StateMachine playerStateMachine;

    private State idleState     = new State("Idle");
    private State moveState     = new State("Move");
    private State damageState   = new State("Damage");
    private State downState     = new State("Down");
    private State slideState    = new State("Slide");
    private State attackState   = new State("Attack");
    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        ani = GetComponent<Animator>();
        rigi = GetComponent<Rigidbody>();
        playerStateMachine = GetComponent<StateMachine>();
        playerStateMachine.SetIntialState(idleState);
        InitializeState();
    }
    private void InitializeState()
    {
        idleState.onActive      = (State prev) => idleTime = 0;
        idleState.onStay        =           () => IdleTimeUpdate();

        moveState.onStay        =           () => Move();

        damageState.onActive    = (State prev) => hitCount = 1;
        damageState.onStay      =           () => HitCountUpdate();

        downState.onActive = (State prev) => 
        {
            isSuperArmour = true;
            ani.SetTrigger("Down");
        };
        downState.onInactive = (State next) =>
        {
            isSuperArmour = false;
        };

        slideState.onActive     = (State prev) => isNotDamage = true;
        slideState.onStay       =           () => Sliding();
        slideState.onInactive   = (State next) => isNotDamage = false;

        attackState.onActive    = (State prev) => ani.SetTrigger("Attack");
        attackState.onInactive  = (State next) => ani.ResetTrigger("Attack");
    }
    public void InputMove(Vector3 moveVecterInput)
    {
        lookVecter = moveVecterInput;

        ani.SetBool("Move", (lookVecter == Vector3.zero) ? false : true);

        if (playerStateMachine.currentState == damageState ||
            playerStateMachine.currentState == slideState ||
            playerStateMachine.currentState == attackState ||
            playerStateMachine.currentState == downState) return;

        if (playerStateMachine.currentState == idleState ||
            playerStateMachine.currentState == moveState)
        {
            playerStateMachine.ChangeState((lookVecter == Vector3.zero) ? idleState : moveState, false);
        }
    }
    public void OnDamage()
    {
        if (isNotDamage) return;

        // hp -= damage

        if (playerStateMachine.currentState == downState) return;

        if (isSuperArmour) return;

        ani.SetTrigger("Damage");

        if (playerStateMachine.currentState == damageState)
            hitCount++;
        else
            playerStateMachine.ChangeState(damageState, false);
    }
    public void OnDown()
    {
        playerStateMachine.ChangeState(downState, false); 
    }
    public void OnSwap()
    {
        if (playerStateMachine.currentState == damageState ||
            playerStateMachine.currentState == attackState ||
            playerStateMachine.currentState == downState ) return;

        ani.SetTrigger( isSword ? "SwapToUnUse" : "SwapToUse");
    }
    public void OnSlide()
    {
        if (playerStateMachine.currentState == damageState ||
            playerStateMachine.currentState == slideState ||
            playerStateMachine.currentState == downState) return;

        ani.SetTrigger("Slide");
        playerStateMachine.ChangeState(slideState, false);
    }
    public void OnAttack(bool attack)
    {
        isAttack = attack;
        if (!isAttack) return;
        if (!isSword) return;
        if (playerStateMachine.currentState == damageState ||
            playerStateMachine.currentState == slideState  ||
            playerStateMachine.currentState == downState    ) return;

        playerStateMachine.ChangeState(attackState, false);
    }
    public void WeaponSwitch()
    {
        isSword = isSword ? false : true;

        sword.parent = isSword ? swordUse : swordUnUse;
        sword.localPosition = Vector3.zero;
        sword.localEulerAngles = Vector3.zero;
        ani.SetFloat("isSword", isSword ? 1 : -1);
    }
    public void ResetState()
    {
        playerStateMachine.ChangeState((lookVecter == Vector3.zero) ? idleState : moveState, false);
    }
    public void ChackNextAttack()
    {
        if (isAttack)
            playerStateMachine.ChangeState(attackState);
        
        else
            ResetState();
    }
    void Move()
    {
        Vector3 lookTarget = Quaternion.AngleAxis(Camera.main.transform.eulerAngles.y, Vector3.up) * lookVecter;
        Vector3 look = Vector3.Slerp(transform.forward, lookTarget.normalized, rotSpeed * Time.deltaTime);
        transform.rotation = Quaternion.LookRotation(look);
        transform.eulerAngles = new Vector3(0, transform.rotation.eulerAngles.y, 0);

        transform.Translate(Vector3.forward * moveSpeed * Time.deltaTime);
    }
    void Sliding()
    {
        transform.Translate(Vector3.forward * moveSpeed * 2f * Time.deltaTime);
    }
    void IdleTimeUpdate()
    {
        idleTime += Time.deltaTime;
        if (idleTime >= idleActionTime)
        {
            idleTime = 0;
            ani.SetTrigger("Idle " + Random.Range(1, 3));
        }
    }
    void HitCountUpdate()
    {
        if (hitCount >= 3)
        {
            OnDown();
        }
    }
}
