using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(TPlayer))]
public class TPlayerInput : MonoBehaviour
{
    private TPlayer player;
    private Vector3 moveVecter;
    private float mouseClikTime = 0;
    private float mouseClikTimeMax = 0.8f;
    private bool sAttackFirst = true;
    public InputManager inputManager;

    RaycastHit hit;
    float MaxDistance = 15f; //Ray의 거리(길이)

    private void Awake() => player = GetComponent<TPlayer>();
    private void Start()
    {
        inputManager = GameObject.FindObjectOfType<InputManager>();
        if (inputManager == null)
        {
            print("널널하다~");
        }
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))   player.OnDamage();    // 임시
        if (Input.GetKeyDown(KeyCode.Alpha2))   player.OnDown();      // 임시
        if (Input.GetAxis("Mouse ScrollWheel") != 0) CameraManager.CM.CameraZoomInOut(Input.GetAxis("Mouse ScrollWheel")); // 임시
    }
    void Move(Vector3 vec) => player.InputMove(vec);
    void Dodge() => player.OnSlide();
    void Attack() => player.OnAttack(); 
    void SAttack() => player.OnSAttack();
    void CameraZoom() => print("스크롤~");
    void OnEnable()
    {
        inputManager.Move += Move;    
        inputManager.Dodge += Dodge;        
        inputManager.Attack += Attack;      
        inputManager.SAttack += SAttack;    
        inputManager.CameraZoom += CameraZoom; 
    }

    void OnDisable()
    {
        inputManager.Move -= Move;
        inputManager.Dodge -= Dodge;
        inputManager.Attack -= Attack;
        inputManager.SAttack -= SAttack;
        inputManager.CameraZoom -= CameraZoom;
    }
}