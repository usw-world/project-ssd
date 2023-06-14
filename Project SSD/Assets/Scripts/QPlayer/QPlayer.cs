using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;
using Mirror;
using UnityEngine.Rendering.Universal;
using Cinemachine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(CapsuleCollider))]
[RequireComponent(typeof(StateMachine))]
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(NavigateableMovement))]
public class QPlayer : NetworkBehaviour
{
    public static QPlayer instance { get; private set; }

    public GameObject tPlayerGobj;

    #region Q-Player Status
    public PlayerStatus status;
    private float qPlayerSp;
    bool canAttack = true;
    bool isCanMove = true;
    #endregion Q-Player Status

    #region Movement
    NavigateableMovement movement;
    Vector3 movingDestination;
    #endregion Movement
    
    #region State Machine
    StateMachine stateMachine;
    Dictionary<string, State> statesMap = new Dictionary<string, State>();
    Dictionary<Skill, State> statesSkillMap = new Dictionary<Skill, State>();

    State attachedState = new State("Attached");
    State separatedState = new State("Separated");
    State returnState = new State("Return");
    State moveState = new State("Move");
    State unityBallState = new State("Unity Ball");
    State aoeState = new State("Aoe");
    State flagitState = new State("flagitState");
    State lightningState = new State("lightningState");
    State shieldState = new State("shieldState");
    State fightGhostFistState = new State("fightGhostFistState");
    State fightGhostFistStayState = new State("fightGhostFistStayState");
	State finishSkillState = new State("finishSkillState");
	State finishSkillRush = new State("finishSkillRush");
	State finishSkillRushStay = new State("finishSkillRushStay");
	State BufferingState = new State("BufferingState");
	State prevState;
    string currentAnimationTrigger = "";
    #endregion State Machine

    #region Stamina
    [SerializeField] private float stamina;
    [SerializeField] private float maxStamina = 10f;
    #endregion Stamina

    #region Animation
    Animator animator;
    const string IDLE_ANIMATION_PARAMETER = "Idle";
    const string FLY_ANIMATION_PARAMETER = "Fly";
    const string OneHandCasting_ANIMATION_PARAMETER = "1H Casting";
    #endregion Animation

    public GameObject qPlayerCamera;
    [SerializeField] private CollisionEventHandler fightGhostFistZone;
	[SerializeField] private List<GameObject> finishSkillCamera = new List<GameObject>();
	[SerializeField] private List<GameObject> finishSkillEffect = new List<GameObject>();
	private Coroutine fightGhostFistCoroutine;
    [SerializeField] private GameObject mesh;
    [SerializeField] private GameObject mesh2;
    [SerializeField] private GameObject rushTril;
    [SerializeField] private AudioSource audioSourceEffect;
    [SerializeField] private AudioSource audioSourceVoice;
    [SerializeField] private AudioSource audioSourceBgm;
    #region Skill
    public List<Skill> skills;
    private Skill usingSkill;
    private Vector3 targetPoint;

    private int aimingSkillIndex = -1;
    private Skill AimingSkill {
        get {
            if(aimingSkillIndex>=0)
                return skills[aimingSkillIndex];
            else
                return null;
        }
    }

    public DecalProjector skillDecalArrow;
    public DecalProjector skillDecalArea;
    bool isAiming = false;
    #endregion Skill

    public override void OnStartLocalPlayer() {
        base.OnStartLocalPlayer();
    }

    private void Awake() {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject); 
        DontDestroyOnLoad(gameObject);

        if (!isLocalPlayer)

        animator = GetComponent<Animator>();
        movement = GetComponent<NavigateableMovement>(); 
        stateMachine = GetComponent<StateMachine>();
        //Cursor.lockState = CursorLockMode.Locked;

