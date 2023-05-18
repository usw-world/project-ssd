using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using Random = UnityEngine.Random;
using Mirror;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(CapsuleCollider))]
[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(TPlayerInput))]
[RequireComponent(typeof(StateMachine))]
[RequireComponent(typeof(Movement))]
public class TPlayer : NetworkBehaviour, IDamageable
{
	static public TPlayer instance { get; private set; }

	#region Show Parameters
	public PlayerStatus status;
	[SerializeField] private WeaponTransform sword;
	[SerializeField] private TPlayerSkillManager skill;
	[SerializeField] private TPlayerTrackEffect trackEffect;
	[SerializeField] private GameObject tPlayerCamera;
	#endregion Show Parameters

	#region Hide Parameters
	private AttachmentManager attachmentManager;
	private Coroutine attackCoroutine;
	private Coroutine dodgeCoroutine;
	private Coroutine rushCoroutine;
	private Coroutine WalkCoroutine;
	private Vector3 lookVector;
	/// <summary>기본적인 이동 이외의 이동들(회피, 공격 파생 이동)의 부드러운 움직임을 위한 Target Point입니다./summary>
	private Vector3 extraMovingPoint;
	private string currentAnimationTrigger = "";
	private float idleTime = 0;
	private float rotateSpeed = 30f;
	private float idleActionTime = 5f;
	private int chargingLevel = 0;
	private float chargingTime = 0;
	private float[] chargingMaxTime = { 0.5f, 0.5f, 0.5f };
	private bool isImmune = false;
	private bool isSuperArmor = false;
	private bool isCanAttack = false;
	private bool isWalk = false;
	private bool isRush = false;
	private bool isPressingMRB;
	private int attackCount = 0;
	private int idleActionIdx = 0;
	private int hitCount = 0;
	#endregion Hide Parameters

	public float DamageAmount{
		get
		{
			return 10f;
		}
	}

	#region Component
	private Movement movement;
    private Animator ani;
    private StateMachine stateMachine;
	#endregion Component

	#region States
	private Dictionary<string, State> statesMap = new Dictionary<string, State>();

	private State idleState1 = new State("Idle1", "Idle");
    private State idleState2 = new State("Idle2", "Idle");
    private State idleState3 = new State("Idle3", "Idle");
    private State moveState = new State("Move");
    private State attackState1 = new State("Attack1", "Basic Attack");
    private State attackState2 = new State("Attack2", "Basic Attack");
    private State attackState3 = new State("Attack3", "Basic Attack");
    private State attackState4 = new State("Attack4", "Basic Attack");
    private State dodgeAttackState = new State("Dodge Attack");
    private State downAttackState = new State("Down Attack");
    private State dodgeState = new State("Dodge");
    private State downState = new State("Down");
    private State damageState = new State("Damage");
	private State chargingStart = new State("chargingStart");
	private State chargingStay = new State("chargingStay");
	private State chargingDrawSwordAttack = new State("chargingDrawSwordAttack");
	private State chargingDrawSwordAttack_7time = new State("chargingDrawSwordAttack_7time");
	private State comboAttack_1 = new State("comboAttack_1");
	
	private List<State> attackStateGroup = new List<State>();
    private List<State> idleStateGroup = new List<State>();
	#endregion States

	#region UI
	public Slider sliderHP;
	public Slider sliderSP;
	public Slider sliderCharging;
	public Image imgCharging;
	public Image imgChargingBackground;
	private Color red = new Color(1f, 0f, 0f);
	private Color orange = new Color(1f, 0.65f, 0f);
	private Color yellow = new Color(1f, 1f, 0f);
	private Color darkYellow = new Color(0.8f, 0.8f, 0f);
	#endregion UI

