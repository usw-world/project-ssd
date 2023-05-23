using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnityBaaaall : UnityBall
{
	public override void OnActive(float damage, float speed)
	{
		speed = speed * 0.5f;
		base.OnActive(damage, speed);
	}
	protected override void OnTriggerEnter(Collider other)
	{
		if (other.gameObject.layer == 8)
		{
			IDamageable target = other.gameObject.GetComponent<IDamageable>();
			Damage damage = new Damage(
				damageAmount,
				.5f,
				(other.transform.position - transform.position).normalized * 10f,
				Damage.DamageType.Normal
			);
			target.OnDamage(damage);
			for (int i = 0; i < attachments.Count; i++)
			{
				Enemy enemy = other.gameObject.GetComponent<Enemy>();
				enemy.AddAttachment(attachments[i]);
			}
		}
	}
}
