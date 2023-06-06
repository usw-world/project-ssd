using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QPlayerSkillFinish : Skill
{
	public override bool CanUse()
	{
		if ( property.nowCoolTime >= property.coolTime )
		{
			return true;
		}
		return false;
	}
}
