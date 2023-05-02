using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


public class QPlayerSkillUnityBall : Skill
{
	public SkillOptionInformation[] options = new SkillOptionInformation[8];

	public float speed;
	public float option01_increasingSkillPower;
	public float option02_increasingSpeed;
	public float option03_buffTime;
	public float option03_healingAmount;
	public float option04_debuffTime;
	public float option04_damageAmount;

	float DamegeAmout {
		get {
			float amount = QPlayer.instance.GetAP();
			amount *= property.skillAP / 100f;
			amount *= 1f + ((options[0].active) ? option01_increasingSkillPower / 100f : 0);
			return amount;
		}
	}
	float LastSpped{
		get	{
			float lastSpped = speed;
			lastSpped *= 1f + ((options[1].active) ? option02_increasingSpeed / 100f : 0);
			return lastSpped;
		}
	}
	Vector3 LastSize{
		get	{
			float size = 1f;
			return Vector3.one * size;
		}
	}

	public override void Use(Vector3 target)
	{
		Vector3 sponPos = QPlayer.instance.transform.position;
		sponPos.y = target.y;
		GameObject obj = Instantiate(info.effect, sponPos, Quaternion.Euler(0,0,0));

		obj.transform.LookAt(target);
		obj.transform.localScale = LastSize;

		UnityBall temp = obj.GetComponent<UnityBall>();
		temp.OnActive(DamegeAmout, LastSpped);

		if (options[2].active)
		{
			// TPlayer 회복 시키는 함수
		}
		if (options[3].active)
		{
			temp.AddDebuff(); // 매게변수로 투사체이 디버프를 추가함
		}
	}
	public override bool CanUse()
	{
		return true;
	}
}

[Serializable] public class SkillOptionInformation
{
	public string name;
	public string info;
	public Sprite image;
	public bool active;
}