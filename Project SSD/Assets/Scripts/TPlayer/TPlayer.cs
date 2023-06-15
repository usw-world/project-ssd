using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using Random = UnityEngine.Random;
using Mirror;
using Cinemachine;

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
	[SerializeField] private TPlayerWeaponTransform sword;
	[SerializeField] private TPlayerSkillManager skill;
	[SerializeField] private TPlayerTrackEffect trackEffect;
	[SerializeField] private SkinnedMeshRenderer[] motionTrailedMeshRenderers;
	[SerializeField] private TPlayerCutSceneCam cutSceneCam;
	[SerializeField] private AudioSource audioSourceEffect;
	[SerializeField] private AudioSource audioSourceVoice;
	[SerializeField] private AudioSource audioSourceBgm;
	[SerializeField] private GameObject tPlayerCameraPrefab;
	[SerializeField] private GameObject tPlayerMesh;
	[SerializeField] private TPayerUI ui;
	#endregion Show Parameters

	#region Hide Parameters
	private AttachmentManager attachmentManager = new AttachmentManager();
	private TPlayerShieldManager shieldManager = new TPlayerShieldManager();
	private List<Transform> lateDamageTarget = new List<Transform>();
	private Coroutine attackCoroutine;
	private Coroutine dodgeCoroutine;
	private Coroutine rushCoroutine;
	private Coroutine WalkCoroutine;
	private Coroutine damageCoroutine;
    private Coroutine downCoroutine;
	private Vector3 lookVector; // 기본적인 이동 이외의 이동들(회피, 공격 파생 이동)의 부드러운 움직임을 위한 Target Point입니다
	private Vector3 nextAttackDirection; // 기본적인 이동 이외의 이동들(회피, 공격 파생 이동)의 부드러운 움직임을 위한 Target Point입니다
	private Vector3 extraMovingPoint;
	private string currentAnimationTrigger = "";
	private float invincibilityAndActionAmount = 0;
	private float[] chargingMaxTime = { 0.5f, 0.5f, 0.5f };
	private float idleActionTime = 5f;
	private float chargingTime = 0;
	private float rotateSpeed = 30f;
	private float idleTime = 0;
	private bool isInvincibilityAndAction = false;
	private bool isCheckLateDamageTarget = false;
	private bool isMotionTrail = false;
	private bool isPressingMRB = false;
	private bool isSuperArmor = false;
	private bool isCanAttack = false;
	private bool isImmune = false;
	private bool isWalk = false;
	private bool isRush = false;
	private bool isAfterBasicAttack = false;
	private int chargingLevel = 0;
	private int idleActionIdx = 0;
	private int attackCount = 0;
	#endregion Hide Parameters

	#region Component
	private LateDamageCntl lateDamageCntl;
    private StateMachine stateMachine;
	private Movement movement;
    private Animator ani;
	#endregion Component

	#region States
	private Dictionary<string, State> statesMap = new Dictionary<string, State>();

	private State idleState1 = new State("Idle1", "Idle");
    private State idleState2 = new State("Idle2", "Idle");
    private State idleState3 = new State("Idle3", "Idle");
    private State moveState = new State("Move");
	private State basicAttackState = new State("Basic Attack");
    private State dodgeAttackState = new State("Dodge Attack");
    private State downAttackState = new State("Down Attack");
    private State dodgeState = new State("Dodge");
    private State downState = new State("Down");
    private State damageState = new State("Damage");
	private State chargingStart = new State("chargingStart");
	private State chargingStay = new State("chargingStay");
	private State chargingDrawSwordAttack = new State("chargingDrawSwordAttack");
	private State chargingDrawSwordAttack_specialStart = new State("chargingDrawSwordAttack_specialStart");
	private State chargingDrawSwordAttack_specialStay = new State("chargingDrawSwordAttack_specialStay");
	private State chargingDrawSwordAttack_specialEnd = new State("chargingDrawSwordAttack_specialEnd");
	private State chargingDrawSwordAttack_nonCharging = new State("chargingDrawSwordAttack_nonCharging");
	private State chargingDrawSwordAttack_7time = new State("chargingDrawSwordAttack_7time");
	private State chargingDrawSwordAttack_2time = new State("chargingDrawSwordAttack_2time");
	private State comboAttack_1 = new State("comboAttack_1");
	
	private List<State> attackStateGroup = new List<State>();
    private List<State> idleStateGroup = new List<State>();
    #endregion States

    #region Initialize
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
		lateDamageCntl = GetComponent<LateDamageCntl>();
	}
    private void Start()
	{
		InitializeStates();
        InitializeStateOnActive();
        InitializeStateOnStay();
        InitializeStateOnInactive();
		ui.InitializeUI();

        stateMachine.SetIntialState(idleState1);
        idleStateGroup.Add(idleState1);
        idleStateGroup.Add(idleState2);
        idleStateGroup.Add(idleState3);

		trackEffect.Set();

		InitializeCamera();
		if (isLocalPlayer) { 
        	Cursor.lockState = CursorLockMode.Locked;
		}
		else{
			ui.canvas.SetActive(false);
		}
	}
    private void InitializeCamera() {
        if(isLocalPlayer &&
		PlayerCamera.instance == null) {
            GameObject camera = Instantiate(tPlayerCameraPrefab);
			camera.GetComponent<PlayerCamera>().SetTarget(this.transform);
			CameraManager.instance.playerCam = camera.GetComponent <CinemachineVirtualCamera>();
			CameraManager.instance.SetPlayerCamera();
        }
    }
	private void InitializeStates() {
		statesMap.Add(idleState1.stateName, idleState1);
		statesMap.Add(idleState2.stateName, idleState2);
		statesMap.Add(idleState3.stateName, idleState3);
		statesMap.Add(moveState.stateName, moveState);
		statesMap.Add(basicAttackState.stateName, basicAttackState);

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
		statesMap.Add(chargingDrawSwordAttack_nonCharging.stateName, chargingDrawSwordAttack_nonCharging);
		statesMap.Add(chargingDrawSwordAttack_2time.stateName, chargingDrawSwordAttack_2time);
		statesMap.Add(chargingDrawSwordAttack_specialStart.stateName, chargingDrawSwordAttack_specialStart);
		statesMap.Add(chargingDrawSwordAttack_specialStay.stateName, chargingDrawSwordAttack_specialStay);
		statesMap.Add(chargingDrawSwordAttack_specialEnd.stateName, chargingDrawSwordAttack_specialEnd);
	}
	private void InitializeStateOnActive()
    {
        idleState1.onActive = (State prev) => {
			idleTime = 0;
			if (prev == basicAttackState)
			{
				if (attackCount != 0)
				{
					ChangeAnimation("PutSword");
				}
				else
				{
					DrawSword(false);
					ChangeAnimation("Idle1");
				}
			}
			else
				ChangeAnimation("Idle1");
			attackCount = 0;
		};
        idleState2.onActive = (State prev) => { ChangeAnimation("Idle2"); };
        idleState3.onActive = (State prev) => { ChangeAnimation("Idle3"); };
        moveState.onActive = (State prev) => {
			ChangeAnimation("Move");
			DrawSword(false);
			trackEffect.moveSmoke.Enable();
			attackCount = 0;
		};
		dodgeAttackState.onActive = (State prev) => {
			ChangeAnimation("DodgeAttack");
			DrawSword(true);
			extraMovingPoint = transform.forward + transform.position + (transform.forward * 5f + Vector3.up * .5f);
			trackEffect.dodgeMaehwa.Enable();
		};
		basicAttackState.onActive = (State prevState) => {
			if(prevState.Compare(basicAttackState))
				ChangeAnimation("Buffered Input Basic Attack");
			else {
				DrawSword(true);
				ChangeAnimation("Basic Attack");
			}
		};
		downAttackState.onActive = (State prev) => {
			ChangeAnimation("DownAttack");
			DrawSword(true);
			extraMovingPoint = transform.forward + transform.position + (transform.forward * 5f + Vector3.up * .5f);
			trackEffect.dodgeMaehwa.Enable();
		};
		dodgeState.onActive = (State prev) => {
			ChangeAnimation("Dodge");
			dodgeCoroutine = StartCoroutine(DodgeCoroutine());
			trackEffect.dodgeMaehwa.Enable();

			if(!prev.Compare("Idle")
			&& !prev.Compare(moveState)) {
				trackEffect.motionTrailEffect.GenerateTrail(motionTrailedMeshRenderers);
			}
			SoundManager.instance.tPlayer.effect.dodge.PlayOneShot(audioSourceEffect, ESoundType.effect);
		};
        downState.onActive = (State prev) => {
			ChangeAnimation("Down");
			isSuperArmor = true;
		};
        damageState.onActive = (State prev) => {
			ChangeAnimation("Damage");
		};
		chargingStart.onActive = (State prev) => {
			ChangeAnimation("Charging");
			chargingLevel = 0;
			chargingTime = 0;
			DrawSword(true);
			ui.SetChargingLevel(chargingLevel, chargingMaxTime[0]);
			if(!prev.Compare("Idle")
			&& !prev.Compare(moveState)) {
				trackEffect.motionTrailEffect.GenerateTrail(motionTrailedMeshRenderers);
			}
		};
		chargingStay.onActive = (State prev) => {
			ui.sliderCharging.gameObject.SetActive(true);
			ui.SetChargingValue(0);
		}; 
		chargingDrawSwordAttack.onActive = (State prev) => {
			ChangeAnimation("DrawSwordAttack");
		};
		chargingDrawSwordAttack_7time.onActive = (State prev) => {
			DrawSword(true);
			ChangeAnimation("Draw Sword Attack 7time");
			SoundManager.instance.tPlayer.voice.attack_combo_7.Play(audioSourceVoice, ESoundType.voice);
		};
		comboAttack_1.onActive = (State prev) => {
			DrawSword(true);
			ChangeAnimation("Combo Attack 01");
			SoundManager.instance.tPlayer.voice.attack_combo_6.Play(audioSourceVoice, ESoundType.voice);
		};
		chargingDrawSwordAttack_nonCharging.onActive = (State prev) => {
			DrawSword(true);
			ChangeAnimation("Draw Sword Attack Non Charging");
		}; 
		chargingDrawSwordAttack_2time.onActive = (State prev) => {
			DrawSword(true);
			ChangeAnimation("Draw Sword Attack 2time");
		}; 
		chargingDrawSwordAttack_specialStart.onActive = (State prev) => {
			ChangeAnimation("Draw Sword Attack Special Start");
			isImmune = true;
			DrawSword(true);
			lateDamageTarget.Clear();
			if(isLocalPlayer)
				CameraManager.instance.SwitchCameara(cutSceneCam.drawAttack[0]);
			SoundManager.instance.tPlayer.voice.drawAttackSpecialReady.Play(audioSourceVoice, ESoundType.voice);
			SoundManager.instance.tPlayer.effect.drawAttackSpecial_start.PlayOneShot(audioSourceEffect, ESoundType.effect);
		};
		chargingDrawSwordAttack_specialStay.onActive = (State prev) => {
			tPlayerMesh.SetActive(false);
			isCheckLateDamageTarget = true;
			StartCoroutine(DrawAttackSpecialCutScene());
			if (isLocalPlayer)
				CameraManager.instance.SwitchCameara(cutSceneCam.drawAttack[1]);
			SoundManager.instance.tPlayer.voice.drawAttackSpecialStart.Play(audioSourceVoice, ESoundType.voice);
			SoundManager.instance.tPlayer.effect.drawAttackSpecial_stay.PlayOneShot(audioSourceEffect, ESoundType.effect);
		};
		chargingDrawSwordAttack_specialEnd.onActive = (State prev) => {
            ChangeAnimation("Draw Sword Attack Special End");
			StartCoroutine(DrawAttackSpecialEnd());
			if (isLocalPlayer)
				CameraManager.instance.SwitchCameara(cutSceneCam.drawAttack[2]);
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
			ChangeSp(status.GetRecoverySp());
		};
        idleState2.onStay = () => {
			ChangeSp(status.GetRecoverySp());
		};
        idleState3.onStay = () => {
			ChangeSp(status.GetRecoverySp());
		};
        moveState.onStay = () => {
			RotateWithCamera();
			if (isRush)
			{
				if (status.sp > 0)
				{
					ChangeSp(-Time.deltaTime * 10f);
					movement.MoveToward(Vector3.forward * status.GetSpeed() * 2f * Time.deltaTime);
					return;
				}
				else
				{
					OnRushToRun();
				}
			}
			ChangeSp(status.GetRecoverySp());
			movement.MoveToward(Vector3.forward * ((isWalk) ? status.GetSpeed() / 2 : status.GetSpeed()) * Time.deltaTime);
		};
		dodgeAttackState.onStay = () => { MoveToTargetPos(); };
		downAttackState.onStay = () => { MoveToTargetPos(); };
		dodgeState.onStay = () => { };
        downState.onStay = () => {
			ChangeSp(status.GetRecoverySp());
		};
        damageState.onStay = () => {};
		chargingStart.onStay = () => {
			ChangeSp(status.GetRecoverySp());
		};
		chargingStay.onStay = () => {
			ChangeSp(status.GetRecoverySp() * 3f);
			if (chargingLevel < chargingMaxTime.Length)
			{
				chargingTime += Time.deltaTime;
				if (chargingTime >= chargingMaxTime[chargingLevel])
				{
					chargingLevel++;
					chargingTime = 0;
					if (chargingLevel < chargingMaxTime.Length)
						ui.SetChargingLevel(chargingLevel, chargingMaxTime[chargingLevel]);
					else {
						ui.SetChargingLevel(chargingLevel, 0); ;
					}
				}
				ui.SetChargingValue(chargingTime);
			}
		};
		basicAttackState.onStay = () => {
			ChangeSp(status.GetRecoverySp() * 0.5f);
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
		basicAttackState.onInactive = (State nextState) => {
			if(attackCoroutine != null)
				StopCoroutine(attackCoroutine);
		};
		dodgeAttackState.onInactive = (State next) => {  };
		downAttackState.onInactive = (State next) => {  };
        dodgeState.onInactive = (State next) => {
			isImmune = false;
			if (dodgeCoroutine != null) StopCoroutine(dodgeCoroutine);
			trackEffect.dodgeMaehwa.Disable();
		};
        downState.onInactive = (State next) => { isSuperArmor = false; };
        damageState.onInactive = (State next) => {
			if(damageCoroutine != null
			&& !next.Compare(damageState))
				StopCoroutine(damageCoroutine);
		};
		chargingStart.onInactive = (State next) => { };
		chargingStay.onInactive = (State next) => {
			ui.sliderCharging.gameObject.SetActive(false);
		};
		chargingDrawSwordAttack_nonCharging.onInactive = (State next) => {
			DrawSword(false);
		};
		chargingDrawSwordAttack_specialEnd.onInactive = (State next) => {
			if (isLocalPlayer)
				CameraManager.instance.SetPlayerCamera();
            isImmune = false;
			DrawSword(false);
		};
		chargingDrawSwordAttack_specialStay.onInactive = (State next) => {
			tPlayerMesh.SetActive(true);
			isCheckLateDamageTarget = false;
		};
	}
	#endregion Initialize

	#region Input Event
	public void InputMove(Vector3 moveVecterInput)
    {
        lookVector = moveVecterInput;

        if (stateMachine.Compare(basicAttackState) ||
            stateMachine.currentState == dodgeAttackState ||
            stateMachine.currentState == dodgeState ||
            stateMachine.currentState == downState ||
            stateMachine.currentState == damageState ||
			stateMachine.currentState == chargingStart ||
			stateMachine.currentState == chargingStay ||
			stateMachine.currentState == chargingDrawSwordAttack_7time ||
			stateMachine.currentState == chargingDrawSwordAttack_2time ||
			stateMachine.currentState == chargingDrawSwordAttack ||
			stateMachine.currentState == chargingDrawSwordAttack_nonCharging ||
			stateMachine.currentState == chargingDrawSwordAttack_specialStay ||
			stateMachine.currentState == chargingDrawSwordAttack_specialStart ||
			stateMachine.currentState == chargingDrawSwordAttack_specialEnd ||
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
		isAfterBasicAttack = false;
        if (lookVector == Vector3.zero)
            ChangeState(idleState1, false);
        else
            ChangeState(moveState, false);
    }
	[Command]
	public void OnDamage(Damage damage) {
		TakeDamage(damage);
	}
	[ClientRpc]
	private void TakeDamage(Damage damage)
	{
		if (isInvincibilityAndAction)
			invincibilityAndActionAmount += damage.amount;

		if (isImmune) // 무적이면 실행 안함
			return;

		damage.amount = shieldManager.UsingShield(damage.amount); // 실드가 있으면 데미지 차감

		status.hp -= damage.amount;
		ui.RefreshHp(status.hp / status.maxHp);

		if (damage.forceVector != Vector3.zero && damage.amount > 0) {
			LookDirection(-damage.forceVector);
		}

		if (stateMachine.Compare(downState)
		|| isSuperArmor
		|| damage.amount == 0
		)
			return;

		if (damageCoroutine != null)
			StopCoroutine(damageCoroutine);
		if(damage.damageType == Damage.DamageType.Normal)
			damageCoroutine = StartCoroutine(DamageCoroutine(damage));
		else
			OnDown(damage);
	}
	private IEnumerator DamageCoroutine(Damage damage) {
		float offset = 0f;
        Vector3 pusingDestination = Vector3.Scale(new Vector3(1, 0, 1), damage.forceVector);
		if(damage.hittingDuration > 0)
			ChangeState(damageState);
		while(offset < damage.hittingDuration) {
            movement.MoveToward(Vector3.Lerp(pusingDestination, Vector3.zero, offset*2) * Time.deltaTime, Space.World);
			offset += Time.deltaTime;
			yield return null;
		}
		ResetState();
	}
	public void OnDown(Damage damage) {
		ChangeState(downState, false); 
		if(downCoroutine != null)
			StopCoroutine(downCoroutine);
		downCoroutine = StartCoroutine(DownCoroutine(damage.forceVector));
	}
	private IEnumerator DownCoroutine(Vector3 forceVector) {
		if(forceVector != Vector3.zero) {
			float offset = 0;
			Vector3 pusingDestination = Vector3.Scale(new Vector3(1, 0, 1), forceVector);
			while(offset < 1) {
            	movement.MoveToward(Vector3.Lerp(pusingDestination, Vector3.zero, offset*2) * Time.deltaTime, Space.World);
				offset += Time.deltaTime;
				yield return null;
			}
		}
	}
    public void OnSlide()
    {
		if (!skill.dodge.CanUse()) return;

		if (stateMachine.currentState == dodgeAttackState) return; 

		if (stateMachine.currentState == downState)
		{
			OnDownAttack();
			return;
		}

		if (stateMachine.currentState == damageState ||
            stateMachine.currentState == downState ||
			stateMachine.currentState == dodgeState ||
			stateMachine.currentState == chargingDrawSwordAttack_7time ||
			stateMachine.currentState == chargingDrawSwordAttack_specialStay ||
			stateMachine.currentState == chargingDrawSwordAttack_specialStart ||
			stateMachine.currentState == chargingDrawSwordAttack_specialEnd ||
			stateMachine.currentState == comboAttack_1) return;
		if (lookVector != Vector3.zero) RotateWithCamera(10f); 
		ChangeState(dodgeState, false);
		skill.dodge.Use();
	}
    public void OnAttack()
    {
		if (!skill.basicAttacks[attackCount].CanUse())
			return;

		if (stateMachine.currentState == dodgeState)
		{
			OnDodgeAttack();
			return;
		}
        if (stateMachine.currentState == damageState
		|| stateMachine.currentState == dodgeState
		|| stateMachine.currentState == dodgeAttackState
		|| stateMachine.currentState == downState
		|| stateMachine.currentState == downAttackState
		|| stateMachine.currentState == chargingStart
		|| stateMachine.currentState == chargingStay
		|| stateMachine.currentState == chargingDrawSwordAttack_7time
		|| stateMachine.currentState == chargingDrawSwordAttack_2time
		|| stateMachine.currentState == chargingDrawSwordAttack
		|| stateMachine.currentState == chargingDrawSwordAttack_nonCharging
		|| stateMachine.currentState == chargingDrawSwordAttack_specialStay
		|| stateMachine.currentState == chargingDrawSwordAttack_specialStart
		|| stateMachine.currentState == chargingDrawSwordAttack_specialEnd
		|| stateMachine.currentState == comboAttack_1)
			return;

		nextAttackDirection = lookVector;

		extraMovingPoint = transform.forward + transform.position + (transform.forward * 1f + Vector3.up * 0.5f);
		ChangeState(basicAttackState, true);
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
			stateMachine.currentState == dodgeState ||
			stateMachine.currentState == damageState ||
			stateMachine.currentState == chargingDrawSwordAttack_7time ||
			stateMachine.currentState == chargingDrawSwordAttack_2time ||
			stateMachine.currentState == chargingDrawSwordAttack ||
			stateMachine.currentState == chargingDrawSwordAttack_nonCharging ||
			stateMachine.currentState == chargingDrawSwordAttack_specialStay ||
			stateMachine.currentState == chargingDrawSwordAttack_specialStart ||
			stateMachine.currentState == chargingDrawSwordAttack_specialEnd ||
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
			if (lookVector != Vector3.zero) RotateWithCamera(15f);
			ChangeState(chargingDrawSwordAttack_nonCharging);
			return;
		}
		if (stateMachine.currentState != chargingStay) return;
		if (lookVector != Vector3.zero) RotateWithCamera(15f);
		switch (chargingLevel)
		{
			case 0: 
				ChangeState(chargingDrawSwordAttack_nonCharging);
				break;
			case 1: ChangeState(chargingDrawSwordAttack); break;
			case 2: ChangeState(chargingDrawSwordAttack_2time); break;
			case 3: ChangeState(chargingDrawSwordAttack_specialStart); break;
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
			stateMachine.currentState == chargingDrawSwordAttack_2time ||
			stateMachine.currentState == chargingDrawSwordAttack_nonCharging ||
			stateMachine.currentState == chargingDrawSwordAttack ||
			stateMachine.currentState == chargingDrawSwordAttack_specialStay ||
			stateMachine.currentState == chargingDrawSwordAttack_specialStart ||
			stateMachine.currentState == chargingDrawSwordAttack_specialEnd ||
			stateMachine.currentState == comboAttack_1)
		{
			return;
		}
		if (skill.combo_1[0].CanUse())
		{
			ChangeState(comboAttack_1, false);
		}
	}
	public void OnDrawSwordAttack7time()
	{
		if (stateMachine.currentState == dodgeAttackState ||
			stateMachine.currentState == downAttackState ||
			stateMachine.currentState == downAttackState ||
			stateMachine.currentState == downAttackState ||
			stateMachine.currentState == damageState ||
			stateMachine.currentState == chargingDrawSwordAttack_7time ||
			stateMachine.currentState == chargingDrawSwordAttack_2time ||
			stateMachine.currentState == chargingDrawSwordAttack_nonCharging ||
			stateMachine.currentState == chargingDrawSwordAttack ||
			stateMachine.currentState == chargingDrawSwordAttack_specialStay ||
			stateMachine.currentState == chargingDrawSwordAttack_specialStart ||
			stateMachine.currentState == chargingDrawSwordAttack_specialEnd ||
			stateMachine.currentState == comboAttack_1)
		{
			return;
		}
		if (skill.charging_DrawSwordAttack_7time[0].CanUse())
		{
			ChangeState(chargingDrawSwordAttack_7time, false);
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
	public void AnimationEvent_StartAttack() {
        if (nextAttackDirection != Vector3.zero)
			RotateWithCamera(15f);
	}
	public void CheckAttackZone()
	{
		attackCount = (attackCount >= skill.basicAttacks.Length) ? 0 : attackCount;
		skill.basicAttacks[attackCount].Use();
		if (isMotionTrail) 
		{
			TPlayerMotionTrail motionTrail = trackEffect.GetMotionTrail();
			motionTrail.transform.position = transform.position;
			motionTrail.transform.rotation = transform.rotation;
			TPlayerSkill currSkill = skill.basicAttacks[attackCount] as TPlayerSkill;
			motionTrail.SetAction(attackCount, currSkill.GetEffectKey());
		}
		attackCount++;
		attackCount = (attackCount >= skill.basicAttacks.Length) ? 0 : attackCount;
		SoundManager.instance.tPlayer.effect.slash_01.PlayOneShot(audioSourceEffect, ESoundType.effect);
		SoundManager.instance.tPlayer.voice.AttackRandom(audioSourceVoice, 60f);
		if (attackCoroutine != null)
			StopCoroutine(attackCoroutine);
		attackCoroutine = StartCoroutine(AttackCoroutine());
	}
	public void CheckDrawSwordAttack_7time(int idx) 
	{
		skill.charging_DrawSwordAttack_7time[idx].Use();
		SoundManager.instance.tPlayer.effect.slash_01.PlayOneShot(audioSourceEffect, ESoundType.effect);
		if (idx != 0) return;
		if (attackCoroutine != null)
			StopCoroutine(attackCoroutine);
		attackCoroutine = StartCoroutine(AttackCoroutine(1.5f));
	}
	public void CheckDodgeAttackZone()
	{
		skill.dodgeAttack.Use();
		trackEffect.dodgeMaehwa.Disable();
		SoundManager.instance.tPlayer.effect.slash_01.PlayOneShot(audioSourceEffect, ESoundType.effect);
		SoundManager.instance.tPlayer.voice.AttackRandom(audioSourceVoice, 60f);
	}
	public void CheckDownAttackZone()
	{
		skill.downAttack.Use();
		trackEffect.dodgeMaehwa.Disable();
		SoundManager.instance.tPlayer.effect.slash_01.PlayOneShot(audioSourceEffect, ESoundType.effect);
		SoundManager.instance.tPlayer.voice.AttackRandom(audioSourceVoice, 60f);
	}
	public void ChackDrawSwordAttack(int sound)
	{
		skill.charging_DrawSwordAttack.Use();
		SoundManager.instance.tPlayer.effect.slash_01.PlayOneShot(audioSourceEffect, ESoundType.effect);
		SoundManager.instance.tPlayer.voice.AttackRandom(audioSourceVoice, 70f);
		if (attackCoroutine != null)
			StopCoroutine(attackCoroutine);
		attackCoroutine = StartCoroutine(AttackCoroutine(3f));
	}
	public void ChackDrawSwordAttackNonCharging()
	{
		skill.charging_DrawSwordAttack_nonCharging.Use();
		SoundManager.instance.tPlayer.effect.slash_02.PlayOneShot(audioSourceEffect, ESoundType.effect);
	}
	public void CheckComboAttack_1(int idx)
	{
		skill.combo_1[idx].Use();
		SoundManager.instance.tPlayer.effect.slash_01.PlayOneShot(audioSourceEffect, ESoundType.effect);
		if (attackCoroutine != null)
			StopCoroutine(attackCoroutine);
		attackCoroutine = StartCoroutine(AttackCoroutine(1.5f));
	}
	public void RunDrawAttackSpecial() 
	{
		stateMachine.ChangeState(chargingDrawSwordAttack_specialStay);
	}
	public void CheckDrawAttackSpecial()
	{
		StartCoroutine(DrawAttackDamage());
	}
	public void OnFootStepClip() 
	{
		SoundManager.instance.tPlayer.effect.footStep.PlayOneShot(audioSourceEffect, ESoundType.effect);
	}
	public void PutSword() 
	{
		if (stateMachine.currentState == idleState1)
		{
			ChangeAnimation("Idle1");
			DrawSword(false);
		}
	}
	#endregion Animation Event

	#region public method
	public void AddAttachment(Attachment attachment) {
		attachmentManager.AddAttachment(attachment);
	}
	public void AddShield(TPlayerShield shield)
	{
		shieldManager.AddShield(shield);
		trackEffect.shield.Enable();
	}
	public void RemoveShield(TPlayerShield shield)
	{
		shieldManager.RemoveShield(shield);
		if (shieldManager.GetCount() == 0)
		{
			trackEffect.shield.Disable();
		}
	}
	public void ChangeHp(float amount)
	{
		status.hp += amount;
		if (status.hp > status.maxHp)
		{
			status.hp = status.maxHp;
		}
		ui.RefreshHp(status.hp);
	}
	public void ChangeSp(float amount)
	{
		status.sp += amount;
		if (status.sp > status.maxSp)
		{
			status.sp = status.maxSp;
		}
		ui.ReFreshStamina(status.sp);
	}
	public float GetAp() 
	{
		return status.GetLastAp();
	}
	public void SetActiveInvincibilityAndAction(bool active, EInvincibilityAndActionType type) 
	{
		if (active)
		{
			isInvincibilityAndAction = true;
			invincibilityAndActionAmount = 0;
		}
		else
		{
			isInvincibilityAndAction = false;
			switch (type)
			{
				case EInvincibilityAndActionType.Counterattack:
					skill.counterattack.Use(invincibilityAndActionAmount);
					break;
				case EInvincibilityAndActionType.Healing:
					ChangeHp(invincibilityAndActionAmount);
					break;
			}
		}
	}
	public void SetACtionMotionTrail(bool active) 
	{
		isMotionTrail = active;
	}
	#endregion public method

	#region private method
	private void RotateWithCamera(float rotateSpeed = 1f)
	{
		Vector3 lookTarget = Quaternion.AngleAxis(Camera.main.transform.eulerAngles.y, Vector3.up) * lookVector;
        Vector3 look = Vector3.Slerp(transform.forward, lookTarget.normalized, this.rotateSpeed * rotateSpeed * Time.deltaTime);
		transform.eulerAngles = new Vector3(0, Quaternion.LookRotation(look).eulerAngles.y, 0);
	}
	private void LookDirection(Vector3 target) {
		Vector3 ylessTargetPos = new Vector3(target.x, 0, target.z);
		transform.LookAt(transform.position + ylessTargetPos);
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
	private void DrawSword(bool use) 
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
	private IEnumerator SmoothConvert(bool fade)
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
	private IEnumerator SmoothConvertRush(bool fade)
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
	private IEnumerator DodgeCoroutine()
	{
		float offset = 0;
		Vector3 targetPoint = Vector3.forward;
		
		while (offset < 1) {
			offset += Time.deltaTime * 2.5f;
			Vector3 d = Vector3.Lerp(Vector3.zero, targetPoint, Mathf.Sin(Mathf.PI * .5f + offset * .5f * Mathf.PI));
			d.y = 0;
			movement.MoveToward(d * Time.deltaTime * 16f);

			yield return null;
		}
	}
	private IEnumerator AttackCoroutine(float speed = 1f)
	{
		float offset = 0;
		float forceCoef = .5f + (lookVector==Vector3.zero ? 0 : 1);
		Vector3 targetPoint = transform.position + transform.forward * forceCoef * speed;
		while (offset < 1)
		{
			offset += Time.deltaTime * 6f;
			targetPoint.y = transform.position.y;
			Vector3 dir = Vector3.Lerp(transform.position, targetPoint, Time.deltaTime * 10f) - transform.position;
			dir.y = 0;
			movement.MoveToward(dir, Space.World, 1<<6 | 1<<8 | 1<<11); 
			yield return null;
		}
	}
	private IEnumerator DrawAttackSpecialCutScene()
	{
		float numTime = 0;
		float colorA = 1f;
		Vector3 targetPoint = Vector3.forward;
		ui.imgCutSceneSwordTrail.gameObject.SetActive(true);
		ui.imgCutSceneSwordTrail.rectTransform.localScale = new Vector3(0.5f, 0.5f);
		ui.imgCutSceneSwordTrail.color = new Color(1f, 1f, 1f, 1f);
		while (ui.imgCutSceneSwordTrail.rectTransform.localScale.y < 21f)
		{
			numTime += Time.deltaTime * 2f;
			float s = 1f - Mathf.Cos(numTime);
			ui.imgCutSceneSwordTrail.rectTransform.localScale += new Vector3(s, s * s);

			Vector3 d = Vector3.Lerp(Vector3.zero, targetPoint, Mathf.Sin(Mathf.PI * .5f + numTime * .5f * Mathf.PI));
			d.y = 0;
			movement.MoveToward(d * Time.deltaTime * 80f);
			yield return null;
		}
		stateMachine.ChangeState(chargingDrawSwordAttack_specialEnd);
		while (ui.imgCutSceneSwordTrail.color.a != 0)
		{
			colorA = (colorA > 0) ? colorA - Time.deltaTime : 0;
			ui.imgCutSceneSwordTrail.color = new Color(1f, 1f, 1f, colorA);
			yield return null;
		}
		ui.imgCutSceneSwordTrail.gameObject.SetActive(false);
	}
	private IEnumerator DrawAttackDamage()
	{
		SoundManager.instance.tPlayer.effect.drawAttackSpecial_end.PlayOneShot(audioSourceEffect, ESoundType.effect);
		for (int i = 0; i < 10; i++)
		{
			Damage damage = new Damage(
				GetAp() * 0.15f,
				.5f,
				Vector3.zero,
				Damage.DamageType.Normal
			) ;
			lateDamageCntl.OnDamage(lateDamageTarget, damage, EHitEffectType.slash_1);
			yield return new WaitForSeconds(0.1f);
		}
		yield return new WaitForSeconds(0.3f);
		ResetState();
	}
	private IEnumerator DrawAttackSpecialEnd()
	{
		ChangeAnimation("Draw Sword Attack Special End Stay");
		yield return new WaitForSeconds(0.3f);
		if (lateDamageTarget.Count != 0) {
			Damage damage = new Damage(
				GetAp() * 2f,
				3f,
				Vector3.zero,
				Damage.DamageType.Normal
			);
			lateDamageCntl.OnDamage(lateDamageTarget, damage, EHitEffectType.slash_1);
			yield return new WaitForSeconds(0.5f);
			ChangeAnimation("Draw Sword Attack Special End");
			SoundManager.instance.tPlayer.voice.drawAttackSpecialEnd.Play(audioSourceVoice, ESoundType.voice);
		} 
		else{
			SoundManager.instance.tPlayer.voice.drawAttackSpecialEnd_Miss.Play(audioSourceVoice, ESoundType.voice);
			yield return new WaitForSeconds(1f);
			ResetState();
		}
	}
	private void OnTriggerEnter(Collider other)
	{
		if (isCheckLateDamageTarget)
		{
			IDamageable checkNull = other.GetComponent<IDamageable>();
			if (checkNull != null)
			{
				lateDamageTarget.Add(other.transform);
			}
		}
	}
	#endregion private method
}
public enum EInvincibilityAndActionType 
{
	None, Counterattack, Healing
}
[Serializable]
public class PlayerStatus
{
	public float speed = 3f;      	 // 이동속도
	public float hp = 100f;    		 // 체력
	public float sp = 100f;     	 // 스테미너 ** 시작하면서 set 하는거 어떤지?
	public float ap = 10f;     		 // 공격력
	public float maxHp = 100f;   	 // 최대 체력
	public float maxSp = 100f;    	 // 최대 스테미너
	public float speedBoost = 1f; 	 // 이동속도 상승량
	public float recoveryHp = 1f; 	 // 체력 회복 속도 상승량
	public float recoverySp = 1f; 	 // 스테미나 회복 속도 상승량
	public float apBoost = 1f;	   	 // 공격력 상승량
	public float GetLastAp() 
	{
		return ap * apBoost;
	}
	public float GetRecoverySp() 
	{
		return Time.deltaTime * 20f * recoverySp;
	}
	public float GetSpeed()
	{
		return speed * speedBoost;
	}
}
[Serializable]
class TPlayerWeaponTransform
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
	public Skill[] basicAttacks;
	public Skill dodge;
	public Skill dodgeAttack;
	public Skill downAttack;
	public Skill charging_DrawSwordAttack;
	public Skill charging_DrawSwordAttack_nonCharging;
	public Skill[] charging_DrawSwordAttack_7time;
	public Skill[] combo_1;
	public Skill counterattack;
}
[Serializable]
class TPlayerCutSceneCam
{
	public CinemachineVirtualCamera[] drawAttack;
}
[Serializable]
class TPlayerTrackEffect
{
	public Effect_MotionTrail motionTrailEffect;
	public TrackEffect dodgeMaehwa;
	public TrackEffect moveSmoke;
	public TrackEffect shield;
	public GameObject motionTrailPrefab;
	[HideInInspector] public string motionTrailKey;
	public void Set() 
	{
		dodgeMaehwa.Enable();
		dodgeMaehwa.Disable();
		moveSmoke.Disable();
		moveSmoke.Enable();
		shield.Enable();
		shield.Disable();
		motionTrailKey = motionTrailPrefab.GetComponent<IPoolableObject>().GetKey();
		PoolerManager.instance.InsertPooler(motionTrailKey, motionTrailPrefab, false);
	}
	public TPlayerMotionTrail GetMotionTrail() 
	{
		GameObject motionTrail = PoolerManager.instance.OutPool(motionTrailKey);
		return motionTrail.GetComponent<TPlayerMotionTrail>();
	}
}
[Serializable]
class TPayerUI 
{
	public GameObject canvas;
	public Slider sliderHP;
	public Slider sliderSP;
	public Slider sliderCharging;
	public Image imgCharging;
	public Image imgChargingBackground;
	public Image imgCutSceneSwordTrail;
	private Color red = new Color(1f, 0f, 0f);
	private Color orange = new Color(1f, 0.65f, 0f);
	private Color yellow = new Color(1f, 1f, 0f);
	private Color darkYellow = new Color(0.8f, 0.8f, 0f);
	public void InitializeUI() 
	{
		PlayerStatus status = TPlayer.instance.status;
		sliderHP.maxValue = status.maxHp;
		sliderSP.maxValue = status.maxSp;
		sliderHP.value = status.hp;
		sliderSP.value = status.sp;
		sliderCharging.gameObject.SetActive(false);
		imgCutSceneSwordTrail.gameObject.SetActive(false);
	}
	public void SetChargingLevel(int level, float maxValue) 
	{
		sliderCharging.maxValue = maxValue;
		switch (level)
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
	public void SetChargingValue(float value)
	{
		sliderCharging.value = value;
	}
	public void RefreshHp(float value) 
	{
		sliderHP.value = value;
	}
	public void ReFreshStamina(float value)
	{
		sliderSP.value = value;
	}
}
class TPlayerShieldManager
{
	private List<TPlayerShield> shields = new List<TPlayerShield>();
	public void AddShield(TPlayerShield shield)
	{
		shields.Add(shield);
	}
	public void RemoveShield(TPlayerShield shield)
	{
		shields.Remove(shield);
	}
	public float UsingShield(float damage)
	{
		float lastDamage = damage;
		for (int i = 0; i < shields.Count; i++)
		{
			if (shields[i].amount != 0)
			{
				if (shields[i].amount >= lastDamage)
				{
					shields[i].amount -= lastDamage;
					lastDamage = 0;
					break;
				}
				else
				{
					lastDamage -= shields[i].amount;
					shields[i].amount = 0;
				}
			}
		}
		return lastDamage;
	}
	public int GetCount() { return shields.Count; }
}
public class TPlayerShield
{
	public float amount;
	public TPlayerShield(float amount)
	{
		this.amount = amount;
	}
}