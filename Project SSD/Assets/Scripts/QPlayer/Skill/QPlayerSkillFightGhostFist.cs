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

	private void Awake()
	{
		effectKey = info.effect.GetComponent<IPoolableObject>().GetKey();
		PoolerManager.instance.InsertPooler(effectKey, info.effect, false);
		explosionKey = explosionPrefab.GetComponent<IPoolableObject>().GetKey();
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
		yield return new WaitForSeconds(5f);
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
