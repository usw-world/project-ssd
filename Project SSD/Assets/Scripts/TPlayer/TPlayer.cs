using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(CapsuleCollider))]
[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(TPlayerInput))]
[RequireComponent(typeof(StateMachine))]
[RequireComponent(typeof(Movement))]
public class TPlayer : MonoBehaviour
{
    [Header("Player Status")]
    private float moveSpeed = 3f;               // 이동속도
    private float rotSpeed = 30f;               // 회전속도
    private float idleTime = 0;                 // idle 시간
    private float idleActionTime = 10;          // idle 액션 시간
    private bool isSuperArmour = false;         // 슈퍼아머?
    private bool isNotDamage = false;           // 무적?
    private bool isCanAttack = true;            // 공격 가능?

    private int hitCount = 0;                   // 연속 피격 횟수
    private int attackCount = 0;                // 연속 공격 횟수
    private int idleActionIdx = 0;              // 아이들 행동 인덱스
    private string nowAnimationTrigger = "";    // 현재 애니메이션 트리거
    private Vector3 lookVecter;

    [Header("Sword Transform")]
    [SerializeField] private Transform sword;       // 무기
    [SerializeField] private Transform swordUnUse;  // 무기 사용 안할때 위치
    [SerializeField] private Transform swordUse;    // 무기 사용 위치

    private Movement movement;
    private Animator ani;
    private Rigidbody rigi;
    private StateMachine stateMachine;

    private State idleState_1 = new State("Idle_1");
    private State idleState_2 = new State("Idle_2");
    private State idleState_3 = new State("Idle_3");
    private State moveState = new State("Move");
    private State attackState_1 = new State("Attack_1");
    private State attackState_2 = new State("Attack_2");
    private State attackState_3 = new State("Attack_3");
    private State attackState_4 = new State("Attack_4");
    private State attackState_S = new State("Attack_S");
    private State slideState = new State("Slide");
    private State downState = new State("Down");
    private State damageState = new State("Damage");
    private State refleshState = new State("Reflesh");

    private List<State> attackStateGroup = new List<State>();
    private List<State> idleStateGroup = new List<State>();
    private void Awake()
    {
        ani = GetComponent<Animator>();
        rigi = GetComponent<Rigidbody>();
        movement = GetComponent<Movement>();
        stateMachine = GetComponent<StateMachine>();
        //Cursor.lockState = CursorLockMode.Locked;
    }
    private void Start()
    {
        InitializeStateOnActive();
        InitializeStateOnStay();
        InitializeStateOnInactive();
        stateMachine.SetIntialState(idleState_1);
        attackStateGroup.Add(attackState_1);
        attackStateGroup.Add(attackState_2);
        attackStateGroup.Add(attackState_3);
        attackStateGroup.Add(attackState_4);
        idleStateGroup.Add(idleState_1);
        idleStateGroup.Add(idleState_2);
        idleStateGroup.Add(idleState_3);
    }
    private void InitializeStateOnActive()
    {
        idleState_1.onActive = (State prev) => { ChangeAnimation("Idle1"); idleTime = 0; };
        idleState_2.onActive = (State prev) => { ChangeAnimation("Idle2"); };
        idleState_3.onActive = (State prev) => { ChangeAnimation("Idle3"); };
        moveState.onActive = (State prev) => { ChangeAnimation("Move"); SwordUse(false); };
        attackState_1.onActive = (State prev) => { ChangeAnimation("Attack1"); SwordUse(true); };
        attackState_2.onActive = (State prev) => { ChangeAnimation("Attack2"); };
        attackState_3.onActive = (State prev) => { ChangeAnimation("Attack3"); };
        attackState_4.onActive = (State prev) => { ChangeAnimation("Attack4"); };
        attackState_S.onActive = (State prev) => { ChangeAnimation("SAttack"); SwordUse(true); };
        slideState.onActive = (State prev) => { ChangeAnimation("Slide"); isNotDamage = true; };
        downState.onActive = (State prev) => { ChangeAnimation("Down"); isSuperArmour = true; };
        damageState.onActive = (State prev) => { hitCount = 0; };
        refleshState.onActive = (State prev) => { ChangeAnimation("Reflesh"); };
    }
    private void InitializeStateOnStay()
    {
        idleState_1.onStay = () => { IdleTimeUpdate(); };
        idleState_2.onStay = () => { };
        idleState_3.onStay = () => { };
        moveState.onStay = () => { Move(); };
        attackState_1.onStay = () => { };
        attackState_2.onStay = () => { };
        attackState_3.onStay = () => { };
        attackState_4.onStay = () => { };
        attackState_S.onStay = () => { Sliding(); };
        slideState.onStay = () => { Sliding(); };
        downState.onStay = () => { };
        damageState.onStay = () => { HitCountUpdate(); };
        refleshState.onStay = () => { };
    }
    private void InitializeStateOnInactive()
    {
        idleState_1.onInactive  = (State next) => {  };
        idleState_2.onInactive = (State next) => {  };
        idleState_3.onInactive = (State next) => {  };
        moveState.onInactive = (State next) => {  };
        attackState_1.onInactive = (State next) => {  };
        attackState_2.onInactive = (State next) => {  };
        attackState_3.onInactive = (State next) => {  };
        attackState_4.onInactive = (State next) => {  };
        attackState_S.onInactive = (State next) => {  };
        slideState.onInactive = (State next) => { isNotDamage = false; };
        downState.onInactive = (State next) => { isSuperArmour = false; };
        damageState.onInactive = (State next) => {  };
        refleshState.onInactive = (State next) => {  };
    }
    
