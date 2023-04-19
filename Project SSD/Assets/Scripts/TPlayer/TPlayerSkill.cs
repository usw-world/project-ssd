using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TPlayerSkill : Skill
{
	public override void Use()
	{
		property.nowCollTime = 0;
		GameObject temp = Instantiate(info.effect);
		SkillEffect skillEffect = temp.GetComponent<SkillEffect>();
		skillEffect.OnActive(property);
	}
}
