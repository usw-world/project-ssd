using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillEffect : MonoBehaviour
{
	protected SkillProperty property;
	protected GameObject origin;
	protected List<Enemy> attackedEnemy = new List<Enemy>();
	private void Start()
	{
		Destroy(gameObject, 1f);
	}
	virtual public void Set(SkillProperty property, GameObject origin)
	{
		this.property = property;
		this.origin = origin;
	}
	private void OnTriggerEnter(Collider other)
	{
		Enemy hitEnemy = other.GetComponent<Enemy>();
		if (hitEnemy != null)
		{
			if (!attackedEnemy.Contains(hitEnemy))
			{
				attackedEnemy.Add(hitEnemy);
				float amount = property.skillAP * origin.GetComponent<QPlyerSkillTest>().GetAP();
				hitEnemy.OnDamage(origin, amount);
			}
		}
	}
}