    public void InputMove(Vector3 moveVecterInput)
    {
        lookVecter = moveVecterInput;

        if (stateMachine.currentState == attackState_1 ||
            stateMachine.currentState == attackState_2 ||
            stateMachine.currentState == attackState_3 ||
            stateMachine.currentState == attackState_4 ||
            stateMachine.currentState == attackState_S ||
            stateMachine.currentState == slideState ||
            stateMachine.currentState == downState ||
            stateMachine.currentState == damageState ||
            stateMachine.currentState == refleshState)
        {
            return;
        }
        if (stateMachine.currentState == idleState_2 || 
            stateMachine.currentState == idleState_3 )
        {
            if (lookVecter == Vector3.zero)
            {
                stateMachine.ChangeState(idleStateGroup[idleActionIdx], false);
            }
            else
            {
                ResetState();
            }
        }
        else
        {
            ResetState();
        }
    }
    public void ResetState()
    {
        isCanAttack = true;
        attackCount = 0;
        if (lookVecter == Vector3.zero)
            stateMachine.ChangeState(idleState_1, false);
        else
            stateMachine.ChangeState(moveState, false);
    }
    public void OnDamage()
    {
        if (isNotDamage) return;    // 무적이면 실행 않함

        // hp -= damage         // hp 감소

        if (stateMachine.currentState == downState) return;
        if (isSuperArmour) return;

        if (stateMachine.currentState == damageState)
        {
            hitCount++;
            ani.SetTrigger("Damage");
        }
        else
        {
            stateMachine.ChangeState(damageState, false);
        }
    }
    public void OnDown() => stateMachine.ChangeState(downState, false);  
    public void OnSlide()
    {
        if (stateMachine.currentState == attackState_S) return; 
        if (stateMachine.currentState == slideState)
        {
            OnSAttack();
            return;
        }

        if (stateMachine.currentState == damageState ||
            stateMachine.currentState == downState) return;

		stateMachine.ChangeState(slideState, false);
    }
    public void OnAttack()
    {
        if (stateMachine.currentState == slideState)
        {
            OnSAttack();
            return;
        }

        if (stateMachine.currentState == damageState ||
            stateMachine.currentState == slideState ||
            stateMachine.currentState == attackState_S ||
            stateMachine.currentState == downState ||
            isCanAttack == false) return;

        isCanAttack = false;

        if (lookVecter != Vector3.zero)
        {
            Vector3 lookTarget = Quaternion.AngleAxis(Camera.main.transform.eulerAngles.y, Vector3.up) * lookVecter;
            Vector3 look = Vector3.Slerp(transform.forward, lookTarget.normalized, rotSpeed * Time.deltaTime * 5f);
            transform.rotation = Quaternion.LookRotation(look);
            transform.eulerAngles = new Vector3(0, transform.rotation.eulerAngles.y, 0);
        }

        

        stateMachine.ChangeState(attackStateGroup[attackCount], false);
        attackCount++;
        attackCount = (attackCount >= attackStateGroup.Count) ? 0 : attackCount;
    }
    public void OnSAttack()
    {
        Vector3 lookTarget = Quaternion.AngleAxis(Camera.main.transform.eulerAngles.y, Vector3.up) * lookVecter;
        Vector3 look = Vector3.Slerp(transform.forward, lookTarget.normalized, rotSpeed * Time.deltaTime * 5f);
        transform.rotation = Quaternion.LookRotation(look);
        transform.eulerAngles = new Vector3(0, transform.rotation.eulerAngles.y, 0);
        stateMachine.ChangeState(attackState_S, false);
        isCanAttack = false;
    }
    public void OnSkill_0()
    {
        
    }
    public void OnSkill_1()
    {
        
    }
    public void BeCanNextAttack() => isCanAttack = true;
    void Move()
    {
        Vector3 lookTarget = Quaternion.AngleAxis(Camera.main.transform.eulerAngles.y, Vector3.up) * lookVecter;
        Vector3 look = Vector3.Slerp(transform.forward, lookTarget.normalized, rotSpeed * Time.deltaTime);
        transform.rotation = Quaternion.LookRotation(look);
        transform.eulerAngles = new Vector3(0, transform.rotation.eulerAngles.y, 0);

        movement.MoveToward(Vector3.forward * moveSpeed * Time.deltaTime);
    }
    void IdleTimeUpdate()
    {
        idleTime += Time.deltaTime;
        if (idleTime >= idleActionTime)
        {
            idleTime = 0;
            idleActionIdx = Random.Range(1, idleStateGroup.Count);
            stateMachine.ChangeState(idleStateGroup[idleActionIdx], false);
        }
    }
    void HitCountUpdate()
    {
        if (hitCount >= 3)
        {
            OnDown();
        }
    }
    void ChangeAnimation(string trigger)
    {
        ani.ResetTrigger(nowAnimationTrigger);
        nowAnimationTrigger = trigger;
        ani.SetTrigger(nowAnimationTrigger);
    }
    void Sliding() => movement.MoveToward(Vector3.forward * moveSpeed * 2f * Time.deltaTime); 
    void SwordUse(bool use)
    {
        sword.parent = (use) ? swordUse : swordUnUse;
        sword.localPosition = Vector3.zero;
        sword.localEulerAngles = Vector3.zero;
        sword.localScale = Vector3.one;
    }
}
