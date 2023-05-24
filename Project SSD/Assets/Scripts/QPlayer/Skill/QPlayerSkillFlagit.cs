using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QPlayerSkillFlagit : Skill
{
	public SkillOptionInformation[] options = new SkillOptionInformation[8];
	/*
		 기본 쿨 8초

		 options[0] = 주변 추가 지속 데미지 o
		 options[1] = 주변 실드 - 티플에 실드 구현

		 options[2] = 쿨감 3초	o
		 options[3] = 경직 시간 4초 o

		 options[4] = 범위 증가 2배	o
		 options[5] = 낙하 속도 증가 o

		 options[6] = 데미지 아주 강하게, 크기 증가 o
		 options[7] = 3개 체인으로 사용 
	*/
	private int chainCount = 0;

	private string effectKey;
	private void Start()
	{
		effectKey = info.effect.GetComponent<IPoolableObject>().GetKey();
		PoolerManager.instance.InsertPooler(effectKey, info.effect, false);
	}
	private void EndChainSkill()
	{
		chainCount = 0;
		// 쿨타임 시작
	}
	IEnumerator ChainSkillTimeOut()
	{
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
			if (chainCount == 1) StartCoroutine(ChainSkillTimeOut());
			if (chainCount >= 3) EndChainSkill(); 
			// sp 감소
		}
		else
		{
			// sp 감소 , 쿨타임 시작
		}

		Vector3 sponPos = target;
		sponPos.y += 3f;

		GameObject flagitObj = null;
		Effect_Flagit flagit = null;

		flagitObj = PoolerManager.instance.OutPool(effectKey);
		flagit = flagitObj.GetComponent<Effect_Flagit>();

		flagit.SetDamage(10f);

		if (options[0].active)
		{
			flagit.ActiveDotDamage(10f);
		}
		if (options[1].active)
		{
			flagit.ActiveShield(10f);
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
		return true;
	}
}
