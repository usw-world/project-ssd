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
    float MaxDistance = 15f; //Ray�� �Ÿ�(����)

    private void Awake() => player = GetComponent<TPlayer>();
    private void Start()
    {
        inputManager = GameObject.FindObjectOfType<InputManager>();
        if (inputManager == null)
        {
            print("�γ��ϴ�~");
        }
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))   player.OnDamage();    // �ӽ�
        if (Input.GetKeyDown(KeyCode.Alpha2))   player.OnDown();      // �ӽ�
        if (Input.GetAxis("Mouse ScrollWheel") != 0) CameraManager.CM.CameraZoomInOut(Input.GetAxis("Mouse ScrollWheel")); // �ӽ�
    }
    void Move(Vector3 vec) => player.InputMove(vec);
    void Dodge() => player.OnSlide();
    void Attack() => player.OnAttack(); 
    void SAttack() => player.OnSAttack();
    void CameraZoom() => print("��ũ��~");
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