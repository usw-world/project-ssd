using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


public class QPlayerSkillUnityBall : Skill
{
	public SkillOptionInformation[] options = new SkillOptionInformation[8];

	string infoEffectKey;
	string option04_effectKey;
	string option06_effectKey;
	string option07_effectKey;

	public float speed;
	public float option00_increasingSkillPower;
	public float option01_increasingSpeed;
	public float option02_buffTime;
	public float option02_healingAmount;
	public float option03_debuffTime;
	public float option03_damageAmount;
	public float option04_explosionDamageAmount;
	public GameObject option04_effect;
	public float option05_increasingSpeed;
	public GameObject option06_effect;
	public float option06_childDamegeAmount;
	public GameObject option07_effect;
	public float option07_increasingSize;
	public float option07_increasingSkillPower;

	float DamageAmout {
		get {
			float amount = QPlayer.instance.GetAP(); // 여기서 버프 계산 해서 가져오면댐
			amount *= property.skillAP * 0.01f;															
			amount *= 1f + ((options[0].active) ? option00_increasingSkillPower * 0.01f : 0);			
			amount *= 1f + ((options[7].active) ? option07_increasingSkillPower * 0.01f : 0);			
			amount *= ((options[6].active) ? option06_childDamegeAmount * 0.01f : 1f);
			return amount;
		}
	}
	float LastExplosionDamegeAmout{
		get	{
			float amount = DamageAmout;
			amount *= 1f + option04_explosionDamageAmount * 0.01f;
			return amount;
		}
	}
	float LastSpped{
		get	{
			float lastSpped = speed;
			lastSpped *= 1f + ((options[1].active) ? option01_increasingSpeed * 0.01f : 0);
			lastSpped *= 1f + ((options[5].active) ? option05_increasingSpeed * 0.01f : 0);
			return lastSpped;
		}
	}
	Vector3 LastSize{
		get	{
			float size = 1f;
			size *= 1f + ((options[7].active) ? option07_increasingSize * 0.01f : 0);
			return Vector3.one * size;
		}
	}
	private void Start()
	{
		infoEffectKey = info.effect.GetComponent<IPoolerableObject>().GetKey();

		option04_effectKey = option04_effect.GetComponent<IPoolerableObject>().GetKey();

		option06_effectKey = option06_effect.GetComponent<IPoolerableObject>().GetKey();
		option06_effect.GetComponent<UnityHubBall>().SetSubObjectKey(infoEffectKey);

		option07_effectKey = option07_effect.GetComponent<IPoolerableObject>().GetKey();

		PoolerManager.instance.InsertPooler(infoEffectKey, info.effect, false);
		PoolerManager.instance.InsertPooler(option04_effectKey, option04_effect, false);
		PoolerManager.instance.InsertPooler(option06_effectKey, option06_effect, false);
		PoolerManager.instance.InsertPooler(option07_effectKey, option07_effect, false);
	}
	public override void Use(Vector3 target)
	{
		Vector3 sponPos = QPlayer.instance.transform.position;
		sponPos.y = target.y + 1;

		GameObject obj = null;
		UnityBall temp = null;

		if (options[6].active){
			obj = PoolerManager.instance.OutPool(option06_effectKey);
			temp = obj.GetComponent<UnityHubBall>();
		}
		else if (options[7].active){
			obj = PoolerManager.instance.OutPool(option07_effectKey);
			temp = obj.GetComponent<UnityBaaaall>();
		}
		else{

			obj = PoolerManager.instance.OutPool(infoEffectKey);
			temp = obj.GetComponent<UnityBall>();
		}
		obj.transform.position = sponPos;
		obj.transform.LookAt(target + Vector3.up);
		obj.transform.localScale = LastSize;
		temp.OnActive(DamageAmout, LastSpped);

		if (options[2].active)
		{
			Attachment attachment = new Attachment(option02_buffTime, 1f, options[2].image);
			attachment.onStay = (gameObject) => {
				TPlayer.instance.ChangeHp(DamageAmout * option02_healingAmount * 0.01f / 1f);
			};
			TPlayer.instance.AddAttachment(attachment);
		}
		if (options[3].active)
		{
			Attachment attachment = new Attachment(option03_debuffTime, 1f, options[3].image);
			attachment.onStay = (gameObject) => {
				Enemy temp = gameObject.GetComponent<Enemy>();
				// 미완성
			};
			temp.AddDebuff(attachment); // 매게변수로 투사체이 디버프를 추가함
		}
		if (options[4].active)
		{
			temp.AddLastExplosion(option04_effectKey, LastExplosionDamegeAmout); // 매게변수로 투사체이 디버프를 추가함
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