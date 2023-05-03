using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


public class QPlayerSkillUnityBall : Skill
{
	public SkillOptionInformation[] options = new SkillOptionInformation[8];

	public float speed;
	public float option00_increasingSkillPower;
	public float option01_increasingSpeed;
	public float option02_buffTime;
	public float option02_healingAmount;
	public float option03_debuffTime;
	public float option03_damageAmount;
	public float option04_explosionDamageAmount;
	public float option05_increasingSpeed;
	public GameObject option06_effect;
	public float option06_childDamegeAmout;
	public GameObject option07_effect;
	public float option07_increasingSize;
	public float option07_increasingSkillPower;

	float DamegeAmout {
		get {
			float amount = QPlayer.instance.GetAP(); // 여기서 버프 계산 해서 가져오면댐
			amount *= 1f + property.skillAP / 100f;
			amount *= 1f + ((options[0].active) ? option00_increasingSkillPower / 100f : 0);
			amount *= 1f + ((options[7].active) ? option07_increasingSkillPower / 100f : 0);
			amount *= ((options[6].active) ? option06_childDamegeAmout / 100f : 0);
			return amount;
		}
	}
	float LastExplosionDamegeAmout{
		get	{
			float amount = DamegeAmout;
			amount *= 1f + option04_explosionDamageAmount / 100f;
			return amount;
		}
	}
	float LastSpped{
		get	{
			float lastSpped = speed;
			lastSpped *= 1f + ((options[1].active) ? option01_increasingSpeed / 100f : 0);
			lastSpped *= 1f + ((options[5].active) ? option05_increasingSpeed / 100f : 0);
			return lastSpped;
		}
	}
	Vector3 LastSize{
		get	{
			float size = 1f;
			size *= 1f + ((options[7].active) ? option07_increasingSize / 100f : 0);
			return Vector3.one * size;
		}
	}
	private void Start() {
		options[2].active = true;
		//options[5].active = true;
		//options[6].active = true;
	}
	public override void Use(Vector3 target)
	{
		Vector3 sponPos = QPlayer.instance.transform.position;
		sponPos.y = target.y + 1;

		GameObject obj = null;
		UnityBall temp = null;

		if (options[6].active){
			obj = Instantiate(option06_effect, sponPos, Quaternion.Euler(0, 0, 0));
			temp = obj.GetComponent<UnityHubBall>();
		}
		else if (options[7].active){
			obj = Instantiate(option07_effect, sponPos, Quaternion.Euler(0, 0, 0));
			temp = obj.GetComponent<UnityBaaaall>();
		}
		else{
			obj = Instantiate(info.effect, sponPos, Quaternion.Euler(0, 0, 0));
			temp = obj.GetComponent<UnityBall>();
		}

		obj.transform.LookAt(target + Vector3.up);
		obj.transform.localScale = LastSize;
		temp.OnActive(DamegeAmout, LastSpped);

		if (options[2].active)
		{
			Attachment attachment = new Attachment(option02_buffTime, 1f, options[2].image);
			attachment.onAction = () => {
				print("TPlayer 회복 시작");
			};
			attachment.onStay = () => {
				print("TPlayer HP +" + (DamegeAmout / option02_buffTime * 100f / 1f));
				TPlayer.instance.status.hp += DamegeAmout / option02_buffTime / 1f;
				if (TPlayer.instance.status.hp > TPlayer.instance.status.maxHp){
					TPlayer.instance.status.hp = TPlayer.instance.status.maxHp;
				}
			};
			attachment.onInactive = () => {
				print("TPlayer 회복 종료");
			};
			TPlayer.instance.AddAttachment(attachment);
		}
		if (options[3].active)
		{
			temp.AddDebuff(); // 매게변수로 투사체이 디버프를 추가함
		}
		if (options[4].active)
		{
			temp.AddLastExplosion(LastExplosionDamegeAmout); // 매게변수로 투사체이 디버프를 추가함
		}
		if (options[5].active)
		{
			temp.OnActiveGuided(); // 매게변수로 투사체이 디버프를 추가함
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