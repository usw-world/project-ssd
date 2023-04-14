using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Mirror;


[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(CapsuleCollider))]
[RequireComponent(typeof(StateMachine))]
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(NavigateableMovement))]
public class QPlayer : MonoBehaviour
{
    #region veriable
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
        Camera camera;
        float moveSpeed = 0.5f;

        public Text testText;
    #endregion

    #region state
        State duel = new State("QP_duel");
        State solo = new State("QP_solo");
    #endregion

    private void Awake()
    {
        camera  = Camera.main;
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();
        movement = GetComponent<Movement>();
        stateMachine = GetComponent<StateMachine>();
        //Cursor.lockState = CursorLockMode.Locked;
        duel.onStay = () => {
            Vector3 targetPos = TPtransform.position;
            float returnSpeed = 10.0f * Time.deltaTime;
            targetPos.y += 2;
            targetPos.x -= 1;
            transform.position = Vector3.SmoothDamp(transform.position, targetPos, ref temp, returnSpeed);
        };

        stateMachine.SetIntialState(duel);
    }
    private void stateInitialize(){

    }

    // Start is called before the first frame update
    void Start()
    {
        distance = 10.0f;
        TPtransform = TPlayer.GetComponent<Transform>();
        timer = 0.0f;
        moveTest = true;
        stamina = 10.0f;
        movePos = TPtransform.position;
        movePos.x -= 2;
        movePos.y += 1;
    }

    // Update is called once per frame
    void Update()
    {       
        Vector3 camPos = transform.position;
        camPos.y = 15;
        camPos.z -= 5;
        camera.transform.position  = camPos;

        #region move
        // 움직이는 부분
        testText.text = stamina+"";
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
        RaycastHit hit;
        Ray ray = camera.ScreenPointToRay(Input.mousePosition);
        
        if (Physics.Raycast(camera.ScreenPointToRay(Input.mousePosition), out hit)) {
            movePos = hit.point;
            Debug.Log($"{hit.transform.position.ToString()} point : {hit.point}");
            movePos.y = 2.0f;
            // Debug.Log("QPLayer Move");
            // Do something with the object that was hit by the raycast.
        }
    }

    public void LB_click(){
        stateMachine.ChangeState(solo);
    }


}
