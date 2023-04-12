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
	void OnCameraZoomIn() => CameraManager.CM.CameraZoomInOut(true);
	void OnCameraZoomOut() => CameraManager.CM.CameraZoomInOut(false);
	void OnSkill01() => print("1번스킬 사용");
	void OnSkill02() => print("2번스킬 사용");
	void OnSkill03() => print("3번스킬 사용");
	void OnSkill04() => print("4번스킬 사용");
	void OnComeBack() => print("ComeBack 사용");
}