	#region Initialize
	public override void OnStartLocalPlayer() {
        base.OnStartLocalPlayer();
    }
	private void Awake()
    {
        if(instance == null)
            instance = this;
        else
            Destroy(this.gameObject);
        DontDestroyOnLoad(gameObject);
		attachmentManager = GetComponent<AttachmentManager>();
        ani = GetComponent<Animator>();
        movement = GetComponent<Movement>();
        stateMachine = GetComponent<StateMachine>();
    }
    private void Start()
	{
		InitializeStates();
        InitializeStateOnActive();
        InitializeStateOnStay();
        InitializeStateOnInactive();

        stateMachine.SetIntialState(idleState1);
        attackStateGroup.Add(attackState1);
        attackStateGroup.Add(attackState2);
        attackStateGroup.Add(attackState3);
        attackStateGroup.Add(attackState4);
        idleStateGroup.Add(idleState1);
        idleStateGroup.Add(idleState2);
        idleStateGroup.Add(idleState3);
		
		sliderHP.maxValue = status.maxHp;
		sliderSP.maxValue = status.maxSp;
		trackEffect.dodgeMaehwa.Enable();
		trackEffect.dodgeMaehwa.Disable();

		InitializeCamera();
		if(isLocalPlayer)
        	Cursor.lockState = CursorLockMode.Locked;
	}
    private void InitializeCamera() {
        if(isLocalPlayer &&
		PlayerCamera.instance == null) {
            GameObject camera = Instantiate(tPlayerCamera);
			camera.GetComponent<PlayerCamera>().SetTarget(this.transform);
        }
    }
	private void InitializeStates() {
		statesMap.Add(idleState1.stateName, idleState1);
		statesMap.Add(idleState2.stateName, idleState2);
		statesMap.Add(idleState3.stateName, idleState3);
		statesMap.Add(moveState.stateName, moveState);
		statesMap.Add(attackState1.stateName, attackState1);
		statesMap.Add(attackState2.stateName, attackState2);
		statesMap.Add(attackState3.stateName, attackState3);
		statesMap.Add(attackState4.stateName, attackState4);
		statesMap.Add(dodgeAttackState.stateName, dodgeAttackState);
		statesMap.Add(downAttackState.stateName, downAttackState);
		statesMap.Add(dodgeState.stateName, dodgeState);
		statesMap.Add(downState.stateName, downState);
		statesMap.Add(damageState.stateName, damageState);
		statesMap.Add(chargingStart.stateName, chargingStart);
		statesMap.Add(chargingStay.stateName, chargingStay);
		statesMap.Add(chargingDrawSwordAttack_7time.stateName, chargingDrawSwordAttack_7time);
		statesMap.Add(comboAttack_1.stateName, comboAttack_1);
		statesMap.Add(chargingDrawSwordAttack.stateName, chargingDrawSwordAttack);
	}
	private void InitializeStateOnActive()
    {
        idleState1.onActive = (State prev) => {
			ChangeAnimation("Idle1");
			idleTime = 0;
		};
        idleState2.onActive = (State prev) => { ChangeAnimation("Idle2"); };
        idleState3.onActive = (State prev) => { ChangeAnimation("Idle3"); };
        moveState.onActive = (State prev) => {
			ChangeAnimation("Move");
			SwordUse(false);
			trackEffect.moveSmoke.Enable();
		};
        attackState1.onActive = (State prev) => {
			ChangeAnimation("Attack1");
			SwordUse(true);
		};
        attackState2.onActive = (State prev) => { ChangeAnimation("Attack2"); };
        attackState3.onActive = (State prev) => { ChangeAnimation("Attack3"); };
        attackState4.onActive = (State prev) => { ChangeAnimation("Attack4"); };
		dodgeAttackState.onActive = (State prev) => {
			ChangeAnimation("DodgeAttack");
			SwordUse(true);
			extraMovingPoint = transform.forward + transform.position + (transform.forward * 5f + Vector3.up * .5f);
			trackEffect.dodgeMaehwa.Enable();
		};
		downAttackState.onActive = (State prev) => {
			ChangeAnimation("DownAttack");
			SwordUse(true);
			extraMovingPoint = transform.forward + transform.position + (transform.forward * 5f + Vector3.up * .5f);
			trackEffect.dodgeMaehwa.Enable();
		};
		dodgeState.onActive = (State prev) => {
			ChangeAnimation("Dodge");
			isImmune = true;
			dodgeCoroutine = StartCoroutine(DodgeCoroutine());
			trackEffect.dodgeMaehwa.Enable();
		};
        downState.onActive = (State prev) => {
			ChangeAnimation("Down");
			isSuperArmor = true;
		};
        damageState.onActive = (State prev) => {
			ChangeAnimation("Damage");
			hitCount = 0;
		};
		chargingStart.onActive = (State prev) => {
			ChangeAnimation("Charging");
			chargingLevel = 0;
			chargingTime = 0;
			SwordUse(true);
		};
		chargingStay.onActive = (State prev) => {
			sliderCharging.gameObject.SetActive(true);
			sliderCharging.value = 0;
			sliderCharging.maxValue = chargingMaxTime[0];
			SetChargingSliderColor();
		}; 
		chargingDrawSwordAttack.onActive = (State prev) => {
			ChangeAnimation("DrawSwordAttack");
		};
		chargingDrawSwordAttack_7time.onActive = (State prev) => {
			ChangeAnimation("Draw Sword Attack 7time");
		};
		comboAttack_1.onActive = (State prev) => {
			SwordUse(true);
			ChangeAnimation("Combo Attack 01");
		};
	}
	private void InitializeStateOnStay()
    {
        idleState1.onStay = () => {
			idleTime += Time.deltaTime;
			if (idleTime >= idleActionTime)
			{
				idleTime = 0;
				idleActionIdx = Random.Range(1, idleStateGroup.Count);
				ChangeState(idleStateGroup[idleActionIdx], false);
			}
			status.Update();
		};
        idleState2.onStay = () => {
			status.Update();
		};
        idleState3.onStay = () => {
			status.Update();
		};
        moveState.onStay = () => {
			Rotate();
			if (isRush)
			{
				if (status.sp > 0)
				{
					status.sp -= Time.deltaTime * 10f;
					movement.MoveToward(Vector3.forward * status.speed * 2f * Time.deltaTime);
					return;
				}
				else
				{
					OnRushToRun();
				}
			}
			status.Update();
			movement.MoveToward(Vector3.forward * ((isWalk) ? status.speed / 2 : status.speed) * Time.deltaTime);
		};
        attackState1.onStay = () => {};
        attackState2.onStay = () => {};
        attackState3.onStay = () => {};
        attackState4.onStay = () => {};
		dodgeAttackState.onStay = () => { MoveToTargetPos(); };
		downAttackState.onStay = () => { MoveToTargetPos(); };
		dodgeState.onStay = () => { };
        downState.onStay = () => { status.Update(); };
        damageState.onStay = () => {
			if (hitCount >= 3) OnDown(); 
		};
		chargingStart.onStay = () => { status.Update(); };
		chargingStay.onStay = () => {
			status.Update(); status.Update(); status.Update();
			if (chargingLevel < chargingMaxTime.Length)
			{
				chargingTime += Time.deltaTime;
				if (chargingTime >= chargingMaxTime[chargingLevel])
				{
					chargingLevel++;
					chargingTime = 0;
					if (chargingLevel < chargingMaxTime.Length)
						sliderCharging.maxValue = chargingMaxTime[chargingLevel];
					SetChargingSliderColor();
				}
				sliderCharging.value = chargingTime;
			}
		};
	}
	private void InitializeStateOnInactive()
    {
        idleState1.onInactive  = (State next) => {  };
        idleState2.onInactive = (State next) => {  };
        idleState3.onInactive = (State next) => {  };
        moveState.onInactive = (State next) => {
			trackEffect.moveSmoke.Disable();
		};
        attackState1.onInactive = (State next) => { 
			if (attackCoroutine != null) StopCoroutine(attackCoroutine); 
		};
        attackState2.onInactive = (State next) => { 
			if (attackCoroutine != null) StopCoroutine(attackCoroutine); 
		};
        attackState3.onInactive = (State next) => { 
			if (attackCoroutine != null) StopCoroutine(attackCoroutine); 
		};
        attackState4.onInactive = (State next) => { 
			if (attackCoroutine != null) StopCoroutine(attackCoroutine); 
		};
		dodgeAttackState.onInactive = (State next) => {  };
		downAttackState.onInactive = (State next) => {  };
        dodgeState.onInactive = (State next) => {
			isImmune = false;
			if (dodgeCoroutine != null) StopCoroutine(dodgeCoroutine);
			trackEffect.dodgeMaehwa.Disable();
		};
        downState.onInactive = (State next) => { isSuperArmor = false; };
        damageState.onInactive = (State next) => {  };
		chargingStart.onInactive = (State next) => { };
		chargingStay.onInactive = (State next) => {
			sliderCharging.gameObject.SetActive(false);
		};
	}
	#endregion Initialize

