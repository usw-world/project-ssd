using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QPlayerSkillFightGhostFist : Skill
{
	public SkillOptionInformation[] options = new SkillOptionInformation[8];
	[SerializeField] private GameObject explosionPrefab;
	private string explosionKey;
	private string effectKey;
	private float usingSp = 30f;
	private bool isRuningChainSkill = false;
	private int chainCount = 0;

	/*
		 @ 0. 데미지 증가         
		 @ 1. 경직 -> 다운
		 @ 2. 폭 증가            
		 @ 3. 시전속도 증가
		 @ 4. 충돌해도 계속 직진      
		 @ 5. 폭발
		 @ 6. TPlayer 충돌 가능, 실드   
		 @ 7. 체인스킬 3연속
	 */
	private void Awake()
	{
		options[0].name = "빠른 주먹";
		options[0].info = "데미지가 50% 증가한다";
		options[1].name = "큰 주먹";
		options[1].info = "경직이 발생하지 않는 대신 적을 다운 시킨다";
		options[2].name = "부우웅";
		options[2].info = "공격 범위가 증가한다";
		options[3].name = "슈우웅";
		options[3].info = "시전 속도가 빨라진다";
		options[4].name = "누구도ㅋ 나를ㅋ 막을순ㅋ 없으셈ㅋ";
		options[4].info = "공격 경로상의 모든 적을 공격한다.";
		options[5].name = "넌 이미 죽어있다";
		options[5].info = "적과 충돌시 폭발이 발생하여 추가 피해를 입힌다";
		options[6].name = "너에게 가는길";
		options[6].info = "파트너와 충돌시 파트너에게 실드를 제공합니다";
		options[7].name = "오라 오라 오라";
		options[7].info = "체인스킬로 변경되며 연속으로 최대 3번 사용 가능하다.";
		//options[0].active = true;
		//options[1].active = true;
		//options[2].active = true;
		//options[3].active = true;
		//options[4].active = true;
		//options[5].active = true;
		//options[6].active = true;
		//options[7].active = true;
	}
	private void Start()
	{
		//effectKey = info.effect.GetComponent<IPoolableObject>().GetKey();
		explosionKey = explosionPrefab.GetComponent<IPoolableObject>().GetKey();

		//PoolerManager.instance.InsertPooler(effectKey, info.effect, false);
		PoolerManager.instance.InsertPooler(explosionKey, explosionPrefab, false);
	}
	public override void Use(Vector3 target)
	{
		if (options[7].active)
		{
			chainCount++;
			QPlayer.instance.status.sp -= usingSp * 0.5f;
			if (chainCount == 1) StartCoroutine(ChainSkillTimeOut());
			else if (chainCount >= 3) EndChainSkill();
		}
		else
		{
			property.nowCoolTime = 0;
			QPlayer.instance.status.sp -= usingSp;
		}
		QPlayer.instance.OnFightGhostFist(true);
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
		yield return new WaitForSeconds(6f);
		EndChainSkill();
	}
	public float GetSkillPower()
	{
		return property.skillAP;
	}
	public Effect_FightGhostFistExplosion GetExplosion()
	{
		GameObject explosion = PoolerManager.instance.OutPool(explosionKey);
		return explosion.GetComponent<Effect_FightGhostFistExplosion>();
	}
	public GameObject GetEffect() 
	{
		return PoolerManager.instance.OutPool(effectKey);
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
