using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QPlayerSkillFlagit : Skill
{
	public SkillOptionInformation[] options = new SkillOptionInformation[8];
	/*
		 기본 쿨 8초

		 options[0] = 주변 추가 지속 데미지 o
		 options[1] = 주변 실드 - 티플에 실드 구현 o

		 options[2] = 쿨감 3초	o
		 options[3] = 경직 시간 4초 o

		 options[4] = 범위 증가 2배	o
		 options[5] = 낙하 속도 증가 o

		 options[6] = 데미지 아주 강하게, 크기 증가 o
		 options[7] = 3개 체인으로 사용 o
	*/
	private string effectKey;
	private float usingSp = 10f;
	private bool isRuningChainSkill = false;
	private int chainCount = 0;
	private void Awake()
	{
		options[0].name = "자기장";
		options[0].info = "주변 적에게 지속 데미지";
		options[1].name = "매트릭스";
		options[1].info = "파트너에게 실드 재공";
		options[2].name = "빠른 준비";
		options[2].info = "쿨타임 3초 감소";
		options[3].name = "감전";
		options[3].info = "경직시간 3초 증가";
		options[4].name = "넓은 공격";
		options[4].info = "범위 증가";
		options[5].name = "빠른 공격";
		options[5].info = "시전시간 감소";
		options[6].name = "차아암!";
		options[6].info = "범위, 대미지 증가";
		options[7].name = "참참참";
		options[7].info = "체인스킬로 변경, 3회 연속사용 가능";
		options[0].active = true;
		//options[1].active = true;
		//options[2].active = true;
		//options[3].active = true;
		//options[4].active = true;
		//options[5].active = true;
		options[6].active = true;
		//options[7].active = true;
	}
	private void Start()
	{
		effectKey = info.effect.GetComponent<IPoolableObject>().GetKey();
		PoolerManager.instance.InsertPooler(effectKey, info.effect, false);
	}
	private void EndChainSkill()
	{ 
		if (isRuningChainSkill)
		{
			isRuningChainSkill = false;
			chainCount = 0;
			property.nowCoolTime = 0;
		}
	}
	IEnumerator ChainSkillTimeOut()
	{
		isRuningChainSkill = true;
		yield return new WaitForSeconds(5f);
		EndChainSkill();
	}
	public override void Use(Vector3 target)
	{
		if (options[2].active)	{
			property.coolTime = 5f;
		}else{
			property.coolTime = 8f;
		}
		if (options[7].active)
		{
			chainCount++;
			QPlayer.instance.status.sp -= usingSp;
			if (chainCount == 1) StartCoroutine(ChainSkillTimeOut());
			else if (chainCount >= 3) EndChainSkill();
		}
		else
		{
			property.nowCoolTime = 0;
			QPlayer.instance.status.sp -= usingSp;
		}

		Vector3 sponPos = target;
		sponPos.y += 3f;

		GameObject flagitObj = null;
		Effect_Flagit flagit = null;

		flagitObj = PoolerManager.instance.OutPool(effectKey);
		flagit = flagitObj.GetComponent<Effect_Flagit>();
		flagitObj.transform.position = target;
		float lastDamage = QPlayer.instance.GetAP() * property.skillAP;

		flagit.Initialize(lastDamage);

		if (options[0].active)
		{
			flagit.ActiveDotDamage(lastDamage * 0.2f);
		}
		if (options[1].active)
		{
			Attachment attachment = new Attachment(2f, 1f, options[1].image, EAttachmentType.shield);
			TPlayerShield shield = new TPlayerShield(lastDamage * 0.5f);
			attachment.onAction = (gameObject) => {
				TPlayer.instance.AddShield(shield);
			};
			attachment.onInactive = (gameObject) => {
				TPlayer.instance.RemoveShield(shield);
			};
			flagit.ActiveShield(attachment);
		}
		if (options[3].active)
		{
			flagit.ActiveFlinching4s();
		}
		if (options[4].active)
		{
			flagit.ActiveAreaTwice();
		}
		if (options[5].active)	{
			flagit.SetSpeedFast();
		}else{
			flagit.SetSpeedNormal();
		}
		if (options[6].active)
		{
			flagit.ActiveBig();
		}
		flagit.Run();
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