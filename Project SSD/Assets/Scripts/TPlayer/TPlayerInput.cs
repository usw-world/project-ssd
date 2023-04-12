using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(TPlayer))]
public class TPlayerInput : MonoBehaviour
{
    TPlayer player;

<<<<<<< Updated upstream
	private void Awake() => player = GetComponent<TPlayer>();
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))   player.OnDamage();    // 임시
        if (Input.GetKeyDown(KeyCode.Alpha2))   player.OnDown();      // 임시
=======
    RaycastHit hit;
    float MaxDistance = 15f; //Ray�� �Ÿ�(����)

    private void Awake() => player = GetComponent<TPlayer>();
   
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
>>>>>>> Stashed changes
    }
	void OnMove(InputValue value) => player.InputMove(value.Get<Vector3>());
	void OnDodge() => player.OnSlide();
	void OnAttack() => player.OnAttack();
	void OnSAttack() => player.OnSAttack();
	void OnCameraZoomIn() => CameraManager.CM.CameraZoomInOut(true);
	void OnCameraZoomOut() => CameraManager.CM.CameraZoomInOut(false);
}