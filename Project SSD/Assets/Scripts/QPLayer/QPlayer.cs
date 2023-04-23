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
    #region veriable
    public PlayerStatus status;
        float speed = 10.0f;
        Vector2 mousePos;
        Vector3 movePos;
        public float stamina = 0.0f;
        public GameObject TPlayer;
        private Transform TPtransform;
        Vector3 min, max;
        float timer;
        public float distance;
        public bool moveTest;
        Animator anim;
        Rigidbody rb;
        Movement movement;
        StateMachine stateMachine;
        Vector3 temp = Vector3.zero;
        new Camera camera;
        float moveSpeed = 0.5f;

        // public Text testText;
    #endregion

    #region state
        State duel = new State("QP_duel");
        State solo = new State("QP_solo");
    #endregion

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
        else Destroy(gameObject); 

        camera  = Camera.main;
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();
        movement = GetComponent<Movement>();
        stateMachine = GetComponent<StateMachine>();
        //Cursor.lockState = CursorLockMode.Locked;


        stateMachine.SetIntialState(duel);
    }
    private void stateInitialize(){

    }

    // Start is called before the first frame update
    void Start()
    {
        TPlayer = GameObject.FindGameObjectWithTag("TPlayer");
        TPtransform = TPlayer.GetComponent<Transform>();
        duel.onStay = () => {
            Vector3 targetPos = TPtransform.position;
            float returnSpeed = 10.0f * Time.deltaTime;
            targetPos.y += 2;
            targetPos.x -= 1;
            transform.position = Vector3.SmoothDamp(transform.position, targetPos, ref temp, returnSpeed);
        };

        distance = 10.0f;
        timer = 0.0f;
        moveTest = true;
        stamina = 10.0f;
        camera = Camera.main;
        movePos = TPtransform.position;
        movePos.x -= 2;
        movePos.y += 1;
    }

    // Update is called once per frame
    void Update()
    {
        SetSkillTargetPos();
        if (!isLocalPlayer)
            return;
        Vector3 camPos = transform.position;
        camera.transform.rotation = Quaternion.Euler(new Vector3(60f, 0, 0));
        camPos.y = 15;
        camPos.z -= 5;
        camera.transform.position  = camPos;

        #region move
        // 움직이는 부분
        // testText.text = stamina+"";
        distance = Vector3.Distance(transform.position, TPtransform.position);

        if(stateMachine.currentState == solo){
        //     if (Input.GetKey(KeyCode.UpArrow))
        //         transform.Translate(transform.forward * speed * Time.deltaTime);
        //     if (Input.GetKey(KeyCode.DownArrow))
        //         transform.Translate(-transform.forward * speed * Time.deltaTime);
        //     if (Input.GetKey(KeyCode.RightArrow))
        //         transform.Translate(transform.right * speed * Time.deltaTime);
        //     if (Input.GetKey(KeyCode.LeftArrow))
        //         transform.Translate(-transform.right * speed * Time.deltaTime);

            transform.position = Vector3.SmoothDamp(transform.position, movePos, ref temp, moveSpeed);
            stamina -= distance * Time.deltaTime * 0.1f;
            if(stamina<0){
                stateMachine.ChangeState(duel);
            }
        } else if(stateMachine.currentState == duel){
            if(stamina<10.0f){
                stamina += Time.deltaTime * 5;
            }
        }
        else{
            Debug.Log("error");
        }
        #endregion

    }

    public void QPreturn(){
        stateMachine.ChangeState(duel);
        
    }
    public void posUpdate(Vector2 mousePos){
        this.mousePos = mousePos;
    }
    
    public void RB_click(){
		if (isLookSkillTarget)
		{
            SkillAreaDisable();
            return;
        }
        
        RaycastHit hit;

        if (Physics.Raycast(camera.ScreenPointToRay(Input.mousePosition), out hit)) {
            movePos = hit.point;
            stateMachine.ChangeState(solo, false) ;
            Debug.Log($"{hit.transform.position.ToString()} point : {hit.point}");
            movePos.y = 2.0f;
            // Debug.Log("QPLayer Move");
            // Do something with the object that was hit by the raycast.
        }
    }

    public void LB_click(){
        UseSkill();
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
