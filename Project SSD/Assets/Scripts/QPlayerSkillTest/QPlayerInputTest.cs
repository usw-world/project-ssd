using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class QPlayerInputTest : MonoBehaviour
{
	QPlyerSkillTest player;
	private void Awake() => player = GetComponent<QPlyerSkillTest>();
	void OnSkill01() => player.OnSkill(0);
	void OnSkill02() => player.OnSkill(1);
	void OnSkill03() => player.OnSkill(2);
	void OnSkill04() => player.OnSkill(3);
}