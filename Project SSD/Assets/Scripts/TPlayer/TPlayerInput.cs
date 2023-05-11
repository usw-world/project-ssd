using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(TPlayer))]
public class TPlayerInput : MonoBehaviour
{
    TPlayer player;

	private void Awake() {
		player = GetComponent<TPlayer>();
	}
	private void Start() {
		if(TPlayer.instance.isLocalPlayer)
			GetComponent<PlayerInput>().enabled = true;
	}
		
	private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))   player.OnDamage(null, 10);    // 임시
        if (Input.GetKeyDown(KeyCode.Alpha2))   player.OnDown();      // 임시
    }
	void OnMove(InputValue value) => player.InputMove(value.Get<Vector3>());
	void OnDodge() => player.OnSlide();
	void OnAttack() => player.OnAttack();
	void OnCameraZoomIn() => PlayerCamera.instance.ZoomIn();
	void OnCameraZoomOut() => PlayerCamera.instance.ZoomOut();
	void OnConvertWalkRun() => player.OnMoveSpeedConvert();
	void OnStartRush() => player.OnRunToRush();
	void OnEndRush() => player.OnRushToRun();
	void OnStartCharging() => player.OnChargingStart();
	void OnEndCharging() => player.OnChargingEnd();
}