        stateMachine.SetIntialState(attachedState);
		Application.targetFrameRate = 60;
    }
    void Start() {
        tPlayerGobj = TPlayer.instance?.gameObject ?? GameObject.FindGameObjectWithTag("TPlayer");
        stamina = maxStamina;

        InitializeCamera();
        InitializedState();
    }
    private void InitializedState() {
        #region Register States
        
        statesMap.Add(BufferingState.stateName, BufferingState);
        statesMap.Add(attachedState.stateName, attachedState);
        statesMap.Add(separatedState.stateName, separatedState);
        statesMap.Add(returnState.stateName, returnState);
        statesMap.Add(moveState.stateName, moveState);
        statesMap.Add(unityBallState.stateName, unityBallState);
        statesMap.Add(aoeState.stateName, aoeState);
        statesMap.Add(flagitState.stateName, flagitState);
        statesMap.Add(lightningState.stateName, lightningState);
        statesMap.Add(shieldState.stateName, shieldState);
        statesMap.Add(fightGhostFistState.stateName, fightGhostFistState);
        statesMap.Add(fightGhostFistStayState.stateName, fightGhostFistStayState);
        statesMap.Add(finishSkillState.stateName, finishSkillState);
        statesMap.Add(finishSkillRush.stateName, finishSkillRush);
        statesMap.Add(finishSkillRushStay.stateName, finishSkillRushStay);
        #endregion Register States

        #region Attached State
        attachedState.onActive += (State prevState) => {
            movement.Stop();
            canAttack = true;
            movement.enabled = false;
            ChangeAnimation(FLY_ANIMATION_PARAMETER);
        };
        attachedState.onStay += () => {
            Vector3 targetPos = tPlayerGobj.transform.position;
            float returnSpeed = 10.0f * Time.deltaTime;
            targetPos.y += 2;
            targetPos.x -= 1;
            Vector3 temp = Vector3.zero;
            transform.position = Vector3.SmoothDamp(transform.position, targetPos, ref temp, returnSpeed);
        };
        attachedState.onInactive += (State nextState) => { };
        #endregion Attached State

        #region Return State
        returnState.onActive += (State prevState) => {
            movement.Stop();
            movement.enabled = false;
            ChangeAnimation(FLY_ANIMATION_PARAMETER);
        };
        returnState.onStay = () =>
        {
            Vector3 targetPos = tPlayerGobj.transform.position;
            float returnSpeed = 10.0f * Time.deltaTime;
            targetPos.y += 2;
            targetPos.x -= 1;
            Vector3 temp = Vector3.zero;
            transform.position = Vector3.SmoothDamp(transform.position, targetPos, ref temp, returnSpeed);
        };
        returnState.onInactive += (State nextState) => { };
        #endregion Return State

        #region Separated State
        separatedState.onActive += (State prevState) => {
            movement.enabled = true;
            canAttack = true;
            ChangeAnimation(IDLE_ANIMATION_PARAMETER);
        };
        separatedState.onStay += () => {
            if (!movement.isArrive)
                ChangeState(moveState, false);
        };
        separatedState.onInactive += (State nextState) => { };
        #endregion Separated State

        #region Move State
        moveState.onActive += (State prevState) => {
            movement.enabled = true;
            canAttack = true;
            ChangeAnimation(FLY_ANIMATION_PARAMETER);
        };
        moveState.onStay += () => {
            if (movement.isArrive)
                ChangeState(separatedState, false);
        };
        moveState.onInactive += (State nextState) => {
            movement.Stop();
        };
        #endregion Move State

        #region UnityBall State
        unityBallState.onActive = (State prevState) =>
        {
            transform.LookAt(targetPoint);
            Vector3 qPlayerRot = transform.eulerAngles;
            qPlayerRot.x = 0;
            qPlayerRot.z = 0;
            transform.eulerAngles = qPlayerRot;
            canAttack = false;
            usingSkill = skills[0];

            string animationParameter = "1H Casting";

			QPlayerSkillUnityBall skillUnityball = usingSkill as QPlayerSkillUnityBall;
			if (skillUnityball != null && skillUnityball.options[7].active)
				animationParameter = "2H Casting";
			ChangeAnimation(animationParameter);
        };
        unityBallState.onStay = () => { };
        unityBallState.onInactive = (State nextState) => { canAttack = true; };
        #endregion UnityBall State

        #region Aoe State

        aoeState.onActive = (State prevState) =>
        {
            transform.LookAt(targetPoint);
            Vector3 qPlayerRot = transform.eulerAngles;
            qPlayerRot.x = 0;
            qPlayerRot.z = 0;
            transform.eulerAngles = qPlayerRot;
            canAttack = false;
            usingSkill = skills[1];
            ChangeAnimation("1H Casting");
        };
        aoeState.onStay = () =>
        {
        };
        aoeState.onInactive = (State nextState) =>
        {
            canAttack = true;
        };
		#endregion  Aoe State

		#region Flagit State
		flagitState.onActive = (State prevState) =>
		{
			transform.LookAt(targetPoint);
			Vector3 qPlayerRot = transform.eulerAngles;
			qPlayerRot.x = 0;
			qPlayerRot.z = 0;
			transform.eulerAngles = qPlayerRot;
			canAttack = false;
			usingSkill = skills[4];

			string animationParameter = "1H Casting";

			ChangeAnimation(animationParameter);
		};
		flagitState.onStay = () => { };
		flagitState.onInactive = (State nextState) => { canAttack = true; };
		#endregion Flagit State

		#region Lightning State
		lightningState.onActive = (State prevState) =>
		{
            transform.LookAt(targetPoint);
			Vector3 qPlayerRot = transform.eulerAngles;
			qPlayerRot.x = 0;
			qPlayerRot.z = 0;
			transform.eulerAngles = qPlayerRot;
			canAttack = false;
            isCanMove = false;
			usingSkill = skills[5];
			QPlayerSkillLightning lightning = usingSkill as QPlayerSkillLightning;

			string animationParameter = "2H Casting";

			if (lightning.options[7].active)
			{
				animationParameter = "2H Stay"; 
			}
			if (lightning.options[1].active)
			{
				animationParameter += "Fast";
			}
			
			ChangeAnimation(animationParameter);
		};
		lightningState.onStay = () => { };
		lightningState.onInactive = (State nextState) => { 
            canAttack = true;
            isCanMove = true;
        };
		#endregion Lightning State

		#region Shield State
		shieldState.onActive = (State prevState) =>
		{
			transform.LookAt(TPlayer.instance.transform);
			Vector3 qPlayerRot = transform.eulerAngles;
			qPlayerRot.x = 0;
			qPlayerRot.z = 0;
			transform.eulerAngles = qPlayerRot;
			canAttack = false;
			usingSkill = skills[3];
			string animationParameter = "1H Casting";
			ChangeAnimation(animationParameter);
		};
		shieldState.onStay = () => { };
		shieldState.onInactive = (State nextState) => { canAttack = true; };
		#endregion Shield State

		#region FightGhostFist State
		fightGhostFistState.onActive = (State prevState) =>
		{
			transform.LookAt(targetPoint);
			Vector3 qPlayerRot = transform.eulerAngles;
			qPlayerRot.x = 0;
			qPlayerRot.z = 0;
			transform.eulerAngles = qPlayerRot;
			canAttack = false;
			isCanMove = false;
			usingSkill = skills[6];
			QPlayerSkillFightGhostFist fightGhostFist = usingSkill as QPlayerSkillFightGhostFist;

			float damageAmount = GetAP() * fightGhostFist.GetSkillPower();

			string animationParameter = "1H Stay";
			if (fightGhostFist.options[3].active)
				animationParameter += "Fast"; 
			ChangeAnimation(animationParameter);

            //fightGhostFist.GetEffect().transform.parent = fightGhostFistZone.transform;

            //fightGhostFistZone.gameObject.SetActive(true);

            ParticleSystem.EmissionModule psEmission = fightGhostFistZone.GetComponent<ParticleSystem>().emission;
            psEmission.rateOverDistance = 5;

            if (fightGhostFist.options[2].active)
                fightGhostFistZone.transform.localScale = Vector3.one * 4f;
			else
                fightGhostFistZone.transform.localScale = Vector3.one * 1.5f;

			List<IDamageable> damageEnemy = new List<IDamageable>();
			fightGhostFistZone.onTriggerEnter = (target) => {
				if (target.gameObject.layer == 8)
				{
					IDamageable enemy = target.GetComponent<IDamageable>();
					if (damageEnemy.Contains(enemy) == false)
					{
						damageEnemy.Add(enemy);
						
						Damage.DamageType type = Damage.DamageType.Normal;
						if (fightGhostFist.options[0].active) damageAmount = damageAmount * 1.5f;
						if (fightGhostFist.options[1].active) type = Damage.DamageType.Down;
						 Damage damage = new Damage(
							damageAmount,
							1f,
							(target.transform.position - transform.position).normalized * 5f,
							type
						);
						enemy.OnDamage(damage);
						if (fightGhostFist.options[5].active)
						{
							Effect_FightGhostFistExplosion explosion = fightGhostFist.GetExplosion();
							explosion.transform.position = target.transform.position;
							explosion.Run(damageAmount * 0.5f);
						}
						if (fightGhostFist.options[4].active == false)
							OnFightGhostFist(false);
					}
				}
				if (fightGhostFist.options[6].active && target.gameObject == TPlayer.instance.gameObject)
				{
					bool isActiveShield = true;
					TPlayerShield shield = new TPlayerShield(damageAmount);
					Attachment attachment = new Attachment(3f, 0.1f, null, EAttachmentType.shield);
					attachment.onAction = (target) => {
						TPlayer.instance.AddShield(shield);
					};
					attachment.onStay = (target) => {
						if (shield.amount == 0 && isActiveShield)
						{
							isActiveShield = false;
							TPlayer.instance.RemoveShield(shield);
						}
					};
					attachment.onInactive = (target) => {
						if(isActiveShield)
							TPlayer.instance.RemoveShield(shield);
					};
					TPlayer.instance.AddAttachment(attachment);
					if (fightGhostFist.options[4].active == false)
						OnFightGhostFist(false);
				}
			};
			/*
				 @ 0. 데미지 증가         
				 @ 1. 경직 -> 다운
				 @ 2. 폭 증가            
				 @ 3. 시전속도 증가
				 @ 4. 충돌해도 계속 직진      
				 @ 5. 폭발
				 @ 6. TPlayer 충돌 가능, 실드   
			 */
		};
		fightGhostFistState.onStay = () => { };
		fightGhostFistState.onInactive = (State nextState) => { };
		#endregion FightGhostFist State

		#region fightGhostFistStay State
		fightGhostFistStayState.onActive = (State prevState) =>
		{
			if (fightGhostFistCoroutine != null) StopCoroutine(fightGhostFistCoroutine);
			fightGhostFistCoroutine = StartCoroutine(TimeOutFightGhostFist());
		};
		fightGhostFistStayState.onStay = () => {
			movement.MoveToward(Vector3.forward * Time.deltaTime * 30f);
		};
		fightGhostFistStayState.onInactive = (State nextState) => { 
            canAttack = true;
            isCanMove = true;
            fightGhostFistZone.onTriggerEnter = null;
            ParticleSystem.EmissionModule psEmission = fightGhostFistZone.GetComponent<ParticleSystem>().emission;
            psEmission.rateOverDistance = 0;
            //fightGhostFistZone.gameObject.SetActive(false);
        };
		#endregion FightGhostFistStay State
		
		#region Buffering State
		
		 BufferingState.onActive = prevState =>
		{
			transform.LookAt(targetPoint);
			Vector3 qPlayerRot = transform.eulerAngles;
			qPlayerRot.x = 0;
			qPlayerRot.z = 0;
			transform.eulerAngles = qPlayerRot;
			canAttack = false;
			isAiming = false;
			usingSkill = skills[2];
			string animationParameter = "1H Casting";
			ChangeAnimation(animationParameter);
			
		};
		BufferingState.onStay = () => { };
		BufferingState.onInactive = (nextState) => { canAttack = true; };
		
		#endregion
		
		#region FinishSkillState State
		finishSkillState.onActive = (State prevState) =>{
			transform.LookAt(targetPoint);
			Vector3 qPlayerRot = transform.eulerAngles;
			qPlayerRot.x = 0;
			qPlayerRot.z = 0;
			transform.eulerAngles = qPlayerRot;
			canAttack = false;
            isCanMove = false;
            ChangeAnimation("finish attack");
		};
		finishSkillState.onStay = () => { };
		finishSkillState.onInactive = (State nextState) => {
		};
        #endregion FinishSkillState State

        #region FinishSkillRush State
        finishSkillRush.onActive = (State prevState) => {
            StartCoroutine(Cut3CameraPosContrl());
        };
        finishSkillRush.onStay = () => {  };
        finishSkillRush.onInactive = (State nextState) => {  };
        #endregion FinishSkillRush State

        #region FinishSkillRushStay State
        finishSkillRushStay.onActive = (State prevState) => {
            //movement.MoveToPoint(targetPoint, 15f);
            //StartCoroutine(FinishSkillRushMove());
            ChangeAnimation("last rush");
        };
        finishSkillRushStay.onStay = () => {
        };
        finishSkillRushStay.onInactive = (State nextState) => {
            canAttack = true;
            isCanMove = true;
        };
		#endregion FinishSkillRushStay State

		statesSkillMap.Add(skills[0], unityBallState);
        statesSkillMap.Add(skills[1], aoeState); 
        statesSkillMap.Add(skills[2], BufferingState);
		statesSkillMap.Add(skills[3], shieldState);
		statesSkillMap.Add(skills[4], flagitState);
		statesSkillMap.Add(skills[5], lightningState);
		statesSkillMap.Add(skills[6], fightGhostFistState);
		statesSkillMap.Add(skills[7], finishSkillState);
	}
    public void FinishSkillCutSceneControl(int idx)
    {
        switch (idx)
		{
            case 0:	// 상승
				ChangeFinishSkillCamera(0);
				ChangeAnimation("face_eyes_close");
				break;
            case 1: // 눈 부릅
				ChangeFinishSkillCamera(1);
				ChangeAnimation("face_eyes_open");
				break;
            case 2: // 카메라 앞으로
				ChangeFinishSkillCamera(2);
                finishSkillEffect[3].SetActive(true);
				break;
            case 3: // 양손 이펙트 활성화
				finishSkillEffect[0].SetActive(true);
				finishSkillEffect[1].SetActive(true);
                CameraManager.instance.ShakeCamera(finishSkillCamera[2], 0.3f, 0.05f);
                SoundManager.instance.qPlayer.effect.finishEffectBlue.PlayOneShot(audioSourceEffect, ESoundType.effect);
                break; 
            case 4: // 애너지파 발사 직전
				ChangeFinishSkillCamera(3);
                finishSkillCamera[3].transform.LookAt(targetPoint);
				break;
            case 5: // 애너지파 발사
                finishSkillEffect[0].SetActive(false);
                finishSkillEffect[1].SetActive(false);
                finishSkillEffect[3].SetActive(false);
                finishSkillEffect[2].SetActive(true);
                finishSkillEffect[2].transform.LookAt(targetPoint);
                CameraManager.instance.ShakeCamera(finishSkillCamera[3], 3.8f, 0.03f);
                StartCoroutine(FinishSkillRailRunDamage());
                StartCoroutine(UpdateTargetPoint());
                SoundManager.instance.qPlayer.effect.finishEffectRed.PlayOneShot(audioSourceEffect, ESoundType.effect);
                SoundManager.instance.qPlayer.effect.finishLaser.PlayOneShot(audioSourceEffect, ESoundType.effect);
                break;
			case 6: // 파워업
				ChangeFinishSkillCamera(2);
				finishSkillEffect[2].SetActive(false);
                finishSkillEffect[4].SetActive(true);
                finishSkillEffect[5].SetActive(true);
                break;
			case 7: // 파워업 이펙트
			    finishSkillEffect[6].SetActive(true);
                CameraManager.instance.ShakeCamera(finishSkillCamera[2], 0.3f, 0.05f);
                SoundManager.instance.qPlayer.effect.finishEffectRed.PlayOneShot(audioSourceEffect, ESoundType.effect);
                break;
			case 8: // 돌진 직전
                stateMachine.ChangeState(finishSkillRush);
                finishSkillEffect[5].SetActive(false);
                finishSkillEffect[4].SetActive(false);
                break;
			case 9: // 돌진 시작
                mesh.SetActive(false);
                mesh2.SetActive(false);
                GameObject obj = Instantiate(rushTril);
                obj.transform.position = Vector3.Lerp(mesh2.transform.position, targetPoint, 0.5f);
                obj.transform.LookAt(targetPoint);
                float dist = Vector3.Distance(mesh2.transform.position, targetPoint);
                obj.transform.localScale = new Vector3(1f, 1f, dist * 1.5f);
                transform.position = targetPoint;
                finishSkillEffect[6].SetActive(false);
                stateMachine.ChangeState(finishSkillRushStay);
                break;
			case 10: // 돌진 종료
                mesh.SetActive(true);
                mesh2.SetActive(true);
                StartCoroutine(ResetDelay());
                CameraManager.instance.ShakeCamera(finishSkillCamera[3], 1f, 0.15f);
                SoundManager.instance.qPlayer.effect.finishExplosion.PlayOneShot(audioSourceEffect, ESoundType.effect);
                break;
		}
    } 
    private IEnumerator ResetDelay()
    {
        float overlabSize = 5f;
        Collider[] hit;
        hit = Physics.OverlapSphere(targetPoint, overlabSize, 1 << 8);
        foreach (var item in hit)
        {
            Damage damage = new Damage(GetAP() * 30f,
               1f,
               (item.transform.position - transform.position) * 5f,
               Damage.DamageType.Down
            );
            item.GetComponent<IDamageable>().OnDamage(damage);
        }
        finishSkillEffect[7].SetActive(true);
        ChangeAnimation("face_idle");
        yield return new WaitForSeconds(1.5f);
        finishSkillEffect[6].SetActive(false);
        finishSkillEffect[7].SetActive(false);
        ChangeFinishSkillCamera(-1);
        ResetState();
    }
    private IEnumerator Cut3CameraPosContrl() 
    {
        Vector3 currPos = finishSkillCamera[3].transform.localPosition;
        Transform currParent = finishSkillCamera[3].transform.parent;
        finishSkillCamera[3].transform.SetParent(null);
        ChangeFinishSkillCamera(3);
        yield return new WaitForSeconds(3f);
        finishSkillCamera[3].transform.SetParent(currParent);
        finishSkillCamera[3].transform.localPosition = currPos;
    }
	public void ChangeFinishSkillCamera(int idx)
	{
		//if (SSDNetworkManager.instance.isHost) return;

		for (int i = 0; i < finishSkillCamera.Count; i++)
			finishSkillCamera[i].SetActive(false);

		if (idx != -1)
			finishSkillCamera[idx].SetActive(true);
	}
    private IEnumerator FinishSkillRailRunDamage() 
    {
        float damageAountAll = GetAP() * 60f;
        float dotDamage = damageAountAll / 35f;
        float overlabSize = 1.25f;
        Collider[] hit;
        Damage damage = new Damage(dotDamage,1f, Vector3.zero, Damage.DamageType.Normal);

		for (int i = 0; i < 35; i++)
		{
            hit = Physics.OverlapSphere(targetPoint, overlabSize, 1 << 8);
			foreach (var item in hit)
			{
                item.GetComponent<IDamageable>().OnDamage(damage);
            }
            yield return new WaitForSeconds(0.1f);
        }
	}
    private IEnumerator UpdateTargetPoint()
    {
		for (float time = 0; time < 3.5f; time += Time.deltaTime)
		{
            Vector3 nextPoint = GetAimingPoint();
			if (Vector3.Distance(transform.position, nextPoint) < 20f)
			{
                targetPoint = GetAimingPoint();
                //finishSkillCamera[3].transform.LookAt(targetPoint);
                Quaternion currRot = finishSkillEffect[2].transform.rotation;
                finishSkillEffect[2].transform.LookAt(targetPoint);
                Quaternion nextRot = finishSkillEffect[2].transform.rotation;
                finishSkillEffect[2].transform.rotation = Quaternion.Slerp(currRot, nextRot, 4f * Time.deltaTime);
            }
            yield return null;
		}
    }
    private IEnumerator TimeOutFightGhostFist()
	{
		yield return new WaitForSeconds(.25f);
		OnFightGhostFist(false);
	}
    private void InitializeCamera() {
        if(isLocalPlayer
		&& PlayerCamera.instance == null) {
            GameObject camera = Instantiate(qPlayerCamera);
            CameraManager.instance.playerCam = camera.GetComponent<CinemachineVirtualCamera>();
			camera.GetComponent<PlayerCamera>().SetTarget(this.transform);
			qPlayerCamera = camera;
			CameraManager.instance.SetPlayerCamera();
		}
    }
    void Update() {
        UpdateTargetArea();
    }
    public void ReturnToTPlayer() {
        if (!isCanMove) return;
        ChangeState(returnState, false);
    }
    public void MouseLeftClick() {
        if(isAiming) {
            CmdUseAmingSkill(aimingSkillIndex, GetAimingPoint());
        }
    }
    public void MouseRightClick() {
        if (!isCanMove) return;
		if (isAiming) {
            DisableAim();
            return;
        }
        RaycastHit hit;
        if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, 150f, 1<<6)) {
            movingDestination = new Vector3(hit.point.x, transform.position.y, hit.point.z);
            movement.MoveToPoint(movingDestination, 5f);
            transform.LookAt(movingDestination);
            ChangeState(moveState, false);
        }
    }
    public void OnRunSkill() // 애니메이션 이벤트
    {
        usingSkill.Use(targetPoint);
    }
    public void ResetState() 
    {
		if (prevState != null)
		{
			ChangeState(prevState, false);
			prevState = null;
		}
		else
			ChangeState(separatedState, false);
    }
	public void OnFightGhostFist(bool active)
	{
		if (active)
		{
			stateMachine.ChangeState(fightGhostFistStayState, false); 
		}
		else
		{
			if (stateMachine.currentState == fightGhostFistStayState)
			{
				ResetState();
			}
		}
	}

	#region Change State With Network
	private void ChangeState(State state, bool intoSelf=true) {
        if(isLocalPlayer)
		    CmdChangeState(state.stateName, intoSelf);
	}
    [Command]
    private void CmdChangeState(string stateName, bool intoSelf) {
        SynchronizeState(stateName, intoSelf);
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
    #endregion Change State With Network

    public void ChangeAnimation(string trigger)
    {
        if (currentAnimationTrigger != "")
            animator.ResetTrigger(currentAnimationTrigger);
        currentAnimationTrigger = trigger;
        animator.SetTrigger(currentAnimationTrigger);
    }
    public void OnSkill(int index) {
        //if (index > skills.Count) {
        //    Debug.LogWarning("Skill index want to use is out of range.");
        //    return;
        //}
        if (!canAttack) return;

        Skill selectedSkill = skills[index]; // 사용하고자 하는 스킬 가져오기
        if (!selectedSkill.CanUse()) return;

        if (!selectedSkill.property.ready ||(  isAiming && aimingSkillIndex == index)) {
            CmdUseAmingSkill(index, GetAimingPoint());
        } 
        else {
            aimingSkillIndex = index;
            EnableAim();
        }
    }
    public float GetAP() {
        return status.ap;
    }
    void EnableAim() {
        int aimingSkillIndexTemp = aimingSkillIndex;
        DisableAim();
        aimingSkillIndex = aimingSkillIndexTemp;
        isAiming = true;
        SkillSize size = AimingSkill.GetAreaAmout();
        AimingSkill.GetAimType();
		switch (AimingSkill.GetAimType())
		{
			case AimType.Arrow:
		        skillDecalArrow.size = new Vector3(size.x, size.y, 100f);
                skillDecalArrow.enabled = true;
				break;
			case AimType.Area:
                skillDecalArea.size = new Vector3(size.x, size.y, 100f);
                skillDecalArea.enabled = true;
				break;
            case AimType.None:
                print("스킬 조준 타입 함수 오버라이드 안댐!");
                break;
		}
    }
    void DisableAim() {
        aimingSkillIndex = -1;
        isAiming = false;
        skillDecalArrow.enabled = false;
        skillDecalArea.enabled = false;
    }
    void UpdateTargetArea() {
        if (isAiming) {
            Vector3 targetPoint = GetAimingPoint();

			switch (AimingSkill.GetAimType())
			{
				case AimType.Arrow:
                    skillDecalArrow.transform.parent.LookAt(targetPoint);
                    Vector3 arrowRot = skillDecalArrow.transform.parent.eulerAngles;
                    arrowRot = new Vector3(0, arrowRot.y + 90f, 0);
                    skillDecalArrow.transform.parent.eulerAngles = arrowRot;
                    break;
				case AimType.Area:
                    skillDecalArea.transform.position = targetPoint + Vector3.up * 3f;
                    break;
			}
			//Vector3 ylessPosition = new Vector3(transform.position.x, targetPoint.y, transform.position.z);
			//Vector3 nextProjectorPosition;
			//if(Vector3.Distance(targetPoint, ylessPosition) > AimingSkill.area.distance) {
			//    Vector3 direction = (targetPoint - transform.position).normalized;
			//    targetPoint = ylessPosition + (direction * AimingSkill.area.distance);
			//}
			//nextProjectorPosition = skillTargetAreaDecal.transform.position;
			//nextProjectorPosition.x = targetPoint.x;
			//nextProjectorPosition.z = targetPoint.z;

			//skillTargetAreaDecal.transform.position = nextProjectorPosition;
			return;
        }
    }
    private Vector3 GetAimingPoint() {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if(Physics.Raycast(ray, out hit, 999f, 1<<6)) {
            return hit.point;
        }
        return Vector3.zero;
    }
    [Command]
    private void CmdUseAmingSkill(int skillIndex, Vector3 targetPoint) {
        UseAmingSkill(skillIndex, targetPoint);
    }
    [ClientRpc]
    private void UseAmingSkill(int skillIndex, Vector3 targetPoint) {
        prevState = stateMachine.currentState;
        canAttack = false;
        this.targetPoint = targetPoint;
        stateMachine.ChangeState(statesSkillMap[skills[skillIndex]]);
        DisableAim();
    }
}
