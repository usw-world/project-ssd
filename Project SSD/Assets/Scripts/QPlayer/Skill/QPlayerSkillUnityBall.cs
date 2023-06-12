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
	public float usingSp = 10f;

	float DamageAmout {
		get {
			float amount = QPlayer.instance.GetAP(); // 여기서 버프 계산 해서 가져오면댐
			amount *= property.skillAP * 0.01f;													
			amount *= 1f + ((options[0].active) ? option00_increasingSkillPower * 0.01f : 0);	
			amount *= 1f + ((options[7].active) ? option07_increasingSkillPower * 0.01f : 0);	
			amount *= ( (options[6].active) ? option06_childDamegeAmount * 0.01f : 1f);
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
	float LastSpeed{
		get	{
			float lastSpeed = speed;
			lastSpeed *= 1f + ((options[1].active) ? option01_increasingSpeed * 0.01f : 0);
			lastSpeed *= 1f + ((options[5].active) ? option05_increasingSpeed * 0.01f : 0);
			return lastSpeed;
		}
	}
	Vector3 LastSize{
		get	{
			float size = 1f;
			size *= 1f + ((options[7].active) ? option07_increasingSize * 0.01f : 0);
			return Vector3.one * size;
		}
	}
	private void Awake()
	{
		options[0].name = "강력한 일격";
		options[0].info = "데미지를 증가한다";
		options[1].name = "빠른 일격";
		options[1].info = "파이어볼의 속도가 증가한다";
		options[2].name = "피닉스의 날개";
		options[2].info = "파트너의 HP를 회복 시킨다";
		options[3].name = "영겁의 불꽃";
		options[3].info = "적에게 5초간 지속피해를 준다";
		options[4].name = "꺼진 불도 다시 보자";
		options[4].info = "파이어볼이 사라질때 폭발하며 추가 피해를 준다";
		options[5].name = "LookAt";
		options[5].info = "유도 기능을 활성화 한다";
		options[6].name = "";
		options[6].info = "";
		options[7].name = "메테오";
		options[7].info = "강력한 공격으로 변경된다";
		//options[0].active = true;
		//options[1].active = true;
		//options[2].active = true;
		//options[3].active = true;
		//options[4].active = true;
		//options[5].active = true;
		//options[6].active = true;
		options[7].active = true;
	}
	private void Start()
	{
		infoEffectKey = info.effect.GetComponent<IPoolableObject>().GetKey();
		option04_effectKey = option04_effect.GetComponent<IPoolableObject>().GetKey();
		option06_effectKey = option06_effect.GetComponent<IPoolableObject>().GetKey();
		option06_effect.GetComponent<UnityHubBall>().SetSubObjectKey(infoEffectKey);
		option07_effectKey = option07_effect.GetComponent<IPoolableObject>().GetKey();

		PoolerManager.instance.InsertPooler(infoEffectKey, info.effect, false);
		PoolerManager.instance.InsertPooler(option04_effectKey, option04_effect, false);
		PoolerManager.instance.InsertPooler(option06_effectKey, option06_effect, false);
		PoolerManager.instance.InsertPooler(option07_effectKey, option07_effect, false);
	}
	public override void Use(Vector3 target)
	{
		QPlayer.instance.status.sp -= usingSp;
		property.nowCoolTime = 0;

		Vector3 sponPos = QPlayer.instance.transform.position;
		sponPos.y = target.y + 1;

		GameObject obj = null;
		UnityBall temp = null;

		if (options[6].active)
		{
			obj = PoolerManager.instance.OutPool(option06_effectKey);
			temp = obj.GetComponent<UnityHubBall>();
			obj.transform.position = sponPos;
			obj.transform.LookAt(target + Vector3.up);
		}
		else if (options[7].active)
		{
			obj = PoolerManager.instance.OutPool(option07_effectKey);
			temp = obj.GetComponent<UnityBaaaall>();
			obj.transform.position = target + new Vector3(0, 0.1f, 0);
			obj.transform.eulerAngles = Vector3.zero;
		}
		else
		{
			obj = PoolerManager.instance.OutPool(infoEffectKey);
			temp = obj.GetComponent<UnityBall>();
			obj.transform.position = sponPos;
			obj.transform.LookAt(target + Vector3.up);
		}
		temp.OnActive(DamageAmout, LastSpeed);

		if (options[2].active)
		{
			Attachment attachment = new Attachment(option02_buffTime, 1f, options[2].image, EAttachmentType.healing);
			attachment.onStay = (gameObject) => {
				TPlayer.instance.ChangeHp(DamageAmout * option02_healingAmount * 0.01f / 1f);
			};
			TPlayer.instance?.AddAttachment(attachment); // 매게변수도 TPlayer에게 버프를 추가함
		}
		if (options[3].active)
		{
			Attachment attachment = new Attachment(option03_debuffTime, 1f, options[3].image, EAttachmentType.damage);
			attachment.onStay = (gameObject) => {
				IDamageable enemy = gameObject.GetComponent<IDamageable>();
				Damage damage = new Damage(DamageAmout * option03_damageAmount * 0.01f, 0, Vector3.zero, Damage.DamageType.Normal);
				enemy.OnDamage(damage);
			};
			temp.AddDebuff(attachment); // 매게변수도 투사체에 디버프를 추가함
		}
		if (options[4].active)
		{
			temp.AddLastExplosion(option04_effectKey, LastExplosionDamegeAmout); // 마지막 폭발 데미지 전달
		}
		if (options[5].active)
		{
			temp.OnActiveGuided(); // 유도기능 활성화
		}
	}
	public override bool CanUse()
	{
		if (
			property.nowCoolTime >= property.coolTime &&
			QPlayer.instance.status.sp >= usingSp
			)
		{
			return true;
		}
		return false;
	}
}

[Serializable] public class SkillOptionInformation
{
	public string name;
	public string info;
	public Sprite image;
	public bool active;
}