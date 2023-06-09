using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TPlayerSkill : Skill
{
	[SerializeField] public float usingSP = 0f;
	[SerializeField] Damage.DamageType damageType = Damage.DamageType.Normal;
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
			GameObject effectGobj = PoolerManager.instance.OutPool(effectKey);
			float damageAmount = TPlayer.instance.GetAp() * property.skillAP;
			var effect = effectGobj.GetComponent<TPlayerAttackEffect>();
			effect.damageType = this.damageType;
			effect.OnActive(damageAmount);
		}
	}
	public override void Use(float damage)
	{
		property.nowCoolTime = 0;
		TPlayer.instance.ChangeSp(-usingSP);

		if (info.effect != null)
		{
			GameObject effectGobj = PoolerManager.instance.OutPool(effectKey);
			var effect = effectGobj.GetComponent<TPlayerAttackEffect>();
			effect.damageType = this.damageType;
			effect.OnActive(damage);
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

	public override AimType GetAimType() => throw new System.NotImplementedException(); 
	public override SkillSize GetAreaAmout() => throw new System.NotImplementedException(); 
}
