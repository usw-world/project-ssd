using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Random = UnityEngine.Random;
using Mirror;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(CapsuleCollider))]
[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(TPlayerInput))]
[RequireComponent(typeof(StateMachine))]
[RequireComponent(typeof(Movement))]
public class TPlayer : MonoBehaviour , IDamageable, IGetAPable
{
	#region Init
	static public TPlayer instance { get; private set; }
    
	[SerializeField] private PlayerStatus status;
	[SerializeField] private WeaponTransform sword;
	[SerializeField] private Skill dodgeAttackSkill;
	[SerializeField] private Skill[] attackSkills;

	Coroutine dodgeCoroutine;
	Coroutine rushCoroutine;
	Coroutine WalkCoroutine;
	RaycastHit[] dodgeHits;
	Vector3 lookVecter;
	Vector3 targetPos;
	string nowAnimationTrigger = "";
	float idleTime = 0;
	float rotSpeed = 30f;
	float idleActionTime = 5f;
	bool isImnune = false;
	bool isSuperArmour = false;
	bool isCanAttack = false;
	bool isWalk = false;
	bool isRush = false;
	int attackCount = 0;
	int idleActionIdx = 0;
	int hitCount = 0;

	#region Component
	private Movement movement;
    private Animator ani;
    private Rigidbody rigi;
    private StateMachine stateMachine;
	#endregion
	#region State
	private State idleState_1 = new State("Idle_1");
    private State idleState_2 = new State("Idle_2");
    private State idleState_3 = new State("Idle_3");
    private State moveState = new State("Move");
    private State attackState_1 = new State("Attack_1");
    private State attackState_2 = new State("Attack_2");
    private State attackState_3 = new State("Attack_3");
    private State attackState_4 = new State("Attack_4");
    private State dodgeAttackState = new State("Dodge Attack");
    private State dodgeState = new State("Dodge");
    private State downState = new State("Down");
    private State damageState = new State("Damage");
    private State refleshState = new State("Reflesh");

    private List<State> attackStateGroup = new List<State>();
    private List<State> idleStateGroup = new List<State>();
	#endregion
	#endregion

