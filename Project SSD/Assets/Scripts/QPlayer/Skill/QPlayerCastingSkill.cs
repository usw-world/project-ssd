using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QPlayerCastingSkill : Skill
{
    [SerializeField] GameObject castingSkillEffect;
    [SerializeField] float castingTime = 2f;
	public override void Use(Vector3 target)
	{
		GameObject effect = Instantiate(castingSkillEffect);
		effect.transform.position = target;
		effect.GetComponent<SkillEffect>()?.OnActive(property);

		StartCoroutine(Attack(target));
	}
	IEnumerator Attack(Vector3 target) 
	{
		yield return new WaitForSeconds(castingTime);

		GameObject effect = Instantiate(info.effect);
		effect.transform.position = target;
		effect.GetComponent<SkillEffect>()?.OnActive(property);
	}
	public override bool CanUse()
	{
		return true;
	}
}
