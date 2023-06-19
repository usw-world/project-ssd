using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QPlayerSkillFinish : Skill
{
	public override void Use()
	{
		if (!CanUse()) return;
		QPlayer.instance.ChangeSp(-80f);
		property.nowCoolTime = 0;
	}
	public override bool CanUse()
	{
		if (
			property.nowCoolTime >= property.coolTime &&
			QPlayer.instance.status.sp >= 90f
			)
		{
			return true;
		}
		return false;
	}
	public override AimType GetAimType()
	{
		return AimType.Area;
	}
	public override SkillSize GetAreaAmout()
	{
		return new SkillSize(1.5f, 1.5f);
	}
}
