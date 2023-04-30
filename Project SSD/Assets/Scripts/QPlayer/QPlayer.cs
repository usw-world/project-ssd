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
    public PlayerStatus status;
    float speed = 10.0f;
    Vector3 movingDestination;
    bool isAttached = true;
    
    #region States
    Dictionary<string, State> statesMap = new Dictionary<string, State>();

    State attachedState = new State("Attached");
    State separatedState = new State("Separated");
    State returnState = new State("Return");
    State moveState = new State("Move");
    #endregion States

    #region Stamina
    [SerializeField] private float stamina;
    [SerializeField] private float maxStamina = 10f;
    #endregion Stamina

    #region Animation
    Animator animator;
    const string IDLE_ANIMATION_PARAMETER = "Idle";
    const string FLY_ANIMATION_PARAMETER = "Fly";
    #endregion Animation

    public GameObject tPlayerGobj;
    public float distance;
    NavigateableMovement movement;
    StateMachine stateMachine;
    Vector3 temp = Vector3.zero;
    [SerializeField] private float moveSpeed = 0.5f;

    [SerializeField] private GameObject qPlayerCamera;

    public DecalProjector skillDistanceArea;
    public DecalProjector skillRangeArea;
    public List<Skill> skills;
    bool isLookSkillTarget = false;
    Skill usingSkill;
    Vector3 skillTargetPos;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject); 
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
        #endregion Register States

        #region Attached State
        attachedState.onActive += (State prevState) => {
            movement.Stop();
            movement.enabled = false;
            animator.SetBool(FLY_ANIMATION_PARAMETER, true);
        };
        attachedState.onStay += () => {};
        attachedState.onInactive += (State nextState) => {
            animator.SetBool(FLY_ANIMATION_PARAMETER, false);
        };
        #endregion Attached State
        
        #region Return State
        returnState.onActive += (State prevState) => {
            movement.Stop();
            movement.enabled = false;
            animator.SetBool(FLY_ANIMATION_PARAMETER, true);
        };
        returnState.onInactive += (State nextState) => {
            animator.SetBool(FLY_ANIMATION_PARAMETER, false);
        };
        #endregion Return State

        #region Separated State
        separatedState.onActive += (State prevState) => {
            movement.enabled = true;
            animator.SetBool(IDLE_ANIMATION_PARAMETER, true);
        };
        separatedState.onStay += () => {
            if(!movement.isArrive)
                ChangeState(moveState);
        };
        separatedState.onInactive += (State nextState) => {
            animator.SetBool(IDLE_ANIMATION_PARAMETER, false);
        };
        #endregion Separated State

        #region Move State
        moveState.onActive += (State prevState) => {
            transform.LookAt(movingDestination);
            movement.enabled = true;
            movement.MoveToPoint(movingDestination, moveSpeed);
            animator.SetBool(FLY_ANIMATION_PARAMETER, true);
        };
        moveState.onStay += () => {
            if(movement.isArrive)
                ChangeState(separatedState);
        };
        moveState.onInactive += (State nextState) => {
            movement.Stop();
            animator.SetBool(FLY_ANIMATION_PARAMETER, false);
        };
        #endregion Move State
    }
    private IEnumerator ReturnCoroutine() {
        float offset = 0;
        while(offset < 1) {
            offset += Time.deltaTime;
            transform.position = Vector3.Lerp(transform.position, tPlayerGobj.transform.position, offset);
            yield return null;
        }
        ChangeState(attachedState);
    }

    private void InitializeCamera() {
        if(isLocalPlayer
		&& PlayerCamera.instance == null) {
            GameObject camera = Instantiate(qPlayerCamera);
			camera.GetComponent<PlayerCamera>().SetTarget(this.transform);
        }
    }
    
    void Update() {
        SetSkillTargetPos();
        distance = Vector3.Distance(transform.position, tPlayerGobj.transform.position);
    }

    public void QPreturn(){
        ChangeState(attachedState);
    }
    
    public void LB_click(){
        UseSkill();
    }
    public void RB_click(){
		if (isLookSkillTarget) {
            SkillAreaDisable();
            return;
        }
        RaycastHit hit;
        if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit)) {
            movingDestination = new Vector3(hit.point.x, transform.position.y, hit.point.z);
            ChangeState(moveState, true);
            Debug.Log($"{hit.transform.position} point : {hit.point}");
        }
    }
    
	private void ChangeState(State state, bool intoSelf=true) {
		SynchromizeState(state.stateName, intoSelf);
	}
	[ClientRpc]
	private void SynchromizeState(string stateName, bool intoSelf) {
		try {
			State nextState = statesMap[stateName];
			if(nextState != null)
				stateMachine.ChangeState(nextState, intoSelf);
		} catch(KeyNotFoundException e) {
			Debug.LogError(e);
		}
	}

    public void OnSkill(int idx) 
    {
        if (idx > skills.Count) return;
        Skill selectSkill = skills[idx];    // 사용하고자 하는 스킬 가져오기
        if (!selectSkill.CanUse()) return;
        if (usingSkill == null) usingSkill = selectSkill; // 널이면 바로 넣기

        if (usingSkill == selectSkill)
        {
            if (usingSkill.property.ready)  // 조준 해야하는 스킬?
            {
                if (usingSkill.property.quickUse) // 바로 사용 ?
                {
                    UseSkill(); // [조준스킬] [퀵 사용] 사용
                }
                else
                {
                    if (isLookSkillTarget)  // 조준 하는 중?
                    {
                        UseSkill(); // [조준스킬] [조준 후 사용] 사용
                    }
                    else SkillAreaEnable();
                }
            }
            else // 즉발 스킬
            {
                UseSkill(); // [즉발스킬] 사용
            }
        }
        else
        {
            usingSkill = selectSkill;
            SkillAreaDisable();
            OnSkill(idx);
        }
    }
    public float GetAP() 
    {
        return status.ap;
    }
    void SkillAreaEnable()
    {
        float distance = usingSkill.area.distance * 2f;
        float range = usingSkill.area.range;
        isLookSkillTarget = true;
        skillDistanceArea.size = new Vector3(distance, distance, 100f);
        skillRangeArea.size = new Vector3(range, range, 100f);
        skillDistanceArea.enabled = true;
        skillRangeArea.enabled = true;
    }
    void SkillAreaDisable()
    {
        usingSkill = null;
        isLookSkillTarget = false;
        skillDistanceArea.enabled = false;
        skillRangeArea.enabled = false;
    }
    void SetSkillTargetPos() 
    {
        if (isLookSkillTarget)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, 10000f, 1 << LayerMask.NameToLayer("Block")))
            {
                skillTargetPos = hit.point;
                Vector3 temp = skillTargetPos;
                temp.y = skillDistanceArea.transform.position.y;

                float dist = Vector3.Distance(temp, skillDistanceArea.transform.position);
                if (dist > usingSkill.area.distance)
                {
                    temp = temp - skillDistanceArea.transform.position;
                    temp = temp.normalized * usingSkill.area.distance;
                    temp.y += 3f;
                    skillRangeArea.transform.localPosition = temp;

                    RaycastHit hit2;
                    if (Physics.Raycast(skillRangeArea.transform.position, skillRangeArea.transform.forward, out hit2, 1 << LayerMask.NameToLayer("Block")))
                    {
                        skillTargetPos = hit2.point;
                    }
                }
                else
                    skillRangeArea.transform.position = temp;
            }
        }
    }
    void UseSkill()
    {
        if (!isLookSkillTarget) return;
        usingSkill.Use(skillTargetPos); // [조준스킬] [조준 후] 사용
        usingSkill = null;
        SkillAreaDisable();
    }
}
