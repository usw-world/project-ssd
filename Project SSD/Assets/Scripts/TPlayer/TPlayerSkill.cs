using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TPlayerSkill : Skill
{
	[SerializeField] float usingSP = 0f;

	PlayerStatus status;
	public override void Use()
	{
		property.nowCoolTime = 0;
		TPlayer.instance.status.sp -= usingSP;

		if (info.effect != null)
		{
			GameObject temp = Instantiate(info.effect);
			SkillEffect skillEffect = temp.GetComponent<SkillEffect>();
			skillEffect.OnActive(property);
		}
	}
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
