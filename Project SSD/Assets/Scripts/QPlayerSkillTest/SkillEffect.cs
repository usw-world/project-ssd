using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillEffect : MonoBehaviour
{
	protected SkillProperty property;
	protected GameObject origin;
	protected List<IDamageable> targets = new List<IDamageable>();
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
		if (other.gameObject.layer == 1 << LayerMask.NameToLayer("Enemy"))
		{
			IDamageable target = other.GetComponent<IDamageable>();
			if (target != null)
			{
				if (!targets.Contains(target))
				{
					targets.Add(target);
					float amount = property.skillAP * origin.GetComponent<QPlyerSkillTest>().GetAP();
					target.OnDamage(origin, amount);
				}
			}
		}
	}
}