	#region Input Event
	public void InputMove(Vector3 moveVecterInput)
    {
        lookVector = moveVecterInput;

        if (stateMachine.currentState == attackState1 ||
            stateMachine.currentState == attackState2 ||
            stateMachine.currentState == attackState3 ||
            stateMachine.currentState == attackState4 ||
            stateMachine.currentState == dodgeAttackState ||
            stateMachine.currentState == dodgeState ||
            stateMachine.currentState == downState ||
            stateMachine.currentState == damageState ||
			stateMachine.currentState == chargingStart ||
			stateMachine.currentState == chargingStay ||
			stateMachine.currentState == chargingDrawSwordAttack_7time ||
			stateMachine.currentState == chargingDrawSwordAttack ||
			stateMachine.currentState == comboAttack_1)
        {
            return;
        }
        if (stateMachine.currentState == idleState2 || 
            stateMachine.currentState == idleState3 )
        {
            if (lookVector == Vector3.zero)
            {
                ChangeState(idleStateGroup[idleActionIdx], false);
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
        if (lookVector == Vector3.zero)
            ChangeState(idleState1, false);
        else
            ChangeState(moveState, false);
    }
	public void OnDamage(Damage damage) {
		if (isImmune) return;    // 무적이면 실행 안함

		status.hp -= damage.amount;

		if (damage.origin != null)
		{
			LookTarget(damage.origin.transform);
			transform.eulerAngles = new Vector3(0, transform.eulerAngles.y, 0);
		}

		if (stateMachine.currentState == downState) return;
		if (isSuperArmor) return;

		if (stateMachine.currentState == damageState)
		{
			hitCount++;
			ani.SetTrigger("Damage");
		}
		else
		{
			ChangeState(damageState, false);
		}
	}
	public void OnDown()
	{ 
		ChangeState(downState, false); 
	}
    public void OnSlide()
    {
		if (!skill.dodge.CanUse()) return;

		if (stateMachine.currentState == dodgeAttackState) return; 
   //     if (stateMachine.currentState == dodgeState)
   //     {
			//OnDodgeAttack();
   //         return;
   //     }
		if (stateMachine.currentState == downState)
		{
			OnDownAttack();
			return;
		}

		if (stateMachine.currentState == damageState ||
            stateMachine.currentState == downState ||
			stateMachine.currentState == dodgeState ||
			stateMachine.currentState == chargingDrawSwordAttack_7time ||
			stateMachine.currentState == comboAttack_1) return;
		if (lookVector != Vector3.zero) Rotate(10f); 
		ChangeState(dodgeState, false);
		skill.dodge.Use();
	}
    public void OnAttack()
    {
		if (!skill.nomalAttacks[attackCount].CanUse()) return;

        if (stateMachine.currentState == dodgeState)
        {
			OnDodgeAttack(); return;
        }
		if (stateMachine.currentState == moveState && isRush)
		{
			if (skill.dodgeAttack.CanUse()) 
			{
				OnDodgeAttack();
				return;
			}
		}
        if (stateMachine.currentState == damageState ||
            stateMachine.currentState == dodgeState ||
            stateMachine.currentState == dodgeAttackState ||
            stateMachine.currentState == downState ||
			stateMachine.currentState == chargingStart || 
			stateMachine.currentState == chargingStay || 
			isCanAttack == false ||
			stateMachine.currentState == chargingDrawSwordAttack_7time ||
			stateMachine.currentState == chargingDrawSwordAttack ||
			stateMachine.currentState == comboAttack_1) return;

		isCanAttack = false;

        if (lookVector != Vector3.zero) Rotate(15f);

		extraMovingPoint = transform.forward + transform.position + (transform.forward * 1f + Vector3.up * 0.5f);
		ChangeState(attackStateGroup[attackCount], false);
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
	public void OnChargingStart()
	{
		if (stateMachine.currentState == dodgeAttackState ||
			stateMachine.currentState == downAttackState ||
			stateMachine.currentState == downAttackState ||
			stateMachine.currentState == downAttackState ||
			stateMachine.currentState == damageState ||
			stateMachine.currentState == chargingDrawSwordAttack_7time ||
			stateMachine.currentState == chargingDrawSwordAttack ||
			stateMachine.currentState == comboAttack_1 )
		{
			return;
		}
		isPressingMRB = true;
		ChangeState(chargingStart, false);
	}
	public void OnChargingEnd()
	{
		isPressingMRB = false;
		if (stateMachine.currentState == chargingStart)
		{
			ChangeState(chargingDrawSwordAttack);
			return;
		}
		if (stateMachine.currentState != chargingStay) return;
		if (lookVector != Vector3.zero) Rotate(15f);
		switch (chargingLevel)
		{
			case 0: ChangeState(chargingDrawSwordAttack); break;
			case 1: ChangeState(chargingDrawSwordAttack); break;
			case 2: ChangeState(chargingDrawSwordAttack_7time); break;
			case 3: ChangeState(chargingDrawSwordAttack); break;
		}
	}
	public void OnComboAttack()
	{
		if (stateMachine.currentState == dodgeAttackState ||
			stateMachine.currentState == downAttackState ||
			stateMachine.currentState == downAttackState ||
			stateMachine.currentState == downAttackState ||
			stateMachine.currentState == damageState ||
			stateMachine.currentState == chargingDrawSwordAttack_7time ||
			stateMachine.currentState == comboAttack_1)
		{
			return;
		}
		if (skill.combo_1[0].CanUse())
		{
			ChangeState(comboAttack_1, false);
		}
	}
	#endregion Input Event

	#region Animation Event
	public void BeCanNextAttack()
	{
		isCanAttack = true; 
	}
	public void ChargingStart()
	{
		if (stateMachine.currentState == chargingStart) {
			if (isPressingMRB)
			{
				ChangeState(chargingStay, false);
			}
			else
			{
				ResetState();
			}
		}
	}
	public void CheckAttackZone()
	{
		attackCount = (attackCount >= skill.nomalAttacks.Length) ? 0 : attackCount;
		skill.nomalAttacks[attackCount].Use();
		attackCount++;
		attackCount = (attackCount >= skill.nomalAttacks.Length) ? 0 : attackCount;
		if (attackCoroutine != null)
			StopCoroutine(attackCoroutine);
		attackCoroutine = StartCoroutine(AttackCoroutine());
	}
	public void CheckDrawSwordAttack_7time(int idx) 
	{
		skill.charging_DrawSwordAttack_7time[idx].Use();
		if (idx != 0) return;
		if (attackCoroutine != null)
			StopCoroutine(attackCoroutine);
		attackCoroutine = StartCoroutine(AttackCoroutine());
	}
	public void CheckDodgeAttackZone()
	{
		skill.dodgeAttack.Use();
		trackEffect.dodgeMaehwa.Disable();
	}
	public void CheckDownAttackZone()
	{
		skill.downAttack.Use();
		trackEffect.dodgeMaehwa.Disable();
	}
	public void ChackDrawSwordAttack()
	{
		skill.charging_DrawSwordAttack.Use();
		if (attackCoroutine != null)
			StopCoroutine(attackCoroutine);
		attackCoroutine = StartCoroutine(AttackCoroutine());
	}
	public void CheckComboAttack_1(int idx)
	{
		skill.combo_1[idx].Use();
		if (attackCoroutine != null)
			StopCoroutine(attackCoroutine);
		attackCoroutine = StartCoroutine(AttackCoroutine());
	}
	#endregion Animation Event

	private void Update()
	{
		sliderSP.value = status.sp;
		sliderHP.value = status.hp;
	}
	public void AddAttachment(Attachment attachment) {
		attachmentManager.AddAttachment(attachment);
	}
	public void ChangeHp(float amount)
	{
		status.hp += amount;
		if (status.hp > status.maxHp)
		{
			status.hp = status.maxHp;
		}
		sliderHP.value = status.hp;
	}


	private void Rotate(float rotSppedPoint = 1f)
	{
		Vector3 lookTarget = Quaternion.AngleAxis(Camera.main.transform.eulerAngles.y, Vector3.up) * lookVector;
		Vector3 look = Vector3.Slerp(transform.forward, lookTarget.normalized, rotateSpeed * rotSppedPoint * Time.deltaTime);
		transform.rotation = Quaternion.LookRotation(look);
		transform.eulerAngles = new Vector3(0, transform.rotation.eulerAngles.y, 0);
	}
	private void LookTarget(Transform target) 
	{
		transform.LookAt(target);
		transform.eulerAngles = new Vector3(0, transform.eulerAngles.y, 0);
	}
	private void MoveToTargetPos()
	{
		extraMovingPoint.y = transform.position.y;
		Vector3 dir = Vector3.Lerp(transform.position, extraMovingPoint, Time.deltaTime * 10f) - transform.position;
		movement.MoveToward(dir, Space.World);
	}
	private void OnDodgeAttack()
    {
		if (!skill.dodgeAttack.CanUse()) return;
		if (lookVector != Vector3.zero)
		{
			Vector3 lookTarget = Quaternion.AngleAxis(Camera.main.transform.eulerAngles.y, Vector3.up) * lookVector;
			Vector3 look = Vector3.Slerp(transform.forward, lookTarget.normalized, rotateSpeed * Time.deltaTime * 7f);
			transform.rotation = Quaternion.LookRotation(look);
			transform.eulerAngles = new Vector3(0, transform.rotation.eulerAngles.y, 0);
		}
        ChangeState(dodgeAttackState, false);
        isCanAttack = false;
    }
	private void OnDownAttack()
	{
		if (!skill.downAttack.CanUse()) return;
		if (lookVector != Vector3.zero)
		{
			Vector3 lookTarget = Quaternion.AngleAxis(Camera.main.transform.eulerAngles.y, Vector3.up) * lookVector;
			Vector3 look = Vector3.Slerp(transform.forward, lookTarget.normalized, rotateSpeed * Time.deltaTime * 7f);
			transform.rotation = Quaternion.LookRotation(look);
			transform.eulerAngles = new Vector3(0, transform.rotation.eulerAngles.y, 0);
		}
		ChangeState(downAttackState, false);
		isCanAttack = false;
	}
	private void ChangeAnimation(string trigger)
    {
		if (currentAnimationTrigger != "") 
			ani.ResetTrigger(currentAnimationTrigger);
        currentAnimationTrigger = trigger;
        ani.SetTrigger(currentAnimationTrigger);
    }
	private void SwordUse(bool use) 
	{
		sword.Set(use); 
	}
	private void ChangeState(State state, bool intoSelf=true) {
		if(isLocalPlayer)
			SynchronizeState(state.stateName, intoSelf);
	}
	[ClientRpc]
	private void SynchronizeState(string stateName, bool intoSelf) {
		try {
			State nextState = statesMap[stateName];
			if(nextState != null)
				stateMachine.ChangeState(nextState, intoSelf);
		} catch(KeyNotFoundException e) {
			Debug.LogError(e);
		}
	}
	private void SetChargingSliderColor() {
		switch (chargingLevel)
		{
			case 0:
				imgCharging.color = yellow;
				imgChargingBackground.color = darkYellow; 
				break;
			case 1:
				imgCharging.color = orange;
				imgChargingBackground.color = yellow;
				break;
			case 2:
				imgCharging.color = red;
				imgChargingBackground.color = orange;
				break;
			case 3:
				imgCharging.color = red;
				imgChargingBackground.color = red;
				break;
		}
	}
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
			offset += Time.deltaTime * 2.5f;
			Vector3 d = Vector3.Lerp(Vector3.zero, targetPoint, Mathf.Sin(Mathf.PI * .5f + offset * .5f * Mathf.PI));
			d.y = 0;
			movement.MoveToward(d * Time.deltaTime * 16f);
			yield return null;
		}
	}
	IEnumerator AttackCoroutine()
	{
		float offset = 0;
		Vector3 targetPoint = transform.position + transform.forward;
		while (offset < 1)
		{
			offset += Time.deltaTime * 6f;
			targetPoint.y = transform.position.y;
			Vector3 dir = Vector3.Lerp(transform.position, targetPoint, Time.deltaTime * 13f) - transform.position;
			dir.y = 0;
			movement.MoveToward(dir, Space.World, 5<<6);
			yield return null;
		}
	}
}
[Serializable]
public class PlayerStatus
{
	public float speed = 3f;     // 이동속도
	public float maxHp = 100f;       // 최대 체력
	public float maxSp = 100f;       // 최대 스테미너
	public float hp = 100f;      // 체력
	public float sp = 100f;      // 스테미너 ** 시작하면서 set 하는거 어떤지?
	public float ap = 10f;      // 공격력
	public float hpRecovery = 1f;
	public float spRecovery = 1f;
	public void Update() 
	{
		hp = (hp < maxHp) ? hp += Time.deltaTime * hpRecovery : maxHp;
		sp = (sp < maxSp) ? sp += Time.deltaTime * spRecovery : maxSp;
	}
}
[Serializable]
class WeaponTransform
{
	public Transform weapon; // 무기
	public Transform unUse;  // 무기 사용 안할 때 위치
	public Transform use;    // 무기 사용 위치
	public void Set(bool useing)
	{
		weapon.parent = (useing) ? use : unUse;
		weapon.localPosition = Vector3.zero;
		weapon.localEulerAngles = Vector3.zero;
		weapon.localScale = Vector3.one;
	}
}
[Serializable]
class TPlayerSkillManager 
{
	public Skill[] nomalAttacks;
	public Skill dodge;
	public Skill dodgeAttack;
	public Skill downAttack;
	public Skill charging_DrawSwordAttack;
	public Skill[] charging_DrawSwordAttack_7time;
	public Skill[] combo_1;
}
[Serializable]
class TPlayerTrackEffect
{
	public TrackEffect dodgeMaehwa;
	public TrackEffect moveSmoke;
}