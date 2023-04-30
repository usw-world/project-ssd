using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(QPlayer))]
public class QPlayerInput : MonoBehaviour
{
    QPlayer player;

	private void Awake() => player = GetComponent<QPlayer>();
	void OnCameraZoomIn() => PlayerCamera.instance.ZoomIn();
	void OnCameraZoomOut() => PlayerCamera.instance.ZoomOut();
	void OnSkill01() => player.OnSkill(0);
	void OnSkill02() => player.OnSkill(1);
	void OnSkill03() => player.OnSkill(2);
	void OnSkill04() => player.OnSkill(3);
	void OnComeBack() => player.QPreturn();
	void OnRB_click() => player.RB_click();
	void OnLB_click() => player.LB_click();

}