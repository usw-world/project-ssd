using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TPlayerSkill : Skill
{
	[SerializeField] private float usingSP = 0f;
	private string effectKey;
	private PlayerStatus status;
	void Start()
	{
		if (info.effect != null)
		{
			effectKey = info.effect?.GetComponent<IPoolableObject>().GetKey();
			PoolerManager.instance.InsertPooler(effectKey, info.effect, false, 5, 2);
		}
	}
	public override void Use()
	{
		property.nowCoolTime = 0;
		TPlayer.instance.ChangeSp(-usingSP);

		if (info.effect != null)
		{
			GameObject effect = PoolerManager.instance.OutPool(effectKey);
			float damageAmount = TPlayer.instance.GetAp() * property.skillAP;
			effect.GetComponent<TPlayerAttackEffect>().OnActive(damageAmount);
		}
	}
	public override void Use(float damage)
	{
		property.nowCoolTime = 0;
		TPlayer.instance.ChangeSp(-usingSP);

		if (info.effect != null)
		{
			GameObject effect = PoolerManager.instance.OutPool(effectKey);
			effect.GetComponent<TPlayerAttackEffect>().OnActive(damage);
		}
	}
	public string GetEffectKey() { return effectKey; }
	public override bool CanUse()
	{
		status = TPlayer.instance.status;

		if (CheckSP() && CheckCoolTime())  
			return true;
		return false;
	}
	bool CheckSP() 
	{
		if (status.sp >= usingSP)
			return true;
		return false;
	}
	bool CheckCoolTime() 
	{
		if (property.nowCoolTime >= property.coolTime)
			return true; ;
		return false;
	}
}
