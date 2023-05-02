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
	protected override void OnCollisionEnter(Collision collision)
	{
		if (collision.gameObject.layer == 8)
		{
			IDamageable temp = collision.gameObject.GetComponent<IDamageable>();
			temp.OnDamage(gameObject, damage);
			// 디버프 넘겨주기
		}
	}
}
