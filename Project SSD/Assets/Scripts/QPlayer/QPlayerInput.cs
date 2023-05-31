using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(QPlayer))]
public class QPlayerInput : MonoBehaviour
{
    QPlayer player;

	private void Awake() {
		player = GetComponent<QPlayer>();
	}
	private void Start() {
		if(QPlayer.instance.isLocalPlayer)
			GetComponent<PlayerInput>().enabled = true;
	}
	void OnCameraZoomIn() => PlayerCamera.instance.ZoomIn();
	void OnCameraZoomOut() => PlayerCamera.instance.ZoomOut();
	void OnSkill01() => player.OnSkill(0);
	void OnSkill02() => player.OnSkill(1);
	void OnSkill03() => player.OnSkill(2);
	void OnSkill04() => player.OnSkill(3);
	void OnSkill05() => player.OnSkill(4);
	void OnSkill06() => player.OnSkill(5);
	void OnSkill07() => player.OnSkill(6);
	void OnSkill08() => player.OnSkill(7);
	void OnComeBack() => player.ReturnToTPlayer();
	void OnRB_click() => player.MouseRightClick();
	void OnLB_click() => player.MouseLeftClick();
	
	void OnEscape() => UIManager.instance.OnPressEscape();
}