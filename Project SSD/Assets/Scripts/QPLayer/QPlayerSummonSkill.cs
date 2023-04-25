using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QPlayerSummonSkill : Skill
{
	[SerializeField] GameObject summonSkillEffect;
	[SerializeField] float summonTime = 0;
	public override void Use(Vector3 target)
	{
		if (summonSkillEffect != null)
		{
			GameObject effect = Instantiate(summonSkillEffect);
			effect.transform.position = target;
			effect.GetComponent<SkillEffect>()?.OnActive(property);
		}
		
		StartCoroutine(Summon(target));
	}
	IEnumerator Summon(Vector3 target)
	{
		yield return new WaitForSeconds(summonTime);

		GameObject effect = Instantiate(info.effect);
		effect.transform.position = target;
		effect.GetComponent<SkillEffect>()?.OnActive(property);
	}
	public override bool CanUse()
	{
		return true;
	}
}
