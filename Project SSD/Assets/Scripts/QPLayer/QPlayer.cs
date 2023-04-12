using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;


[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(CapsuleCollider))]
[RequireComponent(typeof(StateMachine))]
[RequireComponent(typeof(Animator))]
public class QPlayer : MonoBehaviour
{
    #region veriable
        float speed = 10.0f;
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
    #endregion

    #region state
        State idle = new State("QP_idle");
        State move = new State("QP_move");
    #endregion

    private void Awake()
    {
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();
        movement = GetComponent<Movement>();
        stateMachine = GetComponent<StateMachine>();
        //Cursor.lockState = CursorLockMode.Locked;
    }

    // Start is called before the first frame update
    void Start()
    {
        distance = 10.0f;
        TPtransform = TPlayer.GetComponent<Transform>();
        timer = 0.0f;
        moveTest = true;
    }

    // Update is called once per frame
    void Update()
    {
        if(moveTest){
        if (Input.GetKey(KeyCode.UpArrow))
            transform.Translate(transform.forward * speed * Time.deltaTime);
        if (Input.GetKey(KeyCode.DownArrow))
            transform.Translate(-transform.forward * speed * Time.deltaTime);
        if (Input.GetKey(KeyCode.RightArrow))
            transform.Translate(transform.right * speed * Time.deltaTime);
        if (Input.GetKey(KeyCode.LeftArrow))
            transform.Translate(-transform.right * speed * Time.deltaTime);
        }
        else{
            timer += Time.deltaTime;
            if(timer<=5){
                QPreturn(TPtransform.position);
            }
            else{
                moveTest = true;
                timer = 0f;
            }
        }
    }

    void QPreturn(Vector3 tragetPos){
        Vector3 temp = Vector3.zero;
        float returnSpeed = 0.05f;
        tragetPos.y += 2;
        tragetPos.x -= 1;
        transform.position = Vector3.SmoothDamp(transform.position, tragetPos, ref temp, returnSpeed);
    }

    private void stateInitialize(){
    }
}
