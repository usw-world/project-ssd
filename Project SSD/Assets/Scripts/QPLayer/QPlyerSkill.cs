using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QPlyerSkill : Skill
{
	[SerializeField] float usingSp = 0;
	public override void Use(Vector3 target)
	{
		QPlayer.instance.status.SP -= usingSp;

		GameObject effect = Instantiate(info.effect);
		effect.transform.position = target;
		effect.GetComponent<SkillEffect>()?.OnActive(property);
	}
	public override bool CanUse()
	{
		if (QPlayer.instance.status.SP >= usingSp) {
			if (property.nowCoolTime >= property.coolTime)
			{
				return true;
			}
		}
		return false;
	}
}
