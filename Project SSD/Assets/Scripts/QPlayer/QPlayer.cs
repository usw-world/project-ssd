using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Mirror;
using UnityEngine.Rendering.Universal;

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
    bool isAttached = true;
    bool canAttack = true;
    #endregion Q-Player Status

    #region Movement
    NavigateableMovement movement;
    [SerializeField] private float movingSpeed = 10.0f;
    Vector3 movingDestination;
    #endregion Movement
    
    #region State Machine
    StateMachine stateMachine;
    Dictionary<string, State> statesMap = new Dictionary<string, State>();

    State attachedState = new State("Attached");
    State separatedState = new State("Separated");
    State returnState = new State("Return");
    State moveState = new State("Move");
    State unityBallState = new State("Unity Ball");
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


    [SerializeField] private GameObject qPlayerCamera;

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
    /* will be removed. >>
    Vector3 skillTargetPos;
    */

    public DecalProjector skillDistanceDecal;
    public DecalProjector skillTargetAreaDecal;
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
    }
    
    void Start() {
        tPlayerGobj = TPlayer.instance?.gameObject ?? GameObject.FindGameObjectWithTag("TPlayer");
        stamina = maxStamina;

        InitializedState();
        InitializeCamera();
    }

    private void InitializedState() {
        #region Register States
        statesMap.Add(attachedState.stateName, attachedState);
        statesMap.Add(separatedState.stateName, separatedState);
        statesMap.Add(returnState.stateName, returnState);
        statesMap.Add(moveState.stateName, moveState);
        statesMap.Add(unityBallState.stateName, unityBallState);
        //statesMap.Add(oneHandCastingState.stateName, oneHandCastingState);
        #endregion Register States

        #region Attached State
        attachedState.onActive += (State prevState) => {
            movement.Stop();
            canAttack = true;
            movement.enabled = false;
            ChangeAnimation(FLY_ANIMATION_PARAMETER);
        };
        attachedState.onStay += () => {};
        attachedState.onInactive += (State nextState) => { };
        #endregion Attached State
        
        #region Return State
        returnState.onActive += (State prevState) => {
            movement.Stop();
            movement.enabled = false;
            ChangeAnimation(FLY_ANIMATION_PARAMETER);
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
            if(!movement.isArrive)
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
            if(movement.isArrive)
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
            animator.SetTrigger(usingSkill.GetAnimationTigger());
        };
        unityBallState.onStay = () => { };
        unityBallState.onInactive = (State nextState) => { canAttack = true; };
        #endregion UnityBall State
    }
    private IEnumerator ReturnCoroutine() {
        float offset = 0;
        while(offset < 1) {
            offset += Time.deltaTime;
            transform.position = Vector3.Lerp(transform.position, tPlayerGobj.transform.position, offset);
            yield return null;
        }
        ChangeState(attachedState, false);
    }

    private void InitializeCamera() {
        if(isLocalPlayer
		&& PlayerCamera.instance == null) {
            GameObject camera = Instantiate(qPlayerCamera);
			camera.GetComponent<PlayerCamera>().SetTarget(this.transform);
        }
    }
    
    void Update() {
        UpdateTargetArea();
        SeparatedUpdate();
    }
    public void SeparatedUpdate() {

    }
    public void DecreaseStamina() {

    }
    public void ReturnToTPlayer(){
        ChangeState(returnState, false);
    }
    public void MouseLeftClick(){
        if(isAiming) {
            CmdUseAmingSkill(aimingSkillIndex, GetAimingPoint());
        }
    }
    public void MouseRightClick(){
		if (isAiming) {
            DisableAim();
            return;
        }
        RaycastHit hit;
        if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, 150f, 1<<6)) {
            movingDestination = new Vector3(hit.point.x, transform.position.y, hit.point.z);
            movement.MoveToPoint(movingDestination, movingSpeed);
            transform.LookAt(movingDestination);
            ChangeState(moveState, false);
        }
    }
    public void OnRunSkill() 
    {
        usingSkill.Use(targetPoint);
    }
    public void ResetState() 
    {
        ChangeState(separatedState, false);
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
        if (index > skills.Count) {
            Debug.LogWarning("Skill index want to use is out of range.");
            return;
        }
        if (!canAttack) return;

        Skill selectedSkill = skills[index]; // 사용하고자 하는 스킬 가져오기
        if (!selectedSkill.CanUse())
            return;

        if(!selectedSkill.property.ready // if | skill type is instant-using
        ||                               // or
        (  isAiming                      // if | currently player is aiming with skill
        && aimingSkillIndex == index)) { // and| pressed skill is same to currently aiming skill
            CmdUseAmingSkill(index, GetAimingPoint());
        } else {
            aimingSkillIndex = index;
            EnableAim();
        }
    }
    public float GetAP() {
        return status.ap;
    }
    void EnableAim() {
        float distance = AimingSkill.area.distance * 2f;
        float range = AimingSkill.area.range;
        isAiming = true;
        skillDistanceDecal.size = new Vector3(distance, distance, 100f);
        skillTargetAreaDecal.size = new Vector3(range, range, 100f);
        skillDistanceDecal.enabled = true;
        skillTargetAreaDecal.enabled = true;
    }
    void DisableAim() {
        aimingSkillIndex = -1;
        isAiming = false;
        skillDistanceDecal.enabled = false;
        skillTargetAreaDecal.enabled = false;
    }
    void UpdateTargetArea() {
        if (isAiming) {
            Vector3 targetPoint = GetAimingPoint();
            Vector3 ylessPosition = new Vector3(transform.position.x, targetPoint.y, transform.position.z);
            Vector3 nextProjectorPosition;
            if(Vector3.Distance(targetPoint, ylessPosition) > AimingSkill.area.distance) {
                Vector3 direction = (targetPoint - transform.position).normalized;
                targetPoint = ylessPosition + (direction * AimingSkill.area.distance);
            }
            nextProjectorPosition = skillTargetAreaDecal.transform.position;
            nextProjectorPosition.x = targetPoint.x;
            nextProjectorPosition.z = targetPoint.z;

            skillTargetAreaDecal.transform.position = nextProjectorPosition;
            return;
            /* will be removed >>
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, 10000f, 1 << LayerMask.NameToLayer("Block")))
            {
                skillTargetPos = hit.point;
                Vector3 temp = skillTargetPos;
                temp.y = skillDistanceDecal.transform.position.y;

                float dist = Vector3.Distance(temp, skillDistanceDecal.transform.position);
                if (dist > aimingSkill.area.distance)
                {
                    temp = temp - skillDistanceDecal.transform.position;
                    temp = temp.normalized * aimingSkill.area.distance;
                    temp.y += 3f;
                    skillTargetAreaDecal.transform.localPosition = temp;

                    RaycastHit hit2;
                    if (Physics.Raycast(skillTargetAreaDecal.transform.position, skillTargetAreaDecal.transform.forward, out hit2, 1 << LayerMask.NameToLayer("Block")))
                    {
                        skillTargetPos = hit2.point;
                    }
                }
                else
                    skillTargetAreaDecal.transform.position = temp;
            }
             */
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
        usingSkill = skills[skillIndex];
        this.targetPoint = targetPoint;
        stateMachine.ChangeState(unityBallState);
        DisableAim();
    }
}