	private void Awake()
    {
        if(instance == null) {
            instance = this;
        } else {
            Debug.LogWarning("There already is TPlayer on this scene.");
            Destroy(this.gameObject);
        }
        ani = GetComponent<Animator>();
        rigi = GetComponent<Rigidbody>();
        movement = GetComponent<Movement>();
        stateMachine = GetComponent<StateMachine>();
        Cursor.lockState = CursorLockMode.Locked;
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
        idleState_1.onActive = (State prev) => {
			ChangeAnimation("Idle1");
			idleTime = 0;
		};
        idleState_2.onActive = (State prev) => { ChangeAnimation("Idle2"); };
        idleState_3.onActive = (State prev) => { ChangeAnimation("Idle3"); };
        moveState.onActive = (State prev) => {
			ChangeAnimation("Move");
			SwordUse(false);
		};
        attackState_1.onActive = (State prev) => {
			ChangeAnimation("Attack1");
			SwordUse(true);
		};
        attackState_2.onActive = (State prev) => { ChangeAnimation("Attack2"); };
        attackState_3.onActive = (State prev) => { ChangeAnimation("Attack3"); };
        attackState_4.onActive = (State prev) => { ChangeAnimation("Attack4"); };
		dodgeAttackState.onActive = (State prev) => {
			ChangeAnimation("DodgeAttack");
			SwordUse(true);
			dodgeHits = movement.CheckDirection(transform.forward, 8f, 5 << 6);
			targetPos = transform.forward + transform.position + (transform.forward * 5f + Vector3.up * .5f);
		};
        dodgeState.onActive = (State prev) => {
			ChangeAnimation("Dodge");
			isImnune = true;
			dodgeCoroutine = StartCoroutine(DodgeCoroutine());
		};
        downState.onActive = (State prev) => {
			ChangeAnimation("Down");
			isSuperArmour = true;
		};
        damageState.onActive = (State prev) => {
			ChangeAnimation("Damage");
			hitCount = 0;
		};
        refleshState.onActive = (State prev) => { ChangeAnimation("Reflesh"); };
    }
	private void InitializeStateOnStay()
    {
        idleState_1.onStay = () => {
			idleTime += Time.deltaTime;
			if (idleTime >= idleActionTime)
			{
				idleTime = 0;
				idleActionIdx = Random.Range(1, idleStateGroup.Count);
				stateMachine.ChangeState(idleStateGroup[idleActionIdx], false);
			}
		};
        idleState_2.onStay = () => { };
        idleState_3.onStay = () => { };
        moveState.onStay = () => {
			rotate();
			if (isRush)
			{
				movement.MoveToward(Vector3.forward * status.speed * 2f * Time.deltaTime);
				return;
			}
			movement.MoveToward(Vector3.forward * ((isWalk) ? status.speed / 2 : status.speed) * Time.deltaTime);
		};
        attackState_1.onStay = () => {
			MoveToTargetPos();
		};
        attackState_2.onStay = () => {
			MoveToTargetPos();
		};
        attackState_3.onStay = () => {
			MoveToTargetPos();
		};
        attackState_4.onStay = () => {
			MoveToTargetPos();
		};
		dodgeAttackState.onStay = () => {
			MoveToTargetPos();
		};
		dodgeState.onStay = () => { };
        downState.onStay = () => { };
        damageState.onStay = () => {
			if (hitCount >= 3) OnDown(); 
		};
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
		dodgeAttackState.onInactive = (State next) => {
			dodgeHits = null;
		};
        dodgeState.onInactive = (State next) => {
			isImnune = false;
			if (dodgeCoroutine != null) StopCoroutine(dodgeCoroutine);
		};
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
            stateMachine.currentState == dodgeAttackState ||
            stateMachine.currentState == dodgeState ||
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
	public void OnDamage(GameObject origin, float amount)
	{
		if (isImnune) return;    // 무적이면 실행 않함

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
        if (stateMachine.currentState == dodgeAttackState) return; 
        if (stateMachine.currentState == dodgeState)
        {
			OnDodgeAttack();
            return;
        }

        if (stateMachine.currentState == damageState ||
            stateMachine.currentState == downState) return;
		if (lookVecter != Vector3.zero) rotate(10f); 
		stateMachine.ChangeState(dodgeState, false);
    }
    public void OnAttack()
    {
        if (stateMachine.currentState == dodgeState)
        {
			OnDodgeAttack();
            return;
        }
		if (stateMachine.currentState == moveState && isRush)
		{
			OnDodgeAttack();
			return;
		}
        if (stateMachine.currentState == damageState ||
            stateMachine.currentState == dodgeState ||
            stateMachine.currentState == dodgeAttackState ||
            stateMachine.currentState == downState ||
            isCanAttack == false) return;

        isCanAttack = false;

        if (lookVecter != Vector3.zero) rotate(10f);

		//attackCoroutine = StartCoroutine(AttackCoroutine());
		targetPos = transform.forward + transform.position + (transform.forward * 1f + Vector3.up * 0.5f);
		stateMachine.ChangeState(attackStateGroup[attackCount], false);
    }
    public void OnSkill_0()
    {
        
    }
    public void OnSkill_1()
    {
        
    }
	public void OnMoveSpeedConvert()
	{
		if (isRush) return;
		isWalk = (isWalk) ? false : true;
		if(WalkCoroutine != null) StopCoroutine(WalkCoroutine);
		WalkCoroutine = StartCoroutine(SmoothConvert(isWalk));
	}
	public void OnRunToRush()
	{
		isWalk = false;
		if (rushCoroutine != null) StopCoroutine(rushCoroutine); 
		rushCoroutine = StartCoroutine(SmoothConvertRush(true));
	}
	public void OnRushToRun()
	{
		isRush = false;
		if (rushCoroutine != null) StopCoroutine(rushCoroutine);
		rushCoroutine = StartCoroutine(SmoothConvertRush(false));
	}
	public void BeCanNextAttack() => isCanAttack = true;
	public void ChackAttackZone()
	{
		attackCount = (attackCount >= attackStateGroup.Count) ? 0 : attackCount;
		attackSkills[attackCount].Use(); // 임시
		attackCount++;
		attackCount = (attackCount >= attackStateGroup.Count) ? 0 : attackCount;

		//Vector3 position = transform.position - transform.forward + transform.forward + transform.forward + Vector3.up;
		//Vector3 size = new Vector3(1f, 2f, 1f);
		//Collider[] hit = Physics.OverlapBox(position, size, transform.rotation, 1 << LayerMask.NameToLayer("Enemy"));

		//for (int i = 0; i < hit.Length; i++)
		//{
		//	IDamageable target = hit[i].GetComponent<IDamageable>();
		//	target?.OnDamage(gameObject, status.AP);
		//}
	}
	public void ChackDodgeAttackZone()
	{
		dodgeAttackSkill.Use();
		//for (int i = 0; i < dodgeHits.Length; i++)
		//{
		//	IDamageable temp = dodgeHits[i].collider.GetComponent<IDamageable>();
		//	temp?.OnDamage(gameObject, 10f);
		//}
	}
	public float GetAP()
	{
		return status.AP;
	}

	void rotate(float rotSppedPoint = 1f)
	{
		Vector3 lookTarget = Quaternion.AngleAxis(Camera.main.transform.eulerAngles.y, Vector3.up) * lookVecter;
		Vector3 look = Vector3.Slerp(transform.forward, lookTarget.normalized, rotSpeed * rotSppedPoint * Time.deltaTime);
		transform.rotation = Quaternion.LookRotation(look);
		transform.eulerAngles = new Vector3(0, transform.rotation.eulerAngles.y, 0);
	}
	void MoveToTargetPos()
	{
		Vector3 z = Vector3.zero;
		targetPos.y = transform.position.y;
		Vector3 dir = Vector3.SmoothDamp(transform.position, targetPos, ref z, 0.02f) - transform.position;
		movement.MoveToward(dir, Space.World);
	}
	void OnDodgeAttack()
    {
		if (lookVecter != Vector3.zero)
		{
			Vector3 lookTarget = Quaternion.AngleAxis(Camera.main.transform.eulerAngles.y, Vector3.up) * lookVecter;
			Vector3 look = Vector3.Slerp(transform.forward, lookTarget.normalized, rotSpeed * Time.deltaTime * 7f);
			transform.rotation = Quaternion.LookRotation(look);
			transform.eulerAngles = new Vector3(0, transform.rotation.eulerAngles.y, 0);
		}
        stateMachine.ChangeState(dodgeAttackState, false);
        isCanAttack = false;
    }
    void ChangeAnimation(string trigger)
    {
		if (nowAnimationTrigger != "") 
			ani.ResetTrigger(nowAnimationTrigger);
        nowAnimationTrigger = trigger;
        ani.SetTrigger(nowAnimationTrigger);
    }
    void SwordUse(bool use) => sword.Set(use);

	IEnumerator SmoothConvert(bool fade)
	{
		float target = (fade) ? 0 : 0.5f;
		float now = ani.GetFloat("Speed");

		if (fade)
		{
			while (now > target)
			{
				now -= Time.deltaTime * 2f;
				ani.SetFloat("Speed", now);
				yield return null;
			}
		}
		else
		{
			while (now < target)
			{
				now += Time.deltaTime * 2f;
				ani.SetFloat("Speed", now);
				yield return null;
			}
		}
		now = target;
		ani.SetFloat("Speed", now);
	}
	IEnumerator SmoothConvertRush(bool fade)
	{
		float target = (fade) ? 1f : 0.5f;
		float now = ani.GetFloat("Speed");
		
		if (fade)
		{
			isRush = true;
			while (now < target)
			{
				now += Time.deltaTime * 2f;
				ani.SetFloat("Speed", now);
				yield return null;
			}
		}
		else
		{
			while (now > target)
			{
				now -= Time.deltaTime * 2f;
				ani.SetFloat("Speed", now);
				yield return null;
			}
			isRush = false;
		}
		now = target;
		ani.SetFloat("Speed", now);
	}
	IEnumerator DodgeCoroutine()
	{
		float offset = 0;
		Vector3 targetPoint = Vector3.forward;

		while (offset < 1)
		{
			offset += Time.deltaTime * 4;
			Vector3 d = Vector3.Lerp(Vector3.zero, targetPoint, Mathf.Sin(Mathf.PI * .5f + offset * .5f * Mathf.PI));
			d.y = 0;
			movement.MoveToward(d * Time.deltaTime * 30f);
			yield return null;
		}
	}
	void OnDrawGizmos()
	{
		//Vector3 size = new Vector3(1f,2f,1f);
		//Vector3 position = transform.position - transform.forward + transform.forward + transform.forward + Vector3.up;
		//Gizmos.color = new Color(0, 0, 1, 0.5f);
		//Gizmos.DrawCube(position, size);
		//Gizmos.color = Color.white;
		//Gizmos.DrawWireCube(position, size);

		//Gizmos.color = Color.yellow;
		//Vector3 offset = new Vector3(0, 0.5f, 0);
		//Vector3 from = transform.position + offset;
		//Vector3 to = transform.forward - transform.position;

		//to += transform.forward * 5f;
		//to += offset;

		//Gizmos.DrawLine(from, transform.position + (transform.forward * 5f + Vector3.up * .5f));
	}
}
[Serializable]
public class PlayerStatus
{
	public float speed = 3f;     // 이동속도
	public float maxHP = 100f;       // 최대 체력
	public float maxMP = 100f;       // 최대 마나
	public float maxSP = 100f;       // 최대 스테미너
	[HideInInspector] public float HP = 100f;      // 체력
	[HideInInspector] public float MP = 100f;     // 마나 ** 시작하면서 set 하는거 어떤지?
	[HideInInspector] public float SP = 100f;      // 스테미너 ** 시작하면서 set 하는거 어떤지?
	[HideInInspector] public float AP = 10f;      // 공격력
}
[Serializable]
public class WeaponTransform
{
	public Transform weapon; // 무기
	public Transform unUse;  // 무기 사용 안할때 위치
	public Transform use;    // 무기 사용 위치
	public void Set(bool useing)
	{
		weapon.parent = (useing) ? use : unUse;
		weapon.localPosition = Vector3.zero;
		weapon.localEulerAngles = Vector3.zero;
		weapon.localScale = Vector3.one;
	}